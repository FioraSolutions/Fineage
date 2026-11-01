#!/usr/bin/env python3
"""Headless Blender: convert umodel .pskx under export tree to .fbx (Shnok layout)."""
import os
import sys

import bpy

addon_dir = os.path.join(
    os.path.dirname(__file__),
    "../../l2-unity/Assets/Editor/L2EffectGenerator/Blender",
)
addon_dir = os.path.normpath(addon_dir)
sys.path.insert(0, addon_dir)
import io_import_scene_unreal_psa_psk_280  # noqa: E402


def convert_one(model_file: str) -> None:
    out = os.path.splitext(model_file)[0] + ".fbx"
    out = out.replace("StaticMesh" + os.sep, "").replace("StaticMesh/", "")
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
    bpy.ops.export_scene.fbx(filepath=out, use_selection=False, bake_space_transform=True)


def main() -> None:
    if len(sys.argv) < 2 or not os.path.isdir(sys.argv[-1]):
        print("usage: blender -b -P pskx_to_fbx_batch.py -- <export_root>")
        return
    root = sys.argv[-1]
    model_files = []
    for dirpath, _, files in os.walk(root):
        for name in files:
            if name.endswith(".pskx"):
                model_files.append(os.path.join(dirpath, name))
    model_files.sort()
    failed = []
    print("converting", len(model_files), "pskx files under", root)
    for i, path in enumerate(model_files, 1):
        if i % 25 == 0:
            print(i, "/", len(model_files), path)
        try:
            convert_one(path)
        except Exception as exc:  # one broken pskx must not abort the whole tile
            failed.append(path)
            print("FAILED", path, exc)
    print("done, failed:", len(failed))


if __name__ == "__main__":
    main()
