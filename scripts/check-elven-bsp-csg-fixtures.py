#!/usr/bin/env python3
"""Sanity-check Elven Village BSP fixtures for city doorway CSG work."""

from __future__ import annotations

import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[1]
FIX = REPO / "tools/interlude/fixtures/elven_village"


def parse_brushes(text: str) -> list[dict]:
    blocks = re.findall(r"Begin Actor Class=Brush.*?(?=Begin Actor Class=|$)", text, re.DOTALL)
    out = []
    for b in blocks:
        csg = "CSG_Subtract" if "CsgOper=CSG_Subtract" in b else "CSG_Add"
        anti = b.count("AntiPortal")
        solid_sub = 0
        if csg == "CSG_Subtract":
            for m in re.finditer(r"Begin Polygon Texture=([^\s]+)", b):
                tex = m.group(1)
                if any(x in tex for x in ("L2_Skies", "L2_ETC", "Engine.")):
                    continue
                if "Water" in tex:
                    continue
                solid_sub += 1
        out.append({"csg": csg, "anti": anti, "solid_sub_polys": solid_sub})
    return out


def main() -> int:
    errors: list[str] = []
    tile = FIX / "20_20" / "brushes.t3d"
    if not tile.is_file():
        errors.append(f"missing {tile}")
    else:
        brushes = parse_brushes(tile.read_text(errors="replace"))
        subs = sum(1 for b in brushes if b["csg"] == "CSG_Subtract")
        anti = sum(b["anti"] for b in brushes)
        solid_sub = sum(b["solid_sub_polys"] for b in brushes if b["csg"] == "CSG_Subtract")
        if subs < 400:
            errors.append(f"20_20: expected >=400 subtract brushes, got {subs}")
        if anti < 10:
            errors.append(f"20_20: expected AntiPortal doorway caps, got {anti}")
        if solid_sub < 2000:
            errors.append(f"20_20: subtract solid polys too low ({solid_sub}); fixture corrupt?")
        print(f"20_20 brushes={len(brushes)} subtract={subs} anti_portal_polys={anti} subtract_solid_polys={solid_sub}")

    asmdef = REPO / "l2-unity/Assets/Scripts/Fineage.TerrainBsp/Fineage.TerrainBsp.asmdef"
    if not asmdef.is_file():
        errors.append("missing Fineage.TerrainBsp asmdef")

    if errors:
        for e in errors:
            print("ERROR:", e, file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())