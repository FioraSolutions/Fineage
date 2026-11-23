#!/usr/bin/env python3
"""Diffuse textures named by committed material props for lobby packages.

Brush export only walks BSP faces. Static-mesh skins and terrain layers name
textures in *.props.txt (Diffuse = Texture'...') under the package folder.
This lists every such leaf whose PNG is not already where the reference
loads it from. A bare Texture'X' resolves by leaf anywhere (the binder's
rule); a qualified Texture'Pkg.X' / Texture'Pkg.Grp.X' resolves only against
Pkg's folder — the original engine loads it from that package. Rows print
package (where the texture must land), own (the material's package, for
Group.Object references that are exports of it), name, class so step 6 can
export. --verify exits 1 when any row remains.
"""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
MESHES = REPO / "l2-unity/Assets/Resources/Data/StaticMeshes"
ENV = REPO / "l2-unity/docs/interlude-ref/world/env"
LOBBY_DUMP = ENV / "Lobby.unr.json.gz"
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


def package_of(path: Path, tex: Path = TEX) -> str:
    rel = path.relative_to(tex)
    return rel.parts[0]


def leaf_of(token: str) -> str:
    return token.split(".")[-1]


_MATERIAL_LEAVES: dict[Path, set[str]] = {}


def material_leaves(tex: Path = TEX) -> set[str]:
    """Props file stems are materials. A Diffuse pointing at one is not a PNG."""
    if tex not in _MATERIAL_LEAVES:
        _MATERIAL_LEAVES[tex] = {p.name[: -len(".props.txt")].lower() for p in tex.rglob("*.props.txt")}
    return _MATERIAL_LEAVES[tex]


def is_material(leaf: str, tex: Path = TEX) -> bool:
    return leaf.lower() in material_leaves(tex)


_PNG_LEAVES: dict[Path, set[str]] = {}
_PACKAGE_FOLDERS: dict[Path, dict[str, Path]] = {}


def png_exists(leaf: str, tex: Path = TEX) -> bool:
    """The binder resolves an unqualified diffuse by leaf name anywhere under Textures/."""
    if tex not in _PNG_LEAVES:
        _PNG_LEAVES[tex] = {p.name[: -len(".png")].lower() for p in tex.rglob("*.png")}
    return leaf.lower() in _PNG_LEAVES[tex]


def package_folders(tex: Path = TEX) -> dict[str, Path]:
    if tex not in _PACKAGE_FOLDERS:
        _PACKAGE_FOLDERS[tex] = (
            {child.name.lower(): child for child in tex.iterdir() if child.is_dir()}
            if tex.is_dir() else {}
        )
    return _PACKAGE_FOLDERS[tex]


def png_in_package(package: str, leaf: str, tex: Path = TEX) -> bool:
    """A qualified reference is satisfied only by a PNG under the named package.

    The original engine resolves Texture'Deco_T.X' against Deco_T.utx, not
    against a same-named texture in the material's own package.
    """
    folder = package_folders(tex).get(package.lower())
    if folder is None:
        return False
    leaf = leaf.lower()
    return any(p.name[: -len(".png")].lower() == leaf for p in folder.rglob("*.png"))


_ENV_GROUPS: dict[tuple[Path, str], set[tuple[str, str]] | None] = {}


def env_groups(package: str, env: Path = ENV) -> set[tuple[str, str]] | None:
    """(group, name) pairs of every export in the package's committed dump.

    None when the package was never dumped: a two-segment reference is then
    treated as cross-package, and step 6 falls back to the material's own
    .utx when the named one does not exist (a group reference printed as
    Group.Object is an export of the props-own package).
    """
    key = (env, package.lower())
    if key not in _ENV_GROUPS:
        path = env / f"{package}.json"
        if not path.is_file():
            _ENV_GROUPS[key] = None
        else:
            import json
            data = json.loads(path.read_text(encoding="utf-8"))
            pairs = set()
            for entry in data.get("exports", []):
                outer = str(entry.get("outer", ""))
                group = outer.rsplit(":", 1)[-1] if ":" in outer else ""
                pairs.add((group.lower(), str(entry.get("name", "")).lower()))
            _ENV_GROUPS[key] = pairs
    return _ENV_GROUPS[key]


def want_package(token: str, own: str, env: Path = ENV) -> str:
    """Package the reference loads the texture from.

    Bare names load from the material's own package. Two-segment references
    are Package.Object cross-package or Group.Object same-package — the dump
    of the props-own package decides; without a dump, cross-package (step 6
    retries against the own .utx when the named one is absent).
    """
    parts = token.split(".")
    if len(parts) == 1:
        return own
    first = parts[0]
    if first.lower() == own.lower():
        return own
    groups = env_groups(own, env)
    if groups is not None and (first.lower(), parts[-1].lower()) in groups:
        return own
    return first


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


def missing(packages: set[str] | None, tex: Path = TEX,
            skins: set[str] | None = None, env: Path = ENV) -> list[tuple[str, str, str]]:
    rows: list[tuple[str, str, str]] = []
    if not tex.is_dir():
        return rows
    if skins is None:
        skins = lobby_skin_stems() if packages is not None else set()
    seen: set[tuple[str, str, str]] = set()
    for props in tex.rglob("*.props.txt"):
        package = package_of(props, tex)
        stem = props.name[: -len(".props.txt")].lower()
        in_package = packages is None or package.lower() in packages
        if not in_package and stem not in skins:
            continue
        text = props.read_text(encoding="utf-8", errors="replace")
        for match in DIFFUSE.finditer(text):
            token = match.group(1)
            leaf = leaf_of(token)
            if not leaf or leaf.lower() == "none":
                continue
            # The row names the package the original engine loads the texture
            # from. own= stays on the row so step 6 can resolve a group
            # reference (Group.Object) against the material's own .utx.
            want = want_package(token, package, env)
            key = (want.lower(), package.lower(), leaf.lower())
            if key in seen:
                continue
            seen.add(key)
            if is_material(leaf, tex):
                continue
            qualified = "." in token
            have = png_in_package(want, leaf, tex) if qualified else png_exists(leaf, tex)
            if have:
                continue
            rows.append((want, package, leaf))
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
    for want, own, leaf in rows:
        print(f"package={want} own={own} group= name={leaf} class=Texture")
    if args.verify and rows:
        print(f"ERROR {len(rows)} diffuse textures named by props have no PNG", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
