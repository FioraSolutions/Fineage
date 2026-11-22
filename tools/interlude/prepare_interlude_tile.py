#!/usr/bin/env python3
"""Phase A helper: L2TerrainExtractor output -> Fineage Maps/Meta + texture bins (#476)."""

from __future__ import annotations

import argparse
import json
import re
import shutil
import struct
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parents[2]
UNITY = REPO_ROOT / "l2-unity"
MAPS = UNITY / "Assets/Resources/Data/Maps"
TEXTURES = UNITY / "Assets/Resources/Data/Textures"


def unreal_rotator(deg: float) -> int:
    return int(round(deg * 65536.0 / 360.0))


def write_height_bmp(raw_path: Path, bmp_path: Path) -> None:
    raw = raw_path.read_bytes()
    side = int((len(raw) // 2) ** 0.5)
    if side * side * 2 != len(raw):
        raise ValueError(f"unexpected height raw size {len(raw)}")
    # BMP: 54-byte header + 16-bit grayscale rows (bottom-up)
    row_bytes = side * 2
    padded = (row_bytes + 3) & ~3
    pixel_offset = 54
    file_size = pixel_offset + padded * side
    header = bytearray(54)
    header[0:2] = b"BM"
    struct.pack_into("<I", header, 2, file_size)
    struct.pack_into("<I", header, 10, pixel_offset)
    struct.pack_into("<I", header, 14, 40)
    struct.pack_into("<i", header, 18, side)
    struct.pack_into("<i", header, 22, side)
    struct.pack_into("<H", header, 26, 1)
    struct.pack_into("<H", header, 28, 16)
    out = bytearray(header)
    pad = padded - row_bytes
    for row in range(side - 1, -1, -1):
        start = row * row_bytes
        out.extend(raw[start : start + row_bytes])
        if pad:
            out.extend(b"\x00" * pad)
    bmp_path.parent.mkdir(parents=True, exist_ok=True)
    bmp_path.write_bytes(out)


def find_ground_texture(extract_dir: Path, ground: str) -> Path | None:
    d = extract_dir / "terraintextures"
    if not d.is_dir():
        return None
    want = ground.lower() + ".png"
    for f in d.iterdir():
        if f.name.lower() == want:
            return f
    return None


def parse_metadata(meta_path: Path) -> dict:
    splats: list[tuple[int, str, str]] = []
    loc = (0.0, 0.0, 0.0)
    for line in meta_path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if line.startswith("location_x="):
            loc = (float(line.split("=", 1)[1]), loc[1], loc[2])
        elif line.startswith("location_y="):
            loc = (loc[0], float(line.split("=", 1)[1]), loc[2])
        elif line.startswith("location_z="):
            loc = (loc[0], loc[1], float(line.split("=", 1)[1]))
        m = re.match(r"splatmap_(\d+)=[^,]+,(\S+)", line)
        if m:
            ground_ref = m.group(2)
            # Metadata may use Package.TextureName from TerrainInfo refs.
            ground_name = ground_ref.split(".", 1)[-1] if "." in ground_ref else ground_ref
            splats.append((int(m.group(1)), ground_name, ""))
    splats.sort(key=lambda x: x[0])
    return {"location": loc, "splats": splats}



def _quad_visibility_lines(meta: dict) -> list[str]:
    words = meta.get("quad_words")
    if not words:
        words = [-1] * 2048
    if len(words) != 2048:
        raise SystemExit(
            f"ERROR QuadVisibilityBitmap must have 2048 words (256x256 bits), got {len(words)}"
        )
    return [f"QuadVisibilityBitmap({i})={int(w)}" for i, w in enumerate(words)]


def load_quad_visibility(path: Path) -> list[int] | None:
    """Official 2048 signed words, one per line, from DumpTerrainHoles / a Phase A sidecar."""
    if not path.is_file():
        return None
    words = []
    for line in path.read_text(encoding="utf-8").splitlines():
        s = line.split("#", 1)[0].strip()
        if not s:
            continue
        words.append(int(s))
    return words

def build_terrain_info0(tile_id: str, meta: dict, tile_tex_pkg: str) -> str:
    # Layer 0 uses shared base; splat layers use T_XX_YY ground + Height alphas.
    lines = [
        f"TerrainMap=Texture'{tile_tex_pkg}.Height.{tile_id}'",
        "TerrainScale=(X=128.000000,Y=128.000000,Z=76.000000)",
    ]
    layer0 = (
        "Layers(0)=(Texture=Texture'T_texture.Texture.Base',"
        "AlphaMap=Texture'T_texture.Texture.layer0',"
        "UScale=1.000000,VScale=1.000000,"
        "TerrainMatrix=(XPlane=(X=0.007813),YPlane=(Y=0.007813),ZPlane=(Z=3.368421),"
        "WPlane=(W=1.000000,X=0.062500,Y=-0.093750,Z=365031.500000)),"
        "KFriction=196608.000000,KRestitution=-9567.051758,"
        "Scale=(X=128.000000,Y=128.000000,Z=76.000000),"
        "ToWorld(0)=(X=1024.000000,Y=-1536.000000,Z=32171.964844),"
        "ToWorld(1)=(X=128.000000),ToWorld(2)=(Y=128.000000),ToWorld(3)=(Z=0.296875),"
        "ToMaskmap(0)=(X=-131072.000000,Y=196608.000000,Z=-9551.051758),"
        "ToMaskmap(1)=(X=0.007813),ToMaskmap(2)=(Y=0.007813),ToMaskmap(3)=(Z=3.368421))"
    )
    lines.append(layer0)
    idx = 1
    for layer_num, ground, _ in meta["splats"]:
        if layer_num == 0:
            continue
        alpha = f"{tile_id}_splat{layer_num}"
        layer = (
            f"Layers({idx})=(Texture=Texture'{tile_tex_pkg}.{ground}',"
            f"AlphaMap=Texture'{tile_tex_pkg}.Height.{alpha}',"
            "UScale=1.000000,VScale=1.000000,"
            "TerrainMatrix=(XPlane=(X=0.007813),YPlane=(Y=0.007813),ZPlane=(Z=3.368421),"
            "WPlane=(W=1.000000,X=0.062500,Y=-0.093750,Z=365031.500000)),"
            "KFriction=196608.000000,KRestitution=-9567.051758,"
            "Scale=(X=32.000000,Y=32.000000,Z=76.000000),"
            "ToWorld(0)=(X=4096.000000,Y=-6144.000000,Z=32171.964844),"
            "ToWorld(1)=(X=32.000000),ToWorld(2)=(Y=32.000000),ToWorld(3)=(Z=0.296875),"
            "ToMaskmap(0)=(X=-131072.000000,Y=196608.000000,Z=-9551.051758),"
            "ToMaskmap(1)=(X=0.031250),ToMaskmap(2)=(Y=0.031250),ToMaskmap(3)=(Z=3.368421))"
        )
        lines.append(layer)
        idx += 1
    # Official bitmap is 256x256 bits = 2048 uint32 words (not 64). All-visible is -1.
    # If Phase A wrote quadvisibility.txt (one signed int per line), use those words.
    lines.extend(_quad_visibility_lines(meta))
    # World placement: without these the Unity terrain bakes at origin (#559 / Elven Village).
    lx, ly, lz = meta["location"]
    lines.append("GeneratedSectorCounter=256")
    lines.append(f"Location=(X={lx:.6f},Y={ly:.6f},Z={lz:.6f})")
    lines.append("DrawScale=10.000000")
    return "\n".join(lines) + "\n"


def deleted_actor_names(tile_id: str) -> set[str] | None:
    """bDeleteMe/bPendingDelete StaticMeshActor names from the tile's committed
    lossless dump (UE2 destroys them on map load — UnrealEd tombstones).
    StaticMeshExtractor skips them at extraction time; this cross-check repairs
    staticmeshes.json produced before that fix. None when the dump is absent."""
    import gzip

    dump = UNITY / "docs/interlude-ref/world/maps" / f"{tile_id}.unr.json.gz"
    if not dump.is_file():
        return None
    doc = json.load(gzip.open(dump))
    out: set[str] = set()
    for export in doc.get("exports") or []:
        if export.get("class") != "StaticMeshActor":
            continue
        props = {p.get("name"): p for p in (export.get("properties") or [])}
        if props.get("bDeleteMe", {}).get("value") or props.get("bPendingDelete", {}).get("value"):
            out.add(export.get("name") or "")
    out.discard("")
    return out


def filter_deleted_actors(tile_id: str, static_meshes: list[dict]) -> list[dict]:
    deleted = deleted_actor_names(tile_id)
    if deleted is None:
        print(
            f"NOTE {tile_id}: no committed lossless dump for the bDeleteMe cross-check; "
            "relying on StaticMeshExtractor's exclusion"
        )
        return static_meshes
    kept = [m for m in static_meshes if (m.get("name") or "") not in deleted]
    dropped = [m.get("name") or "?" for m in static_meshes if (m.get("name") or "") in deleted]
    if dropped:
        print(
            f"{tile_id}: excluded {len(dropped)} deleted StaticMeshActors "
            f"(bDeleteMe/bPendingDelete per the lossless dump; kept there): "
            + ", ".join(sorted(dropped))
        )
    return kept


def build_t3d(tile_id: str, terrain_info0: str, static_meshes: list[dict]) -> tuple[str, list[str]]:
    out = ["Begin Map", f"Begin Actor Class=TerrainInfo Name=TerrainInfo0"]
    for line in terrain_info0.splitlines():
        if line.startswith("TerrainMap=") or line.startswith("TerrainScale=") or line.startswith("Layers("):
            out.append("    " + line)
        elif line.startswith((
            "DecoLayers(",
            "QuadVisibilityBitmap(",
            "EdgeTurnBitmap(",
            "GeneratedSectorCounter=",
            "Location=",
            "DrawScale=",
        )):
            out.append("    " + line)
    out.append("End Actor")
    individuals: list[str] = []
    for mesh in static_meshes:
        name = mesh.get("name") or "StaticMeshActor"
        mesh_path = mesh.get("meshPath") or f"{mesh['package']}.{mesh['mesh']}"
        loc = mesh["location"]
        rot = mesh.get("rotation") or {}
        pitch = unreal_rotator(float(rot.get("pitch", 0)))
        yaw = unreal_rotator(float(rot.get("yaw", 0)))
        roll = unreal_rotator(float(rot.get("roll", 0)))
        scale = mesh.get("scale") or {}
        draw = float(scale.get("uniform") or 1.0)
        rot_parts = []
        if pitch:
            rot_parts.append(f"Pitch={pitch}")
        if yaw:
            rot_parts.append(f"Yaw={yaw}")
        if roll:
            rot_parts.append(f"Roll={roll}")
        rot_str = ",".join(rot_parts) if rot_parts else "Yaw=0"
        out.append(f"Begin Actor Class=StaticMeshActor Name={name}")
        out.append(f"    StaticMesh=StaticMesh'{mesh_path}'")
        out.append(
            f"    Location=(X={loc['x']:.6f},Y={loc['y']:.6f},Z={loc['z']:.6f})"
        )
        out.append(f"    SwayRotationOrig=({rot_str})")
        out.append(f"    DrawScale={draw:.6f}")
        # DrawScale3D carries mirroring (e.g. X=-1 on Elven plaza pillars); dropping it
        # rendered mirrored instances 90deg off along the curb.
        sx = float(scale.get("x", 1.0)); sy = float(scale.get("y", 1.0)); sz = float(scale.get("z", 1.0))
        if (sx, sy, sz) != (1.0, 1.0, 1.0) and not (sx == sy == sz == draw):
            out.append(f"    DrawScale3D=(X={sx:.6f},Y={sy:.6f},Z={sz:.6f})")
        out.append("End Actor")
        individuals.append(mesh_path)
    out.append("End Map")
    return "\n".join(out) + "\n", individuals


def main() -> None:
    ap = argparse.ArgumentParser()
    ap.add_argument("tile_id", help="e.g. 22_22")
    ap.add_argument(
        "--extract-dir",
        type=Path,
        default=REPO_ROOT / ".work/interlude-extract",
    )
    ap.add_argument(
        "--tile-texture-package",
        default="",
        help="Unreal texture package name (default T_{tile_id})",
    )
    args = ap.parse_args()
    tile_id = args.tile_id
    tile_tex = args.tile_texture_package or f"T_{tile_id}"
    src = args.extract_dir / tile_id
    if not src.is_dir():
        raise SystemExit(f"missing extract dir {src}")

    meta_path = src / f"{tile_id}_metadata.txt"
    raw_path = src / f"{tile_id}_heightmap.raw"
    sm_path = src / "staticmeshes.json"
    for p in (meta_path, raw_path, sm_path):
        if not p.is_file():
            raise SystemExit(f"missing {p}")

    meta = parse_metadata(meta_path)
    qpath = src / "quadvisibility.txt"
    qwords = load_quad_visibility(qpath)
    if qwords is None:
        print(
            f"WARN {tile_id}: no {qpath} — writing 2048 all-visible words; "
            "provide official bits when the tile has hidden quads"
        )
        meta["quad_words"] = [-1] * 2048
    else:
        meta["quad_words"] = qwords
    sm_data = json.loads(sm_path.read_text(encoding="utf-8"))
    static_meshes = filter_deleted_actors(tile_id, sm_data["staticMeshes"])

    meta_dir = MAPS / tile_id / "Meta"
    meta_dir.mkdir(parents=True, exist_ok=True)
    tex_dir = TEXTURES / tile_tex
    tex_dir.mkdir(parents=True, exist_ok=True)

    write_height_bmp(raw_path, tex_dir / f"Height.{tile_id}.bmp")

    for layer_num, ground, _ in meta["splats"]:
        splat_file = src / f"{tile_id}_splatmap{layer_num}_layer{layer_num}.png"
        if not splat_file.is_file():
            continue
        if layer_num == 0:
            dest = TEXTURES / "T_texture" / "layer0.png"
            if dest.exists():
                # Shared across tiles; do not clobber a good base alpha with a later tile's splat.
                print(f"{tile_id}: keeping existing {dest}")
            else:
                shutil.copy2(splat_file, dest)
            continue
        alpha_name = f"{tile_id}_splat{layer_num}"
        shutil.copy2(splat_file, tex_dir / f"{alpha_name}.png")
        # Ground diffuse comes from the extractor's terraintextures/ dump. Never fall back to the
        # splat PNG: that made _Layer.asset == _Alphamap.asset (grayscale masks as ground diffuse)
        # => black/dark holes on the ground (Elven 20_19..21_20).
        ground_png = tex_dir / f"{ground}.png"
        ground_src = find_ground_texture(args.extract_dir, ground)
        if ground_src is not None:
            if not ground_png.exists() or ground_png.read_bytes() == splat_file.read_bytes():
                shutil.copy2(ground_src, ground_png)
        elif not ground_png.exists():
            raise SystemExit(
                f"ERROR {tile_id}: ground texture '{ground}' missing — not in "
                f"{args.extract_dir / 'terraintextures'} and no {ground_png}"
            )

    terrain_info0 = build_terrain_info0(tile_id, meta, tile_tex)
    (meta_dir / "TerrainInfo0.txt").write_text(terrain_info0, encoding="utf-8")

    t3d_body, individuals = build_t3d(tile_id, terrain_info0, static_meshes)
    # BSP brushes (tools/interlude/BrushDump.java -> <extract>/<tile>/brushes.t3d): walk decks/floors that are not
    # StaticMeshes (e.g. Elven Village bridge decks over O_E_Brg). L2BrushBuilder bakes them into Brushes.prefab.
    brushes_t3d = src / "brushes.t3d"
    if brushes_t3d.is_file():
        t3d_body = t3d_body.rstrip("\n") + "\n" + brushes_t3d.read_text(encoding="utf-8")
        print(f"{tile_id}: appended BSP brushes from {brushes_t3d}")
    else:
        print(f"WARN {tile_id}: no {brushes_t3d} (BSP decks/floors will be missing)")
    (meta_dir / f"{tile_id}.t3d").write_text(t3d_body, encoding="utf-8")
    uniq_individuals = sorted(set(individuals))
    (meta_dir / "StaticMeshActorIndividuals.txt").write_text(
        "\n".join(uniq_individuals) + "\n", encoding="utf-8"
    )
    water_src = src / "watervolumes.json"
    water_dst = meta_dir / "watervolumes.json"
    if water_src.is_file():
        shutil.copy2(water_src, water_dst)
        print(f"{tile_id}: copied {water_src.name} into Meta")
    else:
        print(f"WARN {tile_id}: no {water_src} (WaterVolumes will be missing until dumped)")

    print(f"Wrote meta for {tile_id}: {len(static_meshes)} actors, {len(uniq_individuals)} unique meshes")


if __name__ == "__main__":
    main()
