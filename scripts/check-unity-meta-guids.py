#!/usr/bin/env python3
"""Fail CI when any Unity .meta guid is not exactly 32 lowercase hex chars or duplicates another."""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_ROOT = REPO_ROOT / "l2-unity"

GUID_LINE = re.compile(r"^guid:\s*([0-9a-fA-F]+)\s*$", re.MULTILINE)
GUID_VALID = re.compile(r"^[0-9a-f]{32}$")


def scan_meta(meta_path: Path) -> tuple[str | None, str | None]:
    try:
        text = meta_path.read_text(encoding="utf-8", errors="replace")
    except OSError as exc:
        return None, f"unreadable: {exc}"

    match = GUID_LINE.search(text)
    if not match:
        return None, "missing guid line"
    guid = match.group(1).lower()
    if len(guid) != 32:
        return guid, f"length {len(guid)} (expected 32)"
    if not GUID_VALID.match(guid):
        return guid, "non-hex characters"
    return guid, None


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "--root",
        type=Path,
        default=DEFAULT_ROOT,
        help="Unity project root to scan (default: l2-unity/)",
    )
    args = parser.parse_args()
    root = args.root.resolve()
    if not root.is_dir():
        print(f"check-unity-meta-guids: root not found: {root}", file=sys.stderr)
        return 1

    errors: list[str] = []
    seen: dict[str, Path] = {}

    for meta in sorted(root.rglob("*.meta")):
        guid, problem = scan_meta(meta)
        if problem is not None:
            rel = meta.relative_to(root)
            if guid is not None:
                errors.append(f"{rel}: guid '{guid}' — {problem}")
            else:
                errors.append(f"{rel}: {problem}")
            continue

        assert guid is not None
        prev = seen.get(guid)
        if prev is not None:
            rel = meta.relative_to(root)
            prev_rel = prev.relative_to(root)
            errors.append(f"duplicate guid {guid}: {prev_rel} and {rel}")
        else:
            seen[guid] = meta

    if errors:
        print("Unity .meta guid check failed:", file=sys.stderr)
        for line in errors[:50]:
            print(f"  {line}", file=sys.stderr)
        if len(errors) > 50:
            print(f"  ... and {len(errors) - 50} more", file=sys.stderr)
        return 1

    print(f"Unity .meta guids OK ({len(seen)} unique)")
    return 0


if __name__ == "__main__":
    sys.exit(main())