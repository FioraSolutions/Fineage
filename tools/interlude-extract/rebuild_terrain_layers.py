#!/usr/bin/env python3
"""Rebuild TerrainInfo Layers in Meta/t3d from the official #1670 map dump.

Every official slot is written (including TextureRotation, TextureMapAxis,
bUseAlpha, UPan/VPan, layer Scale, LayerRotation). Empty Texture=none slots
are stored but the bake parser skips them. Does not invent Base when official
layer 0 is a real splat (e.g. INNS_05).
"""
from __future__ import annotations

import argparse
import gzip
import json
import re
import sys
import uuid
from pathlib import Path

REPO = Path(__file__).resolve().parents[2]
MAP_DUMPS = REPO / "l2-unity/docs/interlude-ref/world/maps"
MAPS = REPO / "l2-unity/Assets/Resources/Data/Maps"
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
LAYER_RE = re.compile(r"^(\s*)Layers\((\d+)\)=")
AXIS_NAME = {0: "TEXMAPAXIS_XY", 1: "TEXMAPAXIS_XZ", 2: "TEXMAPAXIS_YZ"}

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
    if path.suffix == ".gz" or not path.exists():
        gz = path if path.suffix == ".gz" else Path(str(path) + ".gz")
        with gzip.open(gz, "rt", encoding="utf-8") as f:
            return json.load(f)
    return json.loads(path.read_text(encoding="utf-8"))


def load_map(tile: str) -> dict:
    p = MAP_DUMPS / f"{tile}.unr.json"
    return load_json(p)


def find_ti(doc: dict) -> dict:
    for ex in doc.get("exports") or []:
        if ex.get("class") == "TerrainInfo":
            return ex
    raise SystemExit(f"ERROR no TerrainInfo in map dump")


def import_index(doc: dict) -> dict[str, dict]:
    out = {}
    for im in doc.get("imports") or []:
        out[im["object_name"]] = im
        out[(im.get("class_name"), im["object_name"])] = im
    return out


def package_of(im: dict | None) -> str | None:
    if not im:
        return None
    pkg = im.get("package") or ""
    if ":Package:" in pkg:
        return pkg.split(":Package:")[-1]
    if im.get("class_name") == "Package":
        return im.get("object_name")
    return None


def parse_layers(ti: dict) -> list[dict]:
    layers = []
    for p in ti.get("properties") or []:
        if p.get("name") != "Layers" or p.get("struct") != "TerrainLayer":
            continue
        idx = p.get("array_index") or 0
        fields: dict = {"index": idx, "_raw": p}
        to_world: dict[int, dict] = {}
        to_mask: dict[int, dict] = {}
        for sp_block in p.get("subproperties") or []:
            items = sp_block if isinstance(sp_block, list) else [sp_block]
            for sp in items:
                if not isinstance(sp, dict):
                    continue
                n = sp.get("name")
                if n in ("Texture", "AlphaMap", "LayerWeightMap") and sp.get("ref"):
                    fields[n] = sp["ref"]
                elif n in ("UScale", "VScale", "UPan", "VPan", "TextureRotation", "bUseAlpha", "TextureMapAxis", "KFriction", "KRestitution") and "value" in sp:
                    fields[n] = sp["value"]
                elif n == "LayerRotation" and sp.get("rotator"):
                    fields[n] = sp["rotator"]
                elif n == "Scale" and sp.get("vector"):
                    fields["LayerScale"] = sp["vector"]
                elif n == "ToWorld" and sp.get("vector") is not None:
                    to_world[sp.get("array_index") or 0] = sp["vector"]
                elif n == "ToMaskmap" and sp.get("vector") is not None:
                    to_mask[sp.get("array_index") or 0] = sp["vector"]
                elif n == "TerrainMatrix":
                    fields["TerrainMatrix"] = sp
        if to_world:
            fields["ToWorld"] = to_world
        if to_mask:
            fields["ToMaskmap"] = to_mask
        layers.append(fields)
    layers.sort(key=lambda r: r["index"])
    return layers


def short_name(ref: str | None) -> str | None:
    if not ref or ref == "none":
        return None
    return ref.split(":")[-1]


def fmt_float(v, digits=6) -> str:
    if v is None:
        return "0.000000"
    return f"{float(v):.{digits}f}"


def fmt_vec(v: dict | None) -> str:
    if not v:
        return "(X=0.000000,Y=0.000000,Z=0.000000)"
    return f"(X={fmt_float(v.get('x'))},Y={fmt_float(v.get('y'))},Z={fmt_float(v.get('z'))})"


def texture_ref(pkg: str, name: str, is_texture_pkg: bool, is_tile_alpha: bool, tile_pkg: str) -> str:
    if is_texture_pkg:
        return f"Texture'T_texture.Texture.{name}'"
    if is_tile_alpha:
        return f"Texture'{tile_pkg}.Height.{name}'"
    return f"Texture'{pkg}.{name}'"


def resolve_pkg(imps: dict, ref: str | None, tile_pkg: str) -> tuple[str | None, str | None, bool, bool]:
    """Returns official_pkg, name, is_texture_pkg, is_tile_alpha."""
    name = short_name(ref)
    if not name:
        return None, None, False, False
    im = imps.get(("Texture", name)) or imps.get(name)
    pkg = package_of(im) or tile_pkg
    # Dump often names the group "Height" (T_XX_YY.Height.22_22_S2) rather than the .utx.
    if pkg and pkg.lower() == "height":
        return tile_pkg, name, False, True
    is_tex = pkg.lower() in {"texture", "t_texture"}
    is_tile = pkg.lower() in {tile_pkg.lower(), tile_pkg.lower().removesuffix("_classic"), f"t_{tile_pkg.lower()}"}
    return pkg, name, is_tex, is_tile and name != "layer0" and not is_tex


def format_layer(ly: dict, imps: dict, tile_pkg: str) -> str:
    idx = ly["index"]
    tex_pkg, tex_name, tex_is_tex, _ = resolve_pkg(imps, ly.get("Texture"), tile_pkg)
    alp_pkg, alp_name, alp_is_tex, alp_is_tile = resolve_pkg(imps, ly.get("AlphaMap"), tile_pkg)
    parts = []
    if tex_name:
        parts.append(f"Texture={texture_ref(tex_pkg, tex_name, tex_is_tex, False, tile_pkg)}")
    else:
        parts.append("Texture=None")
    if alp_name:
        # layer0 always shared
        if alp_name == "layer0":
            parts.append("AlphaMap=Texture'T_texture.Texture.layer0'")
        else:
            parts.append(f"AlphaMap={texture_ref(alp_pkg, alp_name, alp_is_tex, alp_is_tile, tile_pkg)}")
    us, vs = ly.get("UScale", 0), ly.get("VScale", 0)
    parts.append(f"UScale={fmt_float(us)}")
    parts.append(f"VScale={fmt_float(vs)}")
    if ly.get("UPan") not in (None, 0, 0.0):
        parts.append(f"UPan={fmt_float(ly['UPan'])}")
    if ly.get("VPan") not in (None, 0, 0.0):
        parts.append(f"VPan={fmt_float(ly['VPan'])}")
    trot = ly.get("TextureRotation")
    if trot is not None:
        parts.append(f"TextureRotation={fmt_float(trot)}")
    axis = ly.get("TextureMapAxis")
    if axis is not None:
        try:
            ai = int(axis)
        except (TypeError, ValueError):
            ai = axis
        if isinstance(ai, int) and ai in AXIS_NAME:
            parts.append(f"TextureMapAxis={AXIS_NAME[ai]}")
        else:
            parts.append(f"TextureMapAxis={axis}")
    if "bUseAlpha" in ly:
        parts.append(f"bUseAlpha={'True' if ly['bUseAlpha'] else 'False'}")
    if ly.get("LayerScale"):
        s = ly["LayerScale"]
        parts.append(f"Scale={fmt_vec(s)}")
    rot = ly.get("LayerRotation")
    if rot and any(float(rot.get(k, 0) or 0) for k in ("pitch", "yaw", "roll")):
        parts.append(
            "LayerRotation=(Pitch={0},Yaw={1},Roll={2})".format(
                fmt_float(rot.get("pitch")), fmt_float(rot.get("yaw")), fmt_float(rot.get("roll"))
            )
        )
    # ToWorld / ToMaskmap (official placement matrices)
    if ly.get("ToWorld"):
        for i in sorted(ly["ToWorld"]):
            parts.append(f"ToWorld({i})={fmt_vec(ly['ToWorld'][i])}")
    if ly.get("ToMaskmap"):
        for i in sorted(ly["ToMaskmap"]):
            parts.append(f"ToMaskmap({i})={fmt_vec(ly['ToMaskmap'][i])}")
    if ly.get("KFriction") is not None:
        parts.append(f"KFriction={fmt_float(ly['KFriction'])}")
    if ly.get("KRestitution") is not None:
        parts.append(f"KRestitution={fmt_float(ly['KRestitution'])}")
    return f"Layers({idx})=(" + ",".join(parts) + ")"


def tile_texture_package(info0: str) -> str:
    m = re.search(r"TerrainMap=Texture'([^']+)'", info0)
    if not m:
        return ""
    # T_22_22.Height.22_22 or T_16_25_Classic.Height.16_25
    return m.group(1).split(".")[0]


def replace_layer_lines(text: str, new_lines: list[str], indent: str) -> str:
    lines = text.splitlines()
    idxs = [i for i, ln in enumerate(lines) if LAYER_RE.match(ln)]
    body = [indent + ln if indent and not ln.startswith(indent) else ln for ln in new_lines]
    if idxs:
        first, last = idxs[0], idxs[-1]
        lines = lines[:first] + body + lines[last + 1 :]
    else:
        insert = next((i for i, ln in enumerate(lines) if "DecoLayers(" in ln or "QuadVisibilityBitmap(" in ln or "GeneratedSectorCounter=" in ln), len(lines))
        lines = lines[:insert] + body + lines[insert:]
    nl = "\n" if not text.endswith("\r\n") else "\r\n"
    return nl.join(lines) + (nl if text.endswith(("\n", "\r\n")) else "")


def patch_info0(path: Path, new_lines: list[str]) -> bool:
    raw = path.read_text(encoding="utf-8")
    new = replace_layer_lines(raw, new_lines, "")
    if new != raw:
        path.write_text(new, encoding="utf-8")
        return True
    return False


def patch_t3d(path: Path, new_lines: list[str]) -> bool:
    raw = path.read_text(encoding="utf-8")
    lines = raw.splitlines()
    start = end = None
    for i, ln in enumerate(lines):
        if ln.strip().startswith("Begin Actor Class=TerrainInfo Name=TerrainInfo0"):
            start = i
            for j in range(i + 1, len(lines)):
                if lines[j].strip().startswith("End Actor"):
                    end = j
                    break
            break
    if start is None or end is None:
        return False
    actor = "\n".join(lines[start + 1 : end])
    patched = replace_layer_lines(actor + "\n", new_lines, "    ")
    actor_lines = patched.splitlines()
    new_lines_all = lines[: start + 1] + actor_lines + lines[end:]
    nl = "\n" if not raw.endswith("\r\n") else "\r\n"
    new = nl.join(new_lines_all) + (nl if raw.endswith(("\n", "\r\n")) else "")
    if new != raw:
        path.write_text(new, encoding="utf-8")
        return True
    return False


def new_guid() -> str:
    return uuid.uuid4().hex


def write_folder_meta(folder: Path, template: Path | None = None) -> None:
    meta = folder.with_suffix(folder.suffix + ".meta") if folder.suffix else Path(str(folder) + ".meta")
    # folder.meta lives beside the folder
    meta = Path(str(folder) + ".meta")
    if meta.exists():
        return
    meta.write_text(FOLDER_META.format(guid=new_guid()), encoding="utf-8")


def write_png_meta(png: Path, template: Path) -> None:
    meta = Path(str(png) + ".meta")
    if meta.exists():
        return
    text = template.read_text(encoding="utf-8")
    text = re.sub(r"^guid: .+$", f"guid: {new_guid()}", text, count=1, flags=re.M)
    meta.write_text(text, encoding="utf-8")


def ensure_png(folder: str, stem: str, template_png_meta: Path) -> bool:
    """If official folder lacks stem.png, copy the first existing PNG of that name."""
    dest_dir = TEX / folder
    dest_dir.mkdir(parents=True, exist_ok=True)
    write_folder_meta(dest_dir)
    dest = dest_dir / (stem + ".png")
    if dest.exists():
        write_png_meta(dest, template_png_meta)
        return True
    for cand in TEX.glob("*/*.png"):
        if cand.stem.lower() == stem.lower():
            dest.write_bytes(cand.read_bytes())
            write_png_meta(dest, template_png_meta)
            print(f"  copy {cand.relative_to(TEX)} -> {dest.relative_to(TEX)}")
            return True
    return False


def copy_exports(export_root: Path, template_png_meta: Path) -> int:
    n = 0
    if not export_root.is_dir():
        return 0
    for pkg_dir in sorted(export_root.iterdir()):
        if not pkg_dir.is_dir():
            continue
        dest = TEX / pkg_dir.name
        dest.mkdir(parents=True, exist_ok=True)
        write_folder_meta(dest)
        for png in pkg_dir.glob("*.png"):
            target = dest / png.name
            if not target.exists() or target.read_bytes() != png.read_bytes():
                target.write_bytes(png.read_bytes())
                n += 1
            write_png_meta(target, template_png_meta)
    return n


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("--export-dir", type=Path, default=REPO / ".work/tex-export")
    args = ap.parse_args()
    template = TEX / "T_22_22/g_01.png.meta"
    copied = copy_exports(args.export_dir, template)
    print(f"copied/updated {copied} PNGs from {args.export_dir}")

    tiles = sorted(d.name for d in MAPS.iterdir() if (d / "Meta" / "TerrainInfo0.txt").exists())
    failures = 0
    for tile in tiles:
        info0 = MAPS / tile / "Meta" / "TerrainInfo0.txt"
        text = info0.read_text(encoding="utf-8")
        tile_pkg = tile_texture_package(text)
        if not tile_pkg:
            print(f"ERROR {tile}: no TerrainMap in TerrainInfo0", file=sys.stderr)
            failures += 1
            continue
        try:
            doc = load_map(tile)
            ti = find_ti(doc)
        except Exception as e:
            print(f"ERROR {tile}: {e}", file=sys.stderr)
            failures += 1
            continue
        imps = import_index(doc)
        layers = parse_layers(ti)
        new_lines = [format_layer(ly, imps, tile_pkg) for ly in layers]
        used = sum(1 for ly in layers if short_name(ly.get("Texture")))
        # Place official-package copies of already-imported PNGs (e.g. T_22_22/GI_G -> T_Giran/GI_G).
        for ly in layers:
            for key, is_alpha in (("Texture", False), ("AlphaMap", True)):
                pkg, name, is_tex, is_tile = resolve_pkg(imps, ly.get(key), tile_pkg)
                if not name or name == "layer0":
                    continue
                folder = "T_texture" if is_tex else (tile_pkg if is_tile else pkg)
                if not ensure_png(folder, name, template):
                    print(f"  MISSING png {folder}/{name}.png for {tile} {key}", file=sys.stderr)
        changed = patch_info0(info0, new_lines)
        t3d = MAPS / tile / "Meta" / f"{tile}.t3d"
        tmsg = ""
        if t3d.exists():
            tchanged = patch_t3d(t3d, new_lines)
            tmsg = f" t3d={'updated' if tchanged else 'ok'}"
        print(f"{tile}: official {len(layers)} slots ({used} used) pkg={tile_pkg} info0={'updated' if changed else 'ok'}{tmsg}")
    if failures:
        raise SystemExit(failures)


if __name__ == "__main__":
    main()
