#!/usr/bin/env python3
"""Fail CI when a character .anim has Euler + quaternion rotation on the same bone path.

PSA/FBX export duplicates rotation channels; Unity applies both and idle feet slide.
"""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ANIM_ROOT = ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "Animations"

BLOCK_SPLIT = re.compile(r"(?=  - serializedVersion: 2\n)")
ATTR_RE = re.compile(r"^\s+attribute: (.+)$", re.M)
PATH_RE = re.compile(r"^\s+path: (.+)$", re.M)


def redundant_paths(text: str) -> list[str]:
    quat_paths: set[str] = set()
    euler_by_path: dict[str, list[str]] = {}
    for block in BLOCK_SPLIT.split(text):
        if "serializedVersion: 2" not in block:
            continue
        am = ATTR_RE.search(block)
        pm = PATH_RE.search(block)
        if not am or not pm:
            continue
        attr = am.group(1).strip()
        path = pm.group(1).strip()
        if attr.startswith("m_LocalRotation."):
            quat_paths.add(path)
        elif attr.startswith("localEulerAngles."):
            euler_by_path.setdefault(path, []).append(attr)
    return sorted(p for p in euler_by_path if p in quat_paths)


def check_file(path: Path) -> list[str]:
    try:
        text = path.read_text(encoding="utf-8", errors="replace")
    except OSError as exc:
        return [f"{path}: read error {exc}"]
    bad = redundant_paths(text)
    if not bad:
        return []
    # report first few paths only
    sample = ", ".join(bad[:3])
    more = f" (+{len(bad) - 3} bones)" if len(bad) > 3 else ""
    return [f"{path.relative_to(ROOT)}: redundant Euler on {len(bad)} bone(s): {sample}{more}"]


def strip_file(path: Path) -> tuple[int, bool]:
    text = path.read_text(encoding="utf-8", errors="replace")
    quat_paths: set[str] = set()
    blocks = BLOCK_SPLIT.split(text)
    if not blocks:
        return 0, False
    header = blocks[0]
    out = [header]
    removed = 0
    for block in blocks[1:]:
        if "serializedVersion: 2" not in block:
            out.append(block)
            continue
        am = ATTR_RE.search(block)
        pm = PATH_RE.search(block)
        if am and pm and am.group(1).strip().startswith("m_LocalRotation."):
            quat_paths.add(pm.group(1).strip())
    for block in blocks[1:]:
        if "serializedVersion: 2" not in block:
            continue
        am = ATTR_RE.search(block)
        pm = PATH_RE.search(block)
        if am and pm:
            attr = am.group(1).strip()
            bone_path = pm.group(1).strip()
            if attr.startswith("localEulerAngles.") and bone_path in quat_paths:
                removed += 1
                continue
        out.append(block)
    if removed == 0:
        return 0, False
    path.write_text("".join(out), encoding="utf-8")
    return removed, True


def main(argv: list[str]) -> int:
    if "--strip" in argv:
        total = 0
        files = 0
        for path in sorted(ANIM_ROOT.rglob("*.anim")):
            n, changed = strip_file(path)
            if changed:
                files += 1
                total += n
        print(f"stripped euler curves={total} files={files}")
        return 0

    errors: list[str] = []
    candidates = [p for p in ANIM_ROOT.rglob("*.anim") if "localEulerAngles" in p.read_text(encoding="utf-8", errors="replace")]
    for path in sorted(candidates):
        text = path.read_text(encoding="utf-8", errors="replace")
        if "m_LocalRotation" not in text:
            continue
        bad = redundant_paths(text)
        if not bad:
            continue
        sample = ", ".join(bad[:3])
        more = f" (+{len(bad) - 3} bones)" if len(bad) > 3 else ""
        errors.append(
            f"{path.relative_to(ROOT)}: redundant Euler on {len(bad)} bone(s): {sample}{more}"
        )
        if len(errors) >= 25:
            break

    if errors:
        print("Animation rotation hygiene failed (Euler + quaternion on same bone):", file=sys.stderr)
        for line in errors:
            print("  " + line, file=sys.stderr)
        if len(errors) >= 25:
            print("  ... (truncated)", file=sys.stderr)
        return 1

    print("anim rotation hygiene OK")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))