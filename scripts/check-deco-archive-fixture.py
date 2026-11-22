#!/usr/bin/env python3
"""Size guard for the synthetic DecoLayer archive. Real tiles use the same writer."""

from pathlib import Path
import sys

FIXTURE = Path("tools/client-domain.Tests/Fixtures/synthetic-deco.l2dc")
MIN_BYTES = 64
MAX_BYTES = 4096


def main() -> int:
    if not FIXTURE.is_file():
        print(f"missing {FIXTURE}", file=sys.stderr)
        return 1
    data = FIXTURE.read_bytes()
    if data[:4] != b"L2DC":
        print(f"{FIXTURE}: magic is {data[:4]!r}, expected L2DC", file=sys.stderr)
        return 1
    if not (MIN_BYTES < len(data) < MAX_BYTES):
        print(
            f"{FIXTURE}: {len(data)} bytes, expected {MIN_BYTES} < size < {MAX_BYTES}",
            file=sys.stderr,
        )
        return 1
    print(f"{FIXTURE}: L2DC {len(data)} bytes")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
