#!/usr/bin/env python3
"""Report prefab renderers whose material GUID is not backed by any .meta in the Unity project.

Reads every ``l2-unity/Assets/**/*.meta`` GUID once into a set, then streams every
``l2-unity/Assets/**/*.prefab`` YAML document by document (one pass, no full-file parse):

* ``GameObject`` (class 1) documents give ``m_Name`` per ``fileID``;
* ``MeshRenderer`` (class 23) and ``SkinnedMeshRenderer`` (class 137) documents give
  ``m_GameObject`` and ``m_Materials``.

A material GUID missing from the .meta index is reported with the prefab path, the owning
GameObject name and the GUID, counted per (prefab, GameObject, GUID) so a mesh used by many
instances is one line with a reference count. Renderers whose GameObject name contains
``click_area`` land in a separate "ignored" list and never affect the exit code.

Exit status: 1 when a missing GUID lives on a renderer outside ``click_area``, else 0.
"""

from __future__ import annotations

import argparse
import re
import sys
from collections import Counter
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[1]
DEFAULT_ROOT = REPO_ROOT / "l2-unity"

GUID_LINE = re.compile(rb"^guid:\s*([0-9a-fA-F]{32})\s*$", re.MULTILINE)

# --- !u!23 &1234567  (class id + local file id of one YAML document)
DOCUMENT = re.compile(r"^--- !u!(\d+) &(-?\d+)")
NAME = re.compile(r"^  m_Name:[ \t]*(.*)$")
GAME_OBJECT = re.compile(r"^  m_GameObject:\s*\{fileID:\s*(-?\d+)\}")
MATERIALS = re.compile(r"^  m_Materials:[ \t]*$")
MATERIAL_ENTRY = re.compile(r"^\s*- \{fileID:\s*-?\d+,\s*guid:\s*([0-9a-fA-F]{32})")

GAMEOBJECT_CLASS = "1"
RENDERER_CLASSES = frozenset({"23", "137"})  # MeshRenderer, SkinnedMeshRenderer
CLICK_AREA = "click_area"

# Unity writes `guid:` on line 2 of every .meta; the head read keeps the index fast, the rest is a fallback.
HEAD_BYTES = 4096


def meta_guids(root: Path) -> set[str]:
    """Every ``guid:`` value under ``root``."""
    guids: set[str] = set()
    for meta in root.rglob("*.meta"):
        try:
            with meta.open("rb") as stream:
                head = stream.read(HEAD_BYTES)
                match = GUID_LINE.search(head) or GUID_LINE.search(head + stream.read())
        except OSError as exc:
            print(f"check-prefab-material-guids: unreadable meta {meta}: {exc}", file=sys.stderr)
            continue
        if match:
            guids.add(match.group(1).decode("ascii").lower())
    return guids


def _yaml_name(raw: str) -> str:
    """Unity YAML single-line string, optionally quoted."""
    value = raw.strip()
    if len(value) >= 2 and value[0] == value[-1] and value[0] in "'\"":
        value = value[1:-1]
    return value


def scan_prefab(path: Path, known: set[str]) -> tuple[Counter[tuple[str, str]], int]:
    """Return (missing (gameobject name, guid) -> refs, renderer material refs seen) for one prefab."""
    name_by_id: dict[int, str] = {}
    renderers: list[tuple[int | None, list[str]]] = []

    class_id: str | None = None
    doc_id: int | None = None
    go_file_id: int | None = None
    in_materials = False
    pending: list[str] = []

    def close_document() -> None:
        nonlocal class_id, doc_id, go_file_id, in_materials, pending
        if class_id in RENDERER_CLASSES and pending:
            renderers.append((go_file_id, pending))
        class_id = None
        doc_id = None
        go_file_id = None
        in_materials = False
        pending = []

    try:
        with path.open("r", encoding="utf-8", errors="replace") as stream:
            for line in stream:
                header = DOCUMENT.match(line)
                if header:
                    close_document()
                    class_id = header.group(1)
                    doc_id = int(header.group(2))
                    continue

                if class_id == GAMEOBJECT_CLASS:
                    name = NAME.match(line)
                    if name is not None and doc_id is not None:
                        name_by_id[doc_id] = _yaml_name(name.group(1))
                    continue

                if class_id not in RENDERER_CLASSES:
                    continue

                if in_materials:
                    entry = MATERIAL_ENTRY.match(line)
                    if entry is not None:
                        pending.append(entry.group(1).lower())
                        continue
                    # A list item without a GUID is a built-in material: keep reading the list.
                    if line.lstrip().startswith("- "):
                        continue
                    # Any other non-blank line at document field indentation closes m_Materials.
                    if line.strip() and not line.startswith("    "):
                        in_materials = False

                if go_file_id is None:
                    owner = GAME_OBJECT.match(line)
                    if owner is not None:
                        go_file_id = int(owner.group(1))
                        continue
                if MATERIALS.match(line):
                    in_materials = True
    except OSError as exc:
        print(f"check-prefab-material-guids: unreadable prefab {path}: {exc}", file=sys.stderr)
        return Counter(), 0

    close_document()

    missing: Counter[tuple[str, str]] = Counter()
    refs = 0
    for go_id, guids in renderers:
        name = name_by_id.get(go_id, "") if go_id is not None else ""
        for guid in guids:
            refs += 1
            if guid not in known:
                missing[(name, guid)] += 1
    return missing, refs


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument(
        "--root",
        type=Path,
        default=DEFAULT_ROOT,
        help="Unity project root to scan (default: l2-unity/)",
    )
    args = parser.parse_args()
    root = args.root.resolve()
    if not root.is_dir():
        print(f"check-prefab-material-guids: root not found: {root}", file=sys.stderr)
        return 2

    known = meta_guids(root)

    missing_rows: list[tuple[str, str, str, int]] = []
    ignored_rows: list[tuple[str, str, str, int]] = []
    prefabs = 0
    refs_total = 0
    for prefab in sorted(root.rglob("*.prefab")):
        prefabs += 1
        missing, refs = scan_prefab(prefab, known)
        refs_total += refs
        rel = prefab.relative_to(root).as_posix()
        for (name, guid), count in sorted(missing.items()):
            (ignored_rows if CLICK_AREA in name else missing_rows).append((rel, name, guid, count))

    missing_refs = sum(count for _, _, _, count in missing_rows)
    ignored_refs = sum(count for _, _, _, count in ignored_rows)

    print(f"Prefab material GUID audit over {root}")
    print(f"meta GUIDs indexed: {len(known)}")
    print(f"prefabs scanned: {prefabs} ({refs_total} MeshRenderer/SkinnedMeshRenderer material refs)")
    print()
    print(f"MISSING material GUIDs outside click_area: {missing_refs} refs, "
          f"{len({guid for _, _, guid, _ in missing_rows})} unique GUIDs")
    for rel, name, guid, count in missing_rows:
        print(f"  {rel}: GameObject '{name}' -> {guid} (x{count})")
    print()
    print(f"IGNORED (click_area): {ignored_refs} refs, {len({guid for _, _, guid, _ in ignored_rows})} unique GUIDs")
    for rel, name, guid, count in ignored_rows:
        print(f"  {rel}: GameObject '{name}' -> {guid} (x{count})")
    print()
    if missing_rows:
        print(f"FAIL: {missing_refs} missing material GUID reference(s) outside click_area")
        return 1
    print("OK: every renderer material GUID resolves to a .meta")
    return 0


if __name__ == "__main__":
    sys.exit(main())
