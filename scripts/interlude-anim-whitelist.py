#!/usr/bin/env python3
"""Emit InterludeAnimResourceTake.tsv: AnimTween sequence id → shipped .anim take (after .ao_)."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ANIM_ROOT = ROOT / "l2-unity/Assets/Resources/Data/Animations"
ANIM_TWEEN = ROOT / "l2-unity/Assets/StreamingAssets/Data/Meta/AnimTween.txt"
OUT = ROOT / "l2-unity/Assets/StreamingAssets/Data/Meta/InterludeAnimResourceTake.tsv"

AO_PATTERN = re.compile(r"\.ao_([^./\\]+)\.anim$", re.IGNORECASE)


def load_anim_tween_sequences() -> dict[str, set[str]]:
    by_mesh: dict[str, set[str]] = {}
    if not ANIM_TWEEN.is_file():
        return by_mesh
    for line in ANIM_TWEEN.read_text(encoding="utf-8", errors="replace").splitlines():
        if not line.strip() or line.startswith("#") or line.startswith("package"):
            continue
        cols = line.split("\t")
        if len(cols) < 3:
            continue
        mesh, seq = cols[1].strip(), cols[2].strip()
        if mesh and seq:
            by_mesh.setdefault(mesh, set()).add(seq)
    return by_mesh


def scan_shipped_takes() -> dict[str, str]:
    """normalized take -> canonical take as on disk."""
    takes: dict[str, str] = {}
    if not ANIM_ROOT.is_dir():
        return takes
    for anim in ANIM_ROOT.rglob("*.anim"):
        m = AO_PATTERN.search(anim.name)
        if not m:
            continue
        take = m.group(1)
        takes[take.lower()] = take
    return takes


def main() -> int:
    tween = load_anim_tween_sequences()
    shipped = scan_shipped_takes()
    rows: list[tuple[str, str, str]] = []
    for mesh, sequences in sorted(tween.items()):
        for seq in sorted(sequences):
            canonical = shipped.get(seq.lower())
            if canonical is None:
                continue
            if canonical == seq:
                continue
            rows.append((mesh, seq, canonical))

    lines = [
        "# meshAnim\tanimTweenSequence\tresourcesTake (generated/curated via scripts/interlude-anim-whitelist.py)",
    ]
    for mesh, seq, take in rows:
        lines.append(f"{mesh}\t{seq}\t{take}")

    OUT.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print(f"wrote {OUT} ({len(rows)} alias rows)")
    return 0


if __name__ == "__main__":
    sys.exit(main())