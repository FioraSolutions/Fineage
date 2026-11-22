#!/usr/bin/env python3
"""Rebuild TerrainInfo DecoLayers in Meta/t3d from the official #1670 map dump.

Every official slot is written (StaticMesh, DensityMap, ColorMap, ScaleMap,
ShowOnTerrain, FadeoutRadius, DensityMultiplier, MaxPerQuad, Seed,
AlignToTerrain, RandomYaw, lighting flags, DrawOrder, ScaleMultiplier).
Does not invent layers when official array_size is 0.
"""
from __future__ import annotations

import argparse
import gzip
import json
import re
import shutil
import sys
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MAP_DUMPS = REPO / "l2-unity/docs/interlude-ref/world/maps"
MAPS = REPO / "l2-unity/Assets/Resources/Data/Maps"
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
DECO_RE = re.compile(r"^(\s*)DecoLayers\((\d+)\)=.*$", re.M)
DRAW = {0: "SORT_NoSort", 1: "SORT_BackToFront"}

PNG_META = """fileFormatVersion: 2
guid: {guid}
TextureImporter:
  internalIDToNameTable: []
  externalObjects: {{}}
  serializedVersion: 13
  mipmaps:
    mipMapMode: 0
    enableMipMap: 1
    sRGBTexture: 0
    linearTexture: 1
    fadeOut: 0
    borderMipMap: 0
    mipMapsPreserveCoverage: 0
    alphaTestReferenceValue: 0.5
    mipMapFadeDistanceStart: 1
    mipMapFadeDistanceEnd: 3
  bumpmap:
    convertToNormalMap: 0
    externalNormalMap: 0
    heightScale: 0.25
    normalMapFilter: 0
    flipGreenChannel: 0
  isReadable: 1
  streamingMipmaps: 0
  streamingMipmapsPriority: 0
  vTOnly: 0
  ignoreMipmapLimit: 0
  grayScaleToAlpha: 0
  generateCubemap: 6
  cubemapConvolution: 0
  seamlessCubemap: 0
  textureFormat: 1
  maxTextureSize: 2048
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    mipBias: 0
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  lightmap: 0
  compressionQuality: 50
  spriteMode: 0
  spriteExtrude: 1
  spriteMeshType: 1
  alignment: 0
  spritePivot: {{x: 0.5, y: 0.5}}
  spritePixelsToUnits: 100
  spriteBorder: {{x: 0, y: 0, z: 0, w: 0}}
  spriteGenerateFallbackPhysicsShape: 1
  alphaUsage: 1
  alphaIsTransparency: 0
  spriteTessellationDetail: -1
  textureType: 0
  textureShape: 1
  singleChannelComponent: 0
  flipbookRows: 1
  flipbookColumns: 1
  maxTextureSizeSet: 0
  compressionQualitySet: 0
  textureFormatSet: 0
  ignorePngGamma: 0
  applyGammaDecoding: 0
  swizzle: 50462976
  cookieLightType: 0
  platformSettings: []
  spriteSheet:
    serializedVersion: 2
    sprites: []
    outline: []
    physicsShape: []
    bones: []
    spriteID: 
    internalID: 0
    vertices: []
    indices: 
    edges: []
    weights: []
    secondaryTextures: []
    nameFileIdTable: {{}}
  mipmapLimitGroupName: 
  pSDRemoveMatte: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""

FOLDER_META = """fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def load_json(path: Path):
    if path.suffix == ".gz" or (not path.exists() and Path(str(path) + ".gz").exists()):
        gz = path if path.suffix == ".gz" else Path(str(path) + ".gz")
        with gzip.open(gz, "rt", encoding="utf-8") as f:
            return json.load(f)
    return json.loads(path.read_text(encoding="utf-8"))


def load_map(tile: str) -> dict:
    return load_json(MAP_DUMPS / f"{tile}.unr.json")


def import_index(doc: dict) -> dict:
    """Index the dump tables for UE2 object-ref resolution.

    "by_index": raw signed UE2 ref -> table entry (negative: import table,
    positive: export table, both 1-based shifted). The Outer chain
    (package_index on imports, outer_index on exports) is walked through
    this map — never through names: Lobby alone imports two Packages named
    "tree" (RionSctgart_tree_s vs speaking_tree_s) and two named "Town",
    and duplicate StaticMesh/Texture/Sound names are common across the tile
    dumps, so any name-keyed walk silently picks the last duplicate.
    "by_ref": canonical dump ref string ("import:<class_pkg>:<class>:<name>")
    -> every matching entry; ambiguous string resolution is a hard error.
    """
    by_index: dict[int, dict] = {}
    for im in doc.get("imports") or []:
        by_index[-(im["import_index"] + 1)] = im
    for ex in doc.get("exports") or []:
        by_index[ex["export_index"] + 1] = ex
    by_ref: dict[str, list[dict]] = {}
    for im in doc.get("imports") or []:
        key = f"import:{im.get('class_package')}:{im.get('class_name')}:{im['object_name']}"
        by_ref.setdefault(key, []).append(im)
    return {"by_index": by_index, "by_ref": by_ref}


def _entry_name(entry: dict) -> str:
    return entry.get("object_name") or entry.get("name") or "?"


def _full_path(imps: dict, entry: dict) -> str:
    parts = [_entry_name(entry)]
    cur = entry
    for _ in range(16):
        outer = cur.get("package_index", cur.get("outer_index")) or 0
        if outer == 0:
            break
        parent = imps["by_index"].get(outer)
        if parent is None:
            raise SystemExit(
                f"ERROR Outer ref {outer} of {_entry_name(cur)} is outside the "
                "import/export tables — corrupt dump"
            )
        parts.insert(0, _entry_name(parent))
        cur = parent
    return ".".join(parts)


def _resolve(imps: dict, ref_or_field, expect_cls: str) -> dict | None:
    if ref_or_field is None or ref_or_field == "none":
        return None
    if isinstance(ref_or_field, dict):
        idx = ref_or_field.get("index") or 0
        if idx == 0:
            ref = ref_or_field.get("ref")
            if ref and ref != "none":
                raise SystemExit(f"ERROR property ref {ref} carries no usable index — dump shape regression")
            return None
        entry = imps["by_index"].get(idx)
        if entry is None:
            raise SystemExit(f"ERROR ref index {idx} ({ref_or_field.get('ref')}) outside the import/export tables")
    else:
        ref = ref_or_field
        if not isinstance(ref, str):
            raise SystemExit(f"ERROR unresolvable ref {ref!r} (expected {expect_cls})")
        if ref.startswith("export:"):
            # export:<pkg>:<0-based export_index>:<class>:<name> — the index is exact.
            try:
                idx = int(ref.split(":")[2]) + 1
            except (ValueError, IndexError):
                raise SystemExit(f"ERROR malformed export ref {ref}")
            entry = imps["by_index"].get(idx)
            if entry is None:
                raise SystemExit(f"ERROR export ref {ref} outside the export table")
        else:
            candidates = imps["by_ref"].get(ref) or []
            if not candidates:
                raise SystemExit(f"ERROR no import matches ref {ref} (expected {expect_cls})")
            if len(candidates) > 1:
                paths = ", ".join(_full_path(imps, c) for c in candidates)
                raise SystemExit(
                    f"ERROR ambiguous ref {ref}: {paths} — pass the property "
                    "field (with its numeric index) instead of the ref string"
                )
            entry = candidates[0]
    cls = entry.get("class_name") or entry.get("class")
    if expect_cls and cls != expect_cls:
        raise SystemExit(f"ERROR ref {ref_or_field} resolves to class {cls}, expected {expect_cls}")
    return entry


def full_name(imps: dict, ref_or_field, expect_cls: str) -> str | None:
    """Full Package[.Group].Object name for a dump Object-property.

    Pass the property field dict ({"ref","index",...}) for exact resolution
    through the raw UE2 ref index; a bare ref string only resolves when the
    (class_package, class, name) tuple is unique in the import table.
    """
    entry = _resolve(imps, ref_or_field, expect_cls)
    if entry is None:
        return None
    return _full_path(imps, entry)


def field(layer: list, name: str) -> dict | None:
    for f in layer:
        if f.get("name") == name:
            return f
    return None


def int_field(layer, name, default=0) -> int:
    f = field(layer, name)
    if not f:
        return default
    return int(f.get("value", default))


def range_minmax(layer, name) -> tuple[float, float]:
    f = field(layer, name)
    mn, mx = 0.0, 0.0
    if not f:
        return mn, mx
    for block in f.get("subproperties") or []:
        items = block if isinstance(block, list) else [block]
        for sp in items:
            if not isinstance(sp, dict):
                continue
            if sp.get("name") == "Min":
                mn = float(sp.get("value") or 0)
            elif sp.get("name") == "Max":
                mx = float(sp.get("value") or 0)
    return mn, mx


def scale_ranges(layer) -> dict:
    # X/Y/Z each Min/Max
    out = {a: (1.0, 1.0) for a in "XYZ"}
    f = field(layer, "ScaleMultiplier")
    if not f:
        return out
    for block in f.get("subproperties") or []:
        items = block if isinstance(block, list) else [block]
        for axis in items:
            if not isinstance(axis, dict):
                continue
            a = axis.get("name")
            if a not in out:
                continue
            mn, mx = 1.0, 1.0
            for inner in axis.get("subproperties") or []:
                inner_items = inner if isinstance(inner, list) else [inner]
                for sp in inner_items:
                    if not isinstance(sp, dict):
                        continue
                    if sp.get("name") == "Min":
                        mn = float(sp.get("value") or 1)
                    elif sp.get("name") == "Max":
                        mx = float(sp.get("value") or 1)
            out[a] = (mn, mx)
    return out


def fmt_f(v: float) -> str:
    return f"{float(v):.6f}"


def tex_ref(full: str | None) -> str:
    return "None" if not full else f"Texture'{full}'"


def mesh_ref(full: str | None) -> str:
    return "None" if not full else f"StaticMesh'{full}'"


def format_layer(i: int, layer: list, imps: dict) -> str:
    sm = full_name(imps, field(layer, "StaticMesh"), "StaticMesh")
    dm = full_name(imps, field(layer, "DensityMap"), "Texture")
    cm = full_name(imps, field(layer, "ColorMap"), "Texture")
    scm = full_name(imps, field(layer, "ScaleMap"), "Texture")
    sc = scale_ranges(layer)
    fade = range_minmax(layer, "FadeoutRadius")
    dens = range_minmax(layer, "DensityMultiplier")
    draw = DRAW.get(int_field(layer, "DrawOrder", 1), "SORT_BackToFront")
    parts = [
        f"ShowOnTerrain={int_field(layer, 'ShowOnTerrain', 1)}",
        f"ScaleMap={tex_ref(scm)}",
        f"DensityMap={tex_ref(dm)}",
        f"ColorMap={tex_ref(cm)}",
        f"StaticMesh={mesh_ref(sm)}",
        "ScaleMultiplier=(X=(Min={0},Max={1}),Y=(Min={2},Max={3}),Z=(Min={4},Max={5}))".format(
            fmt_f(sc["X"][0]), fmt_f(sc["X"][1]),
            fmt_f(sc["Y"][0]), fmt_f(sc["Y"][1]),
            fmt_f(sc["Z"][0]), fmt_f(sc["Z"][1]),
        ),
        f"FadeoutRadius=(Min={fmt_f(fade[0])},Max={fmt_f(fade[1])})",
        f"DensityMultiplier=(Min={fmt_f(dens[0])},Max={fmt_f(dens[1])})",
        f"MaxPerQuad={int_field(layer, 'MaxPerQuad', 0)}",
        f"Seed={int_field(layer, 'Seed', 0)}",
        f"AlignToTerrain={int_field(layer, 'AlignToTerrain', 0)}",
        f"DrawOrder={draw}",
        f"ShowOnInvisibleTerrain={int_field(layer, 'ShowOnInvisibleTerrain', 0)}",
        f"LitDirectional={int_field(layer, 'LitDirectional', 0)}",
        f"DisregardTerrainLighting={int_field(layer, 'DisregardTerrainLighting', 0)}",
        f"RandomYaw={int_field(layer, 'RandomYaw', 1)}",
        f"bForceRender={int_field(layer, 'bForceRender', 0)}",
        'DensityMeshName=""',
    ]
    return f"DecoLayers({i})=({','.join(parts)})"


def replace_deco_blocks(text: str, lines: list[str]) -> str:
    matches = list(DECO_RE.finditer(text))
    if matches:
        start = matches[0].start()
        end = matches[-1].end()
        indent = matches[0].group(1)
        block = "\n".join(indent + ln if not ln.startswith(indent) else ln for ln in lines)
        return text[:start] + block + text[end:]
    # insert before QuadVisibilityBitmap or at end of TerrainInfo
    q = re.search(r"^(\s*)QuadVisibilityBitmap\(", text, re.M)
    if q:
        indent = q.group(1)
        block = "\n".join(indent + ln for ln in lines) + "\n"
        return text[: q.start()] + block + text[q.start():]
    if lines:
        return text.rstrip() + "\n" + "\n".join(lines) + "\n"
    return text


def write_png_meta(path: Path) -> None:
    meta = path.with_suffix(".png.meta")
    if not meta.exists():
        meta.write_text(PNG_META.format(guid=uuid.uuid4().hex))


def ensure_folder_meta(folder: Path) -> None:
    meta = folder.with_suffix(folder.suffix + ".meta") if folder.suffix else Path(str(folder) + ".meta")
    # Unity folder meta is FolderName.meta next to the folder
    meta = folder.parent / (folder.name + ".meta")
    if not meta.exists():
        meta.write_text(FOLDER_META.format(guid=uuid.uuid4().hex))


def copy_density(full: str, export_dir: Path | None) -> bool:
    """Place PNG at Textures/<rootPkg>/<name>.png (GetFolderAndFileFromInfo skips the middle group)."""
    if not full:
        return False
    parts = full.split(".")
    folder, name = parts[0], parts[-1]
    dest_dir = TEX / folder
    dest_dir.mkdir(parents=True, exist_ok=True)
    ensure_folder_meta(dest_dir)
    dest = dest_dir / f"{name}.png"
    if dest.exists():
        return True
    # already-imported elsewhere
    for p in TEX.rglob(f"{name}.png"):
        shutil.copy2(p, dest)
        write_png_meta(dest)
        return True
    if export_dir:
        for p in export_dir.rglob("*.png"):
            if p.stem.lower() == name.lower():
                shutil.copy2(p, dest)
                write_png_meta(dest)
                return True
    return False


def tile_ids() -> list[str]:
    return sorted(
        p.name
        for p in MAPS.iterdir()
        if p.is_dir() and "_" in p.name and p.name[0].isdigit()
    )


def main() -> int:
    ap = argparse.ArgumentParser()
    ap.add_argument("--export-dir", type=Path, default=None, help="Directory of exported official density-map PNGs")
    args = ap.parse_args()
    missing_png: list[str] = []
    n_tiles = 0
    n_layers = 0
    for tile in tile_ids():
        try:
            doc = load_map(tile)
        except FileNotFoundError:
            print(f"ERROR {tile}: official #1670 dump not found")
            return 1
        ti = next((e for e in doc.get("exports") or [] if e.get("class") == "TerrainInfo"), None)
        if not ti:
            print(f"ERROR {tile}: no TerrainInfo in official dump")
            return 1
        imps = import_index(doc)
        deco = next((p for p in (ti.get("properties") or []) if p.get("name") == "DecoLayers"), None)
        layers = list(deco.get("subproperties") or []) if deco else []
        formatted = [format_layer(i, ly, imps) for i, ly in enumerate(layers)]
        # density pngs
        for i, ly in enumerate(layers):
            dm = full_name(imps, (field(ly, "DensityMap") or {}).get("ref"), "Texture")
            if dm and not copy_density(dm, args.export_dir):
                missing_png.append(f"{tile} layer {i} {dm}")
        meta = MAPS / tile / "Meta" / "TerrainInfo0.txt"
        if not meta.exists():
            print(f"ERROR {tile}: TerrainInfo0.txt missing")
            return 1
        meta.write_text(replace_deco_blocks(meta.read_text(errors="replace"), formatted))
        t3d = MAPS / tile / "Meta" / f"{tile}.t3d"
        if t3d.exists() and formatted:
            txt = t3d.read_text(errors="replace")
            # only rewrite if a TerrainInfo actor is present
            if "Class=TerrainInfo" in txt or "DecoLayers(" in txt or "QuadVisibilityBitmap(" in txt:
                t3d.write_text(replace_deco_blocks(txt, formatted))
        n_tiles += 1
        n_layers += len(formatted)
        print(f"{tile}: {len(formatted)} DecoLayers")
    print(f"\nrebuilt {n_layers} layers on {n_tiles} tiles")
    if missing_png:
        print("ERROR missing density-map PNGs:")
        for m in missing_png:
            print(" ", m)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
