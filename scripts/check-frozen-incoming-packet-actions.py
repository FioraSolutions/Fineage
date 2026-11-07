#!/usr/bin/env python3
"""Assert IncomingPacketActions.cs stays deleted (#142 completed the #137 freeze)."""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_CS = (
    REPO_ROOT
    / "l2-unity/Assets/Scripts/Networking/ClientLibrary/Packet/Incoming/IncomingPacketActions.cs"
)


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--cs", type=Path, default=DEFAULT_CS)
    args = parser.parse_args()

    if args.cs.is_file():
        print(
            "IncomingPacketActions.cs was deleted under #142 — do not restore it. "
            f"Found: {args.cs}",
            file=sys.stderr,
        )
        return 1

    print("ok: IncomingPacketActions.cs remains deleted")
    return 0


if __name__ == "__main__":
    sys.exit(main())
