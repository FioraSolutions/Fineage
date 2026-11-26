#!/usr/bin/env python3
"""Derived skill-effect sound table from skillsoundgrp and the lineageeffect dump.

Does not drop sound references. skillsoundgrp spell/shot/exp slots and every
race voice token are kept, plus every Engine.Sound object referenced from a
LineageEffect actor (SpawnSound and any other Sound object property).
"""
from __future__ import annotations

import json
from collections import Counter, defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "l2-unity/docs/interlude-ref"
DUMP = DOCS / "packages/usk-dump"
SKILLS = DOCS / "skills"
DATS = DOCS / "dats"
SOUND_CLIPS = ROOT / "l2-unity/Assets/Resources/Data/Sounds/SkillSound"
VOICE_CLIPS = ROOT / "l2-unity/Assets/Resources/Data/Sounds/SkillVoice"
GRP_JSON = ROOT / "l2-unity/Assets/Resources/Data/SkillSoundGrp.json"

RACE_SUB = (
    "mfighter_sub", "ffighter_sub", "mdarkelf_sub", "fdarkelf_sub",
    "mdwarf_sub", "fdwarf_sub", "melf_sub", "felf_sub",
    "mmagic_sub", "fmagic_sub", "morc_sub", "forc_sub",
    "mshaman_sub", "fshaman_sub",
)
RACE_THROW = (
    "mfighter_throw", "ffighter_throw", "mdarkelf_throw", "fdarkelf_throw",
    "mdwarf_throw", "fdwarf_throw", "melf_throw", "felf_throw",
    "mmagic_throw", "fmagic_throw", "morc_throw", "forc_throw",
    "mshaman_throw", "fshaman_throw",
)
VOICE_PREFIX = {
    "mfighter": "M_HFighter", "ffighter": "F_HFighter",
    "mdarkelf": "M_DarkElf", "fdarkelf": "F_DarkElf",
    "mdwarf": "M_Dwarf", "fdwarf": "F_Dwarf",
    "melf": "M_Elf", "felf": "F_Elf",
    "mmagic": "M_HMagician", "fmagic": "F_HMagician",
    "morc": "M_Orc_Fighter", "forc": "F_Orc_Fighter",
    "mshaman": "M_Orc_Magician", "fshaman": "F_Orc_Magician",
}

SPELL_PHASE = {
    "spelleffect_sound_1": "casting",
    "spelleffect_sound_2": "shot",
    "spelleffect_sound_3": "explosion",
}
SHOT_SLOTS = ("shoteffect_sound_1", "shoteffect_sound_2", "shoteffect_sound_3")
EXP_SLOTS = ("expeffect_sound_1", "expeffect_sound_2", "expeffect_sound_3")

SOUND_COLUMNS = [
    "skill_id", "skill_level", "visual_key", "object_name", "source", "source_slot",
    "phase", "effect_class", "sound_package", "sound_name", "sound_token",
    "volume", "radius", "looping", "pitch_min", "pitch_max",
    "fineage_plays", "fineage_play_path", "fineage_asset", "fineage_asset_path",
    "notes",
]


def load_json(path: Path):
    return json.loads(path.read_text(encoding="utf-8"))


def split_token(token: str) -> tuple[str, str]:
    token = (token or "").strip()
    if not token:
        return "", ""
    if "." in token:
        pkg, name = token.rsplit(".", 1)
        return pkg, name
    return "", token


def clip_stem(token: str) -> str:
    return split_token(token)[1]


def index_clips(folder: Path) -> dict[str, str]:
    found: dict[str, str] = {}
    if not folder.is_dir():
        return found
    for path in folder.iterdir():
        if not path.is_file() or path.suffix.lower() in (".meta",):
            continue
        if path.suffix.lower() not in (".ogg", ".wav", ".mp3"):
            continue
        rel = f"Data/Sounds/{folder.name}/{path.stem}"
        found[path.stem] = rel
        found[path.stem.lower()] = rel
    return found


def voice_kind(chrsound: str) -> str:
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


def parent_import(imp: dict, by_index: dict) -> dict | None:
    idx = imp.get("package_index") or 0
    if not idx:
        return None
    if idx < 0:
        return by_index.get(-idx - 1)
    return by_index.get(idx)


def token_from_import(imp: dict, by_index: dict) -> tuple[str, str, str]:
    name = imp.get("object_name") or ""
    packages = []
    cur = parent_import(imp, by_index)
    seen = set()
    while cur is not None:
        key = cur.get("import_index")
        if key in seen:
            break
        seen.add(key)
        packages.append(cur.get("object_name") or "")
        cur = parent_import(cur, by_index)
    packages.reverse()
    packages = [p for p in packages if p]
    pkg = ".".join(packages)
    token = ".".join(packages + [name]) if name else pkg
    return pkg, name, token


def walk_sound_refs(node, acc: list):
    if isinstance(node, dict):
        ref = node.get("ref") or ""
        typ = node.get("type")
        cls = node.get("class_name") or node.get("class") or ""
        if typ == "Object" and ref:
            acc.append(node)
        elif "Sound" in str(ref) or cls == "Sound":
            acc.append(node)
        for v in node.values():
            walk_sound_refs(v, acc)
    elif isinstance(node, list):
        for x in node:
            walk_sound_refs(x, acc)


def prop_named(export: dict, name: str):
    for p in export.get("properties") or []:
        if isinstance(p, dict) and p.get("name") == name:
            return p
    return None


def float_named(export: dict, name: str):
    p = prop_named(export, name)
    if p is None or p.get("value") is None:
        return None
    try:
        return float(p["value"])
    except (TypeError, ValueError):
        return None


def bool_named(export: dict, name: str) -> bool | None:
    p = prop_named(export, name)
    if p is None:
        return None
    v = p.get("value")
    if v is True or v == 1 or v == "1" or v == "true":
        return True
    if v is False or v == 0 or v == "0" or v == "false":
        return False
    return None


def load_lineageeffect_exports() -> tuple[list, list]:
    man = DUMP / "lineageeffect.u.manifest.json"
    if man.exists():
        info = load_json(man)
        imports = info.get("imports") or []
        files = [DUMP / p["file"] for p in info["parts"]]
    else:
        data = load_json(DUMP / "lineageeffect.u.json")
        imports = data.get("imports") or []
        files = [DUMP / "lineageeffect.u.json"]
    exports = []
    for path in files:
        data = load_json(path)
        exports.extend(data.get("exports") or [])
    return imports, exports


def collect_effect_spawn_sounds() -> tuple[dict, list]:
    """Map effect class name (lower) -> list of spawn-sound dicts.

    Also returns leftover Sound refs that are not on an effect class default.
    """
    imports, exports = load_lineageeffect_exports()
    by_index = {i["import_index"]: i for i in imports}
    sound_imports = {
        i["object_name"]: i
        for i in imports
        if i.get("class_name") == "Sound"
    }

    def resolve(ref: str) -> tuple[str, str, str] | None:
        if not ref or "Sound" not in ref:
            return None
        name = ref.split(":")[-1]
        imp = sound_imports.get(name)
        if imp is None:
            # try parse import:Engine:Sound:name
            for i in imports:
                if i.get("class_name") == "Sound" and i.get("object_name") == name:
                    imp = i
                    break
        if imp is None:
            return "", name, name
        return token_from_import(imp, by_index)

    class_defaults = {}
    others = []
    for exp in exports:
        src = exp.get("properties_source") or ""
        cn = exp.get("class") or ""
        is_default = src == "class_defaults" or cn in ("", "None", "Class")
        refs = []
        walk_sound_refs(exp.get("properties") or [], refs)
        sounds = []
        seen = set()
        for node in refs:
            ref = node.get("ref") or ""
            resolved = resolve(ref)
            if not resolved:
                continue
            pkg, name, token = resolved
            if not name or "WeatherSoundCheck" in str(node.get("name") or ""):
                continue
            if token in seen:
                continue
            # Skip the Class import named Sound
            if name == "Sound" and pkg.endswith("Engine") or (name == "Sound" and not pkg):
                # only skip the class itself, not a cue named Sound
                if node.get("ref", "").endswith(":Sound") and "Engine:Sound:Sound" not in (node.get("ref") or ""):
                    if name == "Sound" and pkg in ("Engine", "Engine.Sound", ""):
                        continue
            if name == "Sound" and pkg in ("Engine", ""):
                continue
            seen.add(token)
            sounds.append({
                "sound_package": pkg,
                "sound_name": name,
                "sound_token": token,
                "volume": float_named(exp, "SoundVolume"),
                "radius": float_named(exp, "SoundRadius"),
                "looping": bool_named(exp, "SoundLooping"),
                "pitch_min": float_named(exp, "SoundPitchMin"),
                "pitch_max": float_named(exp, "SoundPitchMax"),
                "export_name": exp.get("name") or "",
                "export_class": cn,
                "source_slot": node.get("name") or "SpawnSound",
            })
        if is_default and sounds:
            class_defaults[exp["name"].lower()] = sounds
        elif sounds:
            others.append((exp, sounds))
    return class_defaults, others


def load_skillsoundgrp() -> list[dict]:
    path = DATS / "skillsoundgrp.tsv"
    lines = path.read_text(encoding="utf-8").splitlines()
    header = lines[0].split("\t")
    idx = {c: i for i, c in enumerate(header)}
    rows = []
    for line in lines[1:]:
        if not line:
            continue
        cols = line.split("\t")

        def col(name, default=""):
            i = idx.get(name)
            if i is None or i >= len(cols):
                return default
            return cols[i]

        def fcol(name):
            raw = col(name)
            if raw == "":
                return None
            try:
                return float(raw)
            except ValueError:
                return None

        rec = {h: col(h) for h in header}
        rec["_skill_id"] = int(col("skill_id") or 0)
        rec["_level"] = int(float(col("skill_level") or 0))
        rec["_vol"] = fcol("sound_vol")
        rec["_rad"] = fcol("sound_rad")
        rows.append(rec)
    return rows


def load_fineage_grp() -> dict:
    if not GRP_JSON.exists():
        return {}
    data = load_json(GRP_JSON)
    out = {}
    for e in data.get("entries") or []:
        out[(e.get("skillId"), e.get("level"))] = e
    return out


def fmt(v) -> str:
    if v is None:
        return ""
    if isinstance(v, bool):
        return "1" if v else "0"
    if isinstance(v, float):
        if v == int(v):
            return str(int(v))
        return format(v, ".10g")
    return str(v)


def build_sound_rows(placement_rows: list[dict]) -> tuple[list[dict], dict]:
    sfx_clips = index_clips(SOUND_CLIPS)
    voice_clips = index_clips(VOICE_CLIPS)
    fineage = load_fineage_grp()
    grp_rows = load_skillsoundgrp()
    spawn_by_class, leftover = collect_effect_spawn_sounds()

    visuals_by_skill: dict[int, list[str]] = defaultdict(list)
    effects_by_skill: dict[int, list[dict]] = defaultdict(list)
    for r in placement_rows:
        sid = r["skill_id"]
        key = r["visual_key"]
        if key and key not in visuals_by_skill[sid]:
            visuals_by_skill[sid].append(key)
        effects_by_skill[sid].append(r)

    def sfx_asset(token: str) -> tuple[int, str]:
        stem = clip_stem(token)
        if not stem:
            return 0, ""
        path = sfx_clips.get(stem) or sfx_clips.get(stem.lower())
        return (1, path) if path else (0, "")

    def voice_asset(slot: str, token: str) -> tuple[int, str]:
        race = slot.replace("_sub", "").replace("_throw", "")
        prefix = VOICE_PREFIX.get(race, "")
        kind = voice_kind(token)
        if not prefix or not kind:
            return 0, ""
        name = prefix + "_" + kind
        path = voice_clips.get(name) or voice_clips.get(name.lower())
        return (1, path) if path else (0, "")

    rows: list[dict] = []

    def add(**kwargs):
        rec = {c: kwargs.get(c, "") for c in SOUND_COLUMNS}
        for k, v in kwargs.items():
            rec[k] = v
        rows.append(rec)

    # skillsoundgrp: every non-empty token, every level
    for rec in grp_rows:
        sid = rec["_skill_id"]
        level = rec["_level"]
        visual_keys = visuals_by_skill.get(sid) or [""]
        visual_key = visual_keys[0] if len(visual_keys) == 1 else ";".join(visual_keys)
        fe = fineage.get((sid, level)) or fineage.get((sid, 1))
        vol = rec["_vol"]
        rad = rec["_rad"]

        def slot_vol(base, n):
            raw = rec.get(f"{base}_vol_{n}") or ""
            try:
                return float(raw) if raw not in ("", "0") else vol
            except ValueError:
                return vol

        def slot_rad(base, n):
            raw = rec.get(f"{base}_rad_{n}") or ""
            try:
                return float(raw) if raw not in ("", "0") else rad
            except ValueError:
                return rad

        for slot, phase in SPELL_PHASE.items():
            token = (rec.get(slot) or "").strip()
            if not token:
                continue
            pkg, name = split_token(token)
            n = slot[-1]
            if slot.endswith("_1"):
                path = "SkillCastSound.cast"
            elif slot.endswith("_2"):
                path = "SkillCastSound.shot"
            else:
                path = "SkillCastSound.explode"
            asset, asset_path = sfx_asset(token)
            add(
                skill_id=sid, skill_level=level, visual_key=visual_key, object_name="",
                source="skillsoundgrp", source_slot=slot, phase=phase, effect_class="",
                sound_package=pkg, sound_name=name, sound_token=token,
                volume=slot_vol("spelleffect_sound", n), radius=slot_rad("spelleffect_sound", n),
                looping="", pitch_min="", pitch_max="",
                fineage_plays=1, fineage_play_path=path,
                fineage_asset=asset, fineage_asset_path=asset_path, notes="",
            )

        for slot in SHOT_SLOTS:
            token = (rec.get(slot) or "").strip()
            if not token:
                continue
            pkg, name = split_token(token)
            n = slot[-1]
            asset, asset_path = sfx_asset(token)
            add(
                skill_id=sid, skill_level=level, visual_key=visual_key, object_name="",
                source="skillsoundgrp", source_slot=slot, phase="shot", effect_class="",
                sound_package=pkg, sound_name=name, sound_token=token,
                volume=slot_vol("shoteffect_sound", n), radius=slot_rad("shoteffect_sound", n),
                looping="", pitch_min="", pitch_max="",
                fineage_plays=1, fineage_play_path="SkillCastSound.shot_effect",
                fineage_asset=asset, fineage_asset_path=asset_path,
                notes="",
            )

        for slot in EXP_SLOTS:
            token = (rec.get(slot) or "").strip()
            if not token:
                continue
            pkg, name = split_token(token)
            n = slot[-1]
            asset, asset_path = sfx_asset(token)
            add(
                skill_id=sid, skill_level=level, visual_key=visual_key, object_name="",
                source="skillsoundgrp", source_slot=slot, phase="explosion", effect_class="",
                sound_package=pkg, sound_name=name, sound_token=token,
                volume=slot_vol("expeffect_sound", n), radius=slot_rad("expeffect_sound", n),
                looping="", pitch_min="", pitch_max="",
                fineage_plays=1, fineage_play_path="SkillCastSound.exp_effect",
                fineage_asset=asset, fineage_asset_path=asset_path,
                notes="",
            )

        for slot in RACE_SUB + RACE_THROW:
            token = (rec.get(slot) or "").strip()
            if not token:
                continue
            pkg, name = split_token(token)
            is_throw = slot.endswith("_throw")
            phase = "voice_shot" if is_throw else "voice_cast"
            plays = False
            play_path = ""
            if fe:
                kinds = fe.get("throwKinds") if is_throw else fe.get("kinds")
                race_index = (RACE_THROW if is_throw else RACE_SUB).index(slot)
                if kinds and race_index < len(kinds) and kinds[race_index]:
                    plays = True
                    play_path = "SkillCastSound.voice_throw" if is_throw else "SkillCastSound.voice_sub"
            asset, asset_path = voice_asset(slot, token)
            add(
                skill_id=sid, skill_level=level, visual_key=visual_key, object_name="",
                source="skillsoundgrp", source_slot=slot, phase=phase, effect_class="",
                sound_package=pkg, sound_name=name, sound_token=token,
                volume=vol, radius=rad,
                looping="", pitch_min="", pitch_max="",
                fineage_plays=1 if plays else 0, fineage_play_path=play_path,
                fineage_asset=asset, fineage_asset_path=asset_path,
                notes="hero voice; Fineage plays the mapped voice kind for the caster race, not the raw chrsound cue name",
            )

    # LineageEffect SpawnSound: one row per (skill visual action phase, sound)
    # so a shared effect class is not dropped when used by many skills/phases.
    for r in placement_rows:
        effect = (r.get("effect_class") or "").strip()
        if not effect:
            continue
        key = effect.lower().split(".")[-1]
        sounds = spawn_by_class.get(key) or spawn_by_class.get(effect.lower()) or []
        for s in sounds:
            looping = s.get("looping")
            phase = r["phase"]
            note = "Emitter.SpawnSound on the LineageEffect class defaults"
            if looping and phase == "shot":
                phase = "flight_loop"
                note = "SpawnSound with SoundLooping on a shot-phase effect (flight loop)"
            add(
                skill_id=r["skill_id"], skill_level="", visual_key=r["visual_key"],
                object_name=r["object_name"], source="lineageeffect_spawn_sound",
                source_slot="SpawnSound", phase=phase, effect_class=effect,
                sound_package=s["sound_package"], sound_name=s["sound_name"],
                sound_token=s["sound_token"], volume=s.get("volume"), radius=s.get("radius"),
                looping=looping, pitch_min=s.get("pitch_min"), pitch_max=s.get("pitch_max"),
                fineage_plays=1, fineage_play_path="SkillEffectSpawnSound",
                fineage_asset=sfx_asset(s["sound_token"])[0],
                fineage_asset_path=sfx_asset(s["sound_token"])[1],
                notes=note,
            )

    referenced = set()
    for r in placement_rows:
        effect = (r.get("effect_class") or "").strip()
        if effect:
            referenced.add(effect.lower())
            referenced.add(effect.lower().split(".")[-1])
    for cls, sounds in spawn_by_class.items():
        if cls in referenced:
            continue
        for s in sounds:
            add(
                skill_id=0, skill_level="", visual_key="", object_name="",
                source="lineageeffect_spawn_sound", source_slot="SpawnSound",
                phase="", effect_class=s.get("export_name") or cls,
                sound_package=s["sound_package"], sound_name=s["sound_name"],
                sound_token=s["sound_token"], volume=s.get("volume"), radius=s.get("radius"),
                looping=s.get("looping"), pitch_min=s.get("pitch_min"), pitch_max=s.get("pitch_max"),
                fineage_plays=1, fineage_play_path="SkillEffectSpawnSound",
                fineage_asset=sfx_asset(s["sound_token"])[0],
                fineage_asset_path=sfx_asset(s["sound_token"])[1],
                notes="SpawnSound on a LineageEffect class that no skill.usk LocateEffect references; kept so the dump is lossless. Played generically when the effect prefab is spawned.",
            )

    leftover_rows = []
    for exp, sounds in leftover:
        name = (exp.get("name") or "").lower()
        if name in spawn_by_class or name in referenced:
            continue
        leftover_rows.append((exp, sounds))

    # Don't drop leftover Sound refs: record them with skill_id 0
    for exp, sounds in leftover_rows:
        for s in sounds:
            add(
                skill_id=0, skill_level="", visual_key="", object_name=exp.get("name") or "",
                source="lineageeffect_sound_ref", source_slot=s.get("source_slot") or "Sound",
                phase="", effect_class=exp.get("name") or "",
                sound_package=s["sound_package"], sound_name=s["sound_name"],
                sound_token=s["sound_token"], volume=s.get("volume"), radius=s.get("radius"),
                looping=s.get("looping"), pitch_min=s.get("pitch_min"), pitch_max=s.get("pitch_max"),
                fineage_plays=0, fineage_play_path="",
                fineage_asset=sfx_asset(s["sound_token"])[0],
                fineage_asset_path=sfx_asset(s["sound_token"])[1],
                notes="Sound object ref in lineageeffect.u not reached from a skill.usk EffectClass",
            )

    rows.sort(key=lambda r: (
        r["skill_id"] if r["skill_id"] != "" else -1,
        r["skill_level"] if r["skill_level"] != "" else -1,
        r["phase"] or "",
        r["source_slot"] or "",
        r["sound_token"] or "",
        r["visual_key"] or "",
    ))

    stats = {
        "sound_rows": len(rows),
        "skillsoundgrp_rows": sum(1 for r in rows if r["source"] == "skillsoundgrp"),
        "spawn_sound_rows": sum(1 for r in rows if r["source"] == "lineageeffect_spawn_sound"),
        "leftover_sound_rows": sum(1 for r in rows if r["source"] == "lineageeffect_sound_ref"),
        "effect_classes_with_spawn_sound": len(spawn_by_class),
        "leftover_exports": len(leftover_rows),
    }
    return rows, stats


def write_sounds(rows: list[dict], stats: dict) -> None:
    SKILLS.mkdir(parents=True, exist_ok=True)
    lines = ["\t".join(SOUND_COLUMNS)]
    for r in rows:
        lines.append("\t".join(fmt(r.get(c, "")) for c in SOUND_COLUMNS))
    tsv = SKILLS / "skill-effect-sounds.tsv"
    tsv.write_text("\n".join(lines) + "\n", encoding="utf-8")
    js = {
        "sources": [
            "l2-unity/docs/interlude-ref/dats/skillsoundgrp.tsv",
            "l2-unity/docs/interlude-ref/packages/usk-dump/lineageeffect.u.manifest.json",
            "Engine.u Emitter SpawnSound / SoundRadius / SoundVolume / SoundLooping",
        ],
        "fineage_play_reference": "Assets/Scripts/Audio/SkillCastSound.cs (cast, shot, shoteffect, explode, expeffect with table vol/rad) and SkillEffectSpawnSound.cs",
        "columns": SOUND_COLUMNS,
        "stats": stats,
        "rows": rows,
    }
    (SKILLS / "skill-effect-sounds.json").write_text(
        json.dumps(js, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
    )
    print(f"sound rows={len(rows)} -> {tsv}")


def sound_gap_sections(rows: list[dict]) -> list[dict]:
    def skills(pred) -> set:
        return {r["skill_id"] for r in rows if r["skill_id"] not in ("", 0) and pred(r)}

    def count(pred) -> int:
        return sum(1 for r in rows if pred(r))

    sections = []

    def add(title, pred, meaning):
        ids = skills(pred)
        sections.append({
            "feature": title,
            "actions": count(pred),
            "skills": len(ids),
            "meaning": meaning,
        })

    add(
        "skillsoundgrp shoteffect_* not played",
        lambda r: r["source"] == "skillsoundgrp"
        and r["source_slot"].startswith("shoteffect_sound")
        and not r["fineage_plays"],
        "Official shoteffect_sound_1/2/3 cues. SkillCastSound now plays every non-empty shoteffect slot at release with the table volume and radius.",
    )
    add(
        "skillsoundgrp expeffect_* not played",
        lambda r: r["source"] == "skillsoundgrp"
        and r["source_slot"].startswith("expeffect_sound")
        and not r["fineage_plays"],
        "Official expeffect_sound_1/2/3 cues. SkillCastSound now plays every non-empty expeffect slot at impact with the table volume and radius.",
    )
    add(
        "lineageeffect SpawnSound not played",
        lambda r: r["source"] == "lineageeffect_spawn_sound" and not r["fineage_plays"],
        "Emitter.SpawnSound is wired into CompositeEffectV2 spawn via SkillEffectSpawnSound, using the table volume and radius. Missing clips log package+name.",
    )
    add(
        "SpawnSound flight loop not played",
        lambda r: r["phase"] == "flight_loop" and not r["fineage_plays"],
        "SpawnSound with SoundLooping on a shot-phase effect. None of the Interlude class defaults use a looping flight cue.",
    )
    add(
        "skillsoundgrp spell/shot/explode played but clip missing",
        lambda r: r["source"] == "skillsoundgrp"
        and r["source_slot"].startswith(("spelleffect_sound", "shoteffect_sound", "expeffect_sound"))
        and r["fineage_plays"]
        and not r["fineage_asset"],
        "SkillCastSound tries to play the stem under Resources/Data/Sounds/SkillSound (or the matching package folder) and logs the exact package and name when the clip is absent. The 11 rows are listed below.",
    )
    add(
        "hero voice mapped but clip missing",
        lambda r: r["phase"] in ("voice_cast", "voice_shot")
        and r["fineage_plays"]
        and not r["fineage_asset"],
        "SkillCastSound.PlayVoiceKind will look up Data/Sounds/SkillVoice/{racePrefix}_{kind} and miss.",
    )
    add(
        "skillsoundgrp hero-voice volume/radius unused",
        lambda r: r["source"] == "skillsoundgrp"
        and r["phase"] in ("voice_cast", "voice_shot")
        and r["sound_token"]
        and (r["volume"] not in ("", None) or r["radius"] not in ("", None)),
        "Hero voice still uses SkillCastSound.VoiceVolume=1. Effect cues (cast/shot/flight/impact/exp, including shoteffect, expeffect, and SpawnSound) now use the table volume (vol/255) and radius (UU/52.5 meters).",
    )
    add(
        "SpawnSound clip missing",
        lambda r: r["source"] == "lineageeffect_spawn_sound"
        and r["fineage_plays"]
        and not r["fineage_asset"],
        "SkillEffectSpawnSound logs the exact package and name when the clip is not in Resources. ItemSound2 firework cues are the known leftovers.",
    )
    add(
        "lineageeffect SpawnSound not referenced by skill.usk",
        lambda r: r["source"] == "lineageeffect_spawn_sound" and r["skill_id"] in (0, "", None),
        "These Emitter.SpawnSound cues exist on LineageEffect class defaults (teleport, Baium, Frintezza drops, fireworks, meteor, bubbles) but no skill.usk LocateEffect points at that class. They are still in the sound table so the dump is lossless.",
    )
    return sections


def missing_skill_sound_clips(rows: list[dict]) -> list[dict]:
    """skillsoundgrp effect cues Fineage will try to play but has no clip for."""
    out = []
    seen = set()
    for r in rows:
        if r.get("source") != "skillsoundgrp":
            continue
        slot = r.get("source_slot") or ""
        if not slot.startswith(("spelleffect_sound", "shoteffect_sound", "expeffect_sound")):
            continue
        if not r.get("fineage_plays") or r.get("fineage_asset"):
            continue
        key = (r.get("skill_id"), r.get("skill_level"), slot, r.get("sound_token"))
        if key in seen:
            continue
        seen.add(key)
        out.append(r)
    return out
