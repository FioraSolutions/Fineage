#!/usr/bin/env python3
"""Extract LineageEffect .uc files for generate entries in the Human Mage VFX manifest.

Requires:
  - decoded lineageeffect.u (default: .work/umodel-prominence/lineageeffect.dec.u)
  - built tools/l2-effect/UeDecompile

Usage:
  python3 tools/l2-effect/extract_human_mage_ucs.py
"""

from __future__ import annotations

import json
import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MANIFEST = (
    REPO
    / "l2-unity/Assets/Editor/L2EffectGenerator/Data/human_mage_vfx_manifest.json"
)
EFFECTS = REPO / "l2-unity/Assets/Resources/Data/Effects"
DEFAULT_PACKAGE = REPO / ".work/umodel-prominence/lineageeffect.dec.u"
UEDECOMPILE = (
    REPO / "tools/l2-effect/UeDecompile/bin/Release/net10.0/UeDecompile"
)
# Fallback to net8 / dll host
UEDECOMPILE_DLL = (
    REPO / "tools/l2-effect/UeDecompile/bin/Release/net10.0/UeDecompile.dll"
)


def ensure_built() -> list[str]:
    proj = REPO / "tools/l2-effect/UeDecompile/UeDecompile.csproj"
    subprocess.check_call(
        ["dotnet", "build", str(proj), "-c", "Release", "-v", "q"],
        cwd=str(REPO),
    )
    if UEDECOMPILE.exists():
        return [str(UEDECOMPILE)]
    if UEDECOMPILE_DLL.exists():
        return ["dotnet", str(UEDECOMPILE_DLL)]
    raise SystemExit("UeDecompile binary not found after build")


def main() -> int:
    package = Path(sys.argv[1]) if len(sys.argv) > 1 else DEFAULT_PACKAGE
    if not package.is_file():
        raise SystemExit(f"decoded package missing: {package}")

    manifest = json.loads(MANIFEST.read_text())
    cmd_base = ensure_built()

    # Collect unique class names across generate entries (skip *_sound optional)
    classes: list[str] = []
    seen: set[str] = set()
    for entry in manifest["entries"]:
        if entry["mode"] != "generate":
            continue
        for c in entry["effectClasses"]:
            if c in seen:
                continue
            seen.add(c)
            classes.append(c)

    # Dump all classes into a staging dir, then copy into per-visual folders
    staging = REPO / ".work/human-mage-uc-staging"
    staging.mkdir(parents=True, exist_ok=True)

    # Batch in chunks to keep argv reasonable
    chunk = 40
    ok = 0
    missing = []
    for i in range(0, len(classes), chunk):
        batch = classes[i : i + chunk]
        proc = subprocess.run(
            cmd_base + [str(package), str(staging)] + batch,
            capture_output=True,
            text=True,
        )
        sys.stdout.write(proc.stdout)
        sys.stderr.write(proc.stderr)
        for line in (proc.stdout + proc.stderr).splitlines():
            if line.startswith("OK "):
                ok += 1
            if line.startswith("MISSING "):
                missing.append(line.split(" ", 1)[1].strip())

    # Materialize per-visual effect folders (flat .uc at root; one-level also OK)
    written = 0
    for entry in manifest["entries"]:
        if entry["mode"] != "generate":
            continue
        folder = EFFECTS / entry["folderName"]
        folder.mkdir(parents=True, exist_ok=True)
        for c in entry["effectClasses"]:
            src = staging / f"{c}.uc"
            if not src.is_file():
                continue
            dest = folder / f"{c}.uc"
            dest.write_text(src.read_text())
            written += 1

    print(
        json.dumps(
            {
                "classesRequested": len(classes),
                "decompileOkLines": ok,
                "missing": missing,
                "ucFilesWritten": written,
                "staging": str(staging),
            },
            indent=2,
        )
    )
    return 0 if not missing else 0  # missing classes logged; continue batch


if __name__ == "__main__":
    raise SystemExit(main())
