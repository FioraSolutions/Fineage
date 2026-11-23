#!/usr/bin/env python3
"""Resolve Lobby.unr imports by walking package_index.

Two Package imports can share an object name (`tree`, `Town`). Looking the
parent up by that name binds the mesh to the wrong .usx. The UE package index
is unique, so every caller (extract, place, bake meta) walks that index.

  python3 lobby_mesh_resolve.py --check
  python3 lobby_mesh_resolve.py --missing-fbx --client "$L2_INTERLUDE_CLIENT_ROOT"

--check exits 0 only when every Lobby.json StaticMesh import resolves to a
package path. A missing parent is a concrete error naming the mesh and the
package_index that did not resolve. --missing-fbx prints one TSV row per
StaticMesh whose FBX is not already under StaticMeshes/<package>/:

  package<TAB>mesh<TAB>client-relative .usx

and exits 1 when --client is set and that package file is not in the client.
"""
from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
DEFAULT_LOBBY = REPO / "l2-unity/docs/interlude-ref/world/env/Lobby.json"
DEFAULT_SM = REPO / "l2-unity/Assets/Resources/Data/StaticMeshes"

BY_UE = "__by_ue__"
AMBIGUOUS = "__ambiguous__"


class ResolveError(Exception):
    """A mesh/package chain could not be resolved. The message names both."""


def by_ue_index(doc: dict) -> dict[int, dict]:
    """Map UE import index (negative) to the import record."""
    out: dict[int, dict] = {}
    for im in doc.get("imports") or []:
        ue = -(int(im["import_index"]) + 1)
        if ue in out:
            name = im.get("object_name")
            raise ResolveError(
                f"import {name}: duplicate UE index {ue} (import_index {im.get('import_index')})"
            )
        out[ue] = im
    return out


def object_path(im: dict, table: dict[int, dict]) -> str:
    """Dotted object path. parts[0] is the outer package (.usx / folder).

    Raises ResolveError naming the object when package_index does not reach
    a package, the parent import is missing, or the chain cycles.
    """
    name = im.get("object_name") or "?"
    cls = im.get("class_name") or "?"
    parts = [name]
    cur = im
    seen: set[int] = set()
    while True:
        raw = cur.get("package_index")
        pidx = 0 if raw is None else int(raw)
        if pidx >= 0:
            break
        if pidx in seen:
            raise ResolveError(
                f"{cls} {name}: import cycle at package_index {pidx} "
                f"(chain {'.'.join(reversed(parts))})"
            )
        seen.add(pidx)
        parent = table.get(pidx)
        if parent is None:
            raise ResolveError(
                f"{cls} {name}: package_index {pidx} is not an import "
                f"(chain {'.'.join(reversed(parts))})"
            )
        parent_name = parent.get("object_name") or ""
        if not parent_name:
            raise ResolveError(
                f"{cls} {name}: package_index {pidx} has an empty object_name "
                f"(chain {'.'.join(reversed(parts))})"
            )
        parts.append(parent_name)
        cur = parent
        if len(parts) > 16:
            raise ResolveError(f"{cls} {name}: import chain longer than 16")
    if len(parts) < 2 or not parts[-1]:
        raise ResolveError(
            f"{cls} {name}: import chain did not reach a package "
            f"(package_index {im.get('package_index')})"
        )
    parts.reverse()
    return ".".join(parts)


def static_meshes(doc: dict) -> list[dict]:
    """Every StaticMesh import: name, outer package, dotted path, .usx leaf."""
    table = by_ue_index(doc)
    rows: list[dict] = []
    for im in doc.get("imports") or []:
        if im.get("class_name") != "StaticMesh":
            continue
        path = object_path(im, table)
        parts = path.split(".")
        package, mesh = parts[0], parts[-1]
        if not package or not mesh:
            raise ResolveError(f"StaticMesh {im.get('object_name')}: empty package in {path}")
        rows.append(
            {
                "name": mesh,
                "package": package,
                "path": path,
                "usx": f"staticmeshes/{package}.usx",
            }
        )
    if not rows:
        raise ResolveError("dump has no StaticMesh imports")
    return rows


def existing_fbx(sm_root: Path, package: str, name: str) -> Path | None:
    """Case-insensitive StaticMeshes/<package>/<name>.fbx, or None."""
    if not sm_root.is_dir():
        return None
    folder = next((p for p in sm_root.iterdir() if p.is_dir() and p.name.lower() == package.lower()), None)
    if folder is None:
        return None
    want = f"{name}.fbx".lower()
    return next((p for p in folder.iterdir() if p.is_file() and p.name.lower() == want), None)


def client_usx(client: Path, package: str) -> Path:
    """The client's staticmeshes/<package>.usx, preserving on-disk case."""
    if not client.is_dir():
        raise ResolveError(f"package {package}: client root is not a directory ({client})")
    folders = [p for p in client.iterdir() if p.is_dir() and p.name.lower() == "staticmeshes"]
    if not folders:
        raise ResolveError(
            f"package {package}: no staticmeshes/ directory under {client} "
            f"(StaticMesh package file {package}.usx)"
        )
    want = f"{package}.usx".lower()
    hits: list[Path] = []
    for folder in folders:
        hits.extend(p for p in folder.iterdir() if p.is_file() and p.name.lower() == want)
    if not hits:
        raise ResolveError(
            f"package {package}: {package}.usx is not under {client}/staticmeshes "
            f"(StaticMesh package file missing)"
        )
    if len(hits) > 1:
        listed = ", ".join(str(p) for p in hits)
        raise ResolveError(f"package {package}: more than one {package}.usx ({listed})")
    return hits[0]


def _spec_walk(im: dict, table: dict[int, dict]) -> str:
    """Independent package_index walk used only to check object_path."""
    parts = [im["object_name"]]
    cur = im
    seen: set[int] = set()
    while True:
        pidx = int(cur.get("package_index") or 0)
        if pidx >= 0:
            break
        if pidx in seen:
            raise ResolveError(f"spec cycle at {im['object_name']} package_index {pidx}")
        seen.add(pidx)
        parent = table[pidx]
        parts.insert(0, parent["object_name"])
        cur = parent
    return ".".join(parts)


def _synthetic() -> None:
    """Two packages named tree. The mesh must follow package_index, not the name."""
    doc = {
        "imports": [
            {"import_index": 0, "object_name": "Pkg", "class_name": "Package", "package_index": 0},
            {"import_index": 1, "object_name": "tree", "class_name": "Package", "package_index": -1},
            {"import_index": 2, "object_name": "tree", "class_name": "Package", "package_index": 0},
            {"import_index": 3, "object_name": "MeshA", "class_name": "StaticMesh", "package_index": -2},
        ]
    }
    rows = static_meshes(doc)
    if rows != [
        {
            "name": "MeshA",
            "package": "Pkg",
            "path": "Pkg.tree.MeshA",
            "usx": "staticmeshes/Pkg.usx",
        }
    ]:
        raise ResolveError(f"synthetic package_index walk returned {rows}")
    orphan = {
        "imports": [
            {"import_index": 0, "object_name": "Orphan", "class_name": "StaticMesh", "package_index": -9}
        ]
    }
    try:
        static_meshes(orphan)
    except ResolveError as exc:
        text = str(exc)
        if "Orphan" not in text or "-9" not in text:
            raise ResolveError(f"missing-parent error did not name the mesh and index: {text}") from exc
    else:
        raise ResolveError("StaticMesh Orphan with a missing package_index did not error")


def run_check(lobby_path: Path) -> int:
    _synthetic()
    if not lobby_path.is_file():
        print(f"ERROR Lobby dump missing: {lobby_path}", file=sys.stderr)
        return 1
    doc = json.loads(lobby_path.read_text(encoding="utf-8"))
    try:
        rows = static_meshes(doc)
    except ResolveError as exc:
        print(f"ERROR {exc}", file=sys.stderr)
        return 1
    table = by_ue_index(doc)
    by_name = {im["object_name"]: im for im in doc["imports"] if im.get("class_name") == "StaticMesh"}
    mismatches: list[str] = []
    for row in rows:
        spec = _spec_walk(by_name[row["name"]], table)
        if spec != row["path"] or not row["package"] or row["path"].split(".")[0] != row["package"]:
            mismatches.append(f"{row['name']}: resolver {row['path']} spec {spec}")
    # bake meta resolves meshes through full_name, which must use this walk.
    sys.path.insert(0, str(Path(__file__).resolve().parent))
    from rebuild_deco_layers import full_name, import_index

    imps = import_index(doc)
    for row in rows:
        got = full_name(imps, f"import:Engine:StaticMesh:{row['name']}", "StaticMesh")
        if got != row["path"]:
            mismatches.append(f"{row['name']}: full_name {got} resolver {row['path']}")
    dwarfs = [r for r in rows if r["name"] == "Dwarftree01"]
    if len(dwarfs) != 1 or dwarfs[0]["path"] != "RionSctgart_tree_s.tree.Dwarftree01":
        got = dwarfs[0]["path"] if dwarfs else "missing"
        mismatches.append(f"Dwarftree01: expected RionSctgart_tree_s.tree.Dwarftree01, got {got}")
    if mismatches:
        print("ERROR static mesh package resolution:", file=sys.stderr)
        for line in mismatches:
            print(f"  {line}", file=sys.stderr)
        return 1
    packages = sorted({r["package"] for r in rows})
    print(f"{lobby_path.name}: {len(rows)} StaticMesh imports, {len(packages)} packages")
    for row in rows:
        if row["package"] == "RionSctgart_tree_s":
            print(f"  {row['path']}")
    return 0


def run_missing(lobby_path: Path, sm_root: Path, client: Path | None) -> int:
    if not lobby_path.is_file():
        print(f"ERROR Lobby dump missing: {lobby_path}", file=sys.stderr)
        return 1
    doc = json.loads(lobby_path.read_text(encoding="utf-8"))
    try:
        rows = static_meshes(doc)
    except ResolveError as exc:
        print(f"ERROR {exc}", file=sys.stderr)
        return 1
    errors: list[str] = []
    lines: list[str] = []
    for row in rows:
        if existing_fbx(sm_root, row["package"], row["name"]) is not None:
            continue
        usx_rel = row["usx"]
        if client is not None:
            try:
                found = client_usx(client, row["package"])
            except ResolveError as exc:
                errors.append(f"mesh {row['name']} package {row['package']}: {exc}")
                continue
            usx_rel = found.relative_to(client).as_posix()
        lines.append(f"{row['package']}\t{row['name']}\t{usx_rel}")
    if errors:
        print("ERROR lobby mesh package file missing:", file=sys.stderr)
        for line in errors:
            print(f"  {line}", file=sys.stderr)
        return 1
    sys.stdout.write("".join(f"{line}\n" for line in lines))
    return 0


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--check", action="store_true", help="Resolve every Lobby.json StaticMesh import")
    ap.add_argument("--missing-fbx", action="store_true", help="TSV of meshes with no FBX yet")
    ap.add_argument("--lobby", type=Path, default=DEFAULT_LOBBY)
    ap.add_argument("--staticmeshes", type=Path, default=DEFAULT_SM)
    ap.add_argument("--client", type=Path, default=None, help="Interlude client root (staticmeshes/*.usx)")
    args = ap.parse_args()
    if args.check == args.missing_fbx:
        ap.error("pass exactly one of --check or --missing-fbx")
    if args.check:
        return run_check(args.lobby)
    return run_missing(args.lobby, args.staticmeshes, args.client)


if __name__ == "__main__":
    raise SystemExit(main())
