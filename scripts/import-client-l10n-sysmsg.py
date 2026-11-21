#!/usr/bin/env python3
"""Merge Interlude systemmsg-*.dat lines into client strings.json as sysmsg.<id> keys.

Does not read or commit original retail binaries from the repo; point --dat at a local
gitignored ref/ copy. Output is JSON only under l2-unity/Assets/Resources/Data/l10n/.
"""

from __future__ import annotations

import argparse
import json
import re
import struct
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]


def read_dat_strings(path: Path) -> dict[int, str]:
    data = path.read_bytes()
    messages: dict[int, str] = {}
    i = 0
    while i + 6 <= len(data):
        msg_id, size = struct.unpack_from("<IH", data, i)
        i += 6
        if size == 0 or i + size > len(data):
            break
        raw = data[i : i + size]
        i += size
        try:
            text = raw.decode("utf-16-le").rstrip("\x00")
        except UnicodeDecodeError:
            text = raw.decode("latin-1", errors="replace")
        messages[msg_id] = text.replace("$s", "{0}").replace("$c1", "{0}").replace("$S1", "{0}")
    return messages


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--dat", type=Path, required=True, help="Path to systemmsg-e.dat (local ref/)")
    parser.add_argument(
        "--out",
        type=Path,
        default=REPO / "l2-unity/Assets/Resources/Data/l10n/en/strings.json",
    )
    parser.add_argument("--merge", action="store_true", help="Merge into existing strings.json")
    args = parser.parse_args()

    if not args.dat.is_file():
        print(f"dat not found: {args.dat}", file=sys.stderr)
        return 1

    imported = read_dat_strings(args.dat)
    table: dict[str, str] = {}
    if args.merge and args.out.is_file():
        table = json.loads(args.out.read_text(encoding="utf-8"))

    for msg_id, text in sorted(imported.items()):
        if not text.strip():
            continue
        table[f"sysmsg.{msg_id}"] = text

    args.out.parent.mkdir(parents=True, exist_ok=True)
    args.out.write_text(json.dumps(table, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(f"wrote {len(table)} keys to {args.out}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())