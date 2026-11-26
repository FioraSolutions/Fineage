#!/usr/bin/env python3
"""Write the runtime skill-visual tables and the missing-asset report.

The rules match SkillVisualCatalog. Streaming copies are what the client loads.
The EditMode test fails if they drift from skillgrp.tsv, skill-effects-engine.tsv,
or the imported effect prefabs.
"""
from __future__ import annotations

import os
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref"
EFFECTS = ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "Effects"
STREAM = ROOT / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "SkillVisual"
RESOURCES = ROOT / "l2-unity" / "Assets" / "Resources"


def load_tsv(path: Path) -> list[dict]:
    lines = path.read_text(encoding="utf-8").splitlines()
    header = lines[0].split("\t")
    rows = []
    for line in lines[1:]:
        if not line.strip():
            continue
        fields = line.split("\t")
        rows.append({header[i]: fields[i] if i < len(fields) else "" for i in range(len(header))})
    return rows


def object_name(path: str) -> str:
    if not path:
        return ""
    dot = path.rfind(".")
    return path[dot + 1 :] if dot >= 0 else path


def asset_key(effect: str) -> str:
    return object_name(effect).lower()


def is_flying_shot_shape(row: dict) -> bool:
    if row["on_target"] or row["phase"] != "shot":
        return False
    key = row["asset"]
    return key.endswith("_fl") or key.endswith("_pr") or key.endswith("_fo")


def is_projectile(row: dict, flying: float) -> bool:
    if flying <= 0 or row["on_target"] or row["phase"] != "shot":
        return False
    return is_flying_shot_shape(row)


def map_phase(row: dict, has_projectile: bool) -> str | None:
    phase = row["phase"]
    if phase in ("casting", "cast", "channeling", "channel", "preshot", "shot", "explosion", "hit"):
        return None
    return "phase:" + phase + ":" + row["asset"]


def map_attach(row: dict, kind_impact_projectile: bool) -> str | None:
    if kind_impact_projectile:
        return None
    attach = row["attach_on"]
    if attach in (0, 1, 2, 3, 9):
        return None
    if attach in (7, 8):
        if not row["bone"]:
            return f"attach_on={attach} without bone:" + row["asset"]
        return None
    return f"attach_on={attach}:" + row["asset"]


def kind_of(row: dict, has_projectile: bool) -> str:
    phase = row["phase"]
    if phase == "shot" and is_projectile(row, row["flying"]):
        return "projectile"
    if phase == "shot" and row["on_target"]:
        return "impact"
    if phase in ("explosion", "hit"):
        return "impact"
    return "stationary"


def build() -> None:
    grp = load_tsv(DOCS / "dats" / "skillgrp.tsv")
    # SkillVisualCatalogEditModeTests passes skill-effect-placement.tsv as the usk source.
    usk_rows_raw = load_tsv(DOCS / "skills" / "skill-effect-placement.tsv")
    desc: dict[int, dict[int, str]] = {}
    for row in grp:
        sid = int(row["skill_id"])
        level = int(row["skill_level"])
        desc.setdefault(sid, {})[level] = row["desc"].strip()

    by_class: dict[str, list[dict]] = {}
    by_object: dict[str, str] = {}
    by_skill: dict[int, list[dict]] = {}
    names_by_skill: dict[int, set[str]] = {}
    for raw in usk_rows_raw:
        effect = (raw.get("effect_class") or "").strip()
        visual_key = (raw.get("visual_key") or "").strip()
        if not visual_key:
            continue
        row = {
            "skill_id": int(raw["skill_id"]),
            "class_name": visual_key,
            "effect_class": effect,
            "flying": float(raw.get("flying_time") or 0),
            "phase": (raw.get("phase") or "").strip().lower(),
            "asset": asset_key(effect) if effect else "",
            "attach_on": int(raw.get("attach_on_visual") or 0),
            "bone": (raw.get("attach_bone_name") or "").strip(),
            "on_target": (raw.get("b_spawn_on_target") or "").strip() in ("1", "true"),
        }
        by_class.setdefault(row["class_name"].lower(), []).append(row)
        obj = object_name(row["class_name"]).lower()
        prev = by_object.get(obj)
        if prev is None:
            by_object[obj] = row["class_name"]
        elif prev.lower() != row["class_name"].lower():
            by_object[obj] = ""
        by_skill.setdefault(row["skill_id"], []).append(row)
        names_by_skill.setdefault(row["skill_id"], set()).add(row["class_name"].lower())

    prefabs: dict[str, list[str]] = {}
    for dirpath, _, files in os.walk(EFFECTS):
        for name in files:
            if not name.endswith(".prefab"):
                continue
            full = Path(dirpath) / name
            key = name[:-7].lower()
            rel = full.relative_to(RESOURCES).as_posix()[: -len(".prefab")]
            prefabs.setdefault(key, []).append(rel)
    for paths in prefabs.values():
        paths.sort(key=lambda p: (len(p), p))

    def has_asset(key: str) -> bool:
        return key in prefabs

    def match_desc(text: str) -> tuple[str, list[dict]] | None:
        exact = by_class.get(text.lower())
        if exact:
            return exact[0]["class_name"], exact
        obj = object_name(text).lower()
        unique = by_object.get(obj)
        if not unique:
            return None
        rows = by_class.get(unique.lower())
        if not rows:
            return None
        return rows[0]["class_name"], rows

    def single_usk(sid: int) -> tuple[str, list[dict]] | None:
        names = names_by_skill.get(sid)
        if not names or len(names) != 1:
            return None
        rows = by_skill.get(sid) or []
        if not rows:
            return None
        return rows[0]["class_name"], rows

    missing_lines: list[str] = []
    none_ids: set[int] = set()
    ready = 0
    missing_ids: set[int] = set()

    def parse_skill_ref(desc_text: str) -> int | None:
        if not desc_text.lower().startswith("skill."):
            return None
        try:
            sid = int(object_name(desc_text))
            return sid if sid > 0 else None
        except ValueError:
            return None

    def choose_follow_level(follow_id: int, requested: int) -> int:
        levels_map = desc.get(follow_id)
        if not levels_map:
            return requested
        if requested in levels_map:
            return requested
        return sorted(levels_map)[0]

    def consider(sid: int, level: int, source: str, visual: str, rows: list[dict] | None, extra: list[str]) -> str:
        result = evaluate(sid, level, source, visual, rows, extra)
        record(sid, level, result)
        return result[0]

    def evaluate(
        sid: int,
        level: int,
        source: str,
        visual: str,
        rows: list[dict] | None,
        extra: list[str],
    ) -> tuple[str, str, str, list[str]]:
        if extra:
            return "missing", source, visual, list(extra)
        assert rows is not None
        flying = rows[0]["flying"]
        problems: list[str] = []
        for row in rows:
            if is_flying_shot_shape(row) and row["flying"] <= 0:
                problems.append("phase:shot missing field=FlyingTime effect=" + row["asset"])
        projectile = any(is_projectile(r, flying) for r in rows)
        for row in rows:
            phase_err = map_phase(row, projectile)
            if phase_err:
                problems.append(phase_err)
                continue
            kind = kind_of(row, projectile)
            attach_err = map_attach(row, kind == "impact" and projectile)
            if attach_err:
                problems.append(attach_err)
                continue
            if not row["effect_class"]:
                problems.append("phase:" + row["phase"] + " missing field=EffectClass")
                continue
            if is_flying_shot_shape(row) and row["flying"] <= 0:
                continue
            if not has_asset(row["asset"]):
                problems.append(row["asset"])
        if problems:
            return "missing", source, visual, problems
        return "ready", source, visual, []

    def record(sid: int, level: int, result: tuple[str, str, str, list[str]]) -> None:
        status, source, visual, problems = result
        if status != "missing":
            return
        for item in problems:
            missing_lines.append(f"{sid}\t{level}\t{source}\t{visual}\t{item}")

    def resolve_status(sid: int, level: int, visited: set[int] | None, emit: bool = True) -> str:
        visited = visited or set()
        levels_map = desc.get(sid)
        if not levels_map:
            single = single_usk(sid)
            names = names_by_skill.get(sid) or set()
            if single:
                return consider(sid, level, "skill.usk", single[0], single[1], [])
            if len(names) > 1:
                originals = []
                seen = set()
                for row in by_skill[sid]:
                    if row["class_name"].lower() not in seen:
                        seen.add(row["class_name"].lower())
                        originals.append(row["class_name"])
                originals.sort()
                return consider(sid, level, "skill.usk", "ambiguous", None, originals)
            return "none"
        if level not in levels_map:
            result = evaluate(sid, level, "skillgrp", "skillgrp", None, [f"skillgrp level={level}"])
            if emit:
                record(sid, level, result)
            return result[0]
        text = levels_map[level].strip()
        if text.lower().startswith("skill."):
            matched = match_desc(text)
            if matched is not None:
                result = evaluate(sid, level, "skillgrp.desc", matched[0], matched[1], [])
                if emit:
                    record(sid, level, result)
                return result[0]
            follow = parse_skill_ref(text)
            if follow and follow != sid:
                if sid in visited or follow in visited:
                    result = ("missing", "skillgrp.desc", text, [f"{text} followed skill={follow} cycle"])
                    if emit:
                        record(sid, level, result)
                    return "missing"
                next_vis = set(visited)
                next_vis.add(sid)
                follow_level = choose_follow_level(follow, level)
                inner = resolve_status(follow, follow_level, next_vis, emit=False)
                if inner == "ready":
                    return "ready"
                if inner == "missing":
                    follow_text = desc.get(follow, {}).get(follow_level, "").strip()
                    matched_follow = match_desc(follow_text) if follow_text else None
                    if matched_follow:
                        inner_result = evaluate(
                            sid,
                            level,
                            "skillgrp.desc",
                            matched_follow[0],
                            matched_follow[1],
                            [],
                        )
                    else:
                        inner_result = (
                            "missing",
                            "skillgrp.desc",
                            text,
                            [f"{text} followed skill={follow}"],
                        )
                    if emit:
                        record(sid, level, inner_result)
                    return "missing"
                result = (
                    "missing",
                    "skillgrp.desc",
                    text,
                    [f"{text} followed skill={follow} empty desc"],
                )
                if emit:
                    record(sid, level, result)
                return "missing"
            result = evaluate(sid, level, "skillgrp.desc", text, None, [text])
            if emit:
                record(sid, level, result)
            return result[0]
        single = single_usk(sid)
        names = names_by_skill.get(sid) or set()
        if single:
            return consider(sid, level, "skill.usk", single[0], single[1], [])
        if len(names) > 1:
            originals = []
            seen = set()
            for row in by_skill[sid]:
                if row["class_name"].lower() not in seen:
                    seen.add(row["class_name"].lower())
                    originals.append(row["class_name"])
            originals.sort()
            return consider(sid, level, "skill.usk", "ambiguous", None, originals)
        return "none"

    for sid, levels in desc.items():
        any_ready = False
        any_missing = False
        for level in sorted(levels):
            status = resolve_status(sid, level, set(), emit=True)
            if status == "ready":
                any_ready = True
            elif status == "missing":
                any_missing = True
        if any_missing:
            missing_ids.add(sid)
        elif any_ready:
            ready += 1
        else:
            none_ids.add(sid)

    none_file = {int(r["skill_id"]) for r in load_tsv(DOCS / "skills" / "skills-without-unique-effect.tsv")}
    if none_ids != none_file:
        raise SystemExit(f"none set drifted: derived {len(none_ids)} file {len(none_file)} "
                         f"only derived {sorted(none_ids - none_file)[:10]} only file {sorted(none_file - none_ids)[:10]}")
    if ready + len(missing_ids) != 928 or len(none_ids) != 1702:
        raise SystemExit(f"counts ready={ready} missing={len(missing_ids)} none={len(none_ids)}")

    STREAM.mkdir(parents=True, exist_ok=True)
    (STREAM / "skill-effects-engine.txt").write_text(
        (DOCS / "skill-effects-engine.tsv").read_text(encoding="utf-8").replace("\r\n", "\n"),
        encoding="utf-8",
    )
    desc_lines = ["skill_id\tlevel\tdesc"]
    for row in grp:
        desc_lines.append(f"{row['skill_id']}\t{row['skill_level']}\t{row['desc'].strip()}")
    (STREAM / "skillgrp-desc.txt").write_text("\n".join(desc_lines) + "\n", encoding="utf-8")

    index = ["asset_key\tresources_path"]
    for key in sorted(prefabs):
        index.append(key + "\t" + prefabs[key][0])
    (STREAM / "effect-prefabs.txt").write_text("\n".join(index) + "\n", encoding="utf-8")

    missing_lines.sort()
    report = [
        "# Skills that have a skillgrp desc path or a skill.usk entry whose effect asset is not imported.",
        "# Source: skillgrp.tsv desc, then Animations/skill.usk (skill-effects-engine.tsv).",
        "# Lists only leftovers: effect classes still without a prefab, unresolved skillgrp desc keys, or empty EffectClass rows.",
        "# skill.<group>.<id> desc keys that are not themselves a visual class follow that skill's official visual (cycle-checked).",
        "# 1147 has an empty desc at every level and no usk row, so followers stay listed here.",
        "skill_id\tlevel\tsource\tvisual_key\tmissing_asset",
    ]
    report.extend(missing_lines)
    (DOCS / "skills" / "skill-visual-missing-assets.tsv").write_text("\n".join(report) + "\n", encoding="utf-8")
    print(f"ready {ready} missing_ids {len(missing_ids)} missing_rows {len(missing_lines)} none {len(none_ids)} prefabs {len(prefabs)}")


if __name__ == "__main__":
    build()
