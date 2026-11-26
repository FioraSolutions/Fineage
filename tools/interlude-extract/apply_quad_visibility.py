#!/usr/bin/env python3
"""Patch TerrainInfo0.txt and map t3d with official 2048-word QuadVisibilityBitmap.

Reads DumpTerrainHoles sidecars (one signed int per line, 2048 words / 65536 bits).
Does not invent bits and does not keep the historic 64-word all-visible truncation.
"""

from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

WORD_COUNT = 2048
QUAD_LINE = re.compile(r"^(\s*)QuadVisibilityBitmap\((\d+)\)=(-?\d+)\s*$")
INFO0_NAME = "TerrainInfo0.txt"


def load_words(path: Path) -> list[int]:
    words: list[int] = []
    for line in path.read_text(encoding="utf-8").splitlines():
        s = line.split("#", 1)[0].strip()
        if s:
            words.append(int(s, 10))
    if len(words) != WORD_COUNT:
        raise SystemExit(f"ERROR {path} has {len(words)} words, need {WORD_COUNT}")
    return words


def hidden_count(words: list[int]) -> int:
    return sum(32 - (w & 0xFFFFFFFF).bit_count() for w in words)


def format_lines(words: list[int], indent: str = "") -> list[str]:
    return [f"{indent}QuadVisibilityBitmap({i})={int(w)}" for i, w in enumerate(words)]


def replace_quad_block(lines: list[str], words: list[int], indent: str | None = None) -> tuple[list[str], int]:
    """Replace existing QuadVisibilityBitmap(N)= lines (not *Orig) with official 2048 words."""
    idxs = [i for i, line in enumerate(lines) if QUAD_LINE.match(line)]
    if idxs:
        first, last = idxs[0], idxs[-1]
        if indent is None:
            indent = QUAD_LINE.match(lines[first]).group(1)
        old_count = last - first + 1
        new_lines = lines[:first] + format_lines(words, indent) + lines[last + 1 :]
        return new_lines, old_count

    # Insert before GeneratedSectorCounter, else before Location, else at end.
    insert_at = None
    for i, line in enumerate(lines):
        stripped = line.lstrip()
        if stripped.startswith("GeneratedSectorCounter=") or stripped.startswith("Location="):
            insert_at = i
            break
    if indent is None:
        indent = ""
        for line in lines:
            if line.startswith("    ") and line.strip():
                indent = "    "
                break
    block = format_lines(words, indent)
    if insert_at is None:
        return lines + block, 0
    return lines[:insert_at] + block + lines[insert_at:], 0


def patch_terrain_info0(path: Path, words: list[int]) -> tuple[bool, int]:
    raw = path.read_text(encoding="utf-8")
    nl = "\n" if not raw.endswith("\r\n") else "\r\n"
    lines = raw.splitlines()
    new_lines, old_count = replace_quad_block(lines, words, indent="")
    new = nl.join(new_lines) + (nl if raw.endswith(("\n", "\r\n")) else "")
    changed = new != raw
    if changed:
        path.write_text(new, encoding="utf-8")
    return changed, old_count


def patch_t3d(path: Path, words: list[int]) -> tuple[bool, int]:
    raw = path.read_text(encoding="utf-8")
    lines = raw.splitlines()
    start = None
    end = None
    for i, line in enumerate(lines):
        if line.strip().startswith("Begin Actor Class=TerrainInfo Name=TerrainInfo0"):
            start = i
            for j in range(i + 1, len(lines)):
                if lines[j].strip().startswith("End Actor"):
                    end = j
                    break
            break
    if start is None or end is None:
        raise SystemExit(f"ERROR {path} has no TerrainInfo0 actor")
    actor = lines[start + 1 : end]
    new_actor, old_count = replace_quad_block(actor, words, indent="    ")
    new_lines = lines[: start + 1] + new_actor + lines[end:]
    nl = "\n" if not raw.endswith("\r\n") else "\r\n"
    new = nl.join(new_lines) + (nl if raw.endswith(("\n", "\r\n")) else "")
    changed = new != raw
    if changed:
        path.write_text(new, encoding="utf-8")
    return changed, old_count


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--words-dir", type=Path, required=True)
    ap.add_argument("--maps-dir", type=Path, required=True)
    args = ap.parse_args()
    word_files = sorted(args.words_dir.glob("*.quadvisibility.txt"))
    if not word_files:
        raise SystemExit(f"ERROR no *.quadvisibility.txt in {args.words_dir}")
    failures = 0
    for wf in word_files:
        tile = wf.name.removesuffix(".quadvisibility.txt")
        words = load_words(wf)
        hid = hidden_count(words)
        meta = args.maps_dir / tile / "Meta"
        info0 = meta / INFO0_NAME
        t3d = meta / f"{tile}.t3d"
        if not info0.is_file():
            print(f"ERROR {tile}: missing {info0}", file=sys.stderr)
            failures += 1
            continue
        changed, old = patch_terrain_info0(info0, words)
        msg = f"{tile}: TerrainInfo0 {old}->{WORD_COUNT} words hidden={hid}"
        if not changed:
            msg += " (already official)"
        print(msg)
        if t3d.is_file():
            changed_t, old_t = patch_t3d(t3d, words)
            tmsg = f"{tile}: t3d {old_t}->{WORD_COUNT} words"
            if not changed_t:
                tmsg += " (already official)"
            print(tmsg)
        # verify
        verify = []
        for line in info0.read_text(encoding="utf-8").splitlines():
            m = QUAD_LINE.match(line)
            if m:
                verify.append(int(m.group(3)))
        if verify != words:
            print(f"ERROR {tile}: TerrainInfo0 does not match official words", file=sys.stderr)
            failures += 1
    if failures:
        raise SystemExit(failures)


if __name__ == "__main__":
    main()
