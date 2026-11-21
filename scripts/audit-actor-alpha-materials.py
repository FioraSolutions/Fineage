#!/usr/bin/env python3
"""List and optionally fix Actor materials that imply alpha cutout but use opaque setup."""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ACTOR_SHADER_GUID = "c7e2a91b4f6d48e0a1b35d8c90e47a12"
CUTOFF_SUFFIXES = ("_t01", "_t02", "_t03", "_t00_h", "_m00_ah", "_m00_bh")


def implies_cutout(name: str) -> bool:
    n = name.lower()
    if any(n.endswith(s) for s in CUTOFF_SUFFIXES):
        return True
    if n.endswith("_ah") or n.endswith("_bh"):
        return True
    if "_wand" in n or "_wing" in n:
        return True
    return False


def scan_mat(path: Path) -> dict | None:
    text = path.read_text(encoding="utf-8", errors="replace")
    m_name = re.search(r"^\s*m_Name:\s*(.+)$", text, re.M)
    name = m_name.group(1).strip() if m_name else path.stem
    if not implies_cutout(name):
        return None
    has_alpha_kw = "_ALPHATEST_ON" in text
    alpha_clip = re.search(r"^\s*- _AlphaClip:\s*(\d+)", text, re.M)
    clip_on = alpha_clip and int(alpha_clip.group(1)) >= 1
    if clip_on and has_alpha_kw:
        return None
    on_actor = ACTOR_SHADER_GUID in text or "ActorModulate2X" in text
    has_tex = "m_Texture: {fileID: 2800000" in text or "m_Texture: {fileID: 0" not in text
    return {
        "path": str(path.relative_to(ROOT)),
        "name": name,
        "on_actor": on_actor,
        "has_tex": has_tex,
    }


def patch_mat(path: Path) -> bool:
    text = path.read_text(encoding="utf-8", errors="replace")
    if "_ALPHATEST_ON" in text:
        return False
    if implies_cutout(path.stem) is False:
        return False

    # Force ActorModulate2X + cutout block (idempotent for URP Lit).
    text = re.sub(
        r"m_Shader: \{fileID: \d+, guid: [a-f0-9]+, type: 3\}",
        f"m_Shader: {{fileID: 4800000, guid: {ACTOR_SHADER_GUID}, type: 3}}",
        text,
        count=1,
    )
    if "m_ValidKeywords:" in text and "_ALPHATEST_ON" not in text:
        text = text.replace("m_ValidKeywords: []", "m_ValidKeywords:\n  - _ALPHATEST_ON")
        text = re.sub(
            r"m_InvalidKeywords: \[\]",
            "m_InvalidKeywords: []",
            text,
            count=1,
        )
    text = re.sub(r"^\s*- _AlphaClip:\s*0\s*$", "    - _AlphaClip: 1", text, flags=re.M)
    if "_Cutoff:" not in text:
        text = re.sub(
            r"(m_Floats:\s*\n)",
            r"\1    - _Cutoff: 0.498039216\n",
            text,
            count=1,
        )
    text = re.sub(
        r"stringTagMap: \{\}",
        'stringTagMap:\n    RenderType: TransparentCutout',
        text,
        count=1,
    )
    path.write_text(text, encoding="utf-8")
    return True


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--fix", action="store_true", help="Patch materials under SysTextures")
    ap.add_argument("--glob", default="**/Materials/*.mat")
    args = ap.parse_args()
    scan_roots = [
        ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "SysTextures",
        ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "Animations",
    ]
    ACTOR_TEX_DIRS = (
        "LineageMonstersTex",
        "LineageMonstersTex2",
        "LineageMonstersTex3",
        "LineageNpcsTex",
        "LineageNPCsTex",
        "LineageNpcsTex3",
        "MElf",
        "FElf",
        "MOrc",
        "FOrc",
        "MDwarf",
        "FDwarf",
        "MDarkElf",
        "FDarkElf",
        "LineageFaceTex",
    )
    offenders: list[dict] = []
    fixed = 0
    for mats_root in scan_roots:
        for path in sorted(mats_root.rglob("*.mat")):
            if "Materials" not in path.parts:
                continue
            if mats_root.name == "SysTextures":
                rel = path.relative_to(mats_root)
                if rel.parts and rel.parts[0] not in ACTOR_TEX_DIRS:
                    continue
            row = scan_mat(path)
            if row:
                offenders.append(row)
                if args.fix:
                    if patch_mat(path):
                        fixed += 1

    print(f"opaque-alpha mismatches: {len(offenders)}", file=sys.stderr)
    for row in offenders[:80]:
        print(f"  {row['path']} name={row['name']} actor={row['on_actor']} tex={row['has_tex']}")
    if len(offenders) > 80:
        print(f"  ... and {len(offenders) - 80} more", file=sys.stderr)
    if args.fix:
        print(f"patched: {fixed}", file=sys.stderr)
    return 1 if offenders and not args.fix else 0


if __name__ == "__main__":
    raise SystemExit(main())