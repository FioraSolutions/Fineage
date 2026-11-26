#!/usr/bin/env python3
"""Write official MusicVolume placement (world-space brush polys) from regenerated #1670 dumps."""
from __future__ import annotations

import gzip
import json
import math
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DUMPS = ROOT / "l2-unity/docs/interlude-ref/world/maps"
ENV = ROOT / "l2-unity/docs/interlude-ref/world/env"
STREAM = ROOT / "l2-unity/Assets/StreamingAssets/Data/Meta/Sounds"
DOCS = ROOT / "l2-unity/docs/interlude-ref/sounds"


def load_dump(path: Path) -> dict:
    opener = gzip.open if path.suffix == ".gz" or path.name.endswith(".json.gz") else open
    with opener(path, "rt", encoding="utf-8") as f:
        return json.load(f)


def vec(d, default=None):
    if not d:
        return default
    return {"x": float(d.get("x", 0)), "y": float(d.get("y", 0)), "z": float(d.get("z", 0))}


def add(a, b):
    return {"x": a["x"] + b["x"], "y": a["y"] + b["y"], "z": a["z"] + b["z"]}


def sub(a, b):
    return {"x": a["x"] - b["x"], "y": a["y"] - b["y"], "z": a["z"] - b["z"]}


def mul(a, s):
    if isinstance(s, dict):
        return {"x": a["x"] * s["x"], "y": a["y"] * s["y"], "z": a["z"] * s["z"]}
    return {"x": a["x"] * s, "y": a["y"] * s, "z": a["z"] * s}


def rotate(v, rot):
    """UE rotator: pitch/yaw/roll in Unreal rotation units (65536 = 360°)."""
    if not rot:
        return v
    pitch = float(rot.get("pitch") or 0) * (2 * math.pi / 65536.0)
    yaw = float(rot.get("yaw") or 0) * (2 * math.pi / 65536.0)
    roll = float(rot.get("roll") or 0) * (2 * math.pi / 65536.0)
    if pitch == 0 and yaw == 0 and roll == 0:
        return v
    # Unreal: rotation applied as yaw (Z), then pitch (Y), then roll (X) in UE axes.
    x, y, z = v["x"], v["y"], v["z"]
    # roll around X
    if roll:
        cy, sy = math.cos(roll), math.sin(roll)
        y, z = y * cy - z * sy, y * sy + z * cy
    # pitch around Y
    if pitch:
        cy, sy = math.cos(pitch), math.sin(pitch)
        x, z = x * cy + z * sy, -x * sy + z * cy
    # yaw around Z
    if yaw:
        cy, sy = math.cos(yaw), math.sin(yaw)
        x, y = x * cy - y * sy, x * sy + y * cy
    return {"x": x, "y": y, "z": z}


def to_world(local, loc, pivot, scale, rot):
    v = sub(local, pivot)
    v = mul(v, scale)
    v = rotate(v, rot)
    return add(loc, v)


def collect(tile: str, data: dict) -> list[dict]:
    rows = []
    missing = 0
    for e in data.get("exports") or []:
        if e.get("class") != "MusicVolume":
            continue
        vol = e.get("volume") or {}
        model = vol.get("model") or {}
        loc = vec(vol.get("location"), {"x": 0, "y": 0, "z": 0})
        pivot = vec(vol.get("prepivot"), {"x": 0, "y": 0, "z": 0})
        scale = vec(vol.get("scale"), {"x": 1, "y": 1, "z": 1})
        rot = vol.get("rotation") or {}
        polys = []
        for poly in model.get("polys") or []:
            verts = [to_world(vec(v, {"x": 0, "y": 0, "z": 0}), loc, pivot, scale, rot) for v in poly.get("vertices") or []]
            if len(verts) < 3:
                continue
            nrm = vec(poly.get("normal"), None)
            if nrm:
                nrm = rotate(mul(nrm, scale if scale else {"x": 1, "y": 1, "z": 1}), rot)
            polys.append({"normal": nrm, "vertices": verts})
        if not polys:
            missing += 1
        # AABB
        xs = [v["x"] for p in polys for v in p["vertices"]]
        ys = [v["y"] for p in polys for v in p["vertices"]]
        zs = [v["z"] for p in polys for v in p["vertices"]]
        rows.append({
            "map": tile,
            "name": e.get("name") or "",
            "musicId": int(vol.get("music_id") or 0),
            "hasNightMusic": bool(vol.get("has_night_music")),
            "nightMusicId": int(vol.get("night_music_id") or 0),
            "loopMusic": bool(vol.get("loop_music")),
            "forcePlayMusic": bool(vol.get("force_play_music")),
            "min": {"x": min(xs), "y": min(ys), "z": min(zs)} if xs else loc,
            "max": {"x": max(xs), "y": max(ys), "z": max(zs)} if xs else loc,
            "polys": polys,
        })
    if missing:
        print(f"ERROR {tile}: {missing} MusicVolume(s) missing brush polys", file=sys.stderr)
    return rows


def tile_id(path: Path) -> str:
    name = path.name
    for suf in (".unr.json.gz", ".unr.json"):
        if name.endswith(suf):
            return name[: -len(suf)]
    return path.stem


def main() -> int:
    files = sorted(DUMPS.glob("*.unr.json.gz")) + sorted(DUMPS.glob("*.unr.json"))
    files += sorted(ENV.glob("*.unr.json.gz")) + sorted(ENV.glob("*.unr.json"))
    # unique by tile
    by = {}
    for p in files:
        by[tile_id(p)] = p
    all_rows = []
    for tile in sorted(by):
        rows = collect(tile, load_dump(by[tile]))
        all_rows.extend(rows)
        if rows:
            print(f"{tile}: {len(rows)}", file=sys.stderr)
    STREAM.mkdir(parents=True, exist_ok=True)
    payload = {"count": len(all_rows), "volumes": all_rows}
    raw = json.dumps(payload, separators=(",", ":")).encode("utf-8")
    dest = STREAM / "official-music-volumes.json.gz"
    with gzip.open(dest, "wb", compresslevel=9) as f:
        f.write(raw)
    print(f"rows={len(all_rows)} tiles={len(by)} -> {dest} ({dest.stat().st_size} bytes)")
    return 0 if all_rows else 1


if __name__ == "__main__":
    raise SystemExit(main())
