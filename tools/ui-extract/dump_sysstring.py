#!/usr/bin/env python3
"""Decode sysstring-e.dat (Interlude, Ver413) to a committed TSV reference.

The login window definitions in l2-unity/docs/ui-original/Login.json reference
button captions by sysstring id (152 Log In, 153 Exit, 154 New Account, 155 Lost
Account, 156 Options, 805 Production Team, 1251 Replay). Verifying those
captions byte-exact needs the original sysstring-e.dat decoded.

Steps on a machine with the client:

    l2encdec -c decode -p 413 "<client>/system/sysstring-e.dat" sysstring-e.dec
    python3 tools/ui-extract/dump_sysstring.py sysstring-e.dec \
        --out l2-unity/docs/interlude-ref/sysstring-e.tsv

Output: TSV `id<TAB>text`, sorted by id. Text may contain escaped newlines.
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from extract_interface_xdat import parse_sysstring  # noqa: E402


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("decoded", type=Path, help="sysstring-e.dat after Ver413 decrypt")
    ap.add_argument("--out", type=Path, required=True, help="output TSV path")
    ap.add_argument("--ids", type=str, default="", help="comma-separated ids to restrict to")
    args = ap.parse_args()

    data = args.decoded.read_bytes()
    table = parse_sysstring(data)
    if not table:
        print(f"ERROR: no strings parsed from {args.decoded} — is it decrypted (Ver413)?", file=sys.stderr)
        return 2

    wanted: set[int] | None = None
    if args.ids:
        wanted = {int(x) for x in args.ids.split(",")}

    args.out.parent.mkdir(parents=True, exist_ok=True)
    lines = []
    for sid in sorted(table):
        if wanted is not None and sid not in wanted:
            continue
        text = table[sid].replace("\r", "\\r").replace("\n", "\\n").replace("\t", "\\t")
        lines.append(f"{sid}\t{text}")
    args.out.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"wrote {args.out}: {len(lines)} strings")
    if wanted is not None:
        missing = sorted(wanted - set(table))
        if missing:
            print(f"WARN: ids not present in file: {missing}", file=sys.stderr)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
