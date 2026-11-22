#!/usr/bin/env python3
"""Cross-check skillgrp animations vs imported SpAtk clips, effects, and rumble sounds.

Outputs Docs/skill-visual-audit/audit-table.tsv (committed summary).
"""
from __future__ import annotations

import os
import re
import sys
from collections import defaultdict
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
UNITY = REPO / "l2-unity"
SKILLGRP = UNITY / "Assets/StreamingAssets/Data/Meta/Skillgrp_Interlude.txt"
ANIM_COMBO = UNITY / "Assets/StreamingAssets/Data/Meta/AnimationCombo_Interlude.txt"
SOURCE_TAKES = REPO / "Docs/skill-visual-audit/source-takes.tsv"
CLIP_ROOT = UNITY / "Assets/Resources/Data/Animations"
EFFECTS_ROOT = UNITY / "Assets/Resources/Data/Effects"
SKILL_EFFECTS = UNITY / "Assets/Editor/L2EffectGenerator/Data/skill-effects.tsv"
OUT_DIR = REPO / "Docs/skill-visual-audit"
OUT_TABLE = OUT_DIR / "audit-table.tsv"
OUT_MISSING = OUT_DIR / "missing-assets.tsv"
DUAL_DOC = REPO / "Docs/MISSING_MFighter_DUAL_SPATK.md"
CATALOG_CLIP = (
    "l2-unity/Assets/Resources/Data/Animations/Fighter/MFighter/Clips/"
    "MFighter_m000_b.ao_{take}.anim"
)

RACES = [
    "MFighter", "FFighter", "MMagic", "FMagic", "MElf", "FElf",
    "MDarkElf", "FDarkElf", "MDwarf", "FDwarf", "MOrc", "FOrc",
    "MShaman", "FShaman",
]

WEAPON_SUFFIXES = ["1HS", "2HS", "dual", "hand", "pole", "bow"]

CLIP_WEAPON_CANON = {
    "dual": "Dual",
    "hand": "Hand",
    "pole": "Pole",
    "bow": "Bow",
}


def parse_skillgrp(path: Path) -> dict[tuple[int, int], dict]:
    rows = {}
    skill_re = re.compile(
        r"skill_id=(\d+)\s+skill_level=(\d+).*?"
        r"animation=\{\[([^\]]*)\]\}.*?"
        r"skill_visual_effect=\[([^\]]*)\].*?"
        r"rumble_self=(-?\d+)",
        re.DOTALL,
    )
    for line in path.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line.startswith("skill_begin"):
            continue
        m = skill_re.search(line)
        if not m:
            continue
        sid, lvl, anim, vis, rumble = m.groups()
        rows[(int(sid), int(lvl))] = {
            "animation": anim.strip(),
            "visual": vis.strip(),
            "rumble_self": int(rumble),
        }
    return rows


def parse_anim_combo(path: Path) -> dict[str, list[str]]:
    combos = {}
    for i, line in enumerate(path.read_text(encoding="utf-8", errors="replace").splitlines()):
        if i == 0 or not line.strip():
            continue
        parts = line.split("\t")
        if len(parts) < 4:
            continue
        tag, a, b, c = parts[0], parts[1], parts[2], parts[3]
        cycles = []
        for cell in (a, b, c):
            cell = cell.strip()
            if not cell or cell.startswith("a,none"):
                continue
            if cell.startswith("a,"):
                cell = cell[2:]
            cell = cell.replace("\\0", "").replace("AnimName", "").strip()
            if cell and cell.lower() != "none":
                cycles.append(cell)
        combos[tag] = cycles
    return combos


def build_clip_index(root: Path) -> set[str]:
    """Keys: 'MFighter:SpAtk18_1HS'"""
    found = set()
    for anim in root.rglob("*.anim"):
        name = anim.stem
        if ".ao_SpAtk" not in name and ".ao_spatk" not in name.lower():
            continue
        if "_MFighter" in name:
            race = "MFighter"
        elif "_FFighter" in name:
            race = "FFighter"
        elif "_MMagic" in name:
            race = "MMagic"
        elif "_FMagic" in name:
            race = "FMagic"
        elif "_MElf" in name:
            race = "MElf"
        elif "_FElf" in name:
            race = "FElf"
        elif "_MDarkElf" in name:
            race = "MDarkElf"
        elif "_FDarkElf" in name:
            race = "FDarkElf"
        elif "_MDwarf" in name:
            race = "MDwarf"
        elif "_FDwarf" in name:
            race = "FDwarf"
        elif "_MOrc" in name:
            race = "MOrc"
        elif "_FOrc" in name:
            race = "FOrc"
        elif "_MShaman" in name:
            race = "MShaman"
        elif "_FShaman" in name:
            race = "FShaman"
        else:
            continue
        m = re.search(r"\.ao_(SpAtk\d+_[A-Za-z0-9]+)_", name)
        if m:
            found.add(f"{race}:{m.group(1)}")
    return found


def clip_in_index(index: set[str], race: str, state: str) -> bool:
    if "_" not in state:
        return False
    stem, weapon = state.rsplit("_", 1)
    weapon_variants = {weapon, CLIP_WEAPON_CANON.get(weapon.lower(), weapon)}
    for w in weapon_variants:
        if f"{race}:{stem}_{w}" in index:
            return True
    return False


def effect_viz_exists(vis_id: str) -> bool:
    if not vis_id or vis_id.lower() == "none":
        return True
    num = re.match(r"(\d+)", vis_id)
    if not num:
        return False
    vid = num.group(1)
    pattern = f"viz_{vid}"
    return any(EFFECTS_ROOT.glob(f"**/{pattern}_composite.prefab")) or any(
        EFFECTS_ROOT.glob(f"**/*_v{vid}*_composite.prefab")
    )


def parse_effect_attach(path: Path) -> dict[int, list[int]]:
    out = defaultdict(list)
    if not path.is_file():
        return out
    for line in path.read_text(encoding="utf-8", errors="replace").splitlines():
        if line.startswith("#") or not line.strip() or line.startswith("skill_id"):
            continue
        cols = line.split("\t")
        if len(cols) < 4:
            continue
        try:
            sid = int(cols[0])
            attach = int(cols[3]) if cols[3].strip() else 0
            out[sid].append(attach)
        except ValueError:
            continue
    return out


ATTACH_LABEL = {
    0: "default",
    1: "Location",
    2: "RH",
    3: "LH",
    9: "Trail/socket",
}


def shipped_take_keys() -> set[str]:
    """race:take, case-insensitive, for every Resources clip under CLIP_ROOT."""
    found: set[str] = set()
    if not CLIP_ROOT.is_dir():
        return found
    race_set = set(RACES)
    for anim in CLIP_ROOT.rglob("*.anim"):
        stem = anim.name
        if stem.endswith(".anim"):
            stem = stem[: -len(".anim")]
        if ".ao_" not in stem:
            continue
        take = stem.split(".ao_", 1)[1]
        race = next((part for part in anim.parts if part in race_set), None)
        if race is None:
            for candidate in RACES:
                if take.lower().endswith("_" + candidate.lower()):
                    race = candidate
                    break
        if race:
            found.add(f"{race}:{take}".lower())
    return found


def refresh_source_takes(shipped: set[str]) -> list[tuple[str, str, str]]:
    """Rewrite the imported column from the clips on disk. Returns (race, take, imported)."""
    if not SOURCE_TAKES.is_file():
        return []
    rows: list[tuple[str, str, str]] = []
    out_lines: list[str] = []
    for line in SOURCE_TAKES.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line or line.startswith("#") or line.startswith("race"):
            out_lines.append(line)
            continue
        parts = line.split("\t")
        if len(parts) < 5:
            out_lines.append(line)
            continue
        race, take = parts[0], parts[1]
        imported = "1" if f"{race}:{take}".lower() in shipped else "0"
        parts[4] = imported
        out_lines.append("\t".join(parts))
        rows.append((race, take, imported))
    SOURCE_TAKES.write_text("\n".join(out_lines) + "\n", encoding="utf-8")
    return rows


def write_dual_doc(rows: list[tuple[str, str, str]]) -> None:
    dual = [
        (race, take, imported)
        for race, take, imported in rows
        if race == "MFighter" and take.startswith("SpAtk") and "_Dual_" in take
    ]
    dual.sort(key=lambda row: row[1])
    missing = [take for _, take, imported in dual if imported == "0"]
    lines = [
        "# MFighter dual SpAtk takes",
        "",
        "Regenerated by `python3 scripts/skill-visual-audit/run_audit.py`.",
        "The imported column in `Docs/skill-visual-audit/source-takes.tsv` is that script,",
        "not a hand edit.",
        "",
        "Runtime load is one path: AnimTween sequence → `InterludeAnimResourceTake.tsv`",
        "(mesh `MFighter_anim`) → `Resources/Data/Animations/Fighter/MFighter/Clips/`",
        "`MFighter_m000_b.ao_<take>`. A miss logs skill id, race, weapon, and that path.",
        "It does not play `SpAtk01_1HS`.",
        "",
    ]
    if not dual:
        lines.append("No MFighter `SpAtk*_Dual_*` rows in `source-takes.tsv`.")
    elif not missing:
        lines.append(
            f"All {len(dual)} MFighter Dual SpAtk takes in `source-takes.tsv` are in the tree "
            "(imported since #1622, `12fbde26a0`). `missing-assets.tsv` does not list them "
            "as `official_take_not_imported`."
        )
        lines.append("")
        lines.append("| take | Resources clip |")
        lines.append("|------|----------------|")
        for _, take, _ in dual:
            lines.append(f"| `{take}` | `{CATALOG_CLIP.format(take=take)}` |")
    else:
        lines.append("These MFighter Dual SpAtk takes are still absent from Resources:")
        lines.append("")
        for take in missing:
            lines.append(f"- `{take}` — expected `{CATALOG_CLIP.format(take=take)}`")
    lines.append("")
    DUAL_DOC.write_text("\n".join(lines), encoding="utf-8")


def load_take_names() -> set[str]:
    if not SOURCE_TAKES.is_file():
        return set()
    out = set()
    for line in SOURCE_TAKES.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line or line.startswith("#") or line.startswith("race"):
            continue
        parts = line.split("\t")
        if len(parts) >= 2:
            out.add(f"{parts[0]}:{parts[1]}")
    return out


def main() -> int:
    shipped = shipped_take_keys()
    dual_rows = refresh_source_takes(shipped)
    write_dual_doc(dual_rows)

    skills = parse_skillgrp(SKILLGRP)
    combos = parse_anim_combo(ANIM_COMBO)
    effect_attach = parse_effect_attach(SKILL_EFFECTS)
    take_names = load_take_names()
    clip_index = build_clip_index(CLIP_ROOT)

    missing_rows = []
    audit_rows = []
    stats = defaultdict(int)
    seen_skill_anim = set()

    for (sid, lvl), meta in sorted(skills.items()):
        tag = meta["animation"]
        if not tag or tag.lower() == "none":
            continue
        cycle = combos.get(tag)
        if not cycle:
            stats["no_combo"] += 1
            continue
        spatk_cycle = [a for a in cycle if a.startswith("SpAtk")]
        if not spatk_cycle:
            continue
        vis = meta["visual"]
        viz_ok = effect_viz_exists(vis)
        vis_num = int(re.search(r"(\d+)", vis).group(1)) if vis and re.search(r"(\d+)", vis) else 0
        attach = effect_attach.get(vis_num, [])
        attach_str = ",".join(ATTACH_LABEL.get(a, str(a)) for a in attach) if attach else "-"

        for anim in spatk_cycle:
            if (sid, anim) in seen_skill_anim:
                continue
            seen_skill_anim.add((sid, anim))
            for race in RACES:
                for weapon in WEAPON_SUFFIXES:
                    state = f"{anim}_{weapon}"
                    has = clip_in_index(clip_index, race, state)
                    canon = state.replace("dual", "Dual")
                    in_takes = f"{race}:{canon}" in take_names or f"{race}:{state}" in take_names
                    resolution = "playable_clip" if has else (
                        "fallback_SpAtk01" if in_takes else "missing_clip"
                    )
                    stats[resolution] += 1
                    audit_rows.append(
                        "\t".join(
                            [
                                str(sid),
                                str(lvl),
                                tag,
                                anim,
                                race,
                                weapon,
                                "1" if has else "0",
                                "1" if viz_ok else "0",
                                str(meta["rumble_self"]),
                                resolution,
                                attach_str,
                            ]
                        )
                    )
                    if not has and in_takes:
                        missing_rows.append(f"{race}\t{canon}\tclip")

    OUT_DIR.mkdir(parents=True, exist_ok=True)
    header = (
        "skill_id\tlevel\tanim_tag\tanim_name\trace\tweapon\t"
        "clip_imported\teffect_prefab\trumble_self\tanim_resolution\tfx_attach"
    )
    OUT_TABLE.write_text(header + "\n" + "\n".join(audit_rows) + "\n", encoding="utf-8")
    if SOURCE_TAKES.is_file():
        for line in SOURCE_TAKES.read_text(encoding="utf-8", errors="replace").splitlines():
            if not line or line.startswith("#") or line.startswith("race"):
                continue
            parts = line.split("\t")
            if len(parts) >= 5 and parts[4].strip() == "0" and "SpAtk" in parts[1]:
                missing_rows.append(f"{parts[0]}\t{parts[1]}\tofficial_take_not_imported")

    OUT_MISSING.write_text(
        "race\tstate\tkind\n" + "\n".join(sorted(set(missing_rows))) + "\n",
        encoding="utf-8",
    )

    summary_path = OUT_DIR / "audit-summary.txt"
    summary_path.write_text(
        "\n".join(f"{k}\t{v}" for k, v in sorted(stats.items())) + "\n",
        encoding="utf-8",
    )
    print(f"Wrote {OUT_TABLE} ({len(audit_rows)} rows)")
    print(f"Wrote {OUT_MISSING} ({len(set(missing_rows))} missing clips)")
    for k, v in sorted(stats.items()):
        print(f"  {k}: {v}")
    return 0


if __name__ == "__main__":
    sys.exit(main())