#!/usr/bin/env python3
"""Gap report for UE2 world material ops, plus the texture name index the runtime loads.

Reads committed *.props.txt under Resources/Data/Textures. Does not read .utx.
Modifier objects that are referenced but have no props file are listed; their
PanRate / CombineOperation / EnvMapType values are not invented.
"""
from __future__ import annotations

import os
import re
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TEX = ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "Textures"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "world"
INDEX = ROOT / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "Materials" / "ue2-texture-index.tsv"

REF = re.compile(r"([A-Za-z_][A-Za-z0-9_]*)'([^']+)'")
MODIFIER = {"TexPanner", "TexOscillator", "TexEnvMap", "TexScaler", "Combiner", "FinalBlend"}


def main() -> None:
    names: dict[str, Path] = {}
    for dirpath, _, files in os.walk(TEX):
        for fn in files:
            if fn.endswith(".props.txt"):
                names[fn[: -len(".props.txt")].lower()] = Path(dirpath) / fn

    missing: Counter[tuple[str, str]] = Counter()
    present: Counter[str] = Counter()
    for path in names.values():
        text = path.read_text(encoding="utf-8", errors="replace")
        for cls, full in REF.findall(text):
            if cls not in MODIFIER:
                continue
            short = full.split(".")[-1]
            present[cls] += 1
            if short.lower() not in names:
                missing[(cls, short)] += 1

    DOCS.mkdir(parents=True, exist_ok=True)
    lines = [
        "# UE2 world material ops gap",
        "",
        "Source: committed `*.props.txt` under `l2-unity/Assets/Resources/Data/Textures`.",
        "Shared path: `L2Ue2MaterialChain` executes Shader slots (Diffuse, Opacity, Specular,",
        "SpecularityMask, SelfIllumination, SelfIlluminationMask, Detail), Combiner, FinalBlend,",
        "TexPanner, TexOscillator, TexEnvMap, and TexScaler.",
        "",
        "A referenced op with no props file is not given a guessed PanRate or combine mode.",
        "The runtime logs `[Ue2Material] material <name> op <class> '<object>' missing props`.",
        "",
        "## Modifier references whose property dump is missing",
        "",
        "| Op | Object | References |",
        "|---|---|---:|",
    ]
    for (cls, short), count in sorted(missing.items(), key=lambda kv: (-kv[1], kv[0][0], kv[0][1])):
        lines.append(f"| `{cls}` | `{short}` | {count} |")
    lines += [
        "",
        f"Missing objects: **{len(missing)}**. References by op (including ones that resolve): "
        + ", ".join(f"{k} {v}" for k, v in sorted(present.items()))
        + ".",
        "",
        "TexScaler and FinalBlend are part of the same evaluator. Committed world props do not",
        "reference them yet; a chain that does is executed when its props are present.",
        "",
        "The original `.utx` packages are not in this tree, so this pass cannot extend the",
        "catalog with PanRate / PanDirection / CombineOperation / EnvMapType / oscillation",
        "fields. Those fields have to be dumped losslessly from the packages before the",
        "referenced ops can move.",
        "",
        "Some short shader names exist in more than one package with different slot text",
        "(`z_water01` in `interior_aa_t` and `interior_B_t`, plus other `z_*` / grass pairs).",
        "The runtime keys props by that short name. When the two bodies differ ignoring case",
        "it logs `[Ue2Material] material <name> props ambiguous` and does not pick one.",
        "",
    ]
    (DOCS / "material-ops-gap.md").write_text("\n".join(lines), encoding="utf-8")

    INDEX.parent.mkdir(parents=True, exist_ok=True)
    rows = ["name\tpackage\tresource_path"]
    exts = {".png", ".tga", ".jpg", ".jpeg", ".dds"}
    for dirpath, _, files in os.walk(TEX):
        for fn in files:
            ext = Path(fn).suffix.lower()
            if ext not in exts:
                continue
            full = Path(dirpath) / fn
            rel = full.relative_to(TEX).as_posix()
            package = rel.split("/")[0] if "/" in rel else ""
            no_ext = str(Path(rel).with_suffix("")).replace("\\", "/")
            rows.append(f"{Path(fn).stem}\t{package}\tData/Textures/{no_ext}")
    INDEX.write_text("\n".join(rows) + "\n", encoding="utf-8")
    print(f"gap objects {len(missing)} textures {len(rows) - 1}")


if __name__ == "__main__":
    main()
