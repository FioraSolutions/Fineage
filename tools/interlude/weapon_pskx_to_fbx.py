#!/usr/bin/env python3
"""Headless Blender: LineageWeapons .pskx -> .fbx, then solidify single-sided exports."""
import os
import subprocess
import sys

import bpy

addon_dir = os.path.normpath(
    os.path.join(os.path.dirname(__file__), "../../l2-unity/Assets/Editor/L2EffectGenerator/Blender")
)
sys.path.insert(0, addon_dir)
import io_import_scene_unreal_psa_psk_280  # noqa: E402

REPO = os.path.normpath(os.path.join(os.path.dirname(__file__), "../.."))
SOLIDIFY = os.path.join(os.path.dirname(__file__), "weapon_mesh_solidify.py")


def apply_solidify_modifier() -> bool:
    obj = bpy.context.active_object
    if obj is None or obj.type != "MESH":
        return False
    mod = obj.modifiers.new(name="WeaponSolidify", type="SOLIDIFY")
    mod.thickness = max(max(obj.dimensions) * 0.035, 0.0015)
    mod.offset = 1.0
    mod.use_flip_normals = False
    bpy.ops.object.modifier_apply(modifier=mod.name)
    return True


def convert_one(model_file: str) -> None:
    out = os.path.splitext(model_file)[0] + ".fbx"
    if os.path.exists(out):
        return
    bpy.ops.wm.read_factory_settings(use_empty=True)
    io_import_scene_unreal_psa_psk_280.pskimport(
        model_file,
        context=None,
        bImportmesh=True,
        bImportbone=True,
        bSpltiUVdata=False,
        fBonesize=5.0,
        fBonesizeRatio=0.6,
        bDontInvertRoot=True,
        bReorientBones=False,
        bReorientDirectly=False,
        bScaleDown=True,
        bToSRGB=True,
        error_callback=None,
    )
    mesh_obj = next((o for o in bpy.data.objects if o.type == "MESH"), None)
    if mesh_obj is None:
        raise RuntimeError("no mesh imported from " + model_file)
    bpy.context.view_layer.objects.active = mesh_obj
    apply_solidify_modifier()
    bpy.ops.export_scene.fbx(filepath=out, use_selection=False, bake_space_transform=True)


def main() -> None:
    if len(sys.argv) < 2 or not os.path.isdir(sys.argv[-1]):
        print("usage: blender -b -P weapon_pskx_to_fbx.py -- <export_root>")
        return
    root = sys.argv[-1]
    model_files = []
    for dirpath, _, files in os.walk(root):
        for name in files:
            if name.endswith(".pskx"):
                model_files.append(os.path.join(dirpath, name))
    model_files.sort()
    failed = []
    print("converting", len(model_files), "weapon pskx under", root)
    for i, path in enumerate(model_files, 1):
        if i % 25 == 0:
            print(i, "/", len(model_files), path)
        try:
            convert_one(path)
        except Exception as exc:
            failed.append(path)
            print("FAILED", path, exc)
    print("done, failed:", len(failed))


if __name__ == "__main__":
    main()