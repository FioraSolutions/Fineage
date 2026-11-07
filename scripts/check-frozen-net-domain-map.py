#!/usr/bin/env python3
"""Assert NetDomainMap.cs stays deleted (split into Net*Map adapters)."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_CS = (
    REPO_ROOT
    / "l2-unity/Assets/Scripts/Networking/Net/Domain/NetDomainMap.cs"
)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--cs", type=Path, default=DEFAULT_CS)
    args = parser.parse_args()

    if args.cs.is_file():
        print(
            "NetDomainMap.cs was dissolved into NetEntityMap / NetCombatMap / "
            "NetGearMap / NetSocialMap — do not restore the god class. "
            f"Found: {args.cs}",
            file=sys.stderr,
        )
        return 1

    print("ok: NetDomainMap.cs remains deleted")
    return 0


if __name__ == "__main__":
    sys.exit(main())
