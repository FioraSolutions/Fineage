#!/usr/bin/env python3
"""Place exported login-screen assets (PNG textures / FBX meshes) into Unity.

Textures come from ExportNamedTextures.java (no umodel needed); meshes from the
Docker umodel export + pskx_to_fbx_batch.py. Every destination .meta is cloned
from an existing sibling so importer settings match the location.

Mesh packages come from lobby_mesh_resolve (Lobby.json package_index). A mesh
already at StaticMeshes/<package>/<mesh>.fbx is left in place. A mesh with
neither that FBX nor an export under --src is an ERROR naming mesh and package.

  python3 place_login_assets.py textures --src .work/lobby-extract/tex
  python3 place_login_assets.py meshes   --src .work/lobby-extract/mesh-export
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

# (src subdir, dest dir, meta template file, names or None for all)
TEXTURE_MAPS = [
    ("l2_skies", TEX / "l2_skies", TEX / "l2_skies/moon.png.meta", None),
    ("Lobby_terrain_t", TEX / "lobby_terrain_t", TEX / "lobby_terrain_t/H_M_04.png.meta", ["H_M_05"]),
    ("L2Font", UI_LOGIN, UI_LOGIN / "start_logo-EU.png.meta", ["start_logo-k", "start_minilogo-k"]),
]

# umodel-exported UE2 material chains (.props.txt) -> Textures/<pkg>/<Group>/.
# Existing files are kept (they came from earlier verified exports).
PROPS_PKGS = ("l2_skies",)


def clone_meta(template: Path, dest: Path) -> None:
    meta = Path(str(dest) + ".meta")
    if meta.exists():
        return
    if not template.is_file():
        raise SystemExit(f"ERROR meta template missing: {template}")
    text = re.sub(r"^guid: .+$", f"guid: {uuid.uuid4().hex}", template.read_text(encoding="utf-8"), count=1, flags=re.M)
    meta.write_text(text, encoding="utf-8")


def _png_named(src_dir: Path, name: str) -> Path | None:
    if not src_dir.is_dir():
        return None
    direct = src_dir / f"{name}.png"
    if direct.is_file():
        return direct
    want = name.lower()
    return next((p for p in src_dir.glob("*.png") if p.stem.lower() == want), None)


def place_textures(src_root: Path) -> int:
    missing: list[str] = []
    placed = 0
    for src_sub, dest_dir, template, names in TEXTURE_MAPS:
        src_dir = src_root / src_sub
        if names is None:
            existing = list(dest_dir.glob("*.png")) if dest_dir.is_dir() else []
            if not src_dir.is_dir():
                if not existing:
                    missing.append(
                        f"texture package {src_sub}: no export dir {src_dir} and no png under {dest_dir}"
                    )
                else:
                    print(
                        f"textures {src_sub}: export dir absent; "
                        f"{len(existing)} png already under {dest_dir.relative_to(UNITY)}"
                    )
                continue
            dest_dir.mkdir(parents=True, exist_ok=True)
            wrote = 0
            for png in sorted(src_dir.glob("*.png")):
                dest = dest_dir / png.name
                if not dest.exists():
                    shutil.copy2(png, dest)
                    placed += 1
                    wrote += 1
                    print(f"place {png.name} -> {dest.relative_to(UNITY)}")
                clone_meta(template, dest)
            if wrote == 0 and not existing and not any(dest_dir.glob("*.png")):
                missing.append(f"texture package {src_sub}: no png in {src_dir}")
            continue
        for name in names:
            dest = dest_dir / f"{name}.png"
            if dest.is_file():
                if template.is_file():
                    clone_meta(template, dest)
                continue
            src = _png_named(src_dir, name)
            if src is None:
                missing.append(
                    f"texture {name} (package {src_sub}): no {dest} and no {src_dir / (name + '.png')}"
                )
                continue
            dest_dir.mkdir(parents=True, exist_ok=True)
            shutil.copy2(src, dest)
            placed += 1
            print(f"place {src.name} -> {dest.relative_to(UNITY)}")
            clone_meta(template, dest)
    if missing:
        print("ERROR missing exported textures:", file=sys.stderr)
        for m in missing:
            print(f"  {m}", file=sys.stderr)
        return 1
    print(f"textures placed: {placed} new")
    return 0


def place_meshes(src_root: Path) -> int:
    from lobby_mesh_resolve import DEFAULT_LOBBY, ResolveError, existing_fbx, static_meshes

    lobby = DEFAULT_LOBBY
    if not lobby.is_file():
        print(f"ERROR Lobby dump missing: {lobby}", file=sys.stderr)
        return 1
    try:
        import json

        meshes = static_meshes(json.loads(lobby.read_text(encoding="utf-8")))
    except ResolveError as exc:
        print(f"ERROR {exc}", file=sys.stderr)
        return 1
    found: dict[str, Path] = {}
    if src_root.is_dir():
        for fbx in src_root.rglob("*.fbx"):
            found.setdefault(fbx.stem.lower(), fbx)
    missing: list[str] = []
    placed = 0
    for spec in meshes:
        if existing_fbx(SM, spec["package"], spec["name"]) is not None:
            continue
        src = found.get(spec["name"].lower())
        if src is None:
            missing.append(
                f"mesh {spec['name']} package {spec['package']} ({spec['path']}): "
                f"no {SM / spec['package'] / (spec['name'] + '.fbx')} "
                f"and no {spec['name']}.fbx under {src_root}"
            )
            continue
        dest_dir = SM / spec["package"]
        dest_dir.mkdir(parents=True, exist_ok=True)
        dest = dest_dir / f"{spec['name']}.fbx"
        shutil.copy2(src, dest)
        placed += 1
        print(f"place {src.name} -> {dest.relative_to(UNITY)} ({spec['package']})")
        template = next((p for p in dest_dir.glob("*.fbx.meta") if p.name != dest.name + ".meta"), None)
        if template is None:
            missing.append(
                f"mesh {spec['name']} package {spec['package']}: no sibling .fbx.meta under {dest_dir}"
            )
            continue
        clone_meta(template, dest)
    if missing:
        print("ERROR missing converted meshes:", file=sys.stderr)
        for m in missing:
            print(f"  {m}", file=sys.stderr)
        return 1
    print(f"meshes placed: {placed} new")
    return 0


def place_props(src_root: Path) -> int:
    placed = 0
    found = 0
    for pkg in PROPS_PKGS:
        src_dir = src_root / pkg
        dest_root = TEX / pkg
        existing = list(dest_root.rglob("*.props.txt")) if dest_root.is_dir() else []
        if not src_dir.is_dir():
            if not existing:
                print(
                    f"ERROR package {pkg}: no props export at {src_dir} and no .props.txt under {dest_root}",
                    file=sys.stderr,
                )
                return 1
            print(
                f"props {pkg}: export dir absent; {len(existing)} .props.txt already under {dest_root.relative_to(UNITY)}"
            )
            continue
        template = next(dest_root.rglob("*.props.txt.meta"), None) if dest_root.is_dir() else None
        if template is None:
            print(f"ERROR package {pkg}: no .props.txt.meta template under {dest_root}", file=sys.stderr)
            return 1
        for props in sorted(src_dir.rglob("*.props.txt")):
            found += 1
            rel = props.relative_to(src_dir)
            dest = dest_root / rel
            if not dest.exists():
                dest.parent.mkdir(parents=True, exist_ok=True)
                shutil.copy2(props, dest)
                placed += 1
                print(f"place {rel} -> {dest.relative_to(UNITY)}")
            clone_meta(template, dest)
        if found == 0 and not existing:
            print(f"ERROR package {pkg}: no .props.txt in {src_dir}", file=sys.stderr)
            return 1
    print(f"props placed: {placed} new of {found}")
    return 0


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("mode", choices=("textures", "meshes", "props"))
    ap.add_argument("--src", type=Path, required=True)
    args = ap.parse_args()
    if args.mode == "textures":
        return place_textures(args.src)
    if args.mode == "meshes":
        return place_meshes(args.src)
    return place_props(args.src)


if __name__ == "__main__":
    raise SystemExit(main())
