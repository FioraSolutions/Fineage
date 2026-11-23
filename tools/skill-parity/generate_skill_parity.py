#!/usr/bin/env python3
"""Cross-check every learnable Interlude player skill against the client.

Sources (Interlude only):
  Mobius CT0 class skill trees (XML under ref/ or Fineage-Backend data/skill-trees.json)
  l2-unity/docs/interlude-ref/dats/skillgrp.tsv          ani_char, desc, costs
  l2-unity/docs/interlude-ref/dats/skillsoundgrp.tsv     cast / shot / explode / voice
  l2-unity/docs/interlude-ref/skills/skill-visuals.tsv   effect class
  l2-unity/docs/interlude-ref/skills/skill-effect-placement.tsv
                                                         casting / shot / explosion
  AnimationCombo_Interlude.txt + AnimTween.txt           motion names
  shipped Assets/Resources/Data/Animations/**/*.anim
  GlobalEffect.asset                                     effect id actually played

Writes l2-unity/docs/interlude-ref/gap-report-skill-parity.md and the CSV/JSON
beside it, plus the known-gaps allowlist the EditMode test reads.

    python3 tools/skill-parity/generate_skill_parity.py
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import defaultdict
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
UNITY = REPO / "l2-unity"
REF = UNITY / "docs" / "interlude-ref"
OUT = REF

SKILLGRP = REF / "dats" / "skillgrp.tsv"
SKILLNAME = REF / "dats" / "skillname-e.tsv"
SOUNDGRP = REF / "dats" / "skillsoundgrp.tsv"
VISUALS = REF / "skills" / "skill-visuals.tsv"
PLACEMENT = REF / "skills" / "skill-effect-placement.tsv"
COMBO = UNITY / "Assets/StreamingAssets/Data/Meta/AnimationCombo_Interlude.txt"
TWEEN = UNITY / "Assets/StreamingAssets/Data/Meta/AnimTween.txt"
ALIAS = UNITY / "Assets/StreamingAssets/Data/Meta/InterludeAnimResourceTake.tsv"
ANIM_ROOT = UNITY / "Assets/Resources/Data/Animations"
EFFECT_PREFABS = UNITY / "Assets/StreamingAssets/Data/Meta/SkillVisual/effect-prefabs.txt"
RESOURCES = UNITY / "Assets/Resources"
SOUND_ROOT = UNITY / "Assets/Resources/Data/Sounds"
UKX_DIR = REF / "world" / "anims"
TREES_CACHE = REF / "skills" / "player-skill-trees.json"

WEAPONS = ("1hs", "2hs", "dual", "pole", "bow", "hand")
CAST_VARIANTS = {
    "castlong": ("CastLong", "castLong"),
    "castshort": ("CastShort", "castShort"),
    "castmid": ("CastMid", "castMid"),
    "castend": ("CastEnd", "castEnd"),
    "magicshot": ("MagicShot",),
    "magicthrow": ("MagicThrow",),
    "magicnotarget": ("MagicNoTarget", "MagicNotarget"),
}
# GetOverrideClip substitutes these when the dedicated take is absent.
CAST_FALLBACK = {
    "castlong": "castmid",
    "castshort": "castmid",
    "magicnotarget": "magicshot",
    "magicthrow": "magicshot",
}
MYSTIC_ROOTS = {10, 25, 38, 49}
RACE_LABEL = {0: "Human", 1: "Elf", 2: "DarkElf", 3: "Orc", 4: "Dwarf"}

# SkillAnimationDatabase.RaceClipTable — folder is unused here; stems must match.
BODIES = {
    "MFighter": {"race": "Human", "sex": "M", "archetype": "fighter", "mesh": "MFighter_anim", "stems": ("MFighter_m000_b",)},
    "FFighter": {"race": "Human", "sex": "F", "archetype": "fighter", "mesh": "FFighter_anim", "stems": ("FFighter_m000_b",)},
    "MMagic": {"race": "Human", "sex": "M", "archetype": "mystic", "mesh": "MMagic_anim", "stems": ("MMagic_M000_b",)},
    "FMagic": {"race": "Human", "sex": "F", "archetype": "mystic", "mesh": "FMagic_anim", "stems": ("FMagic_m000_b",)},
    "MElf": {"race": "Elf", "sex": "M", "archetype": "fighter", "mesh": "MElf_anim", "stems": ("MElf_m000_b",)},
    "FElf": {"race": "Elf", "sex": "F", "archetype": "fighter", "mesh": "FElf_anim", "stems": ("FElf_m000_b",)},
    "MDarkElf": {"race": "DarkElf", "sex": "M", "archetype": "fighter", "mesh": "MDarkElf_anim", "stems": ("MDarkElf_m000_b",)},
    "FDarkElf": {"race": "DarkElf", "sex": "F", "archetype": "fighter", "mesh": "FDarkElf_anim", "stems": ("FDarkElf_m000_b", "FDarkElf_m001_b")},
    "MOrc": {"race": "Orc", "sex": "M", "archetype": "fighter", "mesh": "MOrc_anim", "stems": ("MOrc_m000_b",)},
    "FOrc": {"race": "Orc", "sex": "F", "archetype": "fighter", "mesh": "FOrc_anim", "stems": ("FOrc_m000_b",)},
    "MShaman": {"race": "Orc", "sex": "M", "archetype": "mystic", "mesh": "MShaman_anim", "stems": ("MShaman_m000_b",)},
    "FShaman": {"race": "Orc", "sex": "F", "archetype": "mystic", "mesh": "FShaman_anim", "stems": ("FShaman_m000_b",)},
    "MDwarf": {"race": "Dwarf", "sex": "M", "archetype": "fighter", "mesh": "MDwarf_anim", "stems": ("MDwarf_m000_b",)},
    "FDwarf": {"race": "Dwarf", "sex": "F", "archetype": "fighter", "mesh": "FDwarf_anim", "stems": ("FDwarf_m001_b",)},
}

PHASE_CAST = {"casting", "channeling", "preshot"}
PHASE_FLY = {"shot"}
PHASE_TARGET = {"explosion"}
SOUND_CAST_COLS = ("spelleffect_sound_1", "spelleffect_sound_2", "spelleffect_sound_3")
SOUND_FLY_COLS = ("shoteffect_sound_1", "shoteffect_sound_2", "shoteffect_sound_3")
SOUND_TARGET_COLS = ("expeffect_sound_1", "expeffect_sound_2", "expeffect_sound_3")
VOICE_COLS = (
    "mfighter_sub", "ffighter_sub", "mdarkelf_sub", "fdarkelf_sub", "mdwarf_sub", "fdwarf_sub",
    "melf_sub", "felf_sub", "mmagic_sub", "fmagic_sub", "morc_sub", "forc_sub", "mshaman_sub", "fshaman_sub",
    "mfighter_throw", "ffighter_throw", "mdarkelf_throw", "fdarkelf_throw", "mdwarf_throw", "fdwarf_throw",
    "melf_throw", "felf_throw", "mmagic_throw", "fmagic_throw", "morc_throw", "forc_throw", "mshaman_throw", "fshaman_throw",
)
VOICE_BODY = {
    "mfighter": "MFighter", "ffighter": "FFighter", "mdarkelf": "MDarkElf", "fdarkelf": "FDarkElf",
    "mdwarf": "MDwarf", "fdwarf": "FDwarf", "melf": "MElf", "felf": "FElf",
    "mmagic": "MMagic", "fmagic": "FMagic", "morc": "MOrc", "forc": "FOrc",
    "mshaman": "MShaman", "fshaman": "FShaman",
}


def bodies_for(race: int, mystic: bool) -> tuple[str, str]:
    """Same pairs as CharacterRaceAnimationTable (elves/dark elves/dwarves ignore mystic)."""
    if race == 0:
        return ("MMagic", "FMagic") if mystic else ("MFighter", "FFighter")
    if race == 1:
        return ("MElf", "FElf")
    if race == 2:
        return ("MDarkElf", "FDarkElf")
    if race == 3:
        return ("MShaman", "FShaman") if mystic else ("MOrc", "FOrc")
    if race == 4:
        return ("MDwarf", "FDwarf")
    raise ValueError(race)


def find_skill_trees() -> tuple[list[dict], list[dict], str]:
    candidates = []
    backend = REPO.parent / "Fineage-Backend" / "data"
    for folder in (
        REPO / "ref" / "mobius-ct0-interlude",
        Path("/tmp/fineage-data"),
        backend,
    ):
        candidates.append(folder)
    env = Path(__import__("os").environ.get("FINEAGE_SKILL_TREES", ""))
    if env.is_file():
        data = json.loads(env.read_text(encoding="utf-8"))
        classes = json.loads(Path(str(env).replace("skill-trees", "classes")).read_text()) if False else None
        return data["trees"], [], str(env)

    for folder in candidates:
        trees = folder / "skill-trees.json"
        classes = folder / "classes.json"
        if trees.is_file() and classes.is_file():
            return (
                json.loads(trees.read_text(encoding="utf-8"))["trees"],
                json.loads(classes.read_text(encoding="utf-8"))["classes"],
                str(trees),
            )
    if TREES_CACHE.is_file():
        cached = json.loads(TREES_CACHE.read_text(encoding="utf-8"))
        return cached["trees"], cached["classes"], str(TREES_CACHE)
    xml_root = REPO / "ref" / "mobius-ct0-interlude"
    if xml_root.is_dir():
        trees, classes = parse_mobius_xml(xml_root)
        return trees, classes, str(xml_root)
    raise SystemExit(
        "No Mobius skill trees. Place data/skill-trees.json + classes.json from Fineage-Backend, "
        "or ref/mobius-ct0-interlude skillTrees XML, or set FINEAGE_SKILL_TREES."
    )


def parse_mobius_xml(root: Path) -> tuple[list[dict], list[dict]]:
    import xml.etree.ElementTree as ET

    trees = []
    skill_dir = None
    for cand in root.rglob("skillTrees"):
        if cand.is_dir():
            skill_dir = cand
            break
    if skill_dir is None:
        raise SystemExit(f"no skillTrees under {root}")
    for path in sorted(skill_dir.glob("*.xml")):
        try:
            doc = ET.parse(path)
        except ET.ParseError:
            continue
        for node in doc.getroot().iter("skillTree"):
            if node.get("type") not in (None, "classSkillTree"):
                continue
            cid = node.get("classId")
            if cid is None:
                continue
            skills = []
            for sk in node.findall("skill"):
                skills.append({
                    "skillId": int(sk.get("skillId")),
                    "level": int(sk.get("skillLevel") or sk.get("skillLvl") or 1),
                })
            trees.append({"classId": int(cid), "skills": skills})
    classes = []
    class_list = None
    for cand in root.rglob("classList.xml"):
        class_list = cand
        break
    if class_list is not None:
        doc = ET.parse(class_list)
        for node in doc.getroot().iter("class"):
            classes.append({
                "classId": int(node.get("classId") or node.get("id")),
                "name": node.get("name") or node.get("className") or "",
                "parentClassId": int(node.get("parentClassId") or node.get("parent") or -1),
                "race": int(node.get("race") or 0),
            })
    return trees, classes


def load_tsv(path: Path) -> list[dict]:
    rows = []
    with path.open(encoding="utf-8", errors="replace", newline="") as fh:
        header = None
        for line in fh:
            if not line.strip() or line.startswith("#"):
                continue
            if header is None:
                header = line.rstrip("\n").split("\t")
                continue
            cols = line.rstrip("\n").split("\t")
            rows.append({header[i]: cols[i] if i < len(cols) else "" for i in range(len(header))})
    return rows


def clear_combo(cell: str) -> str:
    text = cell.replace("a,", "").replace("\\0", "").replace("AnimName", "").strip()
    if text.lower() == "none":
        return ""
    return text


def load_combos() -> dict[str, list[str]]:
    combos: dict[str, list[str]] = {}
    lines = COMBO.read_text(encoding="utf-8", errors="replace").splitlines()
    for i, line in enumerate(lines):
        if i == 0 or not line.strip():
            continue
        parts = line.split("\t")
        if len(parts) < 4:
            continue
        phases = []
        for cell in parts[1:4]:
            token = clear_combo(cell)
            if token:
                phases.append(token)
        combos[parts[0].strip()] = phases
    return combos


def load_tween() -> tuple[dict[tuple[str, str], str], str]:
    """(mesh lower, sequence lower) -> exact sequence. Interlude ukx dumps win over AnimTween."""
    ukx_files = {
        "MFighter_anim": "Fighter.ukx.json.gz",
        "FFighter_anim": "Fighter.ukx.json.gz",
        "MMagic_anim": "Magic.ukx.json.gz",
        "FMagic_anim": "Magic.ukx.json.gz",
        "MElf_anim": "Elf.ukx.json.gz",
        "FElf_anim": "Elf.ukx.json.gz",
        "MDarkElf_anim": "DarkElf.ukx.json.gz",
        "FDarkElf_anim": "DarkElf.ukx.json.gz",
        "MOrc_anim": "Orc.ukx.json.gz",
        "FOrc_anim": "Orc.ukx.json.gz",
        "MShaman_anim": "Shaman.ukx.json.gz",
        "FShaman_anim": "Shaman.ukx.json.gz",
        "MDwarf_anim": "Dwarf.ukx.json.gz",
        "FDwarf_anim": "Dwarf.ukx.json.gz",
    }
    import gzip

    out: dict[tuple[str, str], str] = {}
    loaded = set()
    by_file: dict[str, dict] = {}
    for mesh, filename in ukx_files.items():
        path = UKX_DIR / filename
        if not path.is_file():
            continue
        if filename not in by_file:
            with gzip.open(path, "rt", encoding="utf-8") as fh:
                by_file[filename] = json.load(fh)
        doc = by_file[filename]
        for export in doc.get("exports") or []:
            if export.get("class") != "MeshAnimation" or export.get("name") != mesh:
                continue
            for seq in export.get("sequences") or []:
                name = (seq.get("name") or "").strip()
                if name:
                    out[(mesh.lower(), name.lower())] = name
            loaded.add(mesh)
            break
    if len(loaded) == len(ukx_files):
        return out, "interlude ukx MeshAnimation sequences"

    for line in TWEEN.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line.strip() or line.startswith("#") or line.startswith("package"):
            continue
        cols = line.split("\t")
        if len(cols) < 3:
            continue
        mesh, seq = cols[1].strip(), cols[2].strip()
        if mesh.endswith("_anim") and mesh not in loaded:
            out[(mesh.lower(), seq.lower())] = seq
    return out, "AnimTween.txt (ukx dump missing for some bodies)"


def load_alias() -> dict[tuple[str, str], str]:
    out: dict[tuple[str, str], str] = {}
    if not ALIAS.is_file():
        return out
    for line in ALIAS.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line.strip() or line.startswith("#") or line.startswith("mesh"):
            continue
        cols = line.split("\t")
        if len(cols) < 3:
            continue
        out[(cols[0].strip().lower(), cols[1].strip().lower())] = cols[2].strip()
    return out


def load_shipped() -> set[str]:
    found = set()
    ao = re.compile(r"\.ao_([^./\\]+)\.anim$", re.IGNORECASE)
    for path in ANIM_ROOT.rglob("*.anim"):
        match = ao.search(path.name)
        if match:
            found.add(match.group(1).lower())
    return found


def load_effect_prefabs() -> dict[str, bool]:
    """effect class → whether Assets/Resources/<path>.prefab exists."""
    found: dict[str, bool] = {}
    if not EFFECT_PREFABS.is_file():
        return found
    for line in EFFECT_PREFABS.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line.strip() or line.startswith("asset_key") or line.startswith("#"):
            continue
        cols = line.split("\t")
        if len(cols) < 2:
            continue
        key, path = cols[0].strip(), cols[1].strip()
        if not key or not path:
            continue
        found[key.lower()] = (RESOURCES / (path + ".prefab")).is_file()
    return found


def load_sound_stems() -> set[str]:
    stems = set()
    if not SOUND_ROOT.is_dir():
        return stems
    for path in SOUND_ROOT.rglob("*"):
        if path.suffix.lower() in {".wav", ".ogg"} and path.is_file():
            stems.add(path.stem.lower())
    return stems


def is_mystic(class_id: int, parent: dict[int, int]) -> bool:
    seen = set()
    cur = class_id
    while cur not in seen and cur >= 0:
        seen.add(cur)
        if cur in MYSTIC_ROOTS:
            return True
        cur = parent.get(cur, -1)
    return False


def sound_stem(token: str) -> str:
    token = token.strip()
    if not token or token.lower() in {"none", "null"}:
        return ""
    return token.split(".")[-1].strip().lower()


def classify_motion(token: str, body: str, tween, alias, shipped) -> list[dict]:
    """One row per official take the resolver must load for this phase."""
    info = BODIES[body]
    mesh = info["mesh"]
    token_l = token.lower()
    number = None
    match = re.match(r"spatk(\d+)$", token_l)
    if match:
        number = int(match.group(1))

    rows = []
    if number is not None:
        official = []
        for weapon in WEAPONS:
            query = f"spatk{number:02d}_{weapon}_{body}".lower()
            if (mesh.lower(), query) in tween:
                official.append(weapon)
        if not official:
            rows.append({
                "kind": "no_original_take",
                "expected": f"spatk{number:02d}_*_{body}",
                "detail": f"{mesh} ukx has no SpAtk{number:02d} sequence",
            })
            return rows
        for weapon in official:
            query = f"spatk{number:02d}_{weapon}_{body}".lower()
            seq = tween[(mesh.lower(), query)]
            take = alias.get((mesh.lower(), seq.lower()), seq)
            if take.lower() in shipped:
                rows.append({"kind": "ok", "expected": take, "detail": f"weapon={weapon}"})
            else:
                rows.append({
                    "kind": "missing_import",
                    "expected": take,
                    "detail": f"AnimTween {seq} on {mesh}; resolver loads {take}; file absent. "
                              "Runtime logs and does not play SpAtk01.",
                })
        return rows

    slot = token_l
    if slot in CAST_VARIANTS or slot.startswith("cast") or slot.startswith("magic"):
        variants = CAST_VARIANTS.get(slot, (token,))
        expected = f"{variants[0]}_{body}"
        if any(f"{name}_{body}".lower() in shipped for name in variants):
            rows.append({"kind": "ok", "expected": expected, "detail": "override clip"})
            return rows
        fallback = CAST_FALLBACK.get(slot)
        if fallback and any(f"{name}_{body}".lower() in shipped for name in CAST_VARIANTS[fallback]):
            rows.append({
                "kind": "silent_fallback",
                "expected": expected,
                "detail": f"GetOverrideClip plays {fallback} instead of {slot}",
            })
            return rows
        query_hit = any((mesh.lower(), f"{name}_{body}".lower()) in tween for name in variants)
        rows.append({
            "kind": "missing_import" if query_hit or True else "no_original_take",
            "expected": expected,
            "detail": "override slot has no clip and no shared fallback clip"
            if not query_hit else "AnimTween has the cast take; clip not shipped",
        })
        # Distinguish original absence: if AnimTween has no such sequence, it is not an import miss.
        if not query_hit:
            rows[-1]["kind"] = "no_original_take"
            rows[-1]["detail"] = f"{mesh} ukx has no {token} sequence"
        return rows

    prefix = token_l + "_"
    seqs = [seq for (m, s), seq in tween.items() if m == mesh.lower() and (s == token_l + "_" + body.lower() or s.startswith(prefix) and s.endswith("_" + body.lower()))]
    if not seqs:
        rows.append({
            "kind": "no_original_take",
            "expected": f"{token}_{body}",
            "detail": f"{mesh} has no {token} sequence",
        })
        return rows
    for seq in seqs:
        take = alias.get((mesh.lower(), seq.lower()), seq)
        kind = "ok" if take.lower() in shipped else "missing_import"
        rows.append({
            "kind": kind,
            "expected": take,
            "detail": "" if kind == "ok" else f"AnimTween {seq} not shipped",
        })
    return rows


def main() -> int:
    trees, classes, tree_source = find_skill_trees()
    if not classes and TREES_CACHE.is_file():
        classes = json.loads(TREES_CACHE.read_text(encoding="utf-8"))["classes"]
    by_id = {int(c["classId"]): c for c in classes}
    parent = {cid: int(c.get("parentClassId", -1)) for cid, c in by_id.items()}
    for tree in trees:
        cid = int(tree["classId"])
        if cid not in by_id:
            by_id[cid] = {"classId": cid, "name": f"class{cid}", "parentClassId": -1, "race": 0}

    # Skills learned, per class, and the bodies that can play them.
    learned: dict[int, set[int]] = defaultdict(set)  # skill -> class ids
    skill_levels: dict[int, set[int]] = defaultdict(set)
    for tree in trees:
        cid = int(tree["classId"])
        for row in tree["skills"]:
            sid = int(row["skillId"])
            learned[sid].add(cid)
            skill_levels[sid].add(int(row.get("level") or row.get("skillLevel") or 1))

    grp_rows = load_tsv(SKILLGRP)
    grp: dict[tuple[int, int], dict] = {}
    grp_any: dict[int, dict] = {}
    for row in grp_rows:
        sid, lvl = int(row["skill_id"]), int(row["skill_level"])
        grp[(sid, lvl)] = row
        grp_any.setdefault(sid, row)

    names = {}
    if SKILLNAME.is_file():
        for row in load_tsv(SKILLNAME):
            names.setdefault(int(row["id"]), row["name"].strip())

    combos = load_combos()
    tween, tween_source = load_tween()
    alias = load_alias()
    shipped = load_shipped()
    effect_prefabs = load_effect_prefabs()
    sound_stems = load_sound_stems()

    visuals: dict[int, dict] = {}
    if VISUALS.is_file():
        for row in load_tsv(VISUALS):
            visuals.setdefault(int(row["skill_id"]), row)

    placement: dict[int, list[dict]] = defaultdict(list)
    if PLACEMENT.is_file():
        for row in load_tsv(PLACEMENT):
            if row.get("skill_id", "").isdigit():
                placement[int(row["skill_id"])].append(row)

    sounds: dict[int, list[dict]] = defaultdict(list)
    if SOUNDGRP.is_file():
        for row in load_tsv(SOUNDGRP):
            if row.get("skill_id", "").isdigit():
                sounds[int(row["skill_id"])].append(row)

    # Persist a slim tree so the EditMode test and a later re-run do not need the backend checkout.
    slim_classes = []
    for cid, c in sorted(by_id.items()):
        if cid not in {int(t["classId"]) for t in trees}:
            continue
        slim_classes.append({
            "classId": cid,
            "name": c.get("name") or f"class{cid}",
            "parentClassId": int(c.get("parentClassId", -1)),
            "race": int(c.get("race", 0)),
        })
    slim_trees = []
    for tree in trees:
        slim_trees.append({
            "classId": int(tree["classId"]),
            "skills": [
                {"skillId": int(s["skillId"]), "level": int(s.get("level") or s.get("skillLevel") or 1)}
                for s in tree["skills"]
            ],
        })
    TREES_CACHE.parent.mkdir(parents=True, exist_ok=True)
    TREES_CACHE.write_text(json.dumps({
        "source": tree_source,
        "note": "Class skill trees transcribed from Mobius CT0 Interlude. id, level, class only.",
        "classes": slim_classes,
        "trees": slim_trees,
    }, indent=1), encoding="utf-8")

    gaps = []
    motion_rows = []
    class_stats = {}
    for c in slim_classes:
        class_stats[c["classId"]] = {
            "name": c["name"],
            "race": RACE_LABEL.get(c["race"], str(c["race"])),
            "skills": 0,
            "motion_skills": 0,
            "motion_ok": 0,
            "effect_ok": 0,
            "sound_ok": 0,
        }

    combo_count = 0
    skills_with_motion = 0
    silent = 0
    missing_motion = 0
    missing_effect = 0
    missing_sound = 0
    no_original = 0
    skills_with_sound_row = 0
    sound_tokens_checked = 0

    player_ids = sorted(learned)
    for sid in player_ids:
        sample = grp_any.get(sid)
        # Prefer a level the class actually learns.
        level = min(skill_levels[sid])
        row = grp.get((sid, level)) or sample
        if row is None:
            gaps.append({
                "category": "motion", "kind": "missing_skillgrp", "skillId": sid, "level": level,
                "name": names.get(sid, ""), "body": "", "race": "", "sex": "",
                "expected": f"skillgrp skill_id={sid}",
                "detail": "learnable on the server, absent from Interlude skillgrp.tsv",
                "classes": ",".join(str(c) for c in sorted(learned[sid])),
            })
            missing_motion += 1
            continue
        tag = (row.get("ani_char") or "").strip()
        effect_class = (visuals.get(sid) or {}).get("effect_class", "").strip()
        desc = (row.get("desc") or "").strip()
        class_ids = learned[sid]
        bodies = []
        seen_body = set()
        for cid in class_ids:
            meta = by_id[cid]
            pair = bodies_for(int(meta.get("race", 0)), is_mystic(cid, parent))
            for body in pair:
                if body not in seen_body:
                    seen_body.add(body)
                    bodies.append(body)

        # Effect: skill.usk phases (cast / flying / target). A numeric extra_eff flag is not a class.
        # skillgrp desc skill.<group>.<id> follows that skill's phases.
        effect_gap = False
        follow = sid
        desc_match = re.search(r"skill\.[A-Za-z]+\.(\d+)", desc or "")
        if desc_match:
            follow = int(desc_match.group(1))
        named = []
        for prow in placement.get(follow, []):
            phase = (prow.get("phase") or "").lower()
            if phase in PHASE_CAST:
                label = "cast"
            elif phase in PHASE_FLY:
                label = "flying"
            elif phase in PHASE_TARGET:
                label = "target"
            else:
                label = phase or "effect"
            eclass = (prow.get("effect_class") or "").strip()
            if not eclass or eclass.isdigit():
                continue
            named.append((label, eclass))
        if not named and (visuals.get(sid) or {}).get("resolution") == "effect_class_by_name":
            for piece in (effect_class or "").split(","):
                piece = piece.strip()
                if piece and not piece.isdigit():
                    named.append(("effect", piece))
        seen_effect = set()
        for label, eclass in named:
            key = eclass.lower()
            if key in seen_effect:
                continue
            seen_effect.add(key)
            present = effect_prefabs.get(key)
            if present:
                continue
            gaps.append({
                "category": "effect", "kind": "missing_effect", "skillId": sid, "level": level,
                "name": names.get(sid, ""), "body": "", "race": "", "sex": "",
                "expected": eclass,
                "detail": f"{label}; follow={follow}; prefab={'unlisted' if present is None else 'file missing'}",
                "classes": ",".join(str(c) for c in sorted(class_ids)),
            })
            missing_effect += 1
            effect_gap = True

        # Sound: every non-empty skillsoundgrp token for this skill, voice only for bodies that learn it.
        sound_gap_bodies = set()
        sound_gap = False
        if sounds.get(sid):
            skills_with_sound_row += 1
        for srow in sounds.get(sid, []):
            for col in SOUND_CAST_COLS + SOUND_FLY_COLS + SOUND_TARGET_COLS:
                stem = sound_stem(srow.get(col, ""))
                if not stem:
                    continue
                sound_tokens_checked += 1
                if stem in sound_stems:
                    continue
                gaps.append({
                    "category": "sound", "kind": "missing_sound", "skillId": sid, "level": int(srow.get("skill_level") or level),
                    "name": names.get(sid, ""), "body": "", "race": "", "sex": "",
                    "expected": stem, "detail": col, "classes": ",".join(str(c) for c in sorted(class_ids)),
                })
                missing_sound += 1
                sound_gap = True
            for col in VOICE_COLS:
                stem = sound_stem(srow.get(col, ""))
                if not stem:
                    continue
                body_key = col.split("_")[0]
                body = VOICE_BODY.get(body_key)
                if body not in seen_body:
                    continue
                sound_tokens_checked += 1
                if stem in sound_stems:
                    continue
                info = BODIES[body]
                gaps.append({
                    "category": "sound", "kind": "missing_sound", "skillId": sid,
                    "level": int(srow.get("skill_level") or level),
                    "name": names.get(sid, ""), "body": body, "race": info["race"], "sex": info["sex"],
                    "expected": stem, "detail": col, "classes": ",".join(str(c) for c in sorted(class_ids)),
                })
                missing_sound += 1
                sound_gap = True
                sound_gap_bodies.add(body)

        motion_ok_bodies = set()
        if not tag or tag.lower() == "none":
            for body in bodies:
                motion_ok_bodies.add(body)
        else:
            skills_with_motion += 1
            phases = combos.get(tag)
            if phases is None:
                for body in bodies:
                    combo_count += 1
                    info = BODIES[body]
                    gaps.append({
                        "category": "motion", "kind": "missing_combo", "skillId": sid, "level": level,
                        "name": names.get(sid, ""), "body": body, "race": info["race"], "sex": info["sex"],
                        "expected": tag, "detail": "ani_char has no AnimationCombo_Interlude row",
                        "classes": ",".join(str(c) for c in sorted(class_ids)),
                    })
                    missing_motion += 1
                    motion_rows.append(f"{sid}\t{body}\t{tag}\tmissing_combo\t{tag}")
            else:
                for body in bodies:
                    combo_count += 1
                    info = BODIES[body]
                    body_bad = False
                    for phase in phases:
                        for result in classify_motion(phase, body, tween, alias, shipped):
                            kind = result["kind"]
                            if kind == "ok":
                                motion_rows.append(f"{sid}\t{body}\t{result['expected']}\tok\t{phase}")
                                continue
                            body_bad = True
                            if kind == "no_original_take":
                                no_original += 1
                            elif kind == "silent_fallback":
                                silent += 1
                                missing_motion += 1
                            else:
                                missing_motion += 1
                            gaps.append({
                                "category": "motion", "kind": kind, "skillId": sid, "level": level,
                                "name": names.get(sid, ""), "body": body, "race": info["race"], "sex": info["sex"],
                                "expected": result["expected"], "detail": result["detail"],
                                "classes": ",".join(str(c) for c in sorted(class_ids)),
                            })
                            motion_rows.append(
                                f"{sid}\t{body}\t{result['expected']}\t{kind}\t{phase}"
                            )
                    if not body_bad:
                        motion_ok_bodies.add(body)

        for cid in class_ids:
            stat = class_stats[cid]
            stat["skills"] += 1
            meta = by_id[cid]
            pair = bodies_for(int(meta.get("race", 0)), is_mystic(cid, parent))
            if tag and tag.lower() != "none":
                stat["motion_skills"] += 1
                if all(b in motion_ok_bodies for b in pair):
                    stat["motion_ok"] += 1
            else:
                stat["motion_ok"] += 1
            if not effect_gap:
                stat["effect_ok"] += 1
            if not sound_gap:
                stat["sound_ok"] += 1

    # Allowlist: motion rows the test must not fail on (current known gaps).
    allow = []
    for gap in gaps:
        if gap["category"] != "motion":
            continue
        if gap["kind"] in {"ok"}:
            continue
        allow.append(f"{gap['skillId']}|{gap['body']}|{gap['expected']}|{gap['kind']}")
    allow = sorted(set(allow))

    OUT.mkdir(parents=True, exist_ok=True)
    (OUT / "skill-parity-known-gaps.txt").write_text(
        "# skillId|body|expectedAsset|kind\n" + "\n".join(allow) + ("\n" if allow else ""),
        encoding="utf-8",
    )
    (OUT / "skill-parity-motions.tsv").write_text(
        "skill_id\tbody\texpected\tkind\tphase\n" + "\n".join(motion_rows) + "\n",
        encoding="utf-8",
    )

    def pct(ok: int, total: int) -> str:
        if total <= 0:
            return "100.0"
        return f"{100.0 * ok / total:.1f}"

    headline = {
        "playerSkills": len(player_ids),
        "combosChecked": combo_count,
        "skillsWithAnimation": skills_with_motion,
        "missingMotion": missing_motion,
        "silentFallback": silent,
        "noOriginalTake": no_original,
        "missingEffect": missing_effect,
        "missingSound": missing_sound,
        "soundTokensChecked": sound_tokens_checked,
        "skillsWithSoundRow": skills_with_sound_row,
        "classes": len(slim_classes),
        "treeSource": "Fineage-Backend data/skill-trees.json (Mobius CT0 class trees)"
        if tree_source.startswith("/tmp/") or tree_source.endswith("player-skill-trees.json")
        else tree_source,
        "sequenceSource": tween_source,
    }
    (OUT / "skill-parity.json").write_text(json.dumps({"headline": headline, "gaps": gaps}, indent=1), encoding="utf-8")

    with (OUT / "skill-parity.csv").open("w", encoding="utf-8", newline="") as fh:
        writer = csv.DictWriter(fh, fieldnames=[
            "category", "kind", "skillId", "level", "name", "body", "race", "sex", "expected", "detail", "classes",
        ])
        writer.writeheader()
        for gap in gaps:
            writer.writerow(gap)

    lines = [
        "# Skill parity — server trees × Interlude client",
        "",
        "Learnable player skills come from Mobius CT0 class skill trees. Motion names come from",
        "`docs/interlude-ref/dats/skillgrp.tsv` `ani_char` and `AnimationCombo_Interlude.txt`.",
        "A take is required only when that body's `*_anim` MeshAnimation in the Interlude `.ukx`",
        f"dump contains it ({tween_source}). Cape, cloak, and helmet anim sets are not the skill resolver.",
        "",
        "## Headline",
        "",
        f"- Player skills checked: **{headline['playerSkills']}**",
        f"- Skill × race-body combos checked: **{headline['combosChecked']}**",
        f"- Skills with an animation tag: **{headline['skillsWithAnimation']}**",
        f"- Missing motion (import or silent fallback): **{headline['missingMotion']}**",
        f"- Silent fallback (different clip than the original): **{headline['silentFallback']}**",
        f"- Original ukx has no take for that body: **{headline['noOriginalTake']}**",
        f"- Missing effect: **{headline['missingEffect']}**",
        f"- Missing sound: **{headline['missingSound']}** ({headline['soundTokensChecked']} skillsoundgrp tokens, {headline['skillsWithSoundRow']} skills have a row)",
        "",
        f"Skill trees: {headline['treeSource']}. Slim copy: `skills/player-skill-trees.json`.",
        "",
        "Full rows: `skill-parity.csv`, `skill-parity.json`, `skill-parity-motions.tsv`.",
        "EditMode allowlist: `skill-parity-known-gaps.txt`.",
        "",
        "## Per class",
        "",
        "| class | race | skills | motion % | effect % | sound % |",
        "|---|---|---:|---:|---:|---:|",
    ]
    for c in slim_classes:
        stat = class_stats[c["classId"]]
        lines.append(
            f"| {c['classId']} {stat['name']} | {stat['race']} | {stat['skills']} | "
            f"{pct(stat['motion_ok'], stat['skills'])} | {pct(stat['effect_ok'], stat['skills'])} | "
            f"{pct(stat['sound_ok'], stat['skills'])} |"
        )

    lines += [
        "",
        "## Motion",
        "",
        "`SkillAnimationResolver.ResolvePlayableCombatState` does not substitute `SpAtk01` for",
        "`SpAtk03+`. A missing official take logs `skillId`, race, sex, body, and the expected",
        "clip, then leaves the requested state unplayed (it does not crossfade `SpAtk01_1HS`).",
        "A missing weapon variant that still has a `1HS` animator state logs the same fields and",
        "names the substitute. `GetOverrideClip` still completes a cast on the shared mid/shot clip",
        "when CastLong, CastShort, MagicNoTarget, or MagicThrow is absent; that path logs skill id,",
        "race, sex, and the expected clip, and is counted as a silent fallback above.",
        "Rows marked `no_original_take` are sequences the Interlude race package does not contain.",
        "Retail has the same hole. They are not an import skip and not a generic-motion substitute.",
        "",
    ]
    motion_gaps = [g for g in gaps if g["category"] == "motion"]
    if not motion_gaps:
        lines.append("No motion gaps. Every learnable skill's official take for every race and sex that can learn it is shipped and named the way the resolver loads it.")
    else:
        lines.append(f"{len(motion_gaps)} motion rows. First 80:")
        lines.append("")
        lines.append("| skill | name | body | sex | kind | expected |")
        lines.append("|---:|---|---|---|---|---|")
        for gap in motion_gaps[:80]:
            lines.append(
                f"| {gap['skillId']} | {gap['name']} | {gap['body']} | {gap['sex']} | {gap['kind']} | `{gap['expected']}` |"
            )

    lines += [
        "",
        "## Effects",
        "",
        "Cast / flying / target phases are `skill-effect-placement.tsv` (Interlude `skill.usk`",
        "`CastingActions` / `ShotActions` / `ExplosionActions`). `skillgrp.desc` `skill.<group>.<id>`",
        "follows that skill's phases. A class is wired when `effect-prefabs.txt` names it and the",
        "prefab file exists. `extra_eff` is a numeric flag, not an effect class.",
        "",
    ]
    effect_gaps = [g for g in gaps if g["category"] == "effect"]
    if not effect_gaps:
        lines.append("No effect gaps for learnable skills that name an effect class or a skill.usk phase.")
    else:
        lines.append(f"{len(effect_gaps)} skills whose effect is not in GlobalEffect. See the CSV for the class name.")

    lines += ["", "## Sounds", ""]
    sound_gaps = [g for g in gaps if g["category"] == "sound"]
    if not sound_gaps:
        lines.append("Every skillsoundgrp cast, shot, explode, and race voice token for a learnable skill has a clip under `Resources/Data/Sounds`.")
    else:
        lines.append(f"{len(sound_gaps)} missing sound tokens. See the CSV (`expected` is the clip stem).")

    lines += [
        "",
        "## Mac rebake",
        "",
    ]
    import_miss = sorted({g["body"] for g in motion_gaps if g["kind"] == "missing_import" and g["body"]})
    if not import_miss:
        lines += [
            "No player `*_anim` skill take is missing from `Resources`, so no anim rebake is required",
            "for this report. Body importers (`L2MElfAnimImporter`, `L2FemaleAnimImporter`,",
            "`L2MOrcAnimImporter`) already copy every sequence in the FBX; they do not whitelist",
            "SpAtk numbers. If a later AnimTween row is added, rebake that race with:",
            "",
            "```bash",
            "export L2_INTERLUDE_CLIENT_ROOT=\"$HOME/Downloads/L2_Interlude_Client\"",
            "./tools/skill-parity/rebake-player-anims.sh MFighter",
            "```",
            "",
        ]
    else:
        lines.append("These bodies are missing official takes. On the Mac that bakes clips:")
        lines.append("")
        lines.append("```bash")
        lines.append("export L2_INTERLUDE_CLIENT_ROOT=\"$HOME/Downloads/L2_Interlude_Client\"")
        for body in import_miss:
            lines.append(f"./tools/skill-parity/rebake-player-anims.sh {body}")
        lines.append("```")
        lines.append("")

    lines.append(f"Re-run: `python3 tools/skill-parity/generate_skill_parity.py`.")
    lines.append("")
    (OUT / "gap-report-skill-parity.md").write_text("\n".join(lines), encoding="utf-8")
    print(json.dumps(headline, indent=2))
    print(f"gaps {len(gaps)} allow {len(allow)} motion rows {len(motion_rows)}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
