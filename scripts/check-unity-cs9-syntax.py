#!/usr/bin/env python3
"""Fail when l2-unity/Assets contains C# syntax newer than Unity's C# 9 compiler."""

from __future__ import annotations

import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
ASSETS = REPO_ROOT / "l2-unity/Assets"

# Allow running when invoked from repo root: `python3 scripts/check-unity-cs9-syntax.py`
sys.path.insert(0, str(REPO_ROOT / "scripts"))
from net_contract_unity_cs9 import scan_assets_cs10_plus  # noqa: E402


def main() -> int:
    errors = scan_assets_cs10_plus(ASSETS)
    if errors:
        print("Unity C# 9 syntax check failed (l2-unity/Assets):", file=sys.stderr)
        for line in errors:
            print(f"  - {line}", file=sys.stderr)
        print(
            "\nNet contract files must go through `scripts/sync-net-contract.py --write`.",
            file=sys.stderr,
        )
        return 1
    print("Unity C# 9 syntax OK (l2-unity/Assets)")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())