#!/usr/bin/env python3
"""Guard NPC/monster mesh assets against meter-contract regressions (#1473)."""

from __future__ import annotations

import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
ANIM_ROOTS = (
    REPO / "l2-unity/Assets/Resources/Data/Animations/LineageNPCs",
    REPO / "l2-unity/Assets/Resources/Data/Animations/LineageMonsters",
)

sys.path.insert(0, str(REPO / "tools/interlude"))
import anim_meter_patch as anim  # noqa: E402
import fbx_meter_patch as fbx  # noqa: E402

META_GS = re.compile(r"^\s+globalScale:\s*([0-9.]+)\s*$", re.M)
META_UFS = re.compile(r"^\s+useFileScale:\s*([01])\s*$", re.M)
PREFAB_ROOT_SCALE = re.compile(
    r"m_Name: ([^\n]+)\n(?:.*?\n)*?\s+m_LocalScale: \{x: ([^,]+), y: ([^,]+), z: ([^}\n]+)\}",
    re.M,
)


def check_meta(path: Path) -> list[str]:
    text = path.read_text(encoding="utf-8", errors="replace")
    gs = META_GS.search(text)
    ufs = META_UFS.search(text)
    errors: list[str] = []
    rel = path.relative_to(REPO)
    if gs is None or abs(float(gs.group(1)) - 1.0) > 1e-4:
        errors.append(f"{rel}: globalScale must be 1 (got {gs.group(1) if gs else '?'})")
    if ufs is None or ufs.group(1) != "1":
        errors.append(f"{rel}: useFileScale must be 1 (got {ufs.group(1) if ufs else '?'})")
    return errors


def check_prefab(path: Path) -> list[str]:
    stem = path.stem
    text = path.read_text(encoding="utf-8", errors="replace")
    for m in PREFAB_ROOT_SCALE.finditer(text):
        if m.group(1) != stem:
            continue
        vals = (float(m.group(i)) for i in range(2, 5))
        if any(abs(v - 1.0) > 1e-3 for v in vals):
            rel = path.relative_to(REPO)
            return [
                f"{rel}: prefab root {stem} localScale must be (1,1,1) "
                f"got ({m.group(2)}, {m.group(3)}, {m.group(4)})"
            ]
        return []
    return []


def check_clip(path: Path) -> list[str]:
    text = path.read_text(encoding="utf-8", errors="replace")
    if anim.is_legacy(text):
        return [f"{path.relative_to(REPO)}: clip still carries legacy x100 .ao/.mo scale carrier"]
    return []


def check_fbx(path: Path) -> list[str]:
    try:
        version, nodes, _original, footer_offset = fbx.parse(str(path))
        root = fbx.Node("", [], nodes, wide=version >= 7500)
        legacy, reason = fbx.detect_contract(root)
        if legacy:
            return [f"{path.relative_to(REPO)}: FBX not on meter contract ({reason})"]
    except Exception as exc:
        return [f"{path.relative_to(REPO)}: FBX parse failed ({exc})"]
    return []


def main() -> int:
    errors: list[str] = []
    fbx_count = meta_count = prefab_count = clip_count = 0

    for root in ANIM_ROOTS:
        if not root.is_dir():
            errors.append(f"missing animation root {root.relative_to(REPO)}")
            continue
        for meta in root.rglob("*.fbx.meta"):
            meta_count += 1
            errors.extend(check_meta(meta))
        for prefab in root.rglob("*.prefab"):
            prefab_count += 1
            errors.extend(check_prefab(prefab))
        for clip in root.rglob("*.anim"):
            clip_count += 1
            errors.extend(check_clip(clip))
        for model in root.rglob("*.fbx"):
            fbx_count += 1
            errors.extend(check_fbx(model))

    if errors:
        print("NPC/monster meter contract check failed:", file=sys.stderr)
        for item in errors[:50]:
            print(f"  - {item}", file=sys.stderr)
        if len(errors) > 50:
            print(f"  ... and {len(errors) - 50} more", file=sys.stderr)
        return 1

    print(
        "NPC/monster meter contract OK "
        f"(fbx={fbx_count} meta={meta_count} prefabs={prefab_count} clips={clip_count})."
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())