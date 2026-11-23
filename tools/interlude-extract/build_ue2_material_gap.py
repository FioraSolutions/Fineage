#!/usr/bin/env python3
"""Gap report for UE2 world material ops, plus the texture name index the runtime loads.

Reads committed *.props.txt under Resources/Data/Textures. Does not read .utx.
Modifier objects that are referenced but have no props file are listed; their
PanRate / CombineOperation / EnvMapType values are not invented.

Both artifacts are regenerated deterministically (sorted scans), so
`--check` can fail CI when the committed files drift from the texture tree.
"""
from __future__ import annotations

import os
import re
import sys
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TEX = ROOT / "l2-unity" / "Assets" / "Resources" / "Data" / "Textures"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "world"
INDEX = ROOT / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "Materials" / "ue2-texture-index.tsv"

REF = re.compile(r"([A-Za-z_][A-Za-z0-9_]*)'([^']+)'")
MODIFIER = {"TexPanner", "TexOscillator", "TexEnvMap", "TexScaler", "Combiner", "FinalBlend"}
DOC_NAME = "material-ops-gap.md"


def gap_scan(tex: Path) -> tuple[Counter[tuple[str, str]], Counter[str]]:
    """Every props file counts, including same-stem files in two packages."""
    props_files = sorted(tex.rglob("*.props.txt"))
    known = {p.name[: -len(".props.txt")].lower() for p in props_files}
    missing: Counter[tuple[str, str]] = Counter()
    present: Counter[str] = Counter()
    for path in props_files:
        text = path.read_text(encoding="utf-8", errors="replace")
        for cls, full in REF.findall(text):
            if cls not in MODIFIER:
                continue
            short = full.split(".")[-1]
            present[cls] += 1
            if short.lower() not in known:
                missing[(cls, short)] += 1
    return missing, present


def render_doc(missing: Counter[tuple[str, str]], present: Counter[str]) -> str:
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
    return "\n".join(lines)


def index_rows(tex: Path) -> list[str]:
    """One row per texture resource, sorted. A texture extracted as both .png
    and .tga is one resource (the row has no extension); two rows for it made
    the runtime count two candidates and call a unique name ambiguous. Rows
    are sorted so the committed index does not churn with filesystem order.
    """
    exts = {".png", ".tga", ".jpg", ".jpeg", ".dds"}
    seen: dict[tuple[str, str, str], str] = {}
    for dirpath, _, files in os.walk(tex):
        for fn in files:
            ext = Path(fn).suffix.lower()
            if ext not in exts:
                continue
            full = Path(dirpath) / fn
            rel = full.relative_to(tex).as_posix()
            package = rel.split("/")[0] if "/" in rel else ""
            no_ext = str(Path(rel).with_suffix("")).replace("\\", "/")
            stem = Path(fn).stem
            key = (stem.lower(), package.lower(), no_ext.lower())
            seen.setdefault(key, f"{stem}\t{package}\tData/Textures/{no_ext}")
    return [seen[key] for key in sorted(seen)]


def render_index(tex: Path) -> str:
    return "\n".join(["name\tpackage\tresource_path"] + index_rows(tex)) + "\n"


def main(argv: list[str]) -> int:
    check = "--check" in argv[1:]
    missing, present = gap_scan(TEX)
    doc = render_doc(missing, present)
    index = render_index(TEX)
    if check:
        stale = []
        if not INDEX.is_file() or INDEX.read_text(encoding="utf-8") != index:
            stale.append(str(INDEX))
        doc_path = DOCS / DOC_NAME
        if not doc_path.is_file() or doc_path.read_text(encoding="utf-8") != doc:
            stale.append(str(doc_path))
        if stale:
            print("build_ue2_material_gap: stale generated file(s):", file=sys.stderr)
            for path in stale:
                print(f"  {path}", file=sys.stderr)
            print("run: python3 tools/interlude-extract/build_ue2_material_gap.py", file=sys.stderr)
            return 1
        print(f"build_ue2_material_gap: index and gap doc match the texture tree ({len(index_rows(TEX))} rows)")
        return 0
    DOCS.mkdir(parents=True, exist_ok=True)
    (DOCS / DOC_NAME).write_text(doc, encoding="utf-8")
    INDEX.parent.mkdir(parents=True, exist_ok=True)
    INDEX.write_text(index, encoding="utf-8")
    print(f"gap objects {len(missing)} textures {len(index.splitlines()) - 1}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv))
