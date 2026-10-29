#!/usr/bin/env python3
import json
import os
from pathlib import Path

repo = Path(__file__).resolve().parents[2]
tile = os.environ.get("FINEAGE_INTERLUDE_TILE", "22_22")
export = Path(os.environ.get("L2_UMODEL_EXPORT_DIR", repo / f".work/umodel-export-{tile}"))
static_json = repo / f".work/interlude-extract/{tile}/staticmeshes.json"
data = json.loads(static_json.read_text())
pkgs = sorted({m["package"] for m in data["staticMeshes"]})
lower_dirs = {d.name.lower(): d.name for d in export.iterdir() if d.is_dir()}
for pkg in pkgs:
    target = export / pkg
    if target.exists():
        continue
    low = lower_dirs.get(pkg.lower())
    if not low:
        continue
    os.symlink(low, target)
    print("link", pkg, "->", low)
print("done")
