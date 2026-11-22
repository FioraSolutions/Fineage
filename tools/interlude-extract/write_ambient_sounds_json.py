#!/usr/bin/env python3
"""Write Maps/<tile>/Meta/<tile>_AmbientSounds.json from the #1670 official map dump.

Same schema as existing TI tiles (e.g. 17_24_AmbientSounds.json). Sound package
paths are resolved from the dump import table (AmbSound.Waves.small_03, …).
"""
from __future__ import annotations

import argparse
import gzip
import json
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMPS = ROOT / "l2-unity/docs/interlude-ref/world/maps"
MAPS = ROOT / "l2-unity/Assets/Resources/Data/Maps"


def load_dump(tile: str) -> dict:
    for p in (DUMPS / f"{tile}.unr.json.gz", DUMPS / f"{tile}.unr.json"):
        if p.exists():
            opener = gzip.open if str(p).endswith(".gz") else open
            with opener(p, "rt", encoding="utf-8") as f:
                return json.load(f)
    raise SystemExit(f"ERROR {tile}: official dump missing under {DUMPS}")


def import_by_ue_index(data: dict, index: int):
    if index is None or index >= 0:
        return None
    imps = data.get("imports") or []
    i = -index - 1
    if i < 0 or i >= len(imps):
        return None
    return imps[i]


def import_unreal_path(data: dict, index: int) -> str:
    names: list[str] = []
    walk = index
    guard = 0
    while walk is not None and walk < 0 and guard < 8:
        imp = import_by_ue_index(data, walk)
        if not imp:
            break
        names.append(imp.get("object_name") or "")
        walk = imp.get("package_index")
        if walk == 0:
            break
        guard += 1
    return ".".join(reversed([n for n in names if n]))


def first(pmap: dict, name: str):
    p = pmap.get(name)
    if not p:
        return None
    if "value" in p and p["value"] is not None:
        return p["value"]
    return p.get("ref") or p.get("vector") or p.get("rotator")


def build_ambient(tile: str, data: dict) -> dict:
    rows = []
    missing_sound = 0
    for e in data.get("exports") or []:
        if e.get("class") != "AmbientSoundObject":
            continue
        pmap = {p.get("name") or "": p for p in e.get("properties") or []}
        snd = pmap.get("AmbientSound")
        sound_name = import_unreal_path(data, snd["index"]) if snd and snd.get("index") else ""
        if not sound_name:
            missing_sound += 1
        loc = first(pmap, "Location") or {}
        typ = first(pmap, "AmbientSoundType")
        if typ is None:
            typ = "AST1_Always"
        elif isinstance(typ, int):
            typ = str(typ)
        rows.append({
            "name": f"{tile}_Classic.{e.get('name')}",
            "ambientSoundName": sound_name,
            "ambientSoundType": typ,
            "ambientSoundStartTime": first(pmap, "AmbientSoundStartTime") or 0,
            "ambientRandom": first(pmap, "AmbientRandom") or 0,
            "soundRadius": first(pmap, "SoundRadius") or 0,
            "soundVolume": first(pmap, "SoundVolume") or 0,
            "soundPitch": first(pmap, "SoundPitch") or 0,
            "position": {"x": loc.get("x", 0), "y": loc.get("y", 0), "z": loc.get("z", 0)},
            "groupName": first(pmap, "Group") or "None",
        })
    if missing_sound:
        print(f"ERROR {tile}: {missing_sound} AmbientSoundObject(s) missing AmbientSound ref", file=sys.stderr)
    return {"data": rows}


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("tiles", nargs="+")
    args = ap.parse_args()
    for tile in args.tiles:
        data = load_dump(tile)
        out = build_ambient(tile, data)
        dest = MAPS / tile / "Meta" / f"{tile}_AmbientSounds.json"
        dest.parent.mkdir(parents=True, exist_ok=True)
        dest.write_text(json.dumps(out, separators=(",", ":")), encoding="utf-8")
        print(f"{tile}: {len(out['data'])} ambients -> {dest}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
