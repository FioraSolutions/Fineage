# PSK -> FBX for L2 armor pieces (skinned, refit to race body).
# blender -b -P conv.py -- <psk_dir> <fbx_dir> <addon.py>
import bpy
import os
import sys
import importlib.util
import math


def _argv_after_dash():
    if "--" not in sys.argv:
        raise SystemExit("usage: blender -b -P conv.py -- <psk_dir> <fbx_dir> <addon.py>")
    return sys.argv[sys.argv.index("--") + 1:]


def _load_addon(path):
    spec = importlib.util.spec_from_file_location("io_import_scene_unreal_psa_psk_280", path)
    mod = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(mod)
    return mod


def main():
    args = _argv_after_dash()
    psk_dir, fbx_dir, addon_path = args[0], args[1], args[2]
    os.makedirs(fbx_dir, exist_ok=True)
    mod = _load_addon(addon_path)

    names = sorted(f for f in os.listdir(psk_dir) if f.endswith(".psk"))
    for name in names:
        psk_path = os.path.join(psk_dir, name)
        fbx_path = os.path.join(fbx_dir, os.path.splitext(name)[0] + ".fbx")
        if os.path.exists(fbx_path):
            print("SKIP", fbx_path)
            continue
        bpy.ops.wm.read_factory_settings(use_empty=True)
        ok = mod.pskimport(
            psk_path,
            context=bpy.context,
            bScaleDown=True,
            bDontInvertRoot=True,
            bReorientBones=False,
        )
        if ok is False:
            print("FAIL import", psk_path)
            continue
        meshes = [o for o in bpy.context.scene.objects if o.type == "MESH"]
        if not meshes:
            print("FAIL nomesh", psk_path)
            continue
        for o in meshes:
            print("DIMS", name, tuple(round(v, 4) for v in o.dimensions))
        bpy.ops.export_scene.fbx(
            filepath=fbx_path,
            use_selection=False,
            object_types={"ARMATURE", "MESH"},
            use_mesh_modifiers=True,
            mesh_smooth_type="FACE",
            add_leaf_bones=True,
            primary_bone_axis="Y",
            secondary_bone_axis="X",
            axis_forward="-Z",
            axis_up="Y",
            bake_space_transform=False,
            apply_unit_scale=True,
            apply_scale_options="FBX_SCALE_NONE",
            path_mode="AUTO",
        )
        print("WROTE", fbx_path)


if __name__ == "__main__":
    main()
