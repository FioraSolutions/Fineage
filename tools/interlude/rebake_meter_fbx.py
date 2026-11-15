# Re-bake an existing character FBX to the meter contract (Docs/METER_SCALE_MIGRATION.md §3).
#
# Input FBX keeps its shipping vertices/bones/UVs/weights. We only bake scale:
#   1. import (no animation)
#   2. transform_apply(scale) on roots whose scale != 1   -> node x100 goes into the data
#   3. scale = nodeScale * 0.019 on the same roots, apply -> unit factor baked in
#   4. export with apply_scale_options="FBX_SCALE_ALL"     -> node scale is exactly 1
#
# World-space bone/mesh positions must stay identical; the .anim clips are rescaled
# separately in Unity (L2MeterContractMigration).
#
# blender -b -P rebake_meter_fbx.py -- <in_dir> <out_dir> [name_filter]
import json
import os
import sys

import bpy
from mathutils import Vector

LEGACY_GLOBAL_SCALE = 0.019


def _argv_after_dash():
    if "--" not in sys.argv:
        raise SystemExit("usage: blender -b -P rebake_meter_fbx.py -- <in_dir> <out_dir> [filter]")
    return sys.argv[sys.argv.index("--") + 1:]


def _roots():
    return [o for o in bpy.context.scene.objects if o.parent is None]


def _probe(armature_hint):
    """World-space sanity values for the report."""
    out = {"roots": {}, "bones": {}}
    for o in _roots():
        out["roots"][o.name] = {
            "type": o.type,
            "scale": [round(v, 6) for v in o.scale],
            "loc": [round(v, 6) for v in o.location],
        }
        if o.type == "ARMATURE":
            for b in o.data.bones:
                if b.name.lower() in ("bip01", "weapon_r_bone", "bip01_pelvis"):
                    head = o.matrix_world @ b.head_local
                    out["bones"][b.name] = [round(v, 6) for v in head]
            meshes = [c for c in o.children if c.type == "MESH"]
            for m in meshes[:1]:
                if m.data.vertices:
                    dims = Vector(m.dimensions)
                    out["mesh_dims"] = [round(v, 6) for v in dims]
    if "bip01" not in out["bones"]:
        for o in bpy.context.scene.objects:
            if o.type == "ARMATURE":
                for b in o.data.bones:
                    if b.name.lower() == "bip01":
                        out["bones"][b.name] = [round(v, 6) for v in (o.matrix_world @ b.head_local)]
    return out


def rebake(src, dst):
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=src, use_anim=False)
    before = _probe(src)

    baked = {}
    for o in _roots():
        if o.type not in ("ARMATURE", "MESH"):
            continue
        if max(abs(o.scale[i] - 1.0) for i in range(3)) <= 1e-6:
            continue
        node = tuple(round(v, 6) for v in o.scale)
        bpy.ops.object.select_all(action="DESELECT")
        o.select_set(True)
        bpy.context.view_layer.objects.active = o
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        factor = tuple(node[i] * LEGACY_GLOBAL_SCALE for i in range(3))
        o.scale = factor
        bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
        baked[o.name] = {"node": list(node), "factor": [round(v, 6) for v in factor]}

    after = _probe(src)
    bpy.ops.export_scene.fbx(
        filepath=dst,
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
        apply_scale_options="FBX_SCALE_ALL",
        path_mode="AUTO",
    )
    return {"src": src, "dst": dst, "baked": baked, "before": before, "after": after}


def autoscale_root(src, dst):
    """No legacy node scale: export as-is with the unit factor, node scale 1."""
    bpy.ops.wm.read_factory_settings(use_empty=True)
    bpy.ops.import_scene.fbx(filepath=src, use_anim=False)
    before = _probe(src)
    applied = {}
    for o in _roots():
        if o.type not in ("ARMATURE", "MESH"):
            continue
        bpy.ops.object.select_all(action="DESELECT")
        o.select_set(True)
        bpy.context.view_layer.objects.active = o
        before_scale = tuple(round(v, 6) for v in o.scale)
        if max(abs(o.scale[i] - 1.0) for i in range(3)) > 1e-6:
            bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
            applied[o.name] = {"node": list(before_scale), "kind": "meter"}
            # Already meter-sized: re-apply the factor so the exported unit stays 100/1.
            o.scale = (1.0, 1.0, 1.0)
            bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    after = _probe(src)
    bpy.ops.export_scene.fbx(
        filepath=dst,
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
        apply_scale_options="FBX_SCALE_ALL",
        path_mode="AUTO",
    )
    return {"src": src, "dst": dst, "baked": applied, "before": before, "after": after,
            "mode": "meter-as-is"}


def main():
    args = _argv_after_dash()
    in_dir, out_dir = args[0], args[1]
    name_filter = args[2].split(",") if len(args) > 2 and args[2] else None
    report_path = args[3] if len(args) > 3 else os.path.join(out_dir, "rebake-report.json")
    os.makedirs(out_dir, exist_ok=True)

    results = []
    names = sorted(f for f in os.listdir(in_dir) if f.lower().endswith(".fbx"))
    if name_filter:
        names = [n for n in names if any(k.lower() in n.lower() for k in name_filter)]
    for name in names:
        src = os.path.join(in_dir, name)
        dst = os.path.join(out_dir, name)
        try:
            res = rebake(src, dst)
        except Exception as exc:  # keep the batch going, report the failure
            res = {"src": src, "dst": dst, "error": repr(exc)}
        results.append(res)
        print("RESULT", json.dumps({k: res.get(k) for k in ("src", "error") if k in res}
                                   | {"baked": list(res.get("baked", {}))}))

    with open(report_path, "w", encoding="utf-8") as fh:
        json.dump(results, fh, indent=1)
    print("WROTE", report_path, len(results))


if __name__ == "__main__":
    main()
