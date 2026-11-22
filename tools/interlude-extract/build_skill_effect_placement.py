#!/usr/bin/env python3
"""Build the skill-effect placement table and gap report from the lossless usk dump.

Also shards lineageeffect.u.json into compact parts under 18 MB so the catalog
can be committed, and builds the skill-effect sound table (skillsoundgrp plus
every LineageEffect SpawnSound / Sound object ref). Does not invent fields or
drop sound references.

Usage (after export_package_properties.sh):
  python3 tools/interlude-extract/build_skill_effect_placement.py
"""
from __future__ import annotations

import json
import math
from collections import Counter
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

ROOT = Path(__file__).resolve().parents[2]
DUMP = ROOT / "l2-unity/docs/interlude-ref/packages/usk-dump"
DOCS = ROOT / "l2-unity/docs/interlude-ref"
SKILLS = DOCS / "skills"
README = DOCS / "README.md"
STREAM = ROOT / "l2-unity/Assets/StreamingAssets/Data/Meta/SkillVisual"
MAX_PART = 18 * 1024 * 1024

# Interlude Engine.u SkillAction_LocateEffect.EAttachMethod
ATTACH_RAW = {
    0: "none",
    1: "rh",
    2: "lh",
    3: "bone",
    4: "alias",
    5: "trail",
    6: "rf",
    7: "lf",
}

# Cruma SkillEffects.VISUAL_ATTACH remap of the Interlude enum above
# to Cruma's extended AM_* numbering used by listVisuals().
VISUAL_ATTACH = [0, 2, 3, 7, 8, 9, 101, 102]
ATTACH_VISUAL = {
    0: "none",
    2: "rh",
    3: "lh",
    7: "bone",
    8: "alias",
    9: "trail",
    101: "right_foot",
    102: "left_foot",
}

PHASES = (
    "CastingActions",
    "ChannelingActions",
    "PreshotActions",
    "ShotActions",
    "ExplosionActions",
)
PHASE_NAME = {
    "CastingActions": "casting",
    "ChannelingActions": "channeling",
    "PreshotActions": "preshot",
    "ShotActions": "shot",
    "ExplosionActions": "explosion",
}

# From Engine.u SkillAction_LocateEffect defaultproperties
DEFAULT_RELATIVE_TO_CYLINDER = True


def load_json(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def prop_map(export: dict) -> dict:
    out = {}
    for p in export.get("properties") or []:
        name = p.get("name")
        if name:
            out[name] = p
    return out


def bool_prop(pmap: dict, name: str, default: bool) -> tuple:
    p = pmap.get(name)
    if p is None:
        return default, False
    return bool(p.get("value")), True


def float_prop(pmap: dict, name: str, default=None):
    p = pmap.get(name)
    if p is None:
        return default
    v = p.get("value")
    return float(v) if v is not None else default


def byte_prop(pmap: dict, name: str, default: int = 0) -> int:
    p = pmap.get(name)
    if p is None:
        return default
    v = p.get("value")
    return int(v) if v is not None else default


def name_prop(pmap: dict, name: str) -> str:
    p = pmap.get(name)
    if p is None:
        return ""
    v = p.get("value")
    return "" if v is None or str(v).lower() == "none" else str(v)


def vec_prop(pmap: dict, name: str) -> tuple:
    p = pmap.get(name)
    if p is None:
        return (0.0, 0.0, 0.0)
    v = p.get("vector") or p.get("value")
    if isinstance(v, dict):
        return (float(v.get("x") or 0), float(v.get("y") or 0), float(v.get("z") or 0))
    return (0.0, 0.0, 0.0)


def rot_prop(pmap: dict, name: str):
    p = pmap.get(name)
    if p is None:
        return None
    v = p.get("rotator") or p.get("value")
    if isinstance(v, dict) and any(k in v for k in ("pitch", "yaw", "roll")):
        return (int(v.get("pitch") or 0), int(v.get("yaw") or 0), int(v.get("roll") or 0))
    return None


def object_class(pmap: dict, name: str) -> str:
    p = pmap.get(name)
    if p is None:
        return ""
    ref = p.get("ref") or ""
    parts = ref.split(":")
    if len(parts) >= 4 and parts[0] == "import":
        return parts[-1]
    if p.get("value"):
        return str(p["value"])
    return parts[-1] if parts else ""


def outer_package(export: dict) -> str:
    outer = export.get("outer") or ""
    parts = outer.split(":")
    if parts and len(parts) >= 2 and parts[-2] == "Package":
        return parts[-1]
    return parts[-1] if parts else ""


def fmt_num(v) -> str:
    if v is None:
        return ""
    if isinstance(v, bool):
        return "1" if v else "0"
    if isinstance(v, int):
        return str(v)
    if isinstance(v, float):
        if math.isfinite(v) and v == int(v):
            return str(int(v))
        return f"{v:.6f}".rstrip("0").rstrip(".")
    return str(v)


def shard_lineageeffect() -> None:
    src = DUMP / "lineageeffect.u.json"
    if not src.exists():
        raise SystemExit(f"missing {src}; run tools/interlude-extract/export_package_properties.sh first")
    data = load_json(src)
    if "exports" not in data:
        print("lineageeffect.u.json is already a pointer file; skip shard")
        return
    exports = data["exports"]
    header = {
        "package": data.get("package", "lineageeffect"),
        "source": data.get("source", "system/lineageeffect.u"),
        "tool": data.get("tool"),
        "export_count": data.get("export_count", len(exports)),
        "import_count": data.get("import_count", 0),
        "extract_failures": data.get("extract_failures", 0),
    }
    parts = []
    buf = []
    size = 2
    part_no = 1

    def flush():
        nonlocal buf, size, part_no
        if not buf:
            return
        name = f"lineageeffect.u.part{part_no:02d}.json"
        obj = dict(header)
        obj["part"] = part_no
        obj["exports"] = buf
        path = DUMP / name
        path.write_text(json.dumps(obj, ensure_ascii=False, separators=(",", ":")) + "\n", encoding="utf-8")
        parts.append({"file": name, "exports": len(buf), "bytes": path.stat().st_size})
        part_no += 1
        buf = []
        size = 2

    for exp in exports:
        chunk = json.dumps(exp, ensure_ascii=False, separators=(",", ":"))
        extra = len(chunk) + 1
        if buf and size + extra > MAX_PART:
            flush()
        buf.append(exp)
        size += extra
    flush()

    manifest = {
        "package": header["package"],
        "source": header["source"],
        "tool": header["tool"],
        "export_count": header["export_count"],
        "import_count": header["import_count"],
        "extract_failures": header["extract_failures"],
        "imports": data.get("imports", []),
        "parts": parts,
        "note": "Class exports use ClassDefaults (properties_source=class_defaults). Emitter and other objects use tagged properties at the serial offset. Sound/texture binary payloads are not stored.",
    }
    man_path = DUMP / "lineageeffect.u.manifest.json"
    man_path.write_text(json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    (DUMP / "lineageeffect.u.json").write_text(
        json.dumps(
            {
                "package": header["package"],
                "replaced_by": "lineageeffect.u.manifest.json",
                "parts": [p["file"] for p in parts],
            },
            indent=2,
        )
        + "\n",
        encoding="utf-8",
    )
    print(f"sharded lineageeffect into {len(parts)} parts")
    for p in parts:
        print(f"  {p['file']} exports={p['exports']} bytes={p['bytes']}")


def load_lineageeffect() -> dict:
    by_name = {}
    man = DUMP / "lineageeffect.u.manifest.json"
    if man.exists():
        info = load_json(man)
        files = [DUMP / p["file"] for p in info["parts"]]
    elif (DUMP / "lineageeffect.u.json").exists():
        files = [DUMP / "lineageeffect.u.json"]
    else:
        raise SystemExit("lineageeffect dump missing")
    for path in files:
        data = load_json(path)
        for exp in data.get("exports") or []:
            src = exp.get("properties_source") or ""
            cn = exp.get("class") or ""
            if src == "class_defaults" or cn in ("", "None", "Class"):
                by_name[exp["name"].lower()] = exp
    return by_name


def load_skill() -> dict:
    return load_json(DUMP / "skill.usk.json")


def build_placement():
    skill = load_skill()
    exports = {e["export_index"]: e for e in skill["exports"]}
    classes = load_lineageeffect()

    rows = []
    stats = Counter()
    for exp in skill["exports"]:
        if exp.get("class") != "SkillVisualEffect":
            continue
        object_name = str(exp.get("name") or "")
        leading = ""
        for ch in object_name:
            if ch.isdigit():
                leading += ch
            else:
                break
        if not leading:
            stats["visual_non_numeric_name"] += 1
            skill_id = 0
        else:
            skill_id = int(leading)
            if object_name != leading:
                stats["visual_variant_name"] += 1
        pkg = outer_package(exp)
        pmap = prop_map(exp)
        desc = name_prop(pmap, "Desc")
        flying = float_prop(pmap, "FlyingTime", 0.0) or 0.0
        stats["visuals"] += 1
        any_action = False
        for arr_name in PHASES:
            arr = pmap.get(arr_name)
            subs = (arr or {}).get("subproperties") or []
            for idx, sub in enumerate(subs):
                smap = {p["name"]: p for p in sub if p.get("name")}
                action_ref = (smap.get("Action") or {}).get("ref") or ""
                parts = action_ref.split(":")
                action_exp = None
                if len(parts) >= 3 and parts[0] == "export":
                    try:
                        action_exp = exports[int(parts[2])]
                    except (KeyError, ValueError):
                        action_exp = None
                if action_exp is None:
                    stats["missing_action_export"] += 1
                    continue
                any_action = True
                amap = prop_map(action_exp)
                attach_raw = byte_prop(amap, "AttachOn", 0)
                attach_vis = VISUAL_ATTACH[attach_raw] if attach_raw in range(len(VISUAL_ATTACH)) else -1
                rel, rel_ser = bool_prop(amap, "bRelativeToCylinder", DEFAULT_RELATIVE_TO_CYLINDER)
                abs_, abs_ser = bool_prop(amap, "bAbsolute", False)
                multi, _ = bool_prop(amap, "bOnMultiTarget", False)
                size, _ = bool_prop(amap, "bSizeScale", False)
                on_tgt, _ = bool_prop(amap, "bSpawnOnTarget", False)
                use_rot, _ = bool_prop(amap, "bUseCharacterRotation", False)
                ox, oy, oz = vec_prop(amap, "offset")
                delay = float_prop(amap, "SpawnDelay", 0.0) or 0.0
                bone = name_prop(amap, "AttachBoneName")
                effect = object_class(amap, "EffectClass")
                stage_p = smap.get("SpecificStage")
                stage = int(stage_p["value"]) if stage_p and stage_p.get("value") is not None else 0

                actor = classes.get(effect.lower()) if effect else None
                ap = prop_map(actor) if actor else {}
                draw = float_prop(ap, "DrawScale")
                rot = rot_prop(ap, "Rotation")
                rot_ps = rot_prop(ap, "RotPerSecond")
                b_rot, b_rot_ser = bool_prop(ap, "bRotEmitter", False) if actor else (False, False)
                b_self, b_self_ser = bool_prop(ap, "bSelfRotation", False) if actor else (False, False)
                b_dir, b_dir_ser = bool_prop(ap, "bDirectional", False) if actor else (False, False)
                speed = float_prop(ap, "Speed")
                acc = float_prop(ap, "AccSpeed")

                if actor is None and effect:
                    stats["effect_class_missing_defaults"] += 1

                rows.append({
                    "skill_id": skill_id,
                    "package": pkg,
                    "object_name": object_name,
                    "visual_key": f"skill.{pkg}.{object_name}" if pkg else object_name,
                    "desc": desc,
                    "flying_time": flying,
                    "phase": PHASE_NAME[arr_name],
                    "action_index": idx,
                    "specific_stage": stage,
                    "action_object": action_exp.get("name") or "",
                    "effect_class": effect,
                    "attach_on_raw": attach_raw,
                    "attach_on_name": ATTACH_RAW.get(attach_raw, str(attach_raw)),
                    "attach_on_visual": attach_vis if attach_vis >= 0 else "",
                    "attach_on_visual_name": ATTACH_VISUAL.get(attach_vis, ""),
                    "attach_bone_name": bone,
                    "offset_x": ox,
                    "offset_y": oy,
                    "offset_z": oz,
                    "spawn_delay": delay,
                    "b_absolute": abs_,
                    "b_absolute_serialized": abs_ser,
                    "b_on_multi_target": multi,
                    "b_relative_to_cylinder": rel,
                    "b_relative_to_cylinder_serialized": rel_ser,
                    "b_size_scale": size,
                    "b_spawn_on_target": on_tgt,
                    "b_use_character_rotation": use_rot,
                    "draw_scale": draw,
                    "rotation_pitch": None if rot is None else rot[0],
                    "rotation_yaw": None if rot is None else rot[1],
                    "rotation_roll": None if rot is None else rot[2],
                    "rot_per_second_pitch": None if rot_ps is None else rot_ps[0],
                    "rot_per_second_yaw": None if rot_ps is None else rot_ps[1],
                    "rot_per_second_roll": None if rot_ps is None else rot_ps[2],
                    "b_rot_emitter": b_rot if b_rot_ser else None,
                    "b_self_rotation": b_self if b_self_ser else None,
                    "b_directional": b_dir if b_dir_ser else None,
                    "speed": speed,
                    "acc_speed": acc,
                    "effect_defaults_present": actor is not None,
                })
        if not any_action:
            stats["visual_with_no_actions"] += 1

    order = list(PHASE_NAME.values())
    rows.sort(key=lambda r: (r["skill_id"], order.index(r["phase"]), r["action_index"]))
    stats["action_rows"] = len(rows)
    stats["unique_skills"] = len({r["skill_id"] for r in rows})
    return rows, stats


COLUMNS = [
    "skill_id", "package", "object_name", "visual_key", "desc", "flying_time", "phase",
    "action_index", "specific_stage", "action_object", "effect_class",
    "attach_on_raw", "attach_on_name", "attach_on_visual", "attach_on_visual_name",
    "attach_bone_name", "offset_x", "offset_y", "offset_z", "spawn_delay",
    "b_absolute", "b_absolute_serialized", "b_on_multi_target",
    "b_relative_to_cylinder", "b_relative_to_cylinder_serialized",
    "b_size_scale", "b_spawn_on_target", "b_use_character_rotation",
    "draw_scale", "rotation_pitch", "rotation_yaw", "rotation_roll",
    "rot_per_second_pitch", "rot_per_second_yaw", "rot_per_second_roll",
    "b_rot_emitter", "b_self_rotation", "b_directional", "speed", "acc_speed",
    "effect_defaults_present",
]


def write_placement(rows) -> None:
    SKILLS.mkdir(parents=True, exist_ok=True)
    lines = ["\t".join(COLUMNS)]
    for r in rows:
        vals = []
        for c in COLUMNS:
            v = r[c]
            if isinstance(v, bool):
                vals.append("1" if v else "0")
            elif isinstance(v, str):
                vals.append(v)
            else:
                vals.append(fmt_num(v))
        lines.append("\t".join(vals))
    tsv = SKILLS / "skill-effect-placement.tsv"
    tsv.write_text("\n".join(lines) + "\n", encoding="utf-8")
    js = {
        "source": "l2-unity/docs/interlude-ref/packages/usk-dump/skill.usk.json",
        "effect_defaults": "l2-unity/docs/interlude-ref/packages/usk-dump/lineageeffect.u.manifest.json",
        "engine_script": "Engine.u SkillAction_LocateEffect / SkillVisualEffect",
        "attach_on_raw_enum": ATTACH_RAW,
        "attach_on_visual_remap": VISUAL_ATTACH,
        "defaults": {"b_relative_to_cylinder": True},
        "columns": COLUMNS,
        "rows": rows,
    }
    (SKILLS / "skill-effect-placement.json").write_text(
        json.dumps(js, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    STREAM.mkdir(parents=True, exist_ok=True)
    (STREAM / "skill-effect-placement.txt").write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"placement rows={len(rows)} -> {tsv}")


def write_gap(rows, stats, sound_rows=None, sound_stats=None) -> None:
    def skills_where(pred):
        return {r["skill_id"] for r in rows if pred(r)}

    def actions_where(pred) -> int:
        return sum(1 for r in rows if pred(r))

    sections = []

    def add(title, pred, meaning):
        ids = skills_where(pred)
        sections.append({
            "feature": title,
            "actions": actions_where(pred),
            "skills": len(ids),
            "meaning": meaning,
        })

    add(
        "b_use_character_rotation",
        lambda r: r["b_use_character_rotation"],
        "Serialized on the LocateEffect. Playback now sets CompositePart.inheritRotation from this flag.",
    )
    add(
        "b_size_scale",
        lambda r: r["b_size_scale"],
        "Serialized on the LocateEffect. Playback applies LineageEffect DrawScale as CompositePart.scale, but still does not scale the effect with the pawn when this flag is set (Notify() is packed).",
    )
    add(
        "attach_on_raw rf/lf (feet)",
        lambda r: r["attach_on_raw"] in (6, 7),
        "Interlude EAttachMethod EAM_RF/EAM_LF. Cruma remaps them to AM_RIGHT_FOOT=101 and AM_LEFT_FOOT=102. TryMapAttach now accepts 101/102 as named bones; it logs attach_on=N without bone when AttachBoneName is empty. Interlude skill.usk has zero such rows.",
    )
    add(
        "b_relative_to_cylinder effective true",
        lambda r: r["b_relative_to_cylinder"],
        "Default in Engine.u is true. Playback now treats relative offsets with max |component| <= 5 as cylinder units from the attach point (Unity X = UE Y * radius, Unity Y = UE Z * half-height, Unity Z = UE X * radius) so (0,0,-1) from CasterCenter lands at the feet. Larger relative offsets convert as UU and are logged. This convention is inferred: the native Notify() formula is in packed Engine.dll and was not dumped.",
    )
    add(
        "b_relative_to_cylinder explicitly false",
        lambda r: r["b_relative_to_cylinder_serialized"] and not r["b_relative_to_cylinder"],
        "These actions overrode the default and want a non-cylinder offset.",
    )
    add(
        "b_absolute",
        lambda r: r["b_absolute"],
        "Serialized on the LocateEffect. The catalog only uses this to force Immediate spawn timing when a delay is also set; it does not change the position/orientation space.",
    )
    add(
        "spawn_delay != 0",
        lambda r: (r["spawn_delay"] or 0) != 0,
        "Maps to CompositePart.spawnDelaySeconds. Already applied when delay > 0.",
    )
    add(
        "specific_stage != 0",
        lambda r: r["specific_stage"] != 0,
        "SkillActionInfo.SpecificStage from SkillVisualEffect. The client has no stage filter.",
    )
    add(
        "channeling phase",
        lambda r: r["phase"] == "channeling",
        "ChannelingActions exist in Interlude skill.usk (a few skills). The catalog has a CHANNELING phase but playback does not keep the effect for the channel duration.",
    )
    add(
        "draw_scale present on effect class",
        lambda r: r["draw_scale"] is not None,
        "LineageEffect class default DrawScale (for example Wind Strike ca=0.25, pr/ta=0.1). Playback now uses it as CompositePart.scale when the value is greater than 0.",
    )
    add(
        "Rotation present on effect class",
        lambda r: r["rotation_pitch"] is not None and (r["rotation_pitch"] or r["rotation_yaw"] or r["rotation_roll"]),
        "LineageEffect default Rotation (Unreal rotator units). Playback now applies it as CompositePart.spawnEuler via VectorUtils.ConvertRotToUnity.",
    )
    add(
        "bRotEmitter",
        lambda r: r["b_rot_emitter"] is True,
        "LineageEffect default. Not applied by playback.",
    )
    add(
        "bSelfRotation",
        lambda r: r["b_self_rotation"] is True,
        "LineageEffect default. Not applied by playback.",
    )
    add(
        "bDirectional",
        lambda r: r["b_directional"] is True,
        "LineageEffect default. Not applied by playback.",
    )
    add(
        "projectile Speed/AccSpeed",
        lambda r: r["speed"] is not None or r["acc_speed"] is not None,
        "LineageEffect projectile defaults (Wind Strike and Prominence flying shots use Speed=1000, AccSpeed=3000). Playback uses distance/FlyingTime when FlyingTime > 0, otherwise Speed converted from UU; AccSpeed is still not applied.",
    )
    add(
        "effect class defaults missing",
        lambda r: not r["effect_defaults_present"],
        "The LocateEffect.EffectClass was not found in the lineageeffect.u class-default dump.",
    )
    add(
        "attach bone specified",
        lambda r: bool(r["attach_bone_name"]),
        "AttachBoneName is used when attach_on is bone or alias. Already parsed into the catalog Bone field.",
    )
    add(
        "RotPerSecond present",
        lambda r: r["rot_per_second_pitch"] is not None,
        "LineageEffect default RotPerSecond. Not applied by playback.",
    )

    highlights = {}
    for sid, label in (
        (1177, "Wind Strike"),
        (1230, "Prominence"),
        (1201, "Dryad Root (skill.sp.1201, used by Vortex 1339-1343 via skillgrp desc)"),
    ):
        highlights[label] = [r for r in rows if r["skill_id"] == sid]

    text = []
    text.append("# Skill-effect placement gap report")
    text.append("")
    text.append("Built from the lossless skill.usk / lineageeffect.u dump and compared to the")
    text.append("client resolver (SkillVisualCatalog + SkillVisualPlayback) as of this change.")
    text.append("Field names are the original Unreal properties, not invented aliases.")
    text.append("")
    text.append(f"- SkillVisualEffect entries with actions: {stats.get('unique_skills', 0)}")
    text.append(f"- Action rows (LocateEffect): {stats.get('action_rows', 0)}")
    text.append("")
    text.append("## Features the original data has that the client does not fully use")
    text.append("")
    text.append("| Feature | Actions | Skills | Notes |")
    text.append("|---|---:|---:|---|")
    for s in sections:
        note = s["meaning"].replace("|", "\\|")
        text.append(f"| `{s['feature']}` | {s['actions']} | {s['skills']} | {note} |")
    text.append("")
    text.append("## Fields confirmed absent from Interlude skill.usk actions")
    text.append("")
    text.append("Cruma's SkillEffects.Action exposes scale, turn, curve, forceLife,")
    text.append("adjustLife, chaining and channeling as Action fields. A full pass over the")
    text.append("243 Interlude listVisuals() entries found no non-default values for those")
    text.append("fields. They are not properties of SkillAction_LocateEffect in Engine.u and")
    text.append("are not written into this placement table. Duration and loop flags also do not")
    text.append("exist on the LocateEffect; lifetime lives on the emitter (AutoDestroy, emitter")
    text.append("Lifetime ranges) inside lineageeffect.u.")
    text.append("")
    text.append("PreshotActions is declared on SkillVisualEffect but has zero rows in this")
    text.append("client. UnionTarget is a Cruma phase that Interlude's SkillVisualEffect does")
    text.append("not have.")
    text.append("")
    text.append("## Wind Strike, Prominence, Vortex")
    text.append("")
    for label, rs in highlights.items():
        text.append(f"### {label}")
        text.append("")
        if not rs:
            text.append("No skill.usk entry.")
            text.append("")
            continue
        text.append("| phase | effect | attach raw | offset | flags | actor |")
        text.append("|---|---|---|---|---|---|")
        for r in rs:
            flags = []
            if r["b_relative_to_cylinder"]:
                flags.append("rel_cyl")
            if r["b_spawn_on_target"]:
                flags.append("on_target")
            if r["b_on_multi_target"]:
                flags.append("multi")
            if r["b_use_character_rotation"]:
                flags.append("use_char_rot")
            if r["b_size_scale"]:
                flags.append("size_scale")
            if r["spawn_delay"]:
                flags.append(f"delay={r['spawn_delay']}")
            actor = []
            if r["draw_scale"] is not None:
                actor.append(f"DrawScale={r['draw_scale']}")
            if r["speed"] is not None:
                actor.append(f"Speed={r['speed']}")
            if r["acc_speed"] is not None:
                actor.append(f"AccSpeed={r['acc_speed']}")
            if r["b_rot_emitter"]:
                actor.append("bRotEmitter")
            if r["b_directional"]:
                actor.append("bDirectional")
            text.append(
                f"| {r['phase']} | `{r['effect_class']}` | {r['attach_on_raw']} `{r['attach_on_name']}` | "
                f"({fmt_num(r['offset_x'])}, {fmt_num(r['offset_y'])}, {fmt_num(r['offset_z'])}) | "
                f"{' '.join(flags) or '—'} | {', '.join(actor) or '—'} |"
            )
        text.append("")
    text.append("Vortex skills 1339-1343 are not in skill.usk. Their skillgrp desc is")
    text.append("skill.sp.1201 (Dryad Root), so they share that visual: a casting effect at")
    text.append("offset (0,0,-1) with bRelativeToCylinder and an on-target shot/impact.")
    text.append("")

    if sound_rows:
        from build_skill_effect_sounds import sound_gap_sections
        sound_sections = sound_gap_sections(sound_rows)
        text.append("## Skill sounds")
        text.append("")
        text.append("Every skillsoundgrp token (spell, shot, explosion, and hero voice) and every")
        text.append("LineageEffect SpawnSound / Sound object ref is in")
        text.append("`skills/skill-effect-sounds.tsv` (and `.json`). Nothing is dropped.")
        text.append("skillsoundgrp `spelleffect_sound_1/2/3` are the Interlude cast / launch /")
        text.append("impact cues (Wind Strike: `SkillSound.wind_strike_cast` / `_shot` /")
        text.append("`_explotion`). `shoteffect_*` and `expeffect_*` are additional official")
        text.append("slots. Hero voice is the `*_sub` (cast) and `*_throw` (release) columns.")
        text.append("World, ambient and footstep audio is tracked separately in Fineage #1670")
        text.append("and is not part of this table.")
        text.append("")
        if sound_stats:
            text.append(f"- Sound rows: {sound_stats.get('sound_rows', 0)}")
            text.append(f"- From skillsoundgrp: {sound_stats.get('skillsoundgrp_rows', 0)}")
            text.append(f"- From LineageEffect SpawnSound: {sound_stats.get('spawn_sound_rows', 0)}")
            text.append(f"- Other lineageeffect Sound refs not reached from skill.usk: {sound_stats.get('leftover_sound_rows', 0)}")
            text.append("")
        text.append("| Feature | Rows | Skills | Notes |")
        text.append("|---|---:|---:|---|")
        for s in sound_sections:
            note = s["meaning"].replace("|", "\\|")
            text.append(f"| `{s['feature']}` | {s['actions']} | {s['skills']} | {note} |")
        text.append("")
        from build_skill_effect_sounds import missing_skill_sound_clips
        missing_clips = missing_skill_sound_clips(sound_rows)
        text.append("### Missing skill-sound clips")
        text.append("")
        text.append("SkillCastSound logs `package` and `name` when a table cue has no clip.")
        text.append(f"These {len(missing_clips)} skillsoundgrp effect rows are played from the")
        text.append("table but have no imported .ogg/.wav:")
        text.append("")
        text.append("| skill_id | level | slot | package | name |")
        text.append("|---:|---:|---|---|---|")
        for r in missing_clips:
            text.append(
                f"| {r['skill_id']} | {r['skill_level']} | `{r['source_slot']}` | "
                f"`{r['sound_package']}` | `{r['sound_name']}` |"
            )
        text.append("")
        text.append("### Wind Strike, Prominence, Vortex sounds")
        text.append("")
        for sid, label in (
            (1177, "Wind Strike"),
            (1230, "Prominence"),
            (1201, "Dryad Root (Vortex 1339-1343 share the visual, not this sound row)"),
            (1341, "Vortex (1341)"),
        ):
            rs = [r for r in sound_rows if r["skill_id"] == sid and r["skill_level"] in (1, "", 1.0)]
            # keep one level: level 1 or empty (spawn)
            rs = [r for r in sound_rows if r["skill_id"] == sid and (r["skill_level"] in (1, "", None) or r["skill_level"] == 1)]
            text.append(f"#### {label}")
            text.append("")
            if not rs:
                text.append("No sound rows.")
                text.append("")
                continue
            text.append("| phase | slot | token | Fineage plays | asset |")
            text.append("|---|---|---|---|---|")
            # collapse identical voice tokens? No, but maybe only show non-voice plus a voice summary
            shown = 0
            voice_cast = [r for r in rs if r["phase"] == "voice_cast"]
            voice_shot = [r for r in rs if r["phase"] == "voice_shot"]
            for r in rs:
                if r["phase"] in ("voice_cast", "voice_shot"):
                    continue
                plays = "yes" if r["fineage_plays"] in (1, True, "1") else "no"
                asset = "yes" if r["fineage_asset"] in (1, True, "1") else "no"
                text.append(
                    f"| {r['phase']} | `{r['source_slot']}` | `{r['sound_token']}` | {plays} | {asset} |"
                )
                shown += 1
            if voice_cast:
                tok = sorted({r["sound_token"] for r in voice_cast})
                plays = "yes" if any(r["fineage_plays"] in (1, True, "1") for r in voice_cast) else "no"
                text.append(f"| voice_cast | `*_sub` | {len(voice_cast)} race slots ({', '.join('`'+t+'`' for t in tok)}) | {plays} | — |")
            if voice_shot:
                tok = sorted({r["sound_token"] for r in voice_shot})
                plays = "yes" if any(r["fineage_plays"] in (1, True, "1") for r in voice_shot) else "no"
                text.append(f"| voice_shot | `*_throw` | {len(voice_shot)} race slots ({', '.join('`'+t+'`' for t in tok)}) | {plays} | — |")
            text.append("")
        text.append("Vortex 1339-1343 have their own skillsoundgrp rows")
        text.append("(`SkillSound5.clear_vision_cast` / `_shot`), not Dryad Root's sounds,")
        text.append("even though they reuse the skill.sp.1201 visual.")
        text.append("")

    text.append("## How to regenerate")
    text.append("")
    text.append("```bash")
    text.append("tools/interlude-extract/export_package_properties.sh")
    text.append("python3 tools/interlude-extract/build_skill_effect_placement.py")
    text.append("```")
    text.append("")
    (SKILLS / "gap-report.md").write_text("\n".join(text) + "\n", encoding="utf-8")
    print(f"gap report -> {SKILLS / 'gap-report.md'}")
    for s in sections:
        print(f"  {s['feature']}: actions={s['actions']} skills={s['skills']}")


def main() -> None:
    DUMP.mkdir(parents=True, exist_ok=True)
    src = DUMP / "lineageeffect.u.json"
    if src.exists():
        try:
            data = load_json(src)
        except Exception:
            data = {}
        if "exports" in data:
            shard_lineageeffect()
    rows, stats = build_placement()
    write_placement(rows)
    from build_skill_effect_sounds import build_sound_rows, write_sounds
    sound_rows, sound_stats = build_sound_rows(rows)
    write_sounds(sound_rows, sound_stats)
    write_gap(rows, stats, sound_rows, sound_stats)
    for sid in (1177, 1230, 1201):
        rs = [r for r in rows if r["skill_id"] == sid]
        print(f"skill {sid} actions={len(rs)}")
        for r in rs:
            print(
                f"  {r['phase']} {r['effect_class']} attach={r['attach_on_raw']}/{r['attach_on_name']} "
                f"off=({r['offset_x']},{r['offset_y']},{r['offset_z']}) rel={r['b_relative_to_cylinder']} "
                f"on_tgt={r['b_spawn_on_target']} scale={r['draw_scale']} speed={r['speed']}"
            )
    print("stats", dict(stats))


if __name__ == "__main__":
    main()
