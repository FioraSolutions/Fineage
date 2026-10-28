#!/usr/bin/env python3
"""Fail CI when public static properties or methods change on IncomingPacketActions (#137)."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_CS = (
    REPO_ROOT
    / "l2-unity/Assets/Scripts/Networking/ClientLibrary/Packet/Incoming/IncomingPacketActions.cs"
)
DEFAULT_SNAPSHOT = REPO_ROOT / "scripts/incoming-packet-actions.surface.txt"


def _strip_line_comments(line: str) -> str:
    return re.sub(r"//.*$", "", line)


def extract_public_static_surface(source: str) -> list[tuple[str, str]]:
    """Return sorted (kind, name) pairs: kind is 'property' or 'method'."""
    lines = [_strip_line_comments(line) for line in source.splitlines()]
    members: list[tuple[str, str]] = []
    i = 0
    while i < len(lines):
        stripped = lines[i].strip()
        if not stripped.startswith("public static") or "class IncomingPacketActions" in stripped:
            i += 1
            continue

        chunk = stripped
        while i + 1 < len(lines) and "(" not in chunk and "{" not in chunk:
            i += 1
            chunk += " " + lines[i].strip()

        if "(" in chunk:
            before = chunk[: chunk.index("(")]
            name = before.strip().split()[-1]
            members.append(("method", name))
        elif "{" in chunk:
            before = chunk[: chunk.index("{")]
            name = before.strip().split()[-1]
            members.append(("property", name))
        else:
            print(
                f"check-frozen-incoming-packet-actions: could not parse member near line {i + 1}",
                file=sys.stderr,
            )
            return []

        i += 1

    members.sort(key=lambda item: (item[1], item[0]))
    return members


def format_surface(members: list[tuple[str, str]]) -> str:
    return "\n".join(f"{kind}\t{name}" for kind, name in members) + "\n"


def parse_snapshot(text: str) -> list[tuple[str, str]]:
    members: list[tuple[str, str]] = []
    for line in text.splitlines():
        line = line.strip()
        if not line or line.startswith("#"):
            continue
        parts = line.split("\t", 1)
        if len(parts) != 2 or parts[0] not in ("property", "method"):
            print(f"invalid snapshot line: {line!r}", file=sys.stderr)
            sys.exit(2)
        members.append((parts[0], parts[1]))
    members.sort(key=lambda item: (item[1], item[0]))
    return members


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--cs", type=Path, default=DEFAULT_CS)
    parser.add_argument("--snapshot", type=Path, default=DEFAULT_SNAPSHOT)
    parser.add_argument(
        "--write-snapshot",
        action="store_true",
        help="Write current surface to the snapshot file (maintainer use).",
    )
    args = parser.parse_args()

    if not args.cs.is_file():
        print(f"missing source file: {args.cs}", file=sys.stderr)
        return 2

    current = extract_public_static_surface(args.cs.read_text(encoding="utf-8"))
    if not current:
        return 2

    if args.write_snapshot:
        args.snapshot.write_text(format_surface(current), encoding="utf-8")
        print(f"wrote {args.snapshot}")
        return 0

    if not args.snapshot.is_file():
        print(f"missing snapshot: {args.snapshot}", file=sys.stderr)
        return 2

    expected = parse_snapshot(args.snapshot.read_text(encoding="utf-8"))
    if current == expected:
        return 0

    current_set = set(current)
    expected_set = set(expected)
    added = sorted(current_set - expected_set, key=lambda x: (x[1], x[0]))
    removed = sorted(expected_set - current_set, key=lambda x: (x[1], x[0]))

    print(
        "IncomingPacketActions public static surface changed "
        "(frozen per #137 — update snapshot only with an intentional arch review):",
        file=sys.stderr,
    )
    for kind, name in added:
        print(f"  + {kind} {name}", file=sys.stderr)
    for kind, name in removed:
        print(f"  - {kind} {name}", file=sys.stderr)
    print(f"snapshot: {args.snapshot}", file=sys.stderr)
    return 1


if __name__ == "__main__":
    sys.exit(main())
