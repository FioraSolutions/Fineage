#!/usr/bin/env python3
"""Punch O_E_GDS_Main doorway: 034_shader + wall-facing (±X) blockers; keep floors."""
import bpy
import bmesh
import shutil
from collections import Counter
from mathutils import Vector, Quaternion, Matrix
from mathutils.bvhtree import BVHTree

bak = "/Users/nikitarakitin/Projects/Fineage/.work/O_E_GDS_Main.fbx.bak_sealed"
src = "/Users/nikitarakitin/Projects/Fineage/l2-unity/Assets/Resources/Data/StaticMeshes/Oren_HEV_S/O_E_GDS_Main.fbx"
shutil.copy2(bak, src)

bpy.ops.wm.read_factory_settings(use_empty=True)
bpy.ops.import_scene.fbx(filepath=src)
o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
me = o.data
bm = bmesh.new()
bm.from_mesh(me)
bm.faces.ensure_lookup_table()
bm.normal_update()

SCALE = 0.019

del034 = [
    f
    for f in bm.faces
    if me.materials[f.material_index] and "034_shader" in me.materials[f.material_index].name
]
print("del 034", len(del034))
bmesh.ops.delete(bm, geom=del034, context="FACES")
bm.faces.ensure_lookup_table()
bm.normal_update()

q = Quaternion((0.7071068, 0.0, 0.7071068, 0.0))
R = q.to_matrix()

instances = [
    {
        "pos": Vector((982.0953, -50.628574, 876.64764)),
        "scl": Vector((1, 1, 1)),
        "world_dir": Vector((0, 0, 1)),
        "z0": 868.65,
    },
    {
        "pos": Vector((982.0953, -50.628574, 851.3905)),
        "scl": Vector((-1, 1, 1)),
        "world_dir": Vector((0, 0, -1)),
        "z0": 860.0,
    },
]


def make_Minv(pos, scl):
    M = Matrix.Translation(pos) @ R.to_4x4() @ Matrix.Diagonal((scl.x, scl.y, scl.z, 1))
    return M.inverted(), (R @ Matrix.Diagonal(scl)).inverted()


deleted = Counter()
MAX_ITERS = 120

for it in range(MAX_ITERS):
    bm.faces.ensure_lookup_table()
    bm.normal_update()
    bvh = BVHTree.FromBMesh(bm)
    to_del = []
    seen = set()
    for inst in instances:
        Minv, RSinv = make_Minv(inst["pos"], inst["scl"])
        dl = (RSinv @ inst["world_dir"]).normalized()
        for dy in [i * 0.2 for i in range(2, 12)]:  # 0.4 .. 2.2 — above foot lip
            for dx in [-1.8, -1.2, -0.6, 0, 0.6, 1.2, 1.8]:
                ow = Vector((982.10 + dx, -49.10 + dy, inst["z0"]))
                ol_fbx = (Minv @ ow) / SCALE
                hit = bvh.ray_cast(ol_fbx, dl, 18.0 / SCALE)
                if not hit[0]:
                    continue
                _loc, _nrm, idx, dist_fbx = hit
                if idx is None or idx >= len(bm.faces):
                    continue
                if dist_fbx * SCALE > 12.0:
                    continue
                f = bm.faces[idx]
                if id(f) in seen:
                    continue
                n = f.normal
                # Keep walk decks / near-horizontal lips
                if abs(n.y) >= 0.85:
                    continue
                # Doorway seal faces face local ±X after 90°Y
                if abs(n.x) < 0.25 and abs(n.z) < 0.55:
                    continue
                cen_u = f.calc_center_median() * SCALE
                if cen_u.y > 3.5:
                    continue
                seen.add(id(f))
                to_del.append(f)
                mat = me.materials[f.material_index].name if me.materials[f.material_index] else "?"
                deleted[mat] += 1
    if not to_del:
        print("clear at iter", it)
        break
    print(f"iter {it}: delete {len(to_del)} {dict(Counter(me.materials[f.material_index].name for f in to_del))}")
    bmesh.ops.delete(bm, geom=to_del, context="FACES")
else:
    print("WARN: max iters")

bm.faces.ensure_lookup_table()
bm.normal_update()
bvh = BVHTree.FromBMesh(bm)

remain = 0
for inst in instances:
    Minv, RSinv = make_Minv(inst["pos"], inst["scl"])
    dl = (RSinv @ inst["world_dir"]).normalized()
    for dy in [0.5, 1.0, 1.5, 2.0]:
        for dx in [-1.2, 0, 1.2]:
            ow = Vector((982.10 + dx, -49.10 + dy, inst["z0"]))
            hit = bvh.ray_cast((Minv @ ow) / SCALE, dl, 18.0 / SCALE)
            if hit[0] and hit[3] * SCALE <= 12.0:
                f = bm.faces[hit[2]]
                # count only wall-like remains
                if abs(f.normal.y) < 0.85:
                    remain += 1
                    print(
                        "REMAIN",
                        round(inst["pos"].z, 1),
                        dy,
                        dx,
                        me.materials[f.material_index].name,
                        "ny",
                        round(abs(f.normal.y), 2),
                        "nx",
                        round(abs(f.normal.x), 2),
                        "d",
                        round(hit[3] * SCALE, 2),
                    )

print("remain_wall_hits", remain)
print("deleted_by_mat", dict(deleted))
print("faces", len(bm.faces), "verts", len(bm.verts))

# Floor coverage on east Main (Unity xz)
pos = instances[0]["pos"]
Minv, RSinv = make_Minv(pos, instances[0]["scl"])
dl = (RSinv @ Vector((0, -1, 0))).normalized()
for xz in [
    (982.1, 870),
    (982.1, 872),
    (982.1, 874),
    (982.1, 875),
    (982.1, 876),
    (982.1, 878),
    (982.1, 880),
]:
    ow = Vector((xz[0], -45.0, xz[1]))
    hit = bvh.ray_cast((Minv @ ow) / SCALE, dl, 20.0 / SCALE)
    if hit[0]:
        f = bm.faces[hit[2]]
        print(
            "floor",
            xz,
            "d",
            round(hit[3] * SCALE, 2),
            me.materials[f.material_index].name,
            "ny",
            round(abs(f.normal.y), 2),
        )
    else:
        print("floor", xz, "MISS")

bm.to_mesh(me)
me.update()
bm.free()
bpy.ops.export_scene.fbx(
    filepath=src,
    use_selection=False,
    apply_unit_scale=True,
    bake_space_transform=False,
    axis_forward="-Z",
    axis_up="Y",
    mesh_smooth_type="FACE",
    use_tspace=True,
)
print("exported", src)
