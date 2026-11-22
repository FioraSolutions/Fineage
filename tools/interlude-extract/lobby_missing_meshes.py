#!/usr/bin/env python3
"""List the Lobby-referenced static meshes whose FBX is not yet in Unity.

build_lobby_bake_meta.py verifies every mesh the lobby bake references
(StaticMeshActors + terrain deco layers) against StaticMeshes/<pkg>/<name>.fbx
and fails the Meta build on gaps. This script emits that same gap set as TSV
(package, object name, full UE2 name) so extract_login_lobby.sh step 7 exports
exactly those meshes from the package the Lobby.unr import chain assigns them
to — the package is never a hardcoded name (Lobby imports two different groups
both named "tree", from RionSctgart_tree_s and speaking_tree_s).

Output columns: package <TAB> object_name <TAB> full_name
Exit 0 always; an empty output means nothing is missing.
"""
from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from build_lobby_bake_meta import (  # noqa: E402
    WORLD,
    collect_mesh_refs,
    import_index,
    load_json,
    mesh_on_disk,
)


def main() -> int:
    doc = load_json(WORLD / "env/Lobby.unr.json.gz")
    imps = import_index(doc)
    rows = []
    for full in sorted(collect_mesh_refs(doc, imps)):
        if mesh_on_disk(full):
            continue
        parts = full.split(".")
        rows.append((parts[0], parts[-1], full))
    for pkg, name, full in rows:
        print(f"{pkg}\t{name}\t{full}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
