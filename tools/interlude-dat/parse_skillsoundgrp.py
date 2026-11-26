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


def token_parts(raw: str) -> tuple[str, str]:
    raw = (raw or "").strip()
    if not raw:
        return "", ""
    if "." in raw:
        pkg, name = raw.rsplit(".", 1)
        return pkg, name
    return "", raw


def cue(token: str, vol: float | None, rad: float | None) -> dict:
    pkg, name = token_parts(token)
    return {
        "token": token or "",
        "package": pkg,
        "name": name,
        "clip": clip_name(token),
        "vol": float(vol or 0),
        "rad": float(rad or 0),
    }


def cue_list(tokens: list[str], vr: list[float]) -> list[dict]:
    out = []
    for i, token in enumerate(tokens):
        if not token:
            continue
        vol = vr[i * 2] if i * 2 < len(vr) else 0
        rad = vr[i * 2 + 1] if i * 2 + 1 < len(vr) else 0
        out.append(cue(token, vol, rad))
    return out


def unity_entry(r: dict) -> dict:
    return {
        "skillId": r["skill_id"],
        "level": r["level"],
        "cast": r["cast"],
        "shot": r["shot"],
        "explode": r["explode"],
        "castCue": cue(r.get("cast_token") or "", r.get("cast_vol"), r.get("cast_rad")),
        "shotCue": cue(r.get("shot_token") or "", r.get("shot_vol"), r.get("shot_rad")),
        "explodeCue": cue(r.get("explode_token") or "", r.get("explode_vol"), r.get("explode_rad")),
        "shotEffects": cue_list(r.get("shot_effects") or [], r.get("shot_effect_vol_rad") or []),
        "expEffects": cue_list(r.get("exp_effects") or [], r.get("exp_effect_vol_rad") or []),
        "kinds": r["kinds"],
        "throwKinds": r["throwKinds"],
        "vol": r.get("vol") or 0,
        "rad": r.get("rad") or 0,
    }


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
        spell_vr = []
        for _i in range(6):
            v, off = f32(data, off)
            spell_vr.append(v)
        shot = []
        for _i in range(3):
            s, off = uni(data, off)
            shot.append(s)
        shot_vr = []
        for _i in range(6):
            v, off = f32(data, off)
            shot_vr.append(v)
        exp = []
        for _i in range(3):
            s, off = uni(data, off)
            exp.append(s)
        exp_vr = []
        for _i in range(6):
            v, off = f32(data, off)
            exp_vr.append(v)
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
                "cast_token": spell[0],
                "shot_token": spell[1],
                "explode_token": spell[2],
                "cast_vol": spell_vr[0],
                "cast_rad": spell_vr[1],
                "shot_vol": spell_vr[2],
                "shot_rad": spell_vr[3],
                "explode_vol": spell_vr[4],
                "explode_rad": spell_vr[5],
                "shot_effects": shot,
                "shot_effect_vol_rad": shot_vr,
                "exp_effects": exp,
                "exp_effect_vol_rad": exp_vr,
                "kinds": kinds,
                "throwKinds": throw_kinds,
                "vol": vol,
                "rad": rad,
            }
        )
    return rows


def parse_tsv(path: Path) -> list[dict]:
    """Build the same row dicts from the already-decoded skillsoundgrp TSV."""
    lines = path.read_text(encoding="utf-8").splitlines()
    header = lines[0].split("\t")
    idx = {c: i for i, c in enumerate(header)}

    def col(cols, name, default=""):
        i = idx.get(name)
        if i is None or i >= len(cols):
            return default
        return cols[i]

    def fcol(cols, name):
        raw = col(cols, name)
        if raw == "":
            return 0.0
        try:
            return float(raw)
        except ValueError:
            return 0.0

    rows: list[dict] = []
    for line in lines[1:]:
        if not line:
            continue
        cols = line.split("\t")
        spell = [col(cols, n) for n in ("spelleffect_sound_1", "spelleffect_sound_2", "spelleffect_sound_3")]
        shot = [col(cols, n) for n in ("shoteffect_sound_1", "shoteffect_sound_2", "shoteffect_sound_3")]
        exp = [col(cols, n) for n in ("expeffect_sound_1", "expeffect_sound_2", "expeffect_sound_3")]
        spell_vr = [
            fcol(cols, "spelleffect_sound_vol_1"), fcol(cols, "spelleffect_sound_rad_1"),
            fcol(cols, "spelleffect_sound_vol_2"), fcol(cols, "spelleffect_sound_rad_2"),
            fcol(cols, "spelleffect_sound_vol_3"), fcol(cols, "spelleffect_sound_rad_3"),
        ]
        shot_vr = [
            fcol(cols, "shoteffect_sound_vol_1"), fcol(cols, "shoteffect_sound_rad_1"),
            fcol(cols, "shoteffect_sound_vol_2"), fcol(cols, "shoteffect_sound_rad_2"),
            fcol(cols, "shoteffect_sound_vol_3"), fcol(cols, "shoteffect_sound_rad_3"),
        ]
        exp_vr = [
            fcol(cols, "expeffect_sound_vol_1"), fcol(cols, "expeffect_sound_rad_1"),
            fcol(cols, "expeffect_sound_vol_2"), fcol(cols, "expeffect_sound_rad_2"),
            fcol(cols, "expeffect_sound_vol_3"), fcol(cols, "expeffect_sound_rad_3"),
        ]
        sub = [col(cols, n) for n in (
            "mfighter_sub", "ffighter_sub", "mdarkelf_sub", "fdarkelf_sub",
            "mdwarf_sub", "fdwarf_sub", "melf_sub", "felf_sub",
            "mmagic_sub", "fmagic_sub", "morc_sub", "forc_sub",
            "mshaman_sub", "fshaman_sub",
        )]
        throw = [col(cols, n) for n in (
            "mfighter_throw", "ffighter_throw", "mdarkelf_throw", "fdarkelf_throw",
            "mdwarf_throw", "fdwarf_throw", "melf_throw", "felf_throw",
            "mmagic_throw", "fmagic_throw", "morc_throw", "forc_throw",
            "mshaman_throw", "fshaman_throw",
        )]
        rows.append(
            {
                "skill_id": int(col(cols, "skill_id") or 0),
                "level": int(float(col(cols, "skill_level") or 0)),
                "cast": clip_name(spell[0]),
                "shot": clip_name(spell[1]),
                "explode": clip_name(spell[2]),
                "cast_token": spell[0],
                "shot_token": spell[1],
                "explode_token": spell[2],
                "cast_vol": spell_vr[0],
                "cast_rad": spell_vr[1],
                "shot_vol": spell_vr[2],
                "shot_rad": spell_vr[3],
                "explode_vol": spell_vr[4],
                "explode_rad": spell_vr[5],
                "shot_effects": shot,
                "shot_effect_vol_rad": shot_vr,
                "exp_effects": exp,
                "exp_effect_vol_rad": exp_vr,
                "kinds": [voice_kind(s) for s in sub],
                "throwKinds": [voice_kind(s) for s in throw],
                "vol": fcol(cols, "sound_vol"),
                "rad": fcol(cols, "sound_rad"),
            }
        )
    return rows


def build_maps(rows: list[dict]) -> dict:
    by_level: dict[str, dict] = {}
    by_skill: dict[str, dict] = {}
    for r in rows:
        entry = unity_entry(r)
        by_level[f"{r['skill_id']}:{r['level']}"] = entry
        sid = str(r["skill_id"])
        prev = by_skill.get(sid)
        if prev is None or r["level"] < prev.get("_level", 10**9):
            by_skill[sid] = {**entry, "_level": r["level"]}
    for v in by_skill.values():
        v.pop("_level", None)
    entries = [unity_entry(r) for r in rows]
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
    ap.add_argument(
        "--tsv-in",
        type=Path,
        help="Decoded skillsoundgrp TSV (used when --input dat is missing)",
    )
    args = ap.parse_args()

    if args.tsv_in:
        rows = parse_tsv(args.tsv_in)
    elif args.input.exists():
        rows = parse(args.input.read_bytes())
    else:
        default_tsv = (
            Path(__file__).resolve().parents[2]
            / "l2-unity/docs/interlude-ref/dats/skillsoundgrp.tsv"
        )
        if not default_tsv.exists():
            raise SystemExit(f"no skillsoundgrp input: {args.input} or {default_tsv}")
        rows = parse_tsv(default_tsv)
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
