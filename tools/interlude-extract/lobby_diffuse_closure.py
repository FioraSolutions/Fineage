#!/usr/bin/env python3
"""Diffuse textures named by committed material props for lobby packages.

Brush export only walks BSP faces. Static-mesh skins and terrain layers name
textures in *.props.txt (Diffuse = Texture'...') under the package folder.
This lists every such leaf whose PNG is not already under Textures/<package>/.
A missing file is printed as package, group, name, class so step 6 can export
it. --verify exits 1 when any row remains.
"""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
MESHES = REPO / "l2-unity/Assets/Resources/Data/StaticMeshes"
LOBBY_DUMP = REPO / "l2-unity/docs/interlude-ref/world/env/Lobby.unr.json.gz"
LOBBY_T3D = REPO / "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/l2_lobby.t3d"
DIFFUSE = re.compile(r"^\s*Diffuse\s*=\s*\w+'([^']+)'", re.M)
STATIC_MESH = re.compile(r"StaticMesh=StaticMesh'([^']+)'")


def lobby_packages() -> set[str]:
    import gzip
    import json
    if not LOBBY_DUMP.is_file():
        raise SystemExit(f"ERROR missing {LOBBY_DUMP}")
    with gzip.open(LOBBY_DUMP, "rt", encoding="utf-8") as f:
        data = json.load(f)
    names = set()
    for imp in data.get("imports") or []:
        if imp.get("class_name") == "Package" and imp.get("object_name"):
            names.add(str(imp["object_name"]).lower())
    return names


def package_of(path: Path) -> str:
    rel = path.relative_to(TEX)
    return rel.parts[0]


def leaf_of(token: str) -> str:
    return token.split(".")[-1]


_MATERIAL_LEAVES: set[str] | None = None


def material_leaves() -> set[str]:
    """Props file stems are materials. A Diffuse pointing at one is not a PNG."""
    global _MATERIAL_LEAVES
    if _MATERIAL_LEAVES is None:
        _MATERIAL_LEAVES = {p.name[: -len(".props.txt")].lower() for p in TEX.rglob("*.props.txt")}
    return _MATERIAL_LEAVES


def is_material(leaf: str) -> bool:
    return leaf.lower() in material_leaves()


_PNG_LEAVES: set[str] | None = None


def png_exists(leaf: str) -> bool:
    """The binder resolves a diffuse by leaf name anywhere under Textures/."""
    global _PNG_LEAVES
    if _PNG_LEAVES is None:
        _PNG_LEAVES = {p.name[: -len(".png")].lower() for p in TEX.rglob("*.png")}
    return leaf.lower() in _PNG_LEAVES


def lobby_skin_stems() -> set[str]:
    """Material names on static meshes the lobby t3d places."""
    if not LOBBY_T3D.is_file():
        raise SystemExit(f"ERROR missing {LOBBY_T3D}")
    packages = set()
    for match in STATIC_MESH.finditer(LOBBY_T3D.read_text(encoding="utf-8", errors="replace")):
        packages.add(match.group(1).split(".")[0].lower())
    if not MESHES.is_dir():
        return set()
    folders = {child.name.lower(): child for child in MESHES.iterdir() if child.is_dir()}
    stems: set[str] = set()
    for package in packages:
        folder = folders.get(package)
        if folder is None:
            continue
        for mat in folder.rglob("*.mat"):
            stems.add(mat.stem.lower())
    return stems


def missing(packages: set[str] | None) -> list[tuple[str, str]]:
    rows: list[tuple[str, str]] = []
    if not TEX.is_dir():
        return rows
    skins = lobby_skin_stems() if packages is not None else set()
    seen: set[tuple[str, str]] = set()
    for props in TEX.rglob("*.props.txt"):
        package = package_of(props)
        stem = props.name[: -len(".props.txt")].lower()
        in_package = packages is None or package.lower() in packages
        if not in_package and stem not in skins:
            continue
        text = props.read_text(encoding="utf-8", errors="replace")
        for match in DIFFUSE.finditer(text):
            leaf = leaf_of(match.group(1))
            if not leaf or leaf.lower() == "none":
                continue
            key = (package.lower(), leaf.lower())
            if key in seen:
                continue
            seen.add(key)
            if is_material(leaf) or png_exists(leaf):
                continue
            rows.append((package, leaf))
    rows.sort()
    return rows


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--packages", help="comma-separated package folder names; default is Lobby.unr imports")
    ap.add_argument("--verify", action="store_true")
    args = ap.parse_args()
    if args.packages:
        packages = {p.strip().lower() for p in args.packages.split(",") if p.strip()}
    else:
        packages = lobby_packages()
    rows = missing(packages)
    for package, leaf in rows:
        print(f"package={package} group= name={leaf} class=Texture")
    if args.verify and rows:
        print(f"ERROR {len(rows)} diffuse textures named by props have no PNG", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
