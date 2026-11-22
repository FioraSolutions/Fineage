#!/usr/bin/env python3
"""Export every brush/BSP texture the tile bake will copy.

The want-list is the union of Brushes.json polyData.texture values and
brushes.t3d `Texture=` polygons. A reference whose name contains "Shaders"
is a UE2 material chain (the bake importer skips it; step 9 writes .props.txt),
not a Texture PNG. Every other reference is exported with ExportNamedTextures
into {out}/{package}/Texture/{leaf}.png — the path L2TerrainAssetImporter
rectifies to.

A package that is not in the client, or a name that is not a Texture export
of that package, exits 1 and names the package and the texture. Nothing is
hardcoded per map.
"""
from __future__ import annotations

import argparse
import json
import re
import subprocess
import sys
from collections import defaultdict
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
EXPORT_JAVA = REPO / "tools" / "interlude" / "ExportNamedTextures.java"
TEXTURE_RE = re.compile(r"Texture=(\S+)")


class BrushTexture:
    def __init__(self, package: str, texture: str, full: str) -> None:
        self.package = package
        self.texture = texture
        self.full = full

    def key(self) -> tuple[str, str]:
        return (self.package, self.texture)


def is_brush_png(full: str) -> bool:
    # Same rule as L2JSONBrushImporter.ProcessDataFile: names containing
    # "Shaders" are material chains, not Texture pixels.
    return bool(full) and "Shaders" not in full


def split_ref(full: str) -> BrushTexture | None:
    full = full.strip().strip("'\"")
    if not is_brush_png(full):
        return None
    parts = [p for p in full.split(".") if p]
    if len(parts) < 2:
        raise SystemExit(
            f"brush texture reference is not Package.Name: {full}"
        )
    return BrushTexture(parts[0], parts[-1], full)


def from_json(path: Path) -> list[BrushTexture]:
    data = json.loads(path.read_text(encoding="utf-8"))
    brushes = data.get("data")
    if not isinstance(brushes, list):
        raise SystemExit(f"Brushes.json has no brush array: {path}")
    out: list[BrushTexture] = []
    for brush in brushes:
        model = (brush or {}).get("model") or {}
        poly = model.get("poly") or {}
        for face in poly.get("polyData") or []:
            ref = (face or {}).get("texture") or ""
            parsed = split_ref(ref) if ref else None
            if parsed is not None:
                out.append(parsed)
    return out


def from_t3d(path: Path) -> list[BrushTexture]:
    out: list[BrushTexture] = []
    for line in path.read_text(encoding="utf-8", errors="replace").splitlines():
        match = TEXTURE_RE.search(line)
        if not match:
            continue
        parsed = split_ref(match.group(1))
        if parsed is not None:
            out.append(parsed)
    return out


def collect(paths: list[Path]) -> list[BrushTexture]:
    seen: dict[tuple[str, str], BrushTexture] = {}
    for path in paths:
        if not path.is_file():
            raise SystemExit(f"brush source missing: {path}")
        refs = from_json(path) if path.suffix.lower() == ".json" else from_t3d(path)
        if path.suffix.lower() == ".json" and not refs and path.stat().st_size > 0:
            # A brushes file of only shader faces is a valid empty PNG list.
            pass
        for ref in refs:
            seen.setdefault(ref.key(), ref)
    if not paths:
        raise SystemExit("no brush source (Brushes.json or brushes.t3d)")
    return [seen[k] for k in sorted(seen)]


def find_utx(client: Path, package: str) -> Path | None:
    want = package.lower() + ".utx"
    for folder in ("textures", "systextures", "Textures", "SysTextures"):
        root = client / folder
        if not root.is_dir():
            continue
        for child in root.iterdir():
            if child.is_file() and child.name.lower() == want:
                return child
    return None


def dest_png(out_root: Path, ref: BrushTexture) -> Path:
    return out_root / ref.package / "Texture" / f"{ref.texture}.png"


def missing_messages(
    refs: list[BrushTexture],
    out_root: Path,
    utx_for,
) -> list[str]:
    lines: list[str] = []
    for ref in refs:
        png = dest_png(out_root, ref)
        if png.is_file():
            continue
        utx = utx_for(ref.package)
        if utx is None:
            lines.append(
                f"package={ref.package} texture={ref.texture} "
                f"utx missing under textures/ and systextures/ "
                f"(referenced as {ref.full})"
            )
        else:
            lines.append(
                f"package={ref.package} texture={ref.texture} "
                f"not a Texture export in {utx.name} "
                f"(referenced as {ref.full}) expected={png}"
            )
    return lines


def export(client: Path, jar: Path, refs: list[BrushTexture], out_root: Path) -> None:
    if not jar.is_file():
        raise SystemExit(f"L2TerrainExtractor jar missing: {jar}")
    if not EXPORT_JAVA.is_file():
        raise SystemExit(f"ExportNamedTextures.java missing: {EXPORT_JAVA}")
    by_pkg: dict[str, list[BrushTexture]] = defaultdict(list)
    for ref in refs:
        by_pkg[ref.package].append(ref)
    utx_of: dict[str, Path | None] = {}
    for package, group in sorted(by_pkg.items()):
        utx = find_utx(client, package)
        utx_of[package] = utx
        if utx is None:
            continue
        out_dir = out_root / package / "Texture"
        out_dir.mkdir(parents=True, exist_ok=True)
        names = ",".join(sorted({ref.texture for ref in group}))
        proc = subprocess.run(
            ["java", "-cp", str(jar), str(EXPORT_JAVA), str(utx), str(out_dir), names],
            text=True,
            capture_output=True,
        )
        if proc.returncode != 0:
            sys.stderr.write(proc.stdout)
            sys.stderr.write(proc.stderr)
            sys.stderr.write(
                f"ExportNamedTextures failed for package={package} (exit {proc.returncode})\n"
            )
    missing = missing_messages(refs, out_root, lambda package: utx_of.get(package))
    if missing:
        raise SystemExit("brush texture missing:\n" + "\n".join(missing))
    print(f"exported {len(refs)} brush textures into {out_root}")


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--brushes", action="append", required=True, type=Path)
    parser.add_argument("--client", type=Path)
    parser.add_argument("--out", type=Path)
    parser.add_argument("--jar", type=Path)
    parser.add_argument("--list", action="store_true", help="print package and texture, do not export")
    args = parser.parse_args()
    refs = collect(args.brushes)
    if args.list:
        for ref in refs:
            print(f"{ref.package}\t{ref.texture}\t{ref.full}")
        return
    if args.client is None or args.out is None or args.jar is None:
        raise SystemExit("--client, --out, and --jar are required to export")
    export(args.client, args.jar, refs, args.out)


if __name__ == "__main__":
    main()
