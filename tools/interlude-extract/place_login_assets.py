#!/usr/bin/env python3
"""Place exported login-screen assets (PNG textures / FBX meshes) into Unity.

Textures come from ExportNamedTextures.java (no umodel needed); meshes from the
Docker umodel export + pskx_to_fbx_batch.py. Every destination .meta is cloned
from an existing sibling so importer settings match the location. Only files on
the explicit want-lists are placed; a missing source is an ERROR (exit 1).

  python3 place_login_assets.py textures --src .work/lobby-extract/tex
  python3 place_login_assets.py meshes   --src .work/lobby-extract/mesh-export \
      --list .work/lobby-extract/lobby-meshes.tsv
"""
from __future__ import annotations

import argparse
import re
import shutil
import sys
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
UNITY = REPO / "l2-unity"
TEX = UNITY / "Assets/Resources/Data/Textures"
SM = UNITY / "Assets/Resources/Data/StaticMeshes"
UI_LOGIN = UNITY / "Assets/Resources/Data/UI/Assets/Login"

sys.path.insert(0, str(REPO / "scripts" / "lib"))
from asset_import_policy import (  # noqa: E402
    apply_texture_meta,
    build_texture_expectation,
    load_rules,
    posix_path,
)

# Cloned sibling metas predate the ImportPolicy rules, so a fresh clone drifts
# (CI pr-sanity gate). Every newly placed texture meta is normalized through
# the policy itself — the same apply_texture_meta the gate's --apply uses.
_RULES = load_rules()


def normalize_texture_meta(dest: Path) -> None:
    meta = Path(str(dest) + ".meta")
    if not meta.is_file():
        return
    expectation = build_texture_expectation(posix_path(dest), _RULES, dest)
    if expectation is None:
        raise SystemExit(f"ERROR no texture import rule matched: {posix_path(dest)}")
    updated = apply_texture_meta(meta.read_text(encoding="utf-8"), expectation)
    if updated != meta.read_text(encoding="utf-8"):
        meta.write_text(updated, encoding="utf-8", newline="\n")

# (src subdir, dest dir, meta template file, names or None for all)
TEXTURE_MAPS = [
    ("l2_skies", TEX / "l2_skies", TEX / "l2_skies/moon.png.meta", None),
    ("Lobby_terrain_t", TEX / "lobby_terrain_t", TEX / "lobby_terrain_t/H_M_04.png.meta", ["H_M_05"]),
    ("L2Font", UI_LOGIN, UI_LOGIN / "start_logo-EU.png.meta", ["start_logo-k", "start_minilogo-k"]),
]

# umodel-exported UE2 material chains (.props.txt) -> Textures/<pkg>/<Group>/.
# Only packages whose lobby material chains are not yet committed. Existing
# files are kept when byte-identical; a different existing file is an ERROR
# (it came from an earlier verified export — never silently overwritten).
PROPS_PKGS = ("l2_skies",)


def clone_meta(template: Path, dest: Path) -> None:
    meta = Path(str(dest) + ".meta")
    if meta.exists():
        return
    if not template.is_file():
        raise SystemExit(f"ERROR meta template missing: {template}")
    text = re.sub(r"^guid: .+$", f"guid: {uuid.uuid4().hex}", template.read_text(encoding="utf-8"), count=1, flags=re.M)
    meta.write_text(text, encoding="utf-8")


def place_textures(src_root: Path) -> int:
    missing: list[str] = []
    placed = 0
    for src_sub, dest_dir, template, names in TEXTURE_MAPS:
        src_dir = src_root / src_sub
        want = {n.lower(): n for n in names} if names else None
        if not src_dir.is_dir():
            missing.append(f"{src_dir} (whole export dir)")
            continue
        dest_dir.mkdir(parents=True, exist_ok=True)
        for png in sorted(src_dir.glob("*.png")):
            if want is not None and png.stem.lower() not in want:
                continue
            dest = dest_dir / png.name
            if not dest.exists():
                shutil.copy2(png, dest)
                placed += 1
                print(f"place {png.name} -> {dest.relative_to(UNITY)}")
            clone_meta(template, dest)
            normalize_texture_meta(dest)
        if want is not None:
            have = {p.stem.lower() for p in src_dir.glob("*.png")}
            for n in want.values():
                if n.lower() not in have:
                    missing.append(f"{src_dir / (n + '.png')}")
    if missing:
        print("ERROR missing exported textures:", file=sys.stderr)
        for m in missing:
            print(f"  {m}", file=sys.stderr)
        return 1
    print(f"textures placed: {placed} new")
    return 0


def place_meshes(src_root: Path, want_list: Path) -> int:
    # The want-list is the lobby_missing_meshes.py TSV: package, object name,
    # full UE2 name — the package comes from the Lobby.unr import chain, not
    # from a hardcoded name map.
    want: dict[str, str] = {}
    for line in want_list.read_text(encoding="utf-8").splitlines():
        if not line.strip():
            continue
        pkg, name, full = line.split("\t")
        want[name] = (pkg, full)
    if not want:
        print(f"ERROR empty mesh want-list {want_list}", file=sys.stderr)
        return 1
    missing: list[str] = []
    placed = 0
    found: dict[str, Path] = {}
    for fbx in src_root.rglob("*.fbx"):
        found.setdefault(fbx.stem, fbx)
    for name, (pkg, full) in want.items():
        src = found.get(name)
        if src is None:
            missing.append(f"{name}.fbx ({full}) under {src_root}")
            continue
        dest_dir = SM / pkg
        dest_dir.mkdir(parents=True, exist_ok=True)
        dest = dest_dir / src.name
        if not dest.exists():
            shutil.copy2(src, dest)
            placed += 1
            print(f"place {src.name} -> {dest.relative_to(UNITY)}")
        template = next((p for p in dest_dir.glob("*.fbx.meta") if p.name != dest.name + ".meta"), None)
        if template is not None:
            clone_meta(template, dest)
    if missing:
        print("ERROR missing converted meshes:", file=sys.stderr)
        for m in missing:
            print(f"  {m}", file=sys.stderr)
        return 1
    print(f"meshes placed: {placed} new")
    return 0


def place_props(src_root: Path, packages: list[str] | None = None) -> int:
    placed = 0
    found = 0
    kept = 0
    pkgs = packages if packages else list(PROPS_PKGS)
    for pkg in pkgs:
        src_dir = src_root / pkg
        if not src_dir.is_dir():
            print(f"ERROR missing umodel export dir {src_dir}", file=sys.stderr)
            return 1
        dest_root = TEX / pkg
        template = next(dest_root.rglob("*.props.txt.meta"), None)
        if template is None:
            template = next(TEX.rglob("*.props.txt.meta"), None)
        if template is None:
            print(f"ERROR no .props.txt.meta template for {pkg}", file=sys.stderr)
            return 1
        for props in sorted(src_dir.rglob("*.props.txt")):
            found += 1
            rel = props.relative_to(src_dir)
            # umodel writes <out>/<Package>/<Group>/<name>.props.txt; the
            # destination already IS Textures/<pkg>, so the package folder
            # level must not be repeated under it.
            if rel.parts and rel.parts[0].lower() == pkg.lower():
                rel = Path(*rel.parts[1:])
            dest = dest_root / rel
            if dest.exists():
                if dest.read_bytes() != props.read_bytes():
                    print(
                        f"ERROR {dest} already exists with different content than the fresh export {props}",
                        file=sys.stderr,
                    )
                    return 1
                kept += 1
                continue
            dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(props, dest)
            placed += 1
            print(f"place {rel} -> {dest.relative_to(UNITY)}")
            clone_meta(template, dest)
    if found == 0:
        print(f"ERROR no .props.txt under {src_root}", file=sys.stderr)
        return 1
    print(f"props placed: {placed} new of {found} ({kept} already identical)")
    return 0


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("mode", choices=("textures", "meshes", "props"))
    ap.add_argument("--src", type=Path, required=True)
    ap.add_argument("--list", type=Path, default=None,
                    help="meshes mode: lobby_missing_meshes.py TSV (package, object, full name)")
    ap.add_argument("--packages", default="",
                    help="props mode: comma-separated packages (default: the l2_skies step-9 set)")
    args = ap.parse_args()
    if not args.src.is_dir():
        print(f"ERROR no export dir {args.src}", file=sys.stderr)
        return 1
    if args.mode == "textures":
        return place_textures(args.src)
    if args.mode == "meshes":
        if args.list is None or not args.list.is_file():
            print("ERROR meshes mode wants --list <lobby_missing_meshes TSV>", file=sys.stderr)
            return 1
        return place_meshes(args.src, args.list)
    packages = [p for p in args.packages.split(",") if p] or None
    return place_props(args.src, packages)


if __name__ == "__main__":
    raise SystemExit(main())
