#!/usr/bin/env python3
"""Re-point race armor prefabs at the correct FBX guid.

Overnight import wrote FBX under Animations/{Pkg}/{Race}/Models/ while many
existing prefabs still nested PrefabInstances of Animations/{Pkg}/Models/*.fbx
(or a stale guid). Result: chest/legs fail to resolve → gingerbread orcs.

Usage:
  python3 tools/interlude/repair_armor_prefab_guids.py [--race MOrc] [--dry-run]
"""
from __future__ import annotations

import argparse
import re
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
ANIM = REPO / "l2-unity/Assets/Resources/Data/Animations"

RACES = {
    "MOrc": "Orc",
    "FOrc": "Orc",
    "MShaman": "Shaman",
    "FShaman": "Shaman",
    "MFighter": "Fighter",
    "FFighter": "Fighter",
    "MMagic": "Magic",
    "FMagic": "Magic",
    "MDarkElf": "DarkElf",
    "FDarkElf": "DarkElf",
    "MDwarf": "Dwarf",
    "FDwarf": "Dwarf",
    "MElf": "Elf",
    "FElf": "Elf",
}


def file_id() -> int:
    return uuid.uuid4().int % (2**62)


def read_guid(meta: Path) -> str:
    m = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(), re.M)
    if not m:
        raise RuntimeError(f"no guid in {meta}")
    return m.group(1)


def find_fbx(pkg: str, race: str, asset: str) -> Path | None:
    """Prefer Race/Models (MElf layout), then flat Models/."""
    cands = [
        ANIM / pkg / race / "Models" / f"{asset}.fbx",
        ANIM / pkg / "Models" / f"{asset}.fbx",
    ]
    for p in cands:
        if p.is_file() and p.with_suffix(".fbx.meta").is_file():
            return p
    hits = list((ANIM / pkg).rglob(f"{asset}.fbx"))
    for p in hits:
        if p.with_suffix(".fbx.meta").is_file():
            return p
    return None


def write_prefab(prefab: Path, asset: str, fbx_guid: str) -> None:
    outer_go = file_id()
    outer_tr = file_id()
    inner_tr = file_id()
    prefab_inst = file_id()
    text = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!1 &{outer_go}
GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {outer_tr}}}
  m_Layer: 6
  m_Name: {asset}
  m_TagString: User
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
--- !u!4 &{outer_tr}
Transform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {outer_go}}}
  serializedVersion: 2
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children:
  - {{fileID: {inner_tr}}}
  m_Father: {{fileID: 0}}
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
--- !u!1001 &{prefab_inst}
PrefabInstance:
  m_ObjectHideFlags: 0
  serializedVersion: 2
  m_Modification:
    serializedVersion: 3
    m_TransformParent: {{fileID: {outer_tr}}}
    m_Modifications:
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalPosition.x
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalPosition.y
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalPosition.z
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalRotation.w
      value: 1
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalRotation.x
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalRotation.y
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalRotation.z
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.x
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.y
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
      propertyPath: m_LocalEulerAnglesHint.z
      value: 0
      objectReference: {{fileID: 0}}
    - target: {{fileID: 919132149155446097, guid: {fbx_guid}, type: 3}}
      propertyPath: m_Name
      value: {asset}
      objectReference: {{fileID: 0}}
    m_RemovedComponents: []
    m_RemovedGameObjects: []
    m_AddedGameObjects: []
    m_AddedComponents: []
  m_SourcePrefab: {{fileID: 100100000, guid: {fbx_guid}, type: 3}}
--- !u!4 &{inner_tr} stripped
Transform:
  m_CorrespondingSourceObject: {{fileID: -8679921383154817045, guid: {fbx_guid}, type: 3}}
  m_PrefabInstance: {{fileID: {prefab_inst}}}
  m_PrefabAsset: {{fileID: 0}}
"""
    prefab.write_text(text)
    meta = prefab.with_suffix(".prefab.meta")
    if not meta.exists():
        meta.write_text(
            "fileFormatVersion: 2\n"
            f"guid: {uuid.uuid4().hex}\n"
            "PrefabImporter:\n"
            "  externalObjects: {}\n"
            "  userData: \n"
            "  assetBundleName: \n"
            "  assetBundleVariant: \n"
        )


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--race", action="append", default=[], help="Limit to race prefix(es)")
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument("--only-mismatch", action="store_true", default=True)
    ap.add_argument("--all", action="store_true", help="Rewrite every race armor prefab")
    args = ap.parse_args()
    if args.all:
        args.only_mismatch = False

    races = args.race or list(RACES)
    fixed = 0
    skipped = 0
    missing = 0
    for race in races:
        pkg = RACES.get(race)
        if not pkg:
            print("unknown race", race)
            continue
        for prefab in sorted((ANIM / pkg).glob(f"{race}_*.prefab")):
            asset = prefab.stem
            fbx = find_fbx(pkg, race, asset)
            if fbx is None:
                missing += 1
                continue
            guid = read_guid(fbx.with_suffix(".fbx.meta"))
            current = prefab.read_text()
            ok = guid in current
            if args.only_mismatch and ok:
                skipped += 1
                continue
            preferred = ANIM / pkg / race / "Models" / f"{asset}.fbx"
            # Prefer Race/Models when both exist
            if preferred.is_file() and fbx != preferred:
                fbx = preferred
                guid = read_guid(fbx.with_suffix(".fbx.meta"))
            print(("FIX" if not args.dry_run else "WOULD"), prefab.relative_to(REPO), "->", fbx.relative_to(REPO), guid[:8])
            if not args.dry_run:
                write_prefab(prefab, asset, guid)
            fixed += 1
    print(f"done fixed={fixed} skipped_ok={skipped} missing_fbx={missing}")


if __name__ == "__main__":
    main()
