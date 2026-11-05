#!/usr/bin/env python3
"""Import umodel-exported L2 elf armor pieces (FBX + textures + materials + prefabs) into Unity.

Usage:
  python3 tools/interlude/import_elf_armor.py \
      --fbx-dir .work/elfarmor/Elf/FBX \
      --tex-root .work/elftex \
      --names /tmp/have.txt \
      --armorgrp l2-unity/Assets/StreamingAssets/Data/Meta/Armorgrp_Classic.txt

Writes into l2-unity/Assets/Resources/Data/. Never overwrites existing assets.
"""
import argparse
import os
import re
import shutil
import subprocess
import sys
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
ANIM_ROOT = REPO / "l2-unity/Assets/Resources/Data/Animations/Elf"
TEX_ROOT = REPO / "l2-unity/Assets/Resources/Data/SysTextures"

FBX_META_TEMPLATE = (ANIM_ROOT / "MElf/Models/MElf_m001_u.fbx.meta")
MAT_TEMPLATE = (TEX_ROOT / "MElf/Materials/MElf_m001_t01_u.mat")
MAT_META_TEMPLATE = (TEX_ROOT / "MElf/Materials/MElf_m001_t01_u.mat.meta")
PNG_META_TEMPLATE = (TEX_ROOT / "MElf/MElf_m001_t01_u.png.meta")
PREFAB_TEMPLATE = (ANIM_ROOT / "MElf_m001_u.prefab")


def _template_png_guid():
    m = re.search(r"^guid: ([0-9a-f]{32})$", PNG_META_TEMPLATE.read_text(), re.M)
    if m is None:
        raise RuntimeError(f"no guid in {PNG_META_TEMPLATE}")
    return m.group(1)


TEMPLATE_PNG_GUID = _template_png_guid()


def guid():
    return uuid.uuid4().hex


def file_id():
    """Unity YAML fileIDs must fit in a signed 64-bit int (existing prefabs use ~8.8e18)."""
    return uuid.uuid4().int % (2 ** 62)


def read_template(path):
    return path.read_text()


def write_new(path, text):
    if path.exists():
        return False
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text)
    return True


def strip_guid(text, new_guid):
    return re.sub(r"^guid: [0-9a-f]{32}$", f"guid: {new_guid}", text, count=1, flags=re.M)


def make_fbx_meta(dest_meta: Path):
    if dest_meta.exists():
        return
    g = guid()
    dest_meta.write_text(strip_guid(read_template(FBX_META_TEMPLATE), g))


def make_prefab(asset: str, fbx_guid: str, package: str, overwrite: bool = False):
    """Prefab wrapper around the FBX, mirroring the existing MElf_m001_u.prefab."""
    dest = ANIM_ROOT / f"{asset}.prefab"
    if dest.exists() and not overwrite:
        return False
    meta = ANIM_ROOT / f"{asset}.prefab.meta"
    g = uuid.uuid4().hex
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
    dest.write_text(text)
    meta.write_text(
        "fileFormatVersion: 2\n"
        f"guid: {g}\n"
        "PrefabImporter:\n"
        "  externalObjects: {}\n"
        "  userData: \n"
        "  assetBundleName: \n"
        "  assetBundleVariant: \n"
    )
    return True


TEX_VARIANTS = ("", "_sp", "_ori", "_sp2", "_sp1", "_ori1")


def find_texture(tex_root: Path, name: str):
    """umodel writes `Name.tga` or `Name_sp.tga`. Directory is named after the package."""
    lower = name.lower()
    for variant in TEX_VARIANTS:
        for p in tex_root.rglob(f"{name}{variant}.tga"):
            return p
    for p in tex_root.rglob("*.tga"):
        if p.stem.lower() == lower:
            return p
    return None


def import_texture(tex_root: Path, asset: str, package: str) -> str | None:
    """Copy/convert the TGA to a Unity PNG. Returns the png guid, or None if unavailable."""
    out_dir = TEX_ROOT / package
    out_png = out_dir / f"{asset}.png"
    if out_png.exists():
        meta = out_png.with_suffix(".png.meta")
        if meta.exists():
            m = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(), re.M)
            if m:
                return m.group(1)
    src = find_texture(tex_root, asset)
    if src is None:
        return None
    out_dir.mkdir(parents=True, exist_ok=True)
    subprocess.run(["magick", str(src), str(out_png)], check=True)
    g = guid()
    (out_png.parent / f"{out_png.name}.meta").write_text(strip_guid(read_template(PNG_META_TEMPLATE), g))
    return g


def import_material(asset: str, package: str, png_guid: str):
    out_dir = TEX_ROOT / package / "Materials"
    mat = out_dir / f"{asset}.mat"
    if mat.exists():
        return False
    text = read_template(MAT_TEMPLATE)
    text = re.sub(r"m_Name: .*", f"m_Name: {asset}", text, count=1)
    # Only repoint the texture reference. The shader and the material-version script keep the
    # guids they have in the template; rewriting every guid breaks both and the material renders
    # as the magenta default.
    text = text.replace(TEMPLATE_PNG_GUID, png_guid)
    out_dir.mkdir(parents=True, exist_ok=True)
    mat.write_text(text)
    (out_dir / f"{asset}.mat.meta").write_text(
        strip_guid(read_template(MAT_META_TEMPLATE), guid())
    )
    return True


def parse_armorgrp(path: Path):
    """asset -> (texture, package) for m_Elf / f_Elf rows."""
    mapping = {}
    data = path.read_text(encoding="utf-8", errors="replace")
    for line in data.splitlines():
        for key in ("m_Elf", "f_Elf"):
            for m in re.finditer(rf"\b{key}=\{{\{{\[([^\]]+)\]\}};\{{\[([^\]]+)\]\}}\}}", line):
                mesh, tex = m.group(1), m.group(2)
                if "." not in mesh:
                    continue
                package, asset = mesh.split(".", 1)
                tex_name = tex.split(".")[-1] if tex != "None" else None
                mapping.setdefault(asset, []).append((tex_name, package))
    return mapping


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--fbx-dir", required=True)
    ap.add_argument("--tex-root", required=True)
    ap.add_argument("--names", required=True)
    ap.add_argument("--armorgrp", required=True)
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument(
        "--overwrite-prefabs",
        action="store_true",
        help="Regenerate prefab wrappers even if they exist (used to repair files written by older runs).",
    )
    args = ap.parse_args()

    fbx_dir = Path(args.fbx_dir)
    tex_root = Path(args.tex_root)
    names = [n.strip() for n in Path(args.names).read_text().splitlines() if n.strip()]
    armorgrp = parse_armorgrp(Path(args.armorgrp))

    stats = {"fbx": 0, "prefab": 0, "png": 0, "mat": 0, "no_tex": 0, "no_armorgrp": []}

    for name in names:
        fbx_src = fbx_dir / f"{name}.fbx"
        if not fbx_src.is_file():
            continue
        # Mesh lives in Animations/{package}/{MeshPrefix}/Models; the texture package is the
        # mesh-name prefix (MElf / FElf) because that is what SysTextures/ holds.
        tex_package = "MElf" if name.startswith("MElf") else "FElf"
        tex_entries = armorgrp.get(name, [])
        if not tex_entries:
            stats["no_armorgrp"].append(name)
            package = "Elf"
            tex_names = []
        else:
            package = tex_entries[0][1]
            tex_names = sorted({t for t, _ in tex_entries if t})

        if args.dry_run:
            print(name, package, len(tex_names), "textures")
            continue

        fbx_dest = ANIM_ROOT / tex_package / "Models" / f"{name}.fbx"
        fbx_meta = fbx_dest.with_suffix(".fbx.meta")
        if not fbx_dest.exists():
            fbx_dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(fbx_src, fbx_dest)
            make_fbx_meta(fbx_meta)
            stats["fbx"] += 1
        fbx_guid = re.search(r"^guid: ([0-9a-f]{32})$", fbx_meta.read_text(), re.M).group(1)

        if make_prefab(name, fbx_guid, package, args.overwrite_prefabs):
            stats["prefab"] += 1

        for tex_name in tex_names:
            g = import_texture(tex_root, tex_name, tex_package)
            if g is None:
                stats["no_tex"] += 1
                continue
            stats["png"] += 1
            if import_material(tex_name, tex_package, g):
                stats["mat"] += 1

    print("stats:", stats)


if __name__ == "__main__":
    main()
