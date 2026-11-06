#!/usr/bin/env python3
"""Punch Elven Guild (O_E_GDS_Main) sealed doorway without deleting walk decks.

Removes:
  - all elf_vi_034_shader faces
  - elf_vi_103 faces hit by doorway portal rays (beige door plate)
  - elf_vi_009 lintel undersides that hitch the capsule in the arch

Keeps all near-horizontal floors (world |ny| >= 0.70). Restores Ent from sealed bak
(Ent has the apron deck; do not delete its upward faces).

Run from Fineage repo root:

  blender -b -P tools/interlude/punch_gds_doorway.py
"""
from __future__ import annotations

import os
import shutil
import sys
from collections import Counter
from pathlib import Path

import bpy
import bmesh
from mathutils import Matrix, Quaternion, Vector
from mathutils.bvhtree import BVHTree

ROOT = Path(__file__).resolve().parents[2]
WORK = ROOT / ".work"
MESH = ROOT / "l2-unity/Assets/Resources/Data/StaticMeshes/Oren_HEV_S"

MAIN_BAK = WORK / "O_E_GDS_Main.fbx.bak_sealed"
MAIN_CUR = MESH / "O_E_GDS_Main.fbx"
ENT_BAK = WORK / "O_E_GDS_Ent.fbx.bak_sealed"
ENT_CUR = MESH / "O_E_GDS_Ent.fbx"

SCALE = 0.019
R = Quaternion((0.7071068, 0.0, 0.7071068, 0.0)).to_matrix()

INSTANCES = [
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


def world_n(f):
    return (R @ f.normal).normalized()


def punch_main():
    if not MAIN_BAK.is_file():
        raise SystemExit(f"missing {MAIN_BAK}")
    shutil.copy2(MAIN_BAK, MAIN_CUR)

    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=str(MAIN_CUR))
    o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
    me = o.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.faces.ensure_lookup_table()
    bm.normal_update()

    del034 = [
        f
        for f in bm.faces
        if me.materials[f.material_index]
        and "034_shader" in me.materials[f.material_index].name
    ]
    print("del 034", len(del034))
    bmesh.ops.delete(bm, geom=del034, context="FACES")

    deleted = Counter()
    for _it in range(40):
        bm.faces.ensure_lookup_table()
        bm.normal_update()
        bvh = BVHTree.FromBMesh(bm)
        to_del = []
        seen = set()
        for inst in INSTANCES:
            Minv, RSinv = make_Minv(inst["pos"], inst["scl"])
            dl = (RSinv @ inst["world_dir"]).normalized()
            for dy in [i * 0.12 for i in range(0, 35)]:
                for dx in [-2.2, -1.6, -1.0, -0.5, 0, 0.5, 1.0, 1.6, 2.2]:
                    ow = Vector((982.10 + dx, -50.63 + dy, inst["z0"]))
                    hit = bvh.ray_cast((Minv @ ow) / SCALE, dl, 18.0 / SCALE)
                    if not hit[0]:
                        continue
                    _loc, _nrm, idx, dist_fbx = hit
                    if idx is None or idx >= len(bm.faces) or dist_fbx * SCALE > 12.0:
                        continue
                    f = bm.faces[idx]
                    if id(f) in seen:
                        continue
                    mat = (
                        me.materials[f.material_index].name
                        if me.materials[f.material_index]
                        else "?"
                    )
                    if mat != "elf_vi_103":
                        continue
                    if abs(world_n(f).y) >= 0.70:
                        continue
                    seen.add(id(f))
                    to_del.append(f)
                    deleted[mat] += 1
        if not to_del:
            break
        bmesh.ops.delete(bm, geom=to_del, context="FACES")

    print("del elf_vi_103 seals", dict(deleted))

    # Lintel undersides (elf_vi_009) that hang into the capsule torso band.
    lintel = Counter()
    for _it in range(40):
        bm.faces.ensure_lookup_table()
        bm.normal_update()
        bvh = BVHTree.FromBMesh(bm)
        to_del = []
        seen = set()
        for inst in INSTANCES:
            Minv, RSinv = make_Minv(inst["pos"], inst["scl"])
            dl = (RSinv @ inst["world_dir"]).normalized()
            for dy in [i * 0.1 for i in range(12, 40)]:
                for dx in [-1.6, -0.8, 0, 0.8, 1.6]:
                    ow = Vector((982.10 + dx, -50.63 + dy, inst["z0"]))
                    hit = bvh.ray_cast((Minv @ ow) / SCALE, dl, 12.0 / SCALE)
                    if not hit[0]:
                        continue
                    _loc, _nrm, idx, dist_fbx = hit
                    if idx is None or idx >= len(bm.faces) or dist_fbx * SCALE > 10.0:
                        continue
                    f = bm.faces[idx]
                    if id(f) in seen:
                        continue
                    mat = (
                        me.materials[f.material_index].name
                        if me.materials[f.material_index]
                        else "?"
                    )
                    if mat != "elf_vi_009":
                        continue
                    if world_n(f).y > 0.3:
                        continue
                    seen.add(id(f))
                    to_del.append(f)
                    lintel[mat] += 1
        if not to_del:
            break
        bmesh.ops.delete(bm, geom=to_del, context="FACES")

    print("del elf_vi_009 lintels", dict(lintel), "faces", len(bm.faces))
    bm.to_mesh(me)
    me.update()
    bm.free()

    bpy.ops.export_scene.fbx(
        filepath=str(MAIN_CUR),
        use_selection=False,
        apply_unit_scale=True,
        bake_space_transform=False,
        axis_forward="-Z",
        axis_up="Y",
        mesh_smooth_type="FACE",
        use_tspace=True,
    )
    print("exported", MAIN_CUR, MAIN_CUR.stat().st_size)


def restore_ent():
    if not ENT_BAK.is_file():
        raise SystemExit(f"missing {ENT_BAK}")
    shutil.copy2(ENT_BAK, ENT_CUR)
    print("Ent restored", ENT_CUR.stat().st_size)


def main():
    os.chdir(ROOT)
    restore_ent()
    punch_main()


if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print("ERROR:", e, file=sys.stderr)
        raise
