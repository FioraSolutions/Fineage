#!/usr/bin/env bash
set -euo pipefail
REPO="$(cd "$(dirname "$0")/../.." && pwd)"
CLIENT="${L2_INTERLUDE_CLIENT_ROOT:-/Users/nikitarakitin/Downloads/L2_Interlude_Client/Lineage II}"
EXPORT="${L2_UMODEL_EXPORT_DIR:-$REPO/.work/umodel-export-22_22}"
UMODEL="$REPO/l2-unity/External/UEViewer/umodel"
STATIC_JSON="$REPO/.work/interlude-extract/22_22/staticmeshes.json"
REPO_SM="$REPO/l2-unity/Assets/Resources/Data/StaticMeshes"

mkdir -p "$EXPORT"
export REPO EXPORT CLIENT STATIC_JSON REPO_SM

python3 <<'PY'
import json, os, subprocess
from pathlib import Path
repo = Path(os.environ["REPO"])
export = Path(os.environ["EXPORT"])
client = Path(os.environ["CLIENT"])
umodel = repo / "l2-unity/External/UEViewer/umodel"
static_json = Path(os.environ["STATIC_JSON"])
repo_sm = Path(os.environ["REPO_SM"])
data = json.loads(static_json.read_text())
pkgs = sorted({m["package"] for m in data["staticMeshes"]})
usx_names = []
for pkg in pkgs:
    for cand in (f"{pkg}.usx", f"{pkg}.USX"):
        if (client / "staticmeshes" / cand).is_file():
            usx_names.append(cand)
            break
    else:
        print("skip missing usx", pkg)
if usx_names:
    inner = "apt-get update -qq && apt-get install -y -qq libpng16-16 libsdl2-2.0-0 >/dev/null\n"
    for name in usx_names:
        inner += f'/umodel -path=/client -game=l2 -export -out=/export staticmeshes/{name}\n'
    print("umodel packages:", len(usx_names))
    subprocess.run([
        "docker", "run", "--rm", "--platform", "linux/amd64",
        "-v", f"{client}:/client:ro",
        "-v", f"{export}:/export",
        "-v", f"{umodel}:/umodel:ro",
        "ubuntu:22.04", "bash", "-lc", inner,
    ], check=True)
for m in data["staticMeshes"]:
    pkg, mesh = m["package"], m["mesh"]
    src = repo_sm / pkg / f"{mesh}.fbx"
    if not src.is_file():
        continue
    dest = export / pkg / f"{mesh}.fbx"
    dest.parent.mkdir(parents=True, exist_ok=True)
    if not dest.exists():
        os.symlink(src, dest)
print("done export root", export)
PY
