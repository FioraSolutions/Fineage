#!/usr/bin/env python3
"""Build Human Mystic→3rd VFX coverage manifest (alias | generate | skip).

Usage:
  python3 tools/l2-effect/human_mage_vfx_manifest.py \\
    [--out l2-unity/Assets/Editor/L2EffectGenerator/Data/human_mage_vfx_manifest.json]
"""

from __future__ import annotations

import argparse
import json
import re
from collections import defaultdict
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
BACKEND = REPO.parent / "Fineage-Backend"
DEFAULT_CLASS_IDS = {10, 11, 12, 13, 14, 15, 16, 17, 94, 95, 96, 97, 98}
CRAFT_IDS = {1320, 1321, 1322}

EFFECTS_ROOT = REPO / "l2-unity/Assets/Resources/Data/Effects"


def load_json(path: Path):
    return json.loads(path.read_text())


def parse_bound_ids(global_effect: Path) -> dict[int, str]:
    """id -> prefab guid from GlobalEffect.asset."""
    text = global_effect.read_text()
    bound: dict[int, str] = {}
    blocks = re.split(r"\n  - id: ", text)
    for block in blocks[1:]:
        m = re.match(r"(\d+)", block)
        if not m:
            continue
        sid = int(m.group(1))
        g = re.search(r"prefab: \{fileID: [^,]+, guid: ([a-f0-9]+)", block)
        bound[sid] = g.group(1) if g else ""
    return bound


def parse_skillgrp_visual(path: Path) -> dict[int, int]:
    visual: dict[int, int] = {}
    for line in path.read_text(errors="ignore").splitlines():
        m = re.search(r"skill_id=(\d+)", line)
        v = re.search(r"skill_visual_effect=\[([^\]]*)\]", line)
        if not m or not v:
            continue
        raw = v.group(1).strip()
        if raw.isdigit():
            visual[int(m.group(1))] = int(raw)
    return visual


def parse_tsv_classes(path: Path) -> dict[int, list[str]]:
    by_vid: dict[int, list[str]] = defaultdict(list)
    seen: dict[int, set[str]] = defaultdict(set)
    lines = path.read_text().splitlines()
    for line in lines:
        if not line or line.startswith("#"):
            continue
        parts = line.split("\t")
        if len(parts) < 3 or not parts[0].isdigit():
            continue
        vid = int(parts[0])
        cell = parts[2].strip()
        if not cell.startswith("LineageEffect."):
            continue
        name = cell.split(".", 1)[1]
        if name and name not in seen[vid]:
            seen[vid].add(name)
            by_vid[vid].append(name)
    return by_vid


def is_active_candidate(skill: dict) -> bool:
    sid = int(skill["skillId"])
    if sid in CRAFT_IDS:
        return False
    if skill.get("passive"):
        return False
    hit = int(skill.get("hitTimeMs") or 0)
    op = skill.get("operateType") or ""
    if hit == 0 and op not in ("A1", "A2", "A3", "A4", "CA1", "CA5"):
        return False
    return True


def prefer_folder_name(classes: list[str], visual_id: int) -> str:
    """Unique per visual id; optional stem prefix for readability."""
    usable = [c for c in classes if not c.endswith("_sound")]
    if not usable:
        usable = list(classes)
    stem = None
    priority_prefixes = ("el_", "wh_", "bl_", "su_", "m_u", "s_u", "a_u", "mo_", "sp_")
    for prefix in priority_prefixes:
        for c in usable:
            if c.startswith(prefix):
                stem = c
                for suffix in ("_ca", "_cs", "_fl", "_ta", "_pr", "_ra"):
                    if stem.endswith(suffix) and len(stem) > len(suffix) + 2:
                        stem = stem[: -len(suffix)]
                        break
                break
        if stem:
            break
    if stem:
        return f"{stem}_v{visual_id}"
    return f"viz_{visual_id}"


def resolve_alias_composite(
    visual_id: int,
    bound_composite: dict[int, str],
) -> str | None:
    # Same visual id already registered as a GlobalEffect skill row.
    if visual_id in bound_composite:
        return bound_composite[visual_id]
    # Summon / teleport specials: item skills already bound under different ids.
    if visual_id == 1111 and 2046 in bound_composite:
        return bound_composite[2046]
    if visual_id == 2013 and 2013 in bound_composite:
        return bound_composite[2013]
    return None


def build_guid_to_composite(effects_root: Path) -> dict[str, str]:
    mapping: dict[str, str] = {}
    assets_root = REPO / "l2-unity" / "Assets"
    for meta in effects_root.rglob("*_composite.prefab.meta"):
        text = meta.read_text()
        m = re.search(r"guid: ([a-f0-9]+)", text)
        if not m:
            continue
        prefab_path = Path(str(meta)[: -len(".meta")])
        try:
            rel = "Assets/" + prefab_path.relative_to(assets_root).as_posix()
        except ValueError:
            rel = prefab_path.as_posix()
        mapping[m.group(1)] = rel
    return mapping


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--out",
        default=str(
            REPO
            / "l2-unity/Assets/Editor/L2EffectGenerator/Data/human_mage_vfx_manifest.json"
        ),
    )
    parser.add_argument(
        "--classes",
        default="",
        help="Comma-separated class ids. Default is the human mystic line.",
    )
    parser.add_argument("--reference", default="")
    args = parser.parse_args()
    class_ids = (
        {int(x) for x in args.classes.split(",") if x.strip()}
        if args.classes.strip()
        else set(DEFAULT_CLASS_IDS)
    )

    trees = load_json(BACKEND / "data/skill-trees.json")["trees"]
    skills = {
        int(s["skillId"]): s
        for s in load_json(BACKEND / "data/skills.json")["skills"]
    }
    bound = parse_bound_ids(
        REPO / "l2-unity/Assets/Scripts/Database/Effects/GlobalEffect.asset"
    )
    visual = parse_skillgrp_visual(
        REPO / "l2-unity/Assets/StreamingAssets/Data/Meta/Skillgrp_Classic.txt"
    )
    tsv = parse_tsv_classes(
        REPO / "l2-unity/Assets/Editor/L2EffectGenerator/Data/skill-effects.tsv"
    )
    guid_to_composite = build_guid_to_composite(
        REPO / "l2-unity/Assets/Resources/Data/Effects"
    )

    # Also index bound skill id → composite path via GlobalEffect guids
    bound_composite: dict[int, str] = {}
    for sid, guid in bound.items():
        if guid in guid_to_composite:
            bound_composite[sid] = guid_to_composite[guid]

    union: set[int] = set()
    for t in trees:
        if int(t["classId"]) not in class_ids:
            continue
        for row in t["skills"]:
            union.add(int(row["skillId"]))

    missing: list[int] = []
    for sid in sorted(union):
        skill = skills.get(sid)
        if not skill or not is_active_candidate(skill):
            continue
        if sid in bound:
            continue
        missing.append(sid)

    by_visual: dict[int, list[int]] = defaultdict(list)
    for sid in missing:
        vid = visual.get(sid, sid)
        by_visual[vid].append(sid)

    entries = []
    counts = defaultdict(int)
    all_classes: set[str] = set()

    for vid in sorted(by_visual):
        sids = by_visual[vid]
        classes = list(tsv.get(vid, []))
        all_classes.update(classes)
        folder = prefer_folder_name(classes, vid)
        alias_path = resolve_alias_composite(vid, bound_composite)

        if alias_path:
            mode = "alias"
        elif not classes:
            mode = "skip"
        else:
            mode = "generate"

        counts[mode] += 1
        entries.append(
            {
                "visualId": vid,
                "serverSkillIds": sids,
                "skillNames": [skills.get(s, {}).get("name", "?") for s in sids],
                "effectClasses": classes,
                "folderName": folder,
                "effectRoot": f"Assets/Resources/Data/Effects/{folder}",
                "mode": mode,
                "aliasComposite": alias_path,
            }
        )

    out = {
        "reference": args.reference
        or "Human Mystic line VFX gap (classes 10-17,94-98)",
        "classIds": sorted(class_ids),
        "counts": {
            "missingActives": len(missing),
            "uniqueVisuals": len(entries),
            "alias": counts["alias"],
            "generate": counts["generate"],
            "skip": counts["skip"],
            "uniqueEffectClasses": len(all_classes),
        },
        "entries": entries,
        "allEffectClasses": sorted(all_classes),
    }

    out_path = Path(args.out)
    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_text(json.dumps(out, indent=2) + "\n")
    print(json.dumps(out["counts"], indent=2))
    print("wrote", out_path)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
