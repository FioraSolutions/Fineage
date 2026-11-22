#!/usr/bin/env python3
"""Fail CI when player-race paperdoll armor .mat uses ActorEnvAdd instead of ActorModulate2X."""
from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ENV_ADD_GUID = "a1e9c4d85b2f4c6e9d07a3b8f1e24c60"
ACTOR_GUID = "c7e2a91b4f6d48e0a1b35d8c90e47a12"

RACE_FOLDERS = [
    "MFighter",
    "FFighter",
    "MMagic",
    "FMagic",
    "MElf",
    "FElf",
    "MShaman",
    "FShaman",
    "MOrc",
    "FOrc",
    "MDwarf",
    "FDwarf",
    "MDarkElf",
    "FDarkElf",
]


def main() -> int:
    offenders: list[str] = []
    base = ROOT / "l2-unity/Assets/Resources/Data/SysTextures"
    for race in RACE_FOLDERS:
        folder = base / race / "Materials"
        if not folder.is_dir():
            continue
        for mat in folder.glob("*.mat"):
            text = mat.read_text(encoding="utf-8", errors="replace")
            if ENV_ADD_GUID in text:
                offenders.append(str(mat.relative_to(ROOT)))
    if offenders:
        print(
            "Paperdoll armor must use ActorModulate2X (see L2ActorRaceMaterials); "
            "run scripts/migrate-paperdoll-envadd-to-modulate.py:",
            file=sys.stderr,
        )
        for p in sorted(offenders):
            print(f"  {p}", file=sys.stderr)
        return 1
    print(f"paperdoll armor shader check OK ({len(RACE_FOLDERS)} race folders)")
    return 0


if __name__ == "__main__":
    sys.exit(main())