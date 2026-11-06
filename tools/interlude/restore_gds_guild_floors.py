#!/usr/bin/env python3
"""Restore Elven Guild entrance walk decks, then re-punch the doorway safely.

Prefer tools/interlude/punch_gds_doorway.py (same outcome: Ent from sealed bak +
Main with 034/103/009 seals removed, floors kept).

Requires Blender on PATH. Run from Fineage repo root:

  blender -b -P tools/interlude/restore_gds_guild_floors.py
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

ENT_BAK = WORK / "O_E_GDS_Ent.fbx.bak_sealed"
ENT_CUR = MESH / "O_E_GDS_Ent.fbx"
MAIN_BAK = WORK / "O_E_GDS_Main.fbx.bak_sealed"
MAIN_CUR = MESH / "O_E_GDS_Main.fbx"

SCALE = 0.019
R = Quaternion((0.7071068, 0.0, 0.7071068, 0.0)).to_matrix()


def face_key(f, me):
    verts = tuple(sorted(tuple(round(c, 2) for c in v.co) for v in f.verts))
    mn = me.materials[f.material_index].name if me.materials[f.material_index] else "?"
    return (mn, verts)


def world_n(f):
    return (R @ f.normal).normalized()


def import_info(path: Path):
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=str(path))
    o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
    me = o.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.faces.ensure_lookup_table()
    bm.normal_update()
    keys = set()
    info = []
    for f in bm.faces:
        k = face_key(f, me)
        keys.add(k)
        info.append((k, abs(world_n(f).y) > 0.7, k[0]))
    bm.free()
    return keys, info


def restore_ent():
    if not ENT_BAK.is_file():
        raise SystemExit(f"missing {ENT_BAK}")
    shutil.copy2(ENT_BAK, ENT_CUR)
    print(f"Ent restored from {ENT_BAK.name}")


def restore_main():
    if not MAIN_BAK.is_file():
        raise SystemExit(f"missing {MAIN_BAK}")
    if not MAIN_CUR.is_file():
        raise SystemExit(f"missing {MAIN_CUR}")

    punched = WORK / "O_E_GDS_Main.fbx.punched_tmp"
    shutil.copy2(MAIN_CUR, punched)
    try:
        punched_keys, _ = import_info(punched)
        _, bak_info = import_info(MAIN_BAK)

        intentional = set()
        restore_floor = Counter()
        for k, is_floor, mat in bak_info:
            if k in punched_keys:
                continue
            if mat == "elf_vi_034_shader" or not is_floor:
                intentional.add(k)
            else:
                restore_floor[mat] += 1

        print("Main intentional doorway deletes kept:", len(intentional))
        print("Main floor faces restored:", dict(restore_floor))

        bpy.ops.wm.read_factory_settings(use_empty=True)
        bpy.ops.import_scene.fbx(filepath=str(MAIN_BAK))
        o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
        me = o.data
        bm = bmesh.new()
        bm.from_mesh(me)
        bm.faces.ensure_lookup_table()
        bm.normal_update()
        to_del = [f for f in bm.faces if face_key(f, me) in intentional]
        bmesh.ops.delete(bm, geom=to_del, context="FACES")
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
        print(f"Main exported {MAIN_CUR} ({MAIN_CUR.stat().st_size} bytes)")
    finally:
        if punched.exists():
            punched.unlink()


def verify():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=str(ENT_CUR))
    o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
    me = o.data
    bm = bmesh.new()
    bm.from_mesh(me)
    mats = Counter(
        me.materials[f.material_index].name
        for f in bm.faces
        if me.materials[f.material_index]
    )
    print(
        "Ent faces",
        len(bm.faces),
        "117",
        mats.get("elf_vi_117", 0),
        "111",
        mats.get("elf_vi_111", 0),
    )
    bm.free()

    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=str(MAIN_CUR))
    o = [x for x in bpy.context.scene.objects if x.type == "MESH"][0]
    me = o.data
    bm = bmesh.new()
    bm.from_mesh(me)
    bm.faces.ensure_lookup_table()
    bm.normal_update()
    pos = Vector((982.0953, -50.628574, 876.64764))
    scl = Vector((1, 1, 1))
    M = Matrix.Translation(pos) @ R.to_4x4() @ Matrix.Diagonal((scl.x, scl.y, scl.z, 1))
    Minv = M.inverted()
    RSinv = (R @ Matrix.Diagonal(scl)).inverted()
    down = (RSinv @ Vector((0, -1, 0))).normalized()
    bvh = BVHTree.FromBMesh(bm)
    for z in (872, 874, 876, 878):
        ow = Vector((982.1, -45.0, z))
        hit = bvh.ray_cast((Minv @ ow) / SCALE, down, 20.0 / SCALE)
        if hit[0]:
            f = bm.faces[hit[2]]
            print("Main floor", z, me.materials[f.material_index].name)
        else:
            print("Main floor", z, "MISS")
    mats = Counter(
        me.materials[f.material_index].name
        for f in bm.faces
        if me.materials[f.material_index]
    )
    print("Main 034_shader remaining", mats.get("elf_vi_034_shader", 0), "(want 0)")
    bm.free()


def main():
    os.chdir(ROOT)
    restore_ent()
    restore_main()
    verify()


if __name__ == "__main__":
    # When run via `blender -b -P this.py`, Blender already imported bpy.
    try:
        main()
    except Exception as e:
        print("ERROR:", e, file=sys.stderr)
        raise
