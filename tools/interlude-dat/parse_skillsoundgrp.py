#!/usr/bin/env python3
"""Parse decoded Interlude skillsoundgrp.dat → JSON/TSV for Fineage.

Schema: ellysh/L2FileEdit DAT_defs/Interlude/skillsoundgrp.ddf
Decode first: open-l2encdec `l2encdec -c decode -p 413 -l skillsoundgrp.dat`
"""
from __future__ import annotations

import argparse
import json
import struct
from pathlib import Path

RACE_SLOTS = (
    "mfighter",
    "ffighter",
    "mdarkelf",
    "fdarkelf",
    "mdwarf",
    "fdwarf",
    "melf",
    "felf",
    "mmagic",
    "fmagic",
    "morc",
    "forc",
    "mshaman",
    "fshaman",
)


def u32(data: bytes, o: int) -> tuple[int, int]:
    return struct.unpack_from("<I", data, o)[0], o + 4


def f32(data: bytes, o: int) -> tuple[float, int]:
    return struct.unpack_from("<f", data, o)[0], o + 4


def uni(data: bytes, o: int) -> tuple[str, int]:
    n, o = u32(data, o)
    if n < 0 or o + n > len(data) or n % 2:
        raise ValueError(f"bad UNICODE length {n} at {o - 4}")
    return data[o : o + n].decode("utf-16le").rstrip("\x00"), o + n


def clip_name(raw: str) -> str:
    """SkillSound4.heal_cast → heal_cast (Resources clip stem)."""
    if not raw:
        return ""
    return raw.split(".", 1)[1] if "." in raw else raw


def voice_kind(chrsound: str) -> str:
    """chrsound.m_hmagician_white → White; chrsound.m_dwarf_type_a → Type_A."""
    if not chrsound.startswith("chrsound."):
        return ""
    parts = chrsound.split(".", 1)[1].split("_")
    if parts and parts[0] in ("m", "f"):
        parts = parts[1:]
    if len(parts) >= 2 and parts[-2] == "type":
        return "Type_" + parts[-1].upper()
    last = parts[-1] if parts else ""
    if not last:
        return ""
    if last == "notarget":
        return "Notarget"
    return last[:1].upper() + last[1:].lower()


def parse(data: bytes) -> list[dict]:
    if len(data) % 2:
        data = data[:-1]
    count, off = u32(data, 0)
    rows: list[dict] = []
    for _ in range(count):
        sid, off = u32(data, off)
        lvl, off = u32(data, off)
        spell = []
        for _i in range(3):
            s, off = uni(data, off)
            spell.append(s)
        for _i in range(6):
            _, off = f32(data, off)
        for _i in range(3):
            _, off = uni(data, off)
        for _i in range(6):
            _, off = f32(data, off)
        for _i in range(3):
            _, off = uni(data, off)
        for _i in range(6):
            _, off = f32(data, off)
        sub = []
        for _i in range(15):
            s, off = uni(data, off)
            sub.append(s)
        throw = []
        for _i in range(15):
            s, off = uni(data, off)
            throw.append(s)
        vol, off = f32(data, off)
        rad, off = f32(data, off)
        # sub = cast-time voice; throw = release/shot-time voice (Wind Strike: Element → Shot)
        kinds = [voice_kind(s) for s in sub[:14]]
        throw_kinds = [voice_kind(s) for s in throw[:14]]
        rows.append(
            {
                "skill_id": sid,
                "level": lvl,
                # spelleffect_sound_1/2/3: cast @ start, shot @ HitTime, explotion @ impact
                "cast": clip_name(spell[0]),
                "shot": clip_name(spell[1]),
                "explode": clip_name(spell[2]),
                "kinds": kinds,
                "throwKinds": throw_kinds,
                "vol": vol,
                "rad": rad,
            }
        )
    return rows


def build_maps(rows: list[dict]) -> dict:
    by_level: dict[str, dict] = {}
    by_skill: dict[str, dict] = {}
    for r in rows:
        entry = {
            "cast": r["cast"],
            "shot": r["shot"],
            "explode": r["explode"],
            "kinds": r["kinds"],
            "throwKinds": r["throwKinds"],
        }
        by_level[f"{r['skill_id']}:{r['level']}"] = entry
        sid = str(r["skill_id"])
        prev = by_skill.get(sid)
        if prev is None or r["level"] < prev["_level"]:
            by_skill[sid] = {**entry, "_level": r["level"]}
    for v in by_skill.values():
        del v["_level"]
    # Flat array for Unity JsonUtility (Dictionary JSON is unsupported).
    entries = []
    for r in rows:
        entries.append(
            {
                "skillId": r["skill_id"],
                "level": r["level"],
                "cast": r["cast"],
                "shot": r["shot"],
                "explode": r["explode"],
                "kinds": r["kinds"],
                "throwKinds": r["throwKinds"],
            }
        )
    return {"bySkill": by_skill, "bySkillLevel": by_level, "entries": entries}


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument(
        "--input",
        type=Path,
        default=Path(__file__).with_name("dec-skillsoundgrp.dat"),
    )
    ap.add_argument(
        "--json-out",
        type=Path,
        default=Path(__file__).with_name("skillsoundgrp.json"),
    )
    ap.add_argument(
        "--unity-out",
        type=Path,
        default=Path(__file__).resolve().parents[2]
        / "l2-unity/Assets/Resources/Data/SkillSoundGrp.json",
    )
    ap.add_argument(
        "--tsv-out",
        type=Path,
        default=Path(__file__).with_name("skillsoundgrp.tsv"),
    )
    args = ap.parse_args()

    rows = parse(args.input.read_bytes())
    maps = build_maps(rows)

    args.json_out.write_text(
        json.dumps(maps, ensure_ascii=False, indent=1), encoding="utf-8"
    )
    args.unity_out.parent.mkdir(parents=True, exist_ok=True)
    # Unity JsonUtility-friendly: only the flat entries array.
    unity = {"entries": maps["entries"]}
    args.unity_out.write_text(
        json.dumps(unity, ensure_ascii=False, separators=(",", ":")), encoding="utf-8"
    )

    with args.tsv_out.open("w", encoding="utf-8") as f:
        f.write(
            "skill_id\tlevel\tcast\tshot\texplode\t"
            + "\t".join(f"sub_{s}" for s in RACE_SLOTS)
            + "\t"
            + "\t".join(f"throw_{s}" for s in RACE_SLOTS)
            + "\n"
        )
        for r in rows:
            f.write(
                f"{r['skill_id']}\t{r['level']}\t{r['cast']}\t{r['shot']}\t{r['explode']}\t"
                + "\t".join(r["kinds"])
                + "\t"
                + "\t".join(r["throwKinds"])
                + "\n"
            )

    print(
        f"parsed {len(rows)} rows → {args.json_out.name}, "
        f"{args.unity_out}, {args.tsv_out.name}; skills={len(maps['bySkill'])}"
    )


if __name__ == "__main__":
    main()
