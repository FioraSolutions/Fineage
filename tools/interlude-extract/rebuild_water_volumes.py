#!/usr/bin/env python3
"""Rebuild Maps/<tile>/Meta/watervolumes.json from official WaterVolume bounds
and the official water shader imported by each map (#1670 dump).

Volume brushes themselves are often untextured physics boxes; the map import
table names the official surface shader (almost always
FX_E_T.WaterSurfaceShaderSet.WaterShader01). Missing shader textures must log
an error at bake time — this script never invents a substitute.

Usage:
  java --source 17 -cp tools/interlude/L2TerrainExtractor/build/libs/l2terrain-1.0.0-all.jar \\
    tools/interlude/WaterVolumeDump.java <map.unr> <out.json>
  python3 tools/interlude-extract/rebuild_water_volumes.py \\
    --maps-dir "$CLIENT/maps" --jar <fatjar>
"""
from __future__ import annotations

import argparse
import gzip
import json
import os
import subprocess
import sys
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MAPS = REPO / "l2-unity/Assets/Resources/Data/Maps"
DUMPS = REPO / "l2-unity/docs/interlude-ref/world/maps"
DEFAULT_SHADER = "FX_E_T.WaterSurfaceShaderSet.WaterShader01"
SHADER_CLASSES = {"Shader", "FinalBlend", "TexPanner", "TexEnvMap", "Combiner"}
SHADER_NAMES = {
    "WaterShader01",
    "WaterShaderPan01",
    "WaterFinal",
    "ssq_water_01sh",
}


def load_dump(tile: str) -> dict | None:
    p = DUMPS / f"{tile}.unr.json.gz"
    if not p.exists():
        p = DUMPS / f"{tile}.unr.json"
    if not p.exists():
        return None
    opener = gzip.open if p.suffix == ".gz" else open
    with opener(p, "rt") as f:
        return json.load(f)


def official_shaders(dump: dict) -> list[str]:
    found: list[str] = []
    for im in dump.get("imports") or []:
        name = im.get("object_name") or ""
        cls = im.get("class_name") or ""
        pkg = im.get("package") or ""
        if cls not in SHADER_CLASSES:
            continue
        if name not in SHADER_NAMES and "Water" not in name and "water" not in name:
            continue
        # package field is often "import:Core:Package:WaterSurfaceShaderSet"
        leaf = pkg.split(":")[-1] if pkg else ""
        if leaf in ("WaterSurfaceShaderSet", "FX_E_T", "WaterSurfaceSet"):
            if leaf == "WaterSurfaceShaderSet":
                full = f"FX_E_T.WaterSurfaceShaderSet.{name}"
            elif leaf == "WaterSurfaceSet":
                full = f"FX_E_T.WaterSurfaceSet.{name}"
            else:
                full = f"FX_E_T.{name}"
        else:
            full = f"{leaf}.{name}" if leaf else name
        if full not in found:
            found.append(full)
    # stable: WaterShader01 first when present
    def key(s: str) -> tuple:
        return (0 if s.endswith("WaterShader01") else 1, s)
    return sorted(found, key=key)


def write_meta(json_path: Path) -> None:
    meta = json_path.with_suffix(".json.meta")
    if meta.exists():
        return
    guid = uuid.uuid4().hex
    meta.write_text(
        "fileFormatVersion: 2\n"
        f"guid: {guid}\n"
        "TextScriptImporter:\n"
        "  externalObjects: {}\n"
        "  userData: \n"
        "  assetBundleName: \n"
        "  assetBundleVariant: \n"
    )


def tile_ids() -> list[str]:
    return sorted(
        p.name
        for p in MAPS.iterdir()
        if p.is_dir() and "_" in p.name and p.name[0].isdigit()
    )


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--maps-dir", type=Path, required=True, help="Official client maps/ directory")
    ap.add_argument("--jar", type=Path, required=True, help="l2terrain-*-all.jar")
    ap.add_argument("--dump-java", type=Path, default=REPO / "tools/interlude/WaterVolumeDump.java")
    args = ap.parse_args()
    if not args.jar.is_file():
        print(f"ERROR: jar not found: {args.jar}", file=sys.stderr)
        return 1
    tmp = REPO / ".work" / "water-volumes-1689"
    tmp.mkdir(parents=True, exist_ok=True)

    ok = 0
    missing_maps = []
    missing_dump = []
    no_shader = []
    summary = []
    for tile in tile_ids():
        dump = load_dump(tile)
        if dump is None:
            missing_dump.append(tile)
            print(f"ERROR {tile}: official #1670 dump not found")
            continue
        n_off = sum(1 for e in dump.get("exports") or [] if e.get("class") == "WaterVolume")
        if n_off == 0:
            summary.append((tile, 0, 0, []))
            continue
        unr = args.maps_dir / f"{tile}.unr"
        if not unr.is_file():
            missing_maps.append(tile)
            print(f"ERROR {tile}: official map not found: {unr}")
            continue
        raw_json = tmp / f"{tile}.json"
        r = subprocess.run(
            ["java", "--source", "17", "-cp", str(args.jar), str(args.dump_java), str(unr), str(raw_json)],
            capture_output=True,
            text=True,
        )
        if r.returncode != 0 or not raw_json.is_file():
            print(f"ERROR {tile}: WaterVolumeDump failed: {r.stdout} {r.stderr}")
            continue
        data = json.loads(raw_json.read_text())
        vols = data.get("volumes") or []
        if len(vols) != n_off:
            print(f"ERROR {tile}: dump wrote {len(vols)} volumes, official export count is {n_off}")
            continue
        shaders = official_shaders(dump)
        if not shaders:
            no_shader.append(tile)
            print(f"ERROR {tile}: official dump has {n_off} WaterVolumes but no water Shader/FinalBlend import")
        data["material"] = shaders[0] if shaders else None
        data["materials"] = shaders
        meta_dir = MAPS / tile / "Meta"
        meta_dir.mkdir(parents=True, exist_ok=True)
        out = meta_dir / "watervolumes.json"
        out.write_text(json.dumps(data, indent=2) + "\n")
        write_meta(out)
        ok += 1
        summary.append((tile, n_off, len(vols), shaders))
        print(f"{tile}: {len(vols)} volumes, material={data['material']}")

    print(f"\nwrote {ok} watervolumes.json files")
    if missing_maps or missing_dump or no_shader:
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
