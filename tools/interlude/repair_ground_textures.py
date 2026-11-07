#!/usr/bin/env python3
"""Replace T_XX_YY/<ground>.png files that are byte-copies of a splat mask with the real
ground diffuse from .work/interlude-extract/terraintextures (fixes black ground spots).
Run, then re-run Auto-Bake 'Unity bake' for affected tiles in the Editor."""
from __future__ import annotations
import hashlib, re, shutil, sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
GROUND = REPO / ".work/interlude-extract/terraintextures"

def md5(p: Path) -> str:
    return hashlib.md5(p.read_bytes()).hexdigest()

def main() -> None:
    lut = {f.name.lower(): f for f in GROUND.glob("*.png")} if GROUND.is_dir() else {}
    fixed, missing, touched = 0, [], set()
    for d in sorted(TEX.glob("T_[0-9]*_[0-9]*")):
        splats = {md5(p) for p in d.glob("*_splat*.png")}
        if not splats:
            continue
        for png in d.glob("*.png"):
            if "_splat" in png.name:
                continue
            if md5(png) not in splats:
                continue
            src = lut.get(png.name.lower())
            if src is None:
                missing.append(f"{d.name}/{png.name}")
                continue
            shutil.copy2(src, png)
            fixed += 1
            touched.add(d.name[2:])
    print(f"fixed {fixed} ground textures; tiles: {' '.join(sorted(touched)) or '-'}")
    if missing:
        print("no source in terraintextures/: " + ", ".join(missing))

if __name__ == "__main__":
    main()
