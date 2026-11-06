#!/usr/bin/env python3
"""Import umodel-exported L2 race armor (FBX + textures + materials + prefabs) into Unity.

Generalizes tools/interlude/import_elf_armor.py (MElf reference) for every Interlude race.

Usage:
  python3 tools/interlude/import_race_armor.py \
      --race MDarkElf \
      --fbx-dir .work/race-import/DarkElf/FBX \
      --tex-root .work/race-import/tex \
      --names .work/race-import/m_DarkElf_missing_meshes.txt \
      --armorgrp l2-unity/Assets/StreamingAssets/Data/Meta/Armorgrp_Classic.txt

Never overwrites existing assets unless --overwrite-prefabs.
"""
from __future__ import annotations

import argparse
import re
import shutil
import subprocess
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
ANIM_ROOT = REPO / "l2-unity/Assets/Resources/Data/Animations"
TEX_ROOT = REPO / "l2-unity/Assets/Resources/Data/SysTextures"

# mesh package folder under Animations/, texture folder under SysTextures/
RACE_META = {
    "MElf": ("Elf", "MElf"),
    "FElf": ("Elf", "FElf"),
    "MMagic": ("Magic", "MMagic"),
    "FMagic": ("Magic", "FMagic"),
    "MFighter": ("Fighter", "MFighter"),
    "FFighter": ("Fighter", "FFighter"),
    "MDarkElf": ("DarkElf", "MDarkElf"),
    "FDarkElf": ("DarkElf", "FDarkElf"),
    "MDwarf": ("Dwarf", "MDwarf"),
    "FDwarf": ("Dwarf", "FDwarf"),
    "MOrc": ("Orc", "MOrc"),
    "FOrc": ("Orc", "FOrc"),
    "MShaman": ("Shaman", "MShaman"),
    "FShaman": ("Shaman", "FShaman"),
}

# Classic armorgrp field -> race prefixes that appear in mesh asset names
CLASSIC_FIELDS = {
    "m_Elf": ("MElf",),
    "f_Elf": ("FElf",),
    "m_HumnMyst": ("MMagic",),
    "f_HumnMyst": ("FMagic",),
    "m_HumnFigh": ("MFighter",),
    "f_HumnFigh": ("FFighter",),
    "m_DarkElf": ("MDarkElf",),
    "f_DarkElf": ("FDarkElf",),
    "m_Dorf": ("MDwarf",),
    "f_Dorf": ("FDwarf",),
    "m_OrcFigh": ("MOrc",),
    "f_OrcFigh": ("FOrc",),
    "m_OrcMage": ("MShaman",),
    "f_OrcMage": ("FShaman",),
}

TEX_VARIANTS = ("", "_sp", "_ori", "_sp2", "_sp1", "_ori1")


def race_from_asset(asset: str) -> str | None:
    for race in sorted(RACE_META, key=len, reverse=True):
        if asset.startswith(race + "_") or asset.lower().startswith(race.lower() + "_"):
            return race
    # umodel sometimes lowercases the race prefix
    lower = asset.lower()
    for race in RACE_META:
        if lower.startswith(race.lower() + "_"):
            return race
    return None


def normalize_asset(asset: str, race: str) -> str:
    """Force Unity casing: MDarkElf_m002_u (not mdarkelf_...)."""
    if "_" not in asset:
        return asset
    _, rest = asset.split("_", 1)
    return f"{race}_{rest}"


def pick_templates(race: str) -> tuple[Path, Path, Path, Path]:
    mesh_pkg, tex_pkg = RACE_META[race]
    anim = ANIM_ROOT / mesh_pkg
    tex = TEX_ROOT / tex_pkg

    def first_existing(cands: list[Path]) -> Path:
        for p in cands:
            if p.exists():
                return p
        raise FileNotFoundError(f"no template for {race}: tried {cands}")

    fbx_metas = sorted((anim / tex_pkg / "Models").glob(f"{race}_*.fbx.meta")) if (anim / tex_pkg / "Models").exists() else []
    fbx_metas += sorted(anim.rglob(f"{race}_*.fbx.meta"))
    mats = sorted((tex / "Materials").glob(f"{race}_*.mat")) if (tex / "Materials").exists() else []
    png_metas = sorted(tex.glob(f"{race}_*.png.meta")) if tex.exists() else []
    prefs = sorted(anim.glob(f"{race}_*.prefab"))

    # Prefer m001 body piece templates when present (MElf reference style).
    def prefer(paths: list[Path], needle: str) -> list[Path]:
        hit = [p for p in paths if needle in p.name]
        return hit + [p for p in paths if p not in hit]

    fbx_metas = prefer(fbx_metas, "_m001_")
    mats = prefer(mats, "_m001_")
    png_metas = prefer(png_metas, "_m001_")
    prefs = prefer(prefs, "_m001_")

    # Fallback across sibling races in the same mesh package if this race has no templates yet.
    if not fbx_metas or not mats or not png_metas or not prefs:
        for other, (opkg, otex) in RACE_META.items():
            if opkg != mesh_pkg or other == race:
                continue
            oanim = ANIM_ROOT / opkg
            otexp = TEX_ROOT / otex
            if not fbx_metas:
                fbx_metas = sorted(oanim.rglob(f"{other}_*.fbx.meta"))
            if not mats:
                mats = sorted((otexp / "Materials").glob(f"{other}_*.mat")) if (otexp / "Materials").exists() else []
            if not png_metas:
                png_metas = sorted(otexp.glob(f"{other}_*.png.meta")) if otexp.exists() else []
            if not prefs:
                prefs = sorted(oanim.glob(f"{other}_*.prefab"))

    return (
        first_existing(fbx_metas),
        first_existing(mats),
        first_existing(mats).with_suffix(".mat.meta"),
        first_existing(png_metas),
    )


def guid() -> str:
    return uuid.uuid4().hex


def file_id() -> int:
    return uuid.uuid4().int % (2**62)


def strip_guid(text: str, new_guid: str) -> str:
    return re.sub(r"^guid: [0-9a-f]{32}$", f"guid: {new_guid}", text, count=1, flags=re.M)


def make_fbx_meta(dest_meta: Path, template: Path) -> None:
    if dest_meta.exists():
        return
    dest_meta.write_text(strip_guid(template.read_text(), guid()))


def make_prefab(asset: str, fbx_guid: str, mesh_pkg: str, overwrite: bool = False) -> bool:
    dest = ANIM_ROOT / mesh_pkg / f"{asset}.prefab"
    if dest.exists() and not overwrite:
        return False
    meta = ANIM_ROOT / mesh_pkg / f"{asset}.prefab.meta"
    g = guid()
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
    dest.parent.mkdir(parents=True, exist_ok=True)
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


def find_texture(tex_root: Path, name: str) -> Path | None:
    lower = name.lower()
    for variant in TEX_VARIANTS:
        for p in tex_root.rglob(f"{name}{variant}.tga"):
            return p
        for p in tex_root.rglob(f"{name}{variant}.png"):
            return p
    for p in tex_root.rglob("*"):
        if p.suffix.lower() in {".tga", ".png"} and p.stem.lower() == lower:
            return p
        for variant in TEX_VARIANTS[1:]:
            if p.suffix.lower() in {".tga", ".png"} and p.stem.lower() == lower + variant:
                return p
    return None


def import_texture(tex_root: Path, asset: str, tex_pkg: str, png_meta_template: Path) -> str | None:
    out_dir = TEX_ROOT / tex_pkg
    out_png = out_dir / f"{asset}.png"
    if out_png.exists():
        meta = out_png.with_suffix(".png.meta")
        if meta.exists():
            m = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(), re.M)
            if m:
                return m.group(1)
    src = find_texture(tex_root, asset)
    if src is None:
        # try lowercase asset (umodel dump)
        src = find_texture(tex_root, asset.lower())
    if src is None:
        return None
    out_dir.mkdir(parents=True, exist_ok=True)
    if src.suffix.lower() == ".png":
        shutil.copy2(src, out_png)
    else:
        subprocess.run(["magick", str(src), str(out_png)], check=True)
    g = guid()
    (out_png.parent / f"{out_png.name}.meta").write_text(strip_guid(png_meta_template.read_text(), g))
    return g


def import_material(asset: str, tex_pkg: str, png_guid: str, mat_template: Path, mat_meta_template: Path, template_png_guid: str) -> bool:
    out_dir = TEX_ROOT / tex_pkg / "Materials"
    mat = out_dir / f"{asset}.mat"
    if mat.exists():
        return False
    text = mat_template.read_text()
    text = re.sub(r"m_Name: .*", f"m_Name: {asset}", text, count=1)
    text = text.replace(template_png_guid, png_guid)
    out_dir.mkdir(parents=True, exist_ok=True)
    mat.write_text(text)
    (out_dir / f"{asset}.mat.meta").write_text(strip_guid(mat_meta_template.read_text(), guid()))
    return True


def parse_armorgrp(path: Path, race_filter: set[str] | None = None) -> dict[str, list[tuple[str | None, str]]]:
    """asset -> [(texture, mesh_package), ...] from Classic key=value armorgrp."""
    mapping: dict[str, list[tuple[str | None, str]]] = {}
    data = path.read_text(encoding="utf-8", errors="replace")
    for field, races in CLASSIC_FIELDS.items():
        if race_filter and not (set(races) & race_filter):
            continue
        for m in re.finditer(rf"\b{field}=\{{\{{\[([^\]]+)\]\}};\{{\[([^\]]+)\]\}}\}}", data):
            mesh, tex = m.group(1), m.group(2)
            if "." not in mesh:
                continue
            package, asset = mesh.split(".", 1)
            race = race_from_asset(asset)
            if race is None:
                continue
            if race_filter and race not in race_filter:
                continue
            asset = normalize_asset(asset, race)
            tex_name = tex.split(".")[-1] if tex != "None" else None
            if tex_name and tex_name != "None":
                tr = race_from_asset(tex_name)
                if tr:
                    tex_name = normalize_asset(tex_name, tr)
            mapping.setdefault(asset, []).append((tex_name, package))
    return mapping


def parse_armorgrp_interlude_tsv(path: Path, race_filter: set[str] | None = None) -> dict[str, list[tuple[str | None, str]]]:
    """Fallback: Interlude TSV armorgrp columns."""
    mapping: dict[str, list[tuple[str | None, str]]] = {}
    lines = path.read_text(encoding="utf-8-sig", errors="replace").splitlines()
    header = lines[0].split("\t")
    pairs = []
    for i, h in enumerate(header):
        m = re.match(r"(m_Elf|f_Elf|m_DarkElf|f_DarkElf|m_Dorf|f_Dorf|m_HumnFigh|f_HumnFigh|m_HumnMyst|f_HumnMyst|m_OrcFigh|f_OrcFigh|m_OrcMage|f_OrcMage)_m\[(\d)\]$", h)
        if not m:
            continue
        tcol = header.index(f"{m.group(1)}_t[{m.group(2)}]") if f"{m.group(1)}_t[{m.group(2)}]" in header else None
        pairs.append((i, tcol))
    for line in lines[1:]:
        cols = line.split("\t")
        for mi, ti in pairs:
            if mi >= len(cols) or not cols[mi] or "." not in cols[mi]:
                continue
            package, asset = cols[mi].split(".", 1)
            race = race_from_asset(asset)
            if race is None or (race_filter and race not in race_filter):
                continue
            asset = normalize_asset(asset, race)
            tex_name = None
            if ti is not None and ti < len(cols) and cols[ti] and "." in cols[ti]:
                tex_name = cols[ti].split(".")[-1]
                tr = race_from_asset(tex_name)
                if tr:
                    tex_name = normalize_asset(tex_name, tr)
            mapping.setdefault(asset, []).append((tex_name, package))
    return mapping


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--race", help="Limit to one race prefix, e.g. MDarkElf")
    ap.add_argument("--fbx-dir", required=True)
    ap.add_argument("--tex-root", required=True)
    ap.add_argument("--names", required=True, help="File with asset names (one per line), with or without package.")
    ap.add_argument("--armorgrp", required=True)
    ap.add_argument("--dry-run", action="store_true")
    ap.add_argument("--overwrite-prefabs", action="store_true")
    args = ap.parse_args()

    race_filter = {args.race} if args.race else None
    fbx_dir = Path(args.fbx_dir)
    tex_root = Path(args.tex_root)
    raw_names = [n.strip() for n in Path(args.names).read_text().splitlines() if n.strip()]
    names = []
    for n in raw_names:
        asset = n.split(".", 1)[-1]
        race = race_from_asset(asset)
        if race is None:
            continue
        if race_filter and race not in race_filter:
            continue
        names.append(normalize_asset(asset, race))

    agr_path = Path(args.armorgrp)
    if agr_path.name.lower().endswith("interlude.txt") or "\tm_Elf_m[0]\t" in agr_path.read_text(encoding="utf-8-sig", errors="replace")[:4000]:
        armorgrp = parse_armorgrp_interlude_tsv(agr_path, race_filter)
    else:
        armorgrp = parse_armorgrp(agr_path, race_filter)

    # templates cached per race
    templates: dict[str, tuple] = {}
    template_png_guids: dict[str, str] = {}

    stats = {"fbx": 0, "prefab": 0, "png": 0, "mat": 0, "no_tex": 0, "no_fbx": 0, "no_template": [], "no_armorgrp": []}

    for name in names:
        race = race_from_asset(name)
        if race is None or race not in RACE_META:
            continue
        mesh_pkg, tex_pkg = RACE_META[race]
        if race not in templates:
            try:
                fbx_meta_t, mat_t, mat_meta_t, png_meta_t = pick_templates(race)
            except FileNotFoundError as e:
                stats["no_template"].append(str(e))
                continue
            templates[race] = (fbx_meta_t, mat_t, mat_meta_t, png_meta_t)
            m = re.search(r"^guid: ([0-9a-f]{32})$", png_meta_t.read_text(), re.M)
            template_png_guids[race] = m.group(1)

        fbx_meta_t, mat_t, mat_meta_t, png_meta_t = templates[race]

        # source FBX may use any casing
        fbx_src = None
        for cand in (fbx_dir / f"{name}.fbx", fbx_dir / f"{name.lower()}.fbx"):
            if cand.is_file():
                fbx_src = cand
                break
        if fbx_src is None:
            # search recursively
            hits = list(fbx_dir.rglob(f"{name}.fbx")) + list(fbx_dir.rglob(f"{name.lower()}.fbx"))
            fbx_src = hits[0] if hits else None
        if fbx_src is None:
            stats["no_fbx"] += 1
            continue

        tex_entries = armorgrp.get(name, [])
        if not tex_entries:
            stats["no_armorgrp"].append(name)
            package = mesh_pkg
            tex_names: list[str] = []
        else:
            package = tex_entries[0][1] or mesh_pkg
            tex_names = sorted({t for t, _ in tex_entries if t})

        if args.dry_run:
            print(name, package, len(tex_names), "textures", fbx_src)
            continue

        fbx_dest = ANIM_ROOT / mesh_pkg / tex_pkg / "Models" / f"{name}.fbx"
        fbx_meta = fbx_dest.with_suffix(".fbx.meta")
        if not fbx_dest.exists():
            fbx_dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copy2(fbx_src, fbx_dest)
            make_fbx_meta(fbx_meta, fbx_meta_t)
            stats["fbx"] += 1
        if not fbx_meta.exists():
            make_fbx_meta(fbx_meta, fbx_meta_t)
        fbx_guid = re.search(r"^guid: ([0-9a-f]{32})$", fbx_meta.read_text(), re.M).group(1)

        if make_prefab(name, fbx_guid, mesh_pkg, args.overwrite_prefabs):
            stats["prefab"] += 1

        for tex_name in tex_names:
            g = import_texture(tex_root, tex_name, tex_pkg, png_meta_t)
            if g is None:
                stats["no_tex"] += 1
                continue
            stats["png"] += 1
            if import_material(tex_name, tex_pkg, g, mat_t, mat_meta_t, template_png_guids[race]):
                stats["mat"] += 1

    print("stats:", stats)


if __name__ == "__main__":
    main()
