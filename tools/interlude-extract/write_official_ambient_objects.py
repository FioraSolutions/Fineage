#!/usr/bin/env python3
"""Write official AmbientSoundObject / AmbientSound placement from #1670 map dumps."""
from __future__ import annotations

import csv
import sys
import gzip
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
from write_ambient_sounds_json import DUMPS, load_dump, first, import_unreal_path  # noqa: E402

ROOT = Path(__file__).resolve().parents[2]
STREAM = ROOT / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "Sounds"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "sounds"

TYPE_NAMES = {
    0: "AST1_Always",
    1: "AST1_Day",
    2: "AST1_Night",
    3: "AST1_Water",
    4: "AST1_RangeTime",
    "0": "AST1_Always",
    "1": "AST1_Day",
    "2": "AST1_Night",
    "3": "AST1_Water",
    "4": "AST1_RangeTime",
    "AST1_Always": "AST1_Always",
    "AST1_Day": "AST1_Day",
    "AST1_Night": "AST1_Night",
    "AST1_Water": "AST1_Water",
    "AST1_RangeTime": "AST1_RangeTime",
}

FIELDS = ["map", "name", "sound", "x", "y", "z", "radius", "volume", "pitch", "type", "random", "start"]


def map_type(typ) -> str:
    if typ is None or typ == "":
        return "AST1_Always"
    if typ in TYPE_NAMES:
        return TYPE_NAMES[typ]
    if isinstance(typ, float) and typ == int(typ) and int(typ) in TYPE_NAMES:
        return TYPE_NAMES[int(typ)]
    return str(typ)


def tile_id(path: Path) -> str:
    name = path.name
    if name.endswith(".unr.json.gz"):
        return name[: -len(".unr.json.gz")]
    if name.endswith(".unr.json"):
        return name[: -len(".unr.json")]
    return path.stem


def collect(tile: str, data: dict) -> tuple[list[dict], int]:
    rows = []
    missing_sound = 0
    for e in data.get("exports") or []:
        if e.get("class") not in ("AmbientSoundObject", "AmbientSound"):
            continue
        pmap = {p.get("name") or "": p for p in e.get("properties") or []}
        snd = pmap.get("AmbientSound")
        sound_name = import_unreal_path(data, snd["index"]) if snd and snd.get("index") else ""
        if not sound_name:
            missing_sound += 1
        loc = first(pmap, "Location") or {}
        rows.append({
            "map": tile,
            "name": e.get("name") or "",
            "sound": sound_name,
            "x": loc.get("x", 0),
            "y": loc.get("y", 0),
            "z": loc.get("z", 0),
            "radius": first(pmap, "SoundRadius") or 0,
            "volume": first(pmap, "SoundVolume") or 0,
            "pitch": first(pmap, "SoundPitch") or 0,
            "type": map_type(first(pmap, "AmbientSoundType")),
            "random": first(pmap, "AmbientRandom") or 0,
            "start": first(pmap, "AmbientSoundStartTime") or 0,
        })
    return rows, missing_sound


def main() -> int:
    dumps = sorted(set(DUMPS.glob("*.unr.json")) | set(DUMPS.glob("*.unr.json.gz")))
    # prefer .gz when both exist? load_dump checks gz first then json
    tiles = {}
    for p in dumps:
        tiles[tile_id(p)] = p
    all_rows = []
    missing = 0
    for tile in sorted(tiles):
        data = load_dump(tile)
        rows, miss = collect(tile, data)
        missing += miss
        all_rows.extend(rows)
        print(f"{tile}: {len(rows)}", file=sys.stderr)
    STREAM.mkdir(parents=True, exist_ok=True)
    dest = STREAM / "official-ambient-objects.tsv"
    with dest.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=FIELDS, delimiter="\t", lineterminator="\n")
        w.writeheader()
        for row in all_rows:
            w.writerow(row)
    gz = dest.with_suffix(".tsv.gz")
    import gzip
    with gzip.open(gz, "wb", compresslevel=9) as zf:
        zf.write(dest.read_bytes())
    dest.unlink()
    print(f"rows={len(all_rows)} tiles={len(tiles)} missing_sound={missing} -> {gz}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
