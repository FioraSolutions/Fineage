#!/usr/bin/env python3
"""CI guard: LineageWeapons FBX referenced by weapongrp must not be single-sided cardboard."""
from __future__ import annotations

import subprocess
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
AUDIT = REPO / "tools/interlude/weapon_mesh_solidify.py"


def main() -> int:
    proc = subprocess.run(
        [sys.executable, str(AUDIT), "--audit"],
        cwd=REPO,
        capture_output=True,
        text=True,
    )
    sys.stdout.write(proc.stdout)
    sys.stderr.write(proc.stderr)
    if proc.returncode != 0:
        print(
            "Weapon mesh topology check failed. Repair with:\n"
            "  python3 tools/interlude/weapon_mesh_solidify.py --fix\n"
            "or on Mac:\n"
            "  ./tools/interlude/export_lineage_weapons.sh",
            file=sys.stderr,
        )
    return proc.returncode


if __name__ == "__main__":
    raise SystemExit(main())