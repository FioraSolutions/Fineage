#!/usr/bin/env python3
"""Assert dissolved NetDomainMap adapters stay deleted (client C1 freeze)."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DOMAIN_DIR = REPO_ROOT / "l2-unity/Assets/Scripts/Networking/Net/Domain"

FROZEN_MAPS = (
    "NetDomainMap.cs",
    "NetCombatMap.cs",
    "NetSocialMap.cs",
    "NetGearMap.cs",
    "NetEntityMap.cs",
)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--domain-dir",
        type=Path,
        default=DOMAIN_DIR,
        help="Net/Domain folder that must not contain frozen map types",
    )
    args = parser.parse_args()

    violations: list[Path] = []
    for name in FROZEN_MAPS:
        path = args.domain_dir / name
        if path.is_file():
            violations.append(path)

    if violations:
        print(
            "Frozen Net domain map types were deleted under C1 — do not restore them.",
            file=sys.stderr,
        )
        for path in violations:
            print(f"  found: {path}", file=sys.stderr)
        return 1

    print("ok: frozen Net domain map files remain deleted")
    return 0


if __name__ == "__main__":
    sys.exit(main())
