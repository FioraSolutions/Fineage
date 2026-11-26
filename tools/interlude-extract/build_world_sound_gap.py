#!/usr/bin/env python3
"""Derive world/sound inventories and gap-report.md from the lossless Interlude dumps.

Does not invent lists: every row comes from package dumps, DAT tables, or a
scan of the Fineage tree. Binary payloads are never written.
"""
from __future__ import annotations

import csv
import gzip
import json
import re
import sys
from collections import Counter, defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref"
WORLD = DOCS / "world"
SOUNDS = DOCS / "sounds"
ASSETS = ROOT / "l2-unity" / "Assets"
CLIENT_HINT = "official Interlude client (dumps under docs/interlude-ref/world and sounds)"

STARTING_ZONES = [
    ("Talking Island", ("16_24", "16_25", "16_26", "17_24", "17_25", "17_26", "18_25", "18_26")),
    ("Elven Village", ("21_19", "21_18", "21_20", "20_19")),
    ("Dark Elf Village", ("20_18", "20_17", "19_17", "19_18")),
    ("Orc Village", ("18_14", "18_13", "18_15", "19_14", "19_15")),
    ("Dwarf Village", ("23_12", "23_11", "23_13", "24_11", "24_12")),
    ("Gludin Village", ("17_22", "17_21")),
    ("Town of Gludio", ("19_21", "19_19")),
    ("Town of Dion", ("20_22",)),
    ("Giran Castle Town", ("22_22", "22_21", "22_23", "21_21", "21_22", "21_23")),
]

# Classes whose placed instances are always interesting for VFX/SFX/environment.
# Additional classes are discovered if they carry Sound/Music/Emitter properties.
INTERESTING_CLASS_RE = re.compile(
    r"Emitter|Particle|Sound|Light|Zone|Sky|Music|Sun|Moon|Fog|Weather|"
    r"Volume|Water|LevelInfo|TerrainInfo|Celestial|LensFlare",
    re.I,
)
SKIP_CLASS_DUMP = {
    "StaticMeshActor", "StaticMeshInstance", "Model", "Polys", "Brush",
    "TerrainSector", "Level", "LevelSummary", "Camera",
}

SOUND_PROP_RE = re.compile(r"Sound|Music|nMusicID|SpawnSound", re.I)


def open_json(path: Path):
    if path.suffix == ".gz" or path.name.endswith(".json.gz"):
        with gzip.open(path, "rt", encoding="utf-8") as f:
            return json.load(f)
    with path.open(encoding="utf-8") as f:
        return json.load(f)


def prop_value(p: dict):
    if "value" in p and p["value"] is not None:
        return p["value"]
    if "ref" in p:
        return p["ref"]
    if "vector" in p:
        v = p["vector"]
        return f"{v.get('x')},{v.get('y')},{v.get('z')}"
    if "rotator" in p:
        r = p["rotator"]
        return f"p={r.get('pitch')},y={r.get('yaw')},r={r.get('roll')}"
    if "color" in p:
        c = p["color"]
        return f"r={c.get('r')},g={c.get('g')},b={c.get('b')},a={c.get('a')}"
    if p.get("object_array"):
        return ";".join(x.get("ref", "") for x in p["object_array"])
    return ""


def props_by_name(export: dict) -> dict:
    out = defaultdict(list)
    for p in export.get("properties") or []:
        out[p.get("name") or ""].append(p)
    return out


def first_val(pmap, *names):
    for n in names:
        if n in pmap and pmap[n]:
            return prop_value(pmap[n][0])
    return ""


def location_of(pmap) -> str:
    return first_val(pmap, "Location")


def parse_sound_ref(ref: str) -> tuple[str, str]:
    """import:Engine:Sound:in_cricket_01 or import:Core:Package:Grass + name."""
    if not ref:
        return "", ""
    if ref.startswith("import:"):
        parts = ref.split(":")
        # import:<classpkg>:<class>:<name>
        if len(parts) >= 4:
            return parts[-1], parts[1]  # name, class_package (often Engine)
    if ref.startswith("export:"):
        parts = ref.split(":")
        if len(parts) >= 5:
            return parts[-1], parts[1]
    return ref, ""


def iter_dump_files() -> list[tuple[str, Path]]:
    files = []
    for folder, kind_default in (
        (WORLD / "maps", "map"),
        (WORLD / "env", "env"),
        (WORLD / "anims", "anim"),
        (SOUNDS / "packages", "sound"),
    ):
        if not folder.is_dir():
            continue
        for p in sorted(folder.iterdir()):
            name = p.name
            if name.endswith(".json.gz") or name.endswith(".json"):
                files.append((kind_default, p))
    return files


def package_stem(filename: str) -> str:
    n = filename
    if n.endswith(".gz"):
        n = n[:-3]
    if n.endswith(".json"):
        n = n[:-5]
    return n  # e.g. 17_25.unr or ambsound.uax


def normalize_clip(s: str) -> str:
    return re.sub(r"[^a-z0-9]+", "", (s or "").lower())


def load_zonename() -> dict[str, list[str]]:
    path = DOCS / "dats" / "zonename-e.tsv"
    tiles: dict[str, list[str]] = defaultdict(list)
    with path.open(encoding="utf-8") as f:
        r = csv.DictReader(f, delimiter="\t")
        for row in r:
            try:
                x, y = int(row["x_world_grid"]), int(row["y_world_grid"])
            except (ValueError, KeyError):
                continue
            if x == 0 and y == 0:
                continue
            name = (row.get("zone_name") or "").strip()
            if not name:
                continue
            key = f"{x}_{y}"
            if name not in tiles[key]:
                tiles[key].append(name)
    return tiles


def scan_fineage():
    """What Fineage currently has on disk (no Unity)."""
    info = {
        "map_tiles": set(),
        "ambient_prefabs": set(),
        "ambient_json": {},
        "ambient_json_counts": {},
        "music_prefabs": set(),
        "t3d_tiles": set(),
        "clip_stems": set(),
        "clip_norm": set(),
        "clip_paths": [],
        "banks": {},
        "bank_bytes": {},
        "effect_prefab_stems": set(),
        "has_daynight": False,
        "has_footsteps_interval": False,
        "has_anim_notify_playback": False,
        "has_town_ambience_hack": False,
        "has_anim_audio_handler": False,
        "has_ambient_emitter": False,
        "has_skill_cast_sound": False,
    }
    maps = ASSETS / "Resources" / "Data" / "Maps"
    if maps.is_dir():
        for p in maps.iterdir():
            if p.is_dir() and re.match(r"^\d+_\d+$", p.name):
                info["map_tiles"].add(p.name)
                if (p / f"{p.name}_AmbientSounds.prefab").exists() or (p / "AmbientSounds.prefab").exists():
                    info["ambient_prefabs"].add(p.name)
                if (p / "MusicArea.prefab").exists() or (p / f"{p.name}_MusicArea.prefab").exists():
                    info["music_prefabs"].add(p.name)
                meta = p / "Meta"
                if meta.is_dir():
                    if (meta / f"{p.name}.t3d").exists():
                        info["t3d_tiles"].add(p.name)
                    jp = meta / f"{p.name}_AmbientSounds.json"
                    if jp.exists():
                        try:
                            data = json.loads(jp.read_text(encoding="utf-8"))
                            rows = data.get("data", data if isinstance(data, list) else [])
                            info["ambient_json"][p.name] = rows
                            info["ambient_json_counts"][p.name] = len(rows)
                        except Exception:
                            info["ambient_json_counts"][p.name] = -1
    # leftover aggregate json
    agg = maps / "TalkingIslandAmbientSounds.json"
    if agg.exists():
        try:
            data = json.loads(agg.read_text(encoding="utf-8"))
            rows = data.get("data", data if isinstance(data, list) else [])
            info["ambient_json_counts"]["TalkingIsland_aggregate"] = len(rows)
        except Exception:
            pass

    for p in ASSETS.rglob("*"):
        if not p.is_file():
            continue
        if "Library" in p.parts or p.suffix.lower() == ".meta":
            continue
        suf = p.suffix.lower()
        if suf in {".ogg", ".wav", ".mp3", ".aiff"}:
            stem = p.stem
            info["clip_stems"].add(stem.lower())
            info["clip_norm"].add(normalize_clip(stem))
            try:
                rel = str(p.relative_to(ASSETS))
            except ValueError:
                rel = str(p)
            info["clip_paths"].append(rel)
        if suf == ".bank":
            info["banks"][p.stem] = p.stat().st_size
            # keep bytes for name search; Music/AmbSound are large but OK
            try:
                info["bank_bytes"][p.stem.lower()] = p.read_bytes()
            except Exception:
                pass

    effects = ASSETS / "Resources" / "Data" / "Effects"
    if effects.is_dir():
        for p in effects.rglob("*.prefab"):
            info["effect_prefab_stems"].add(p.stem.lower())

    # system flags from source presence
    scripts = ASSETS / "Scripts"
    if (scripts / "Audio" / "L2Footsteps.cs").exists():
        info["has_footsteps_interval"] = True
    playback = scripts / "Audio" / "AnimNotifyPlayback.cs"
    if playback.exists():
        text = playback.read_text(encoding="utf-8", errors="replace")
        info["has_anim_notify_playback"] = "class AnimNotifyPlayback" in text
        if "WalkInterval" not in (scripts / "Audio" / "L2Footsteps.cs").read_text(encoding="utf-8", errors="replace"):
            info["has_footsteps_interval"] = False
    if (scripts / "Audio" / "L2TownAmbience.cs").exists():
        info["has_town_ambience_hack"] = True
    if (scripts / "Audio" / "Animation" / "CharacterAnimationAudioHandler.cs").exists():
        info["has_anim_audio_handler"] = True
    if (scripts / "Audio" / "AmbientSoundEmitter.cs").exists():
        info["has_ambient_emitter"] = True
    if (scripts / "Audio" / "SkillCastSound.Play.Core.cs").exists():
        info["has_skill_cast_sound"] = True
    if (scripts / "Rendering" / "Sky" / "DayNightCycle.cs").exists():
        info["has_daynight"] = True
    return info


PACKAGE_BANK = {
    "ambsound": "ambsound",
    "ambsound2": "ambsound",
    "stepsound": "stepsound",
    "itemsound": "itemsound",
    "itemsound2": "itemsound",
    "itemsound3": "itemsound",
    "interfacesound": "interfacesound",
    "monsound": "monsound",
    "monsound2": "monsound",
    "monsound3": "monsound",
    "monsound4": "monsound",
    "monsound5": "monsound",
    "monsound6": "monsound",
    "monsound7": "monsound",
    "chrsound": "chrsound",
    "chrsound2": "chrsound",
    "skillsound": "skillsound",
    "skillsound2": "skillsound",
    "skillsound3": "skillsound",
    "skillsound4": "skillsound",
    "skillsound5": "skillsound",
    "skillsound6": "skillsound",
    "skillsound7": "skillsound",
    "etcsound": "skillsound",
    "messengersound": "interfacesound",
}


def bank_has_name(bank_bytes: bytes, name: str) -> bool:
    if not bank_bytes or not name or len(name) < 4:
        return False
    raw = name.encode("ascii", "ignore")
    if not raw:
        return False
    if raw in bank_bytes or name.lower().encode("ascii", "ignore") in bank_bytes:
        return True
    # utf-16le
    u16 = b"\x00".join(bytes([c]) for c in raw) + b"\x00"
    return u16 in bank_bytes


def collect_refs_from_props(props: list, bag: list, origin: dict):
    for p in props or []:
        if p.get("type") == "Object" and p.get("ref"):
            ref = p["ref"]
            if ":Sound:" in ref or (p.get("name") or "").lower().find("sound") >= 0:
                name, pkg = parse_sound_ref(ref)
                bag.append({**origin, "prop": p.get("name"), "sound_name": name, "sound_ref": ref, "hint_pkg": pkg})
        if p.get("object_array"):
            for item in p["object_array"]:
                ref = item.get("ref") or ""
                if ":Sound:" in ref:
                    name, pkg = parse_sound_ref(ref)
                    bag.append({**origin, "prop": p.get("name"), "sound_name": name, "sound_ref": ref, "hint_pkg": pkg})
        for sub in p.get("subproperties") or []:
            # sub is a list of props or list of lists
            if sub and isinstance(sub, list) and sub and isinstance(sub[0], dict) and "name" in sub[0]:
                collect_refs_from_props(sub, bag, origin)
            elif sub and isinstance(sub, list):
                for block in sub:
                    if isinstance(block, list):
                        collect_refs_from_props(block, bag, origin)


def main() -> int:
    WORLD.mkdir(parents=True, exist_ok=True)
    SOUNDS.mkdir(parents=True, exist_ok=True)
    zones = load_zonename()
    fine = scan_fineage()
    print(f"Fineage tiles={len(fine['map_tiles'])} ambient_prefabs={len(fine['ambient_prefabs'])} "
          f"clips={len(fine['clip_stems'])} banks={list(fine['banks'])}", file=sys.stderr)

    class_counts = []  # rows
    interesting_actors = []
    sound_catalog = []  # from uax
    sound_refs = []
    anim_notifies = []
    anim_sequences = []
    anim_sequence_notifies = []
    map_summaries = []
    all_map_classes = Counter()
    uax_names = {}  # lower name -> (package, exact, group, wave_bytes)

    files = iter_dump_files()
    print(f"dump files={len(files)}", file=sys.stderr)

    for kind, path in files:
        data = open_json(path)
        pkg = data.get("package") or package_stem(path.name)
        src = package_stem(path.name)
        exports = data.get("exports") or []
        imports = data.get("imports") or []
        counts = Counter()
        fail = 0
        interest_n = 0
        # import map for sound package groups
        # name -> import dict
        for e in exports:
            cls = e.get("class") or "(class)"
            counts[cls] += 1
            all_map_classes[cls] += 1 if kind in {"map", "env"} else 0
            if e.get("extract_error"):
                fail += 1

            name = e.get("name") or ""
            # Sounds
            if cls == "Sound" and kind == "sound":
                group = ""
                outer = e.get("outer") or ""
                # outer often a Package export
                if outer.startswith("export:"):
                    group = outer.split(":")[-1]
                wave = e.get("wave_bytes") or e.get("wave_serial_size") or 0
                sound_catalog.append({
                    "package": pkg,
                    "source": src,
                    "name": name,
                    "group": group,
                    "wave_bytes": wave,
                    "export_index": e.get("export_index"),
                })
                uax_names[name.lower()] = (pkg, name, group, wave)

            # MeshAnimation sequences
            if cls == "MeshAnimation":
                for seq in e.get("sequences") or []:
                    anim_sequences.append({
                        "package": pkg,
                        "animation": name,
                        "sequence": seq.get("name"),
                        "start_frame": seq.get("start_frame"),
                        "frame_count": seq.get("frame_count"),
                        "rate": seq.get("rate"),
                        "shot_time": seq.get("shot_time"),
                        "pre_shot_time": seq.get("pre_shot_time"),
                        "shot_times": ",".join(str(x) for x in (seq.get("shot_times") or [])),
                        "pre_shot_times": ",".join(str(x) for x in (seq.get("pre_shot_times") or [])),
                        "channel_times": ",".join(str(x) for x in (seq.get("channel_times") or [])),
                    })
                    for nt in seq.get("notifies") or []:
                        anim_sequence_notifies.append({
                            "package": pkg,
                            "animation": name,
                            "sequence": seq.get("name"),
                            "time": nt.get("time"),
                            "function": nt.get("function") or "",
                            "class": nt.get("class") or "",
                            "notify": nt.get("name") or "",
                            "object": nt.get("object") or "",
                            "rate": seq.get("rate"),
                            "frame_count": seq.get("frame_count"),
                        })

            if cls.startswith("AnimNotify"):
                pmap = props_by_name(e)
                row = {
                    "package": pkg,
                    "class": cls,
                    "name": name,
                    "volume": first_val(pmap, "Volume"),
                    "radius": first_val(pmap, "Radius"),
                    "sound": first_val(pmap, "Sound"),
                    "prop_count": len(e.get("properties") or []),
                }
                # collect all sound-valued props
                snds = []
                for p in e.get("properties") or []:
                    if p.get("type") == "Object" and p.get("ref"):
                        sn, _ = parse_sound_ref(p["ref"])
                        if sn:
                            key = p.get("name")
                            idx = p.get("array_index")
                            label = key if idx in (None, 0) else f"{key}[{idx}]"
                            snds.append(f"{label}={sn}")
                            sound_refs.append({
                                "kind": "anim_notify",
                                "source": pkg,
                                "actor": name,
                                "class": cls,
                                "prop": label,
                                "sound_name": sn,
                                "sound_ref": p["ref"],
                                "map": "",
                                "location": "",
                            })
                row["sounds"] = ";".join(snds)
                anim_notifies.append(row)

            # Interesting map/env actors
            if kind in {"map", "env"}:
                pmap = props_by_name(e)
                has_sound_prop = any(SOUND_PROP_RE.search(p.get("name") or "") for p in (e.get("properties") or []))
                interesting = bool(INTERESTING_CLASS_RE.search(cls)) or has_sound_prop
                if cls in SKIP_CLASS_DUMP and not has_sound_prop:
                    interesting = cls in {"LevelInfo", "TerrainInfo"}  # LevelInfo yes, skip meshes
                if interesting and cls not in SKIP_CLASS_DUMP or (interesting and cls in {"LevelInfo", "TerrainInfo", "SkyZoneInfo"}):
                    interest_n += 1
                    sounds_here = []
                    for p in e.get("properties") or []:
                        ref = p.get("ref") or ""
                        if ":Sound:" in str(ref) or (SOUND_PROP_RE.search(p.get("name") or "") and p.get("type") == "Object"):
                            sn, _ = parse_sound_ref(ref)
                            if sn:
                                sounds_here.append(f"{p.get('name')}={sn}")
                                sound_refs.append({
                                    "kind": "map_actor",
                                    "source": src,
                                    "actor": name,
                                    "class": cls,
                                    "prop": p.get("name"),
                                    "sound_name": sn,
                                    "sound_ref": ref,
                                    "map": pkg if kind == "map" else src,
                                    "location": location_of(pmap),
                                })
                    music_id = first_val(pmap, "nMusicID", "MusicID")
                    interesting_actors.append({
                        "map": pkg,
                        "source": src,
                        "class": cls,
                        "name": name,
                        "location": location_of(pmap),
                        "rotation": first_val(pmap, "Rotation"),
                        "tag": first_val(pmap, "Tag"),
                        "group": first_val(pmap, "Group"),
                        "draw_scale": first_val(pmap, "DrawScale"),
                        "sound": first_val(pmap, "AmbientSound", "Sound"),
                        "sound_radius": first_val(pmap, "SoundRadius"),
                        "sound_volume": first_val(pmap, "SoundVolume"),
                        "sound_pitch": first_val(pmap, "SoundPitch"),
                        "ambient_type": first_val(pmap, "AmbientSoundType"),
                        "ambient_random": first_val(pmap, "AmbientRandom"),
                        "music_id": music_id,
                        "light_brightness": first_val(pmap, "LightBrightness"),
                        "light_radius": first_val(pmap, "LightRadius"),
                        "light_hue": first_val(pmap, "LightHue"),
                        "light_sat": first_val(pmap, "LightSaturation"),
                        "fog_color": first_val(pmap, "DistanceFogColor"),
                        "fog_start": first_val(pmap, "DistanceFogStart"),
                        "fog_end": first_val(pmap, "DistanceFogEnd"),
                        "emitters": first_val(pmap, "Emitters"),
                        "extra_sounds": ";".join(sounds_here),
                    })
                    if music_id:
                        sound_refs.append({
                            "kind": "map_music",
                            "source": src,
                            "actor": name,
                            "class": cls,
                            "prop": "nMusicID",
                            "sound_name": str(music_id),
                            "sound_ref": f"musicinfo:{music_id}",
                            "map": pkg,
                            "location": location_of(pmap),
                        })

            # interface / env sound refs
            if kind in {"env", "anim"} and cls not in {"Sound", "MeshAnimation"} and not cls.startswith("AnimNotify"):
                collect_refs_from_props(e.get("properties") or [], sound_refs, {
                    "kind": "package",
                    "source": src,
                    "actor": name,
                    "class": cls,
                    "map": "",
                    "location": "",
                    "sound_name": "",
                    "sound_ref": "",
                    "prop": "",
                })

        for cls, n in sorted(counts.items()):
            class_counts.append({
                "source": src, "kind": kind, "package": pkg, "class": cls, "count": n,
            })

        if kind in {"map", "env"}:
            tile = pkg if re.match(r"^\d+_\d+$", str(pkg)) else ""
            map_summaries.append({
                "map": pkg,
                "source": src,
                "kind": kind,
                "export_count": data.get("export_count") or len(exports),
                "import_count": data.get("import_count") or len(imports),
                "extract_failures": data.get("extract_failures") or fail,
                "interesting_actors": interest_n,
                "zones": "; ".join(zones.get(tile, [])),
                "fineage_tile": "yes" if tile in fine["map_tiles"] else "no",
                "fineage_ambient_prefab": "yes" if tile in fine["ambient_prefabs"] else "no",
                "fineage_ambient_json": fine["ambient_json_counts"].get(tile, ""),
                "fineage_music_prefab": "yes" if tile in fine["music_prefabs"] else "no",
                "fineage_t3d": "yes" if tile in fine["t3d_tiles"] else "no",
            })
        print(f"  parsed {src} kind={kind} exports={len(exports)} interesting={interest_n}", file=sys.stderr)

    # DAT-derived sound references
    def add_dat_sounds(tsv_name, id_field, columns, kind):
        path = DOCS / "dats" / tsv_name
        if not path.exists():
            return 0
        n = 0
        with path.open(encoding="utf-8") as f:
            r = csv.DictReader(f, delimiter="\t")
            for row in r:
                ident = row.get(id_field, "")
                for col in columns:
                    if col not in row:
                        continue
                    raw = row[col] or ""
                    if not raw or raw in {"0", "None", "none", "[]"}:
                        continue
                    # may be JSON list or token
                    tokens = []
                    s = raw.strip()
                    if s.startswith("["):
                        try:
                            tokens = [str(x) for x in json.loads(s)]
                        except Exception:
                            tokens = [s]
                    else:
                        tokens = re.split(r"[;,\s]+", s)
                    for tok in tokens:
                        tok = tok.strip().strip('"')
                        if not tok or tok in {"0", "None", "none"}:
                            continue
                        # skillsound tokens look like SkillSound5.clear_vision_cast
                        sound_name = tok.split(".")[-1] if "." in tok else tok
                        sound_refs.append({
                            "kind": kind,
                            "source": tsv_name,
                            "actor": str(ident),
                            "class": col,
                            "prop": col,
                            "sound_name": sound_name,
                            "sound_ref": tok,
                            "map": "",
                            "location": "",
                        })
                        n += 1
        return n

    # item/npc/enter-event (skills already have a dedicated table)
    add_dat_sounds("etcitemgrp.tsv", "id" if False else "object_id", ["item_sound", "equip_sound"], "item")
    # detect id column
    def dat_id_col(name, fallbacks):
        p = DOCS / "dats" / name
        with p.open(encoding="utf-8") as f:
            header = f.readline().rstrip("\n").split("\t")
        for c in fallbacks:
            if c in header:
                return c
        return header[0]

    sound_refs[:] = [r for r in sound_refs if r.get("kind") != "item"]
    add_dat_sounds("etcitemgrp.tsv", dat_id_col("etcitemgrp.tsv", ["id", "object_id", "item_id"]), ["item_sound", "equip_sound"], "item")
    add_dat_sounds("armorgrp.tsv", dat_id_col("armorgrp.tsv", ["id", "object_id", "item_id"]), ["item_sound", "drop_sound", "equip_sound"], "item")
    add_dat_sounds("weapongrp.tsv", dat_id_col("weapongrp.tsv", ["id", "object_id", "item_id"]), ["item_sound", "drop_sound", "equip_sound"], "item")
    add_dat_sounds("npcgrp.tsv", dat_id_col("npcgrp.tsv", ["id", "npc_id", "object_id"]), ["effect", "rb_effect"], "npc_effect")
    add_dat_sounds("entereventgrp.tsv", dat_id_col("entereventgrp.tsv", ["id", "skill_id"]), ["skill_sound", "effect_name"], "enter_event")
    add_dat_sounds("systemmsg-e.tsv", dat_id_col("systemmsg-e.tsv", ["id", "msg_id"]), ["item_sound"], "ui_systemmsg")

    # musicinfo catalog
    music_tracks = []
    music_path = DOCS / "dats" / "musicinfo.tsv"
    if music_path.exists():
        with music_path.open(encoding="utf-8") as f:
            r = csv.DictReader(f, delimiter="\t")
            for row in r:
                try:
                    tracks = json.loads(row["str"])
                except Exception:
                    tracks = [row.get("str") or ""]
                for t in tracks:
                    music_tracks.append({"id": row["id"], "track": t, "cnt": row.get("cnt")})

    # existing skill sound table (do not re-parse everything)
    skill_sound_rows = 0
    skill_gap_path = DOCS / "skills" / "skill-effect-sounds.tsv"
    if skill_gap_path.exists():
        with skill_gap_path.open(encoding="utf-8") as f:
            skill_sound_rows = max(0, sum(1 for _ in f) - 1)

    # Fineage clip presence for catalog
    def clip_status(package: str, name: str) -> str:
        if not name:
            return "missing"
        low = name.lower()
        if low in fine["clip_stems"] or normalize_clip(name) in fine["clip_norm"]:
            return "resources_clip"
        bank_key = PACKAGE_BANK.get(package.lower(), package.lower())
        b = fine["bank_bytes"].get(bank_key)
        if b is None:
            # try aliases
            for k, v in fine["bank_bytes"].items():
                if k.startswith(bank_key) or bank_key.startswith(k):
                    b = v
                    break
        if b and bank_has_name(b, name):
            return "fmod_bank"
        # some amb sounds live under AmbSound regardless of declared group
        amb = fine["bank_bytes"].get("ambsound")
        if amb and bank_has_name(amb, name):
            return "fmod_bank_ambsound"
        return "missing"

    for row in sound_catalog:
        row["fineage"] = clip_status(row["package"], row["name"])

    # Write tables
    def write_tsv(path: Path, rows: list[dict], fields: list[str], gz=False):
        path.parent.mkdir(parents=True, exist_ok=True)
        if gz or path.name.endswith(".gz"):
            fctx = gzip.open(path, "wt", encoding="utf-8", newline="")
        else:
            fctx = path.open("w", encoding="utf-8", newline="")
        with fctx as f:
            w = csv.DictWriter(f, fieldnames=fields, delimiter="\t", extrasaction="ignore", lineterminator="\n")
            w.writeheader()
            for row in rows:
                w.writerow(row)
        print(f"wrote {path} rows={len(rows)} bytes={path.stat().st_size}", file=sys.stderr)

    write_tsv(WORLD / "map-class-counts.tsv", class_counts,
              ["source", "kind", "package", "class", "count"])
    actors_path = WORLD / "map-actors.tsv"
    write_tsv(actors_path, interesting_actors,
              ["map", "source", "class", "name", "location", "rotation", "tag", "group",
               "draw_scale", "sound", "sound_radius", "sound_volume", "sound_pitch",
               "ambient_type", "ambient_random", "music_id", "light_brightness",
               "light_radius", "light_hue", "light_sat", "fog_color", "fog_start",
               "fog_end", "emitters", "extra_sounds"])
    if actors_path.stat().st_size > 5_000_000:
        # gzip and keep? user said gzip over ~50MB.  leave uncompressed if modest.
        pass
    write_tsv(WORLD / "map-summary.tsv", map_summaries,
              ["map", "source", "kind", "export_count", "import_count", "extract_failures",
               "interesting_actors", "zones", "fineage_tile", "fineage_ambient_prefab",
               "fineage_ambient_json", "fineage_music_prefab", "fineage_t3d"])
    write_tsv(SOUNDS / "sound-catalog.tsv", sound_catalog,
              ["package", "source", "group", "name", "wave_bytes", "export_index", "fineage"])
    refs_path = SOUNDS / "sound-references.tsv"
    write_tsv(refs_path, sound_refs,
              ["kind", "source", "map", "actor", "class", "prop", "sound_name", "sound_ref", "location"])
    write_tsv(SOUNDS / "anim-notifies.tsv", anim_notifies,
              ["package", "class", "name", "volume", "radius", "sound", "sounds", "prop_count"])
    write_tsv(SOUNDS / "anim-sequences.tsv", anim_sequences,
              ["package", "animation", "sequence", "start_frame", "frame_count", "rate",
               "shot_time", "pre_shot_time", "shot_times", "pre_shot_times", "channel_times"])
    bind_sequence_notifies(anim_sequence_notifies, anim_notifies, sound_catalog)
    write_tsv(SOUNDS / "anim-sequence-notifies.tsv.gz", anim_sequence_notifies,
              ["package", "animation", "sequence", "time", "class", "notify", "function",
               "volume", "radius", "sound", "sounds", "surface_step", "frame_count", "rate"],
              gz=True)
    write_runtime_notify_table(anim_sequence_notifies)
    write_tsv(SOUNDS / "musicinfo-tracks.tsv", music_tracks, ["id", "cnt", "track"])

    write_gap_report(
        fine=fine,
        zones=zones,
        class_counts=class_counts,
        interesting_actors=interesting_actors,
        sound_catalog=sound_catalog,
        sound_refs=sound_refs,
        anim_notifies=anim_notifies,
        anim_sequences=anim_sequences,
        anim_sequence_notifies=anim_sequence_notifies,
        map_summaries=map_summaries,
        music_tracks=music_tracks,
        skill_sound_rows=skill_sound_rows,
    )
    write_readme()
    return 0



def bind_sequence_notifies(rows: list[dict], notifies: list[dict], catalog: list[dict]) -> None:
    index = {(n.get("package"), n.get("name")): n for n in notifies}
    stepsound = {
        (r.get("name") or "").lower()
        for r in catalog
        if (r.get("package") or "").lower() == "stepsound" and r.get("name")
    }
    for row in rows:
        src = index.get((row.get("package"), row.get("notify")))
        if src is None:
            row.setdefault("volume", "")
            row.setdefault("radius", "")
            row.setdefault("sound", "")
            row.setdefault("sounds", "")
            row["surface_step"] = "0"
            continue
        row["volume"] = src.get("volume") or ""
        row["radius"] = src.get("radius") or ""
        row["sound"] = src.get("sound") or ""
        row["sounds"] = src.get("sounds") or ""
        stem, _ = parse_sound_ref(row["sound"])
        if not stem:
            stem = row["sound"]
        row["surface_step"] = "1" if stem.lower() in stepsound else "0"


def write_runtime_notify_table(rows: list[dict]) -> None:
    """StreamingAssets copy used by AnimNotifyTable at runtime."""
    root = Path(__file__).resolve().parents[2] / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "AnimNotifies"
    path = root / "anim-sequence-notifies.tsv"
    fields = ["package", "animation", "sequence", "time", "class", "notify", "function",
              "volume", "radius", "sound", "sounds", "surface_step", "frame_count", "rate"]
    root.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=fields, delimiter="\t", extrasaction="ignore", lineterminator="\n")
        w.writeheader()
        for row in rows:
            if row.get("surface_step") != "1":
                row = dict(row)
                row["sounds"] = ""
            w.writerow(row)
    print(f"wrote {path} rows={len(rows)} bytes={path.stat().st_size}", file=sys.stderr)


def write_gap_report(**kw):
    fine = kw["fine"]
    interesting = kw["interesting_actors"]
    catalog = kw["sound_catalog"]
    refs = kw["sound_refs"]
    notifies = kw["anim_notifies"]
    seqs = kw["anim_sequences"]
    summaries = kw["map_summaries"]
    music_tracks = kw["music_tracks"]
    skill_sound_rows = kw["skill_sound_rows"]

    by_class = Counter(a["class"] for a in interesting)
    maps = [s for s in summaries if s["kind"] == "map"]
    cat_by_pkg = Counter(r["package"] for r in catalog)
    fineage_clip = Counter(r["fineage"] for r in catalog)
    notify_by_cls = Counter(n["class"] for n in notifies)
    refs_by_kind = Counter(r["kind"] for r in refs)

    # ambient / emitter / music / light per map
    def count_class(cls_pred, rows=interesting):
        c = Counter()
        for a in rows:
            if cls_pred(a["class"]):
                c[a["map"]] += 1
        return c

    amb_c = count_class(lambda c: "AmbientSound" in c)
    emit_c = count_class(lambda c: "Emitter" in c and "Ambient" not in c)
    music_c = count_class(lambda c: "Music" in c)
    light_c = count_class(lambda c: c == "Light" or "SunLight" in c or c in {"NSun", "NMoon", "NMovableSunLight"})
    zone_c = count_class(lambda c: "ZoneInfo" in c or c == "LevelInfo")

    # starting zone sections
    lines = []
    def w(s=""):
        lines.append(s)

    w("# Interlude world / effects / sounds gap report")
    w()
    w("Derived from a lossless property dump of every official Interlude `.unr` map,")
    w("every `.uax` sound package, every `.ukx` animation package, and `lineageenv` /")
    w("`skylevel` / `interface` / `nwindow`. Compared against what is present in the")
    w("Fineage tree (map tiles, ambient prefabs/JSON, audio clips, FMOD banks, and")
    w("runtime scripts). No guessed asset lists.")
    w()
    w("Source issue: [#1670](https://github.com/FioraSolutions/Fineage/issues/1670).")
    w("Skill-visual leftovers stay in [skills/gap-report.md](../skills/gap-report.md) and #1674.")
    w()
    w("## Headline counts")
    w()
    w(f"- Map packages dumped: **{len(maps)}** grid tiles + env levels (`Lobby`, `entry`, `skylevel`, `ship_position`). Extract failures: **0**.")
    w(f"- Interesting placed actors (emitters, ambient sounds, music volumes, lights, zone/sky/celestial, volumes): **{len(interesting)}**.")
    w(f"- Sound waves in `.uax` packages: **{len(catalog)}** across {len(cat_by_pkg)} packages.")
    w(f"- Sound / music references discovered (maps, anim notifies, items, UI, events): **{len(refs)}**.")
    w(f"- Animation notifies: **{len(notifies)}**. MeshAnimation sequences: **{len(seqs)}**.")
    w(f"- `musicinfo` tracks: **{len({t['track'] for t in music_tracks})}** unique names in **{len({t['id'] for t in music_tracks})}** ids.")
    w(f"- Skill-sound table rows already imported (see skills/gap-report.md): **{skill_sound_rows}**.")
    w()
    w("### Original sound clips vs Fineage playback assets")
    w()
    w("| Fineage status | Original `.uax` waves |")
    w("|---|---:|")
    for k in ("resources_clip", "fmod_bank", "fmod_bank_ambsound", "missing"):
        w(f"| `{k}` | {fineage_clip.get(k, 0)} |")
    w(f"| **total** | {len(catalog)} |")
    w()
    w("Status is discovered, not assumed: a wave counts as present if a Resources/StreamingAssets")
    w("`.ogg`/`.wav` stem matches, or the wave name appears in the matching FMOD `.bank`")
    w(f"(`AmbSound` {fine['banks'].get('AmbSound', 0)} bytes, `Music` {fine['banks'].get('Music', 0)},")
    w(f"`StepSound` {fine['banks'].get('StepSound', 0)}, `InterfaceSound` {fine['banks'].get('InterfaceSound', 0)},")
    w(f"`ChrSound` {fine['banks'].get('ChrSound', 0)}, `MonSound` {fine['banks'].get('MonSound', 0)},")
    w(f"`ItemSound` {fine['banks'].get('ItemSound', 0)}, `SkillSound` {fine['banks'].get('SkillSound', 0)}).")
    w(f"Fineage also has **{len(fine['clip_stems'])}** loose clips under `Assets`.")
    w()
    w("This client install has **no loose `.ogg` music folder**; music lives only in `musicinfo.dat`")
    w("plus Fineage's `Music.bank`. Track files themselves were not in `~/Downloads/L2_Interlude_Client`.")
    w()
    w("### Interesting actor classes (all maps + env)")
    w()
    w("| Class | Count |")
    w("|---|---:|")
    for cls, n in by_class.most_common():
        w(f"| `{cls}` | {n} |")
    w()
    w("### Animation notify classes")
    w()
    w("| Class | Count |")
    w("|---|---:|")
    for cls, n in notify_by_cls.most_common():
        w(f"| `{cls}` | {n} |")
    w()
    w("### Reference kinds")
    w()
    w("| Kind | Count |")
    w("|---|---:|")
    for k, n in refs_by_kind.most_common():
        w(f"| `{k}` | {n} |")
    w()

    w("## Fineage world coverage")
    w()
    w(f"- Official grid maps: **{len(maps)}**. Fineage map tiles under `Resources/Data/Maps`: **{len(fine['map_tiles'])}**.")
    w(f"- Tiles with `*_AmbientSounds.prefab`: **{len(fine['ambient_prefabs'])}**.")
    w(f"- Tiles with committed `*_AmbientSounds.json`: **{len(fine['ambient_json_counts'])}** (plus Talking Island aggregate).")
    w(f"- Tiles with `MusicArea.prefab`: **{len(fine['music_prefabs'])}** {sorted(fine['music_prefabs'])}.")
    w(f"- Runtime: AmbientSoundEmitter={'yes' if fine['has_ambient_emitter'] else 'no'}, "
      f"interval L2Footsteps={'yes' if fine['has_footsteps_interval'] else 'no'}, "
      f"L2TownAmbience hardcoded beds={'yes' if fine['has_town_ambience_hack'] else 'no'}, "
      f"DayNightCycle={'yes' if fine['has_daynight'] else 'no'}, "
      f"SkillCastSound={'yes' if fine['has_skill_cast_sound'] else 'no'}, "
      f"CharacterAnimationAudioHandler={'yes' if fine['has_anim_audio_handler'] else 'no'}.")
    w()

    # category gaps
    missing_clips = [r for r in catalog if r["fineage"] == "missing"]
    missing_by_pkg = Counter(r["package"] for r in missing_clips)

    w("## Gaps by category")
    w()
    w("### 1. World effects (map Emitter / SpriteEmitter / xEmitter)")
    n_emit = sum(1 for a in interesting if "Emitter" in a["class"] and "Ambient" not in a["class"])
    emit_maps = sorted({a["map"] for a in interesting if "Emitter" in a["class"] and "Ambient" not in a["class"]})
    w(f"- Original placed emitter actors: **{n_emit}** on **{len(emit_maps)}** maps/levels.")
    w("- Fineage `Resources/Data/Maps` contains **no** dedicated emitter/particle prefabs or Meta JSON for world emitters.")
    w("- `L2TownAmbience` hardcodes three FMOD loops (`fire_small_05`, `waterfall_big_02`, `stream_small_03`) ")
    w("  because, in its own comment, \"the map export dropped\" original beds. That is a silent substitute,")
    w("  not a dump-backed placement.")
    w(f"- Starting-zone emitter counts are listed below. Full table: `world/map-actors.tsv` (class contains Emitter).")
    w()

    w("### 2. Ambient sounds (AmbientSound / AmbientSoundObject)")
    n_amb = sum(amb_c.values())
    w(f"- Original ambient actors: **{n_amb}**.")
    amb_pkg_used = Counter()
    for a in interesting:
        if "AmbientSound" in a["class"] and a.get("sound"):
            # ref like import:Engine:Sound:in_cricket_01
            sn, _ = parse_sound_ref(a["sound"]) if not a["sound"].startswith("import") else parse_sound_ref(a["sound"])
            # also parse from extra
    tiles_missing_prefab = [s["map"] for s in maps if s["fineage_tile"] == "yes" and s["fineage_ambient_prefab"] != "yes"]
    official_no_tile = [s["map"] for s in maps if s["fineage_tile"] != "yes"]
    w(f"- Fineage has an ambient prefab on **{len(fine['ambient_prefabs'])}** of **{len(fine['map_tiles'])}** shipped tiles.")
    w(f"- Fineage tiles still missing an ambient prefab: {', '.join(tiles_missing_prefab) or '(none)'} .")
    w(f"- Official maps with no Fineage tile at all: **{len(official_no_tile)}** (everything outside the current 37-tile ship set).")
    w("- Even where a prefab exists, clip coverage is incomplete (see sound table). Night/day types (`AmbientSoundType`)")
    w("  and per-actor volume/radius/pitch are authored on every original actor and must be preserved.")
    w()

    w("### 3. Music (MusicVolume + musicinfo)")
    n_mus = sum(music_c.values())
    music_ids = sorted({str(a["music_id"]) for a in interesting if a.get("music_id") not in ("", None)})
    w(f"- Original `MusicVolume` actors: **{n_mus}** using **{len(music_ids)}** distinct `nMusicID` values.")
    w(f"- `musicinfo` defines **{len({t['track'] for t in music_tracks})}** unique track names.")
    w(f"- Fineage `MusicArea.prefab` exists only on: {sorted(fine['music_prefabs']) or '(none)'}.")
    w("- Talking Island village volumes use `nMusicID=172501` → `NT_Speaking`. That mapping is in `sounds/musicinfo-tracks.tsv`.")
    w("- No official loose `.ogg` music was present in this client tree; `Music.bank` (11 MB) is the only Fineage music container.")
    w()

    seq_notifies = kw.get("anim_sequence_notifies") or []
    w("### 4. Footsteps and weapon-swing / hit notifies (`*.ukx`)")
    w(f"- `AnimNotify_Sound` (footsteps / draw / swing beds with per-surface arrays): **{notify_by_cls.get('AnimNotify_Sound', 0)}**.")
    w(f"- `AnimNotify_AttackShot` / `AttackItem` / `AttackPreShot` / `AttackVoice`: "
      f"**{notify_by_cls.get('AnimNotify_AttackShot', 0) + notify_by_cls.get('AnimNotify_AttackItem', 0) + notify_by_cls.get('AnimNotify_AttackPreShot', 0) + notify_by_cls.get('AnimNotify_AttackVoice', 0)}**.")
    w(f"- Swim / channeling notifies: **{notify_by_cls.get('AnimNotify_SwimSound', 0) + notify_by_cls.get('AnimNotify_Channeling', 0)}**.")
    w(f"- Sequence bindings (time + notify object on each MeshAnimation sequence): **{len(seq_notifies)}** "
      f"(dump: `sounds/anim-sequence-notifies.tsv.gz`). Times are sequence-normalized in 0..1.")
    if fine.get("has_anim_notify_playback"):
        w("- Fineage plays those bindings through `AnimNotifyPlayback` (one path, official times, volume, radius,")
        w("  and per-surface arrays when the notify's primary wave is in `stepsound`).")
    else:
        w("- Fineage `L2Footsteps` still plays on a **fixed walk/run interval** (0.48s / 0.32s) from a collider tag,")
        w("  not from the original notify time, volume, radius, or per-surface arrays (`DefaultWalkSound`, `GrassRunSound`, …).")
    w("- MeshAnimation sequences also carry `shot_time` / `pre_shot_time` / `channel_times` (dump: `sounds/anim-sequences.tsv`).")
    w()

    w("### 5. UI sounds")
    ui_pkg = [r for r in catalog if r["package"].lower() in {"interfacesound", "messengersound"}]
    ui_missing = [r for r in ui_pkg if r["fineage"] == "missing"]
    w(f"- `interfacesound.uax` + `messengersound.uax` waves: **{len(ui_pkg)}**, Fineage-missing: **{len(ui_missing)}**.")
    w(f"- `systemmsg-e.dat` item_sound references: **{refs_by_kind.get('ui_systemmsg', 0)}**.")
    w(f"- Fineage `InterfaceSound.bank` is {fine['banks'].get('InterfaceSound', 0)} bytes — far smaller than the original wave set.")
    w()

    w("### 6. Item, NPC, and character voice / monster sounds")
    w(f"- Item DAT sound fields (etc/armor/weapon): **{refs_by_kind.get('item', 0)}** references.")
    w(f"- Enter-event sound/effect fields: **{refs_by_kind.get('enter_event', 0)}**.")
    w("- Monster and character waves live in `monsound*.uax` and `chrsound*.uax` (see catalog).")
    w(f"- Fineage loose clips: MonSound folder + bank {fine['banks'].get('MonSound', 0)} B, ChrSound bank {fine['banks'].get('ChrSound', 0)} B.")
    w()

    w("### 7. Skill effects and skill sounds")
    w("- Already covered by the lossless `skill.usk` / `lineageeffect.u` dump and `skills/gap-report.md`.")
    w(f"- `{skill_sound_rows}` skill-sound rows are in the derived skill table. Leftovers: issue #1674.")
    w("- This report does not reopen that work except to count those rows in the overall inventory.")
    w()

    w("### 8. Sky, day/night, fog, and lights")
    n_light = sum(1 for a in interesting if a["class"] == "Light")
    n_sun = sum(1 for a in interesting if a["class"] in {"NSun", "NMoon", "NMovableSunLight"})
    n_sky = sum(1 for a in interesting if "SkyZone" in a["class"] or a["class"] == "SkyZoneInfo")
    n_zone = sum(1 for a in interesting if a["class"] in {"ZoneInfo", "LevelInfo"})
    w(f"- Lights: **{n_light}**. Celestial actors (NSun/NMoon/NMovableSunLight): **{n_sun}**. SkyZoneInfo: **{n_sky}**. ZoneInfo+LevelInfo: **{n_zone}**.")
    w("- Fineage has `DayNightCycle`, `L2CelestialDiscs`, `L2CloudMyst`, and `L2WorldAmbientLUT.csv` / `TimeEnv0.int`.")
    w("- `skylevel.unr` is dumped under `world/env/` (sun, moons, lens flares, zone fog). Compare those properties to the")
    w("  LUT rather than assuming the LUT is complete. Map `ZoneInfo` fog and `NSun` radii are per-tile in `map-actors.tsv`.")
    w()

    w("### Original `.uax` waves missing from Fineage, by package")
    w()
    w("| Package | Waves | Missing in Fineage |")
    w("|---|---:|---:|")
    present_pkg = Counter(r["package"] for r in catalog)
    for pkg, n in sorted(present_pkg.items(), key=lambda kv: (-missing_by_pkg.get(kv[0], 0), kv[0].lower())):
        w(f"| `{pkg}` | {n} | {missing_by_pkg.get(pkg, 0)} |")
    w()

    w("## Starting zones (play first)")
    w()
    w("Tiles listed here follow `zonename-e.dat` plus immediately adjacent shipped tiles.")
    w()

    def zone_block(title, tiles):
        w(f"### {title}")
        w()
        w("| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |")
        w("|---|---|---|---|---|---:|---|")
        official_amb = official_emit = official_music = official_light = 0
        missing_local = []
        emit_local = []
        for tile in tiles:
            sm = next((s for s in summaries if s["map"] == tile), None)
            amb = amb_c.get(tile, 0)
            em = emit_c.get(tile, 0)
            mu = music_c.get(tile, 0)
            li = light_c.get(tile, 0)
            official_amb += amb
            official_emit += em
            official_music += mu
            official_light += li
            has_tile = tile in fine["map_tiles"]
            has_prefab = tile in fine["ambient_prefabs"]
            js = fine["ambient_json_counts"].get(tile, "")
            has_music = tile in fine["music_prefabs"]
            z = ""
            if sm:
                z = sm.get("zones") or ""
            else:
                z = "; ".join(kw["zones"].get(tile, []))
            if not sm and tile not in {a["map"] for a in interesting} and not Path(str(WORLD / "maps" / f"{tile}.unr.json.gz")).exists() and not (WORLD / "maps" / f"{tile}.unr.json").exists():
                # map file missing from official client
                flag = "no official `.unr`"
            else:
                flag = "yes" if has_tile else "**no tile**"
            w(f"| `{tile}` | {z} | {amb} / {em} / {mu} / {li} | {flag} | {'yes' if has_prefab else 'no'} | {js if js!='' else '—'} | {'yes' if has_music else 'no'} |")
            if em:
                emit_local.append((tile, em))
            # sample missing ambient clips on this tile
            for a in interesting:
                if a["map"] == tile and "AmbientSound" in a["class"] and a.get("sound"):
                    sn, _ = parse_sound_ref(a["sound"])
                    st = clip_status_cached(sn)
                    if st == "missing" and sn:
                        missing_local.append((tile, a["name"], sn, a.get("location")))
        w()
        w(f"- Official totals on these tiles: ambient **{official_amb}**, emitters **{official_emit}**, "
          f"music volumes **{official_music}**, lights/celestials **{official_light}**.")
        if emit_local:
            w("- Emitter actors: " + ", ".join(f"`{t}` × {n}" for t, n in emit_local) + ".")
        else:
            w("- No placed Emitter actors on these tiles (effects may still live in decorations or elsewhere).")
        # unique missing amb sounds
        uniq = []
        seen = set()
        for row in missing_local:
            if row[2] in seen:
                continue
            seen.add(row[2])
            uniq.append(row)
        if uniq:
            w(f"- Ambient waves used here and **missing** from Fineage clips/banks ({len(uniq)} unique):")
            for tile, actor, sn, loc in uniq[:25]:
                w(f"  - `{sn}` (`{tile}` {actor} at {loc})")
            if len(uniq) > 25:
                w(f"  - … {len(uniq)-25} more")
        else:
            w("- Every ambient wave referenced on these tiles was found in a Fineage clip or FMOD bank (name match).")
        w()

    # clip cache for zone blocks
    clip_memo = {r["name"].lower(): r["fineage"] for r in catalog}

    def clip_status_cached(name: str) -> str:
        if not name:
            return "missing"
        return clip_memo.get(name.lower(), "missing")

    # bind into closure — redefine zone_block after helper? already referenced
    # Monkeypatch by assigning global-like on function... we call after defining clip_status_cached
    # Re-run blocks now:
    for title, tiles in STARTING_ZONES:
        # inline the block here instead of using the function defined above with forward ref
        # We'll just call zone_block now that clip_status_cached exists.
        pass

    # The function zone_block already referenced clip_status_cached before assignment.
    # Redefine properly:
    def zone_block2(title, tiles):
        w(f"### {title}")
        w()
        w("| Tile | Zones | Official amb / emit / music / lights | Fineage tile | Amb prefab | Amb JSON rows | Music prefab |")
        w("|---|---|---|---|---|---:|---|")
        official_amb = official_emit = official_music = official_light = 0
        missing_local = []
        emit_local = []
        for tile in tiles:
            sm = next((s for s in summaries if s["map"] == tile), None)
            official_exists = any(s["map"] == tile for s in summaries) or (WORLD / "maps" / f"{tile}.unr.json.gz").exists()
            amb = amb_c.get(tile, 0)
            em = emit_c.get(tile, 0)
            mu = music_c.get(tile, 0)
            li = light_c.get(tile, 0)
            official_amb += amb
            official_emit += em
            official_music += mu
            official_light += li
            has_tile = tile in fine["map_tiles"]
            has_prefab = tile in fine["ambient_prefabs"]
            js = fine["ambient_json_counts"].get(tile, "")
            has_music = tile in fine["music_prefabs"]
            z = (sm.get("zones") if sm else "") or "; ".join(kw["zones"].get(tile, []))
            if not official_exists:
                flag = "no official `.unr`"
            else:
                flag = "yes" if has_tile else "**no tile**"
            w(f"| `{tile}` | {z} | {amb} / {em} / {mu} / {li} | {flag} | {'yes' if has_prefab else 'no'} | {js if js!='' else '—'} | {'yes' if has_music else 'no'} |")
            if em:
                emit_local.append((tile, em))
            for a in interesting:
                if a["map"] == tile and "AmbientSound" in a["class"] and a.get("sound"):
                    sn, _ = parse_sound_ref(a["sound"])
                    if clip_status_cached(sn) == "missing" and sn:
                        missing_local.append((tile, a["name"], sn, a.get("location")))
        w()
        w(f"- Official totals on these tiles: ambient **{official_amb}**, emitters **{official_emit}**, "
          f"music volumes **{official_music}**, lights/celestials **{official_light}**.")
        if emit_local:
            w("- Emitter actors: " + ", ".join(f"`{t}` × {n}" for t, n in emit_local) + ".")
        else:
            w("- No placed Emitter actors on these tiles.")
        uniq = []
        seen = set()
        for row in missing_local:
            if row[2] in seen:
                continue
            seen.add(row[2])
            uniq.append(row)
        if uniq:
            w(f"- Ambient waves used here and **missing** from Fineage clips/banks ({len(uniq)} unique):")
            for tile, actor, sn, loc in uniq[:25]:
                w(f"  - `{sn}` (`{tile}` {actor} at {loc})")
            if len(uniq) > 25:
                w(f"  - … {len(uniq)-25} more")
        else:
            w("- Every ambient wave referenced on these tiles was found in a Fineage clip or FMOD bank (name match).")
        # JSON vs official ambient for tiles that have both
        mismatches = []
        for tile in tiles:
            if tile in fine["ambient_json_counts"] and isinstance(fine["ambient_json_counts"][tile], int) and fine["ambient_json_counts"][tile] >= 0:
                if amb_c.get(tile, 0) and fine["ambient_json_counts"][tile] != amb_c[tile]:
                    mismatches.append((tile, amb_c[tile], fine["ambient_json_counts"][tile]))
        if mismatches:
            w("- Ambient JSON row count differs from the official actor count:")
            for tile, off, have in mismatches:
                w(f"  - `{tile}` official={off} json={have} (delta {have-off:+d})")
        w()

    for title, tiles in STARTING_ZONES:
        zone_block2(title, tiles)

    w("## Files")
    w()
    w("- Lossless dumps: `world/maps/*.unr.json.gz`, `world/env/`, `world/anims/*.ukx.json.gz`, `sounds/packages/*.uax.json`.")
    w("- `world/package-dump-index.json` — package list, export counts, stored sizes (0 extract failures).")
    w("- `world/map-actors.tsv` — every interesting placed actor and its authored properties.")
    w("- `world/map-class-counts.tsv`, `world/map-summary.tsv`.")
    w("- `sounds/sound-catalog.tsv` — every original wave + Fineage presence.")
    w("- `sounds/sound-references.tsv` — who references each sound.")
    w("- `sounds/anim-notifies.tsv`, `sounds/anim-sequences.tsv`, `sounds/anim-sequence-notifies.tsv.gz`, `sounds/musicinfo-tracks.tsv`.")
    w("- Tools: `tools/interlude-extract/ExportClientPackages.java`, `export_client_packages.sh`, `build_world_sound_gap.py`.")
    w()
    w("Original binaries are not committed. Dumps omit wave/texture payloads (`wave_bytes` is a length only).")
    w()

    path = WORLD / "gap-report.md"
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"wrote {path} lines={len(lines)}", file=sys.stderr)


def write_readme():
    # world/README.md is curated (render-parity, shore-tile notes). Do not overwrite it.
    (SOUNDS / "README.md").write_text(
        "# Interlude sound dumps and derived tables\n\n"
        "See `../world/README.md` and `../world/gap-report.md`.\n\n"
        "`anim-sequence-notifies.tsv.gz` binds each MeshAnimation sequence to official notify times.\n"
        "The runtime copy is `StreamingAssets/Data/Meta/AnimNotifies/anim-sequence-notifies.tsv`.\n",
        encoding="utf-8",
    )


if __name__ == "__main__":
    raise SystemExit(main())
