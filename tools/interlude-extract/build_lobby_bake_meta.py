#!/usr/bin/env python3
"""Build the l2_lobby bake Meta from the lossless Lobby.unr dump (#932, gap G9).

Every other tile has its bake inputs committed (Meta/TerrainInfo0.txt,
Meta/<tile>.t3d, Meta/StaticMeshActorIndividuals.txt, produced by
tools/interlude/prepare_interlude_tile.py from the L2TerrainExtractor output).
The lobby bake inputs were never committed. This generator emits the same
shared Meta format for l2_lobby, sourced from the lossless dumps so the shared
bake pipeline (L2InterludeTileBakePipeline) consumes the lobby exactly like a
tile. (The old derived LoginForestActors.json side format was removed with the
LoginForestPresenter in #932 step 3 — the vista is the baked lobby scene.)

Inputs (committed, no client needed):
  l2-unity/docs/interlude-ref/world/env/Lobby.unr.json.gz
  l2-unity/docs/interlude-ref/world/quadvisibility/Lobby.quadvisibility.txt
  l2-unity/docs/interlude-ref/world/env/Lobby.brushes.t3d

Known non-blocking gaps (extraction list in Docs/LOBBY_LOGIN_EXTRACTION_AUDIT.md):
  - EdgeTurnBitmap bits are not extracted for any map (only counts).

Class-default resolutions come from the committed Engine.u .uc dump (gap G1):
cameras without a stored FovAngle (e.g. Camera25, the login vista camera)
inherit the class default walked up the extends chain (Camera.uc does not
override; PlayerController.uc FovAngle=50.0), and AmbientSoundType bytes are
mapped through AmbientSoundObject.uc enum ASType1. A missing .uc dump or
missing property is a hard error, never a guessed value.

Every texture PNG / mesh FBX referenced by the generated Meta is verified on
disk. Missing assets are listed as ERRORs and the exit code is 1 — nothing is
silently dropped.
"""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))

from apply_quad_visibility import load_words  # noqa: E402
from rebuild_deco_layers import (  # noqa: E402
    format_layer as format_deco_layer,
    full_name,
    import_index,
    load_json,
)
from rebuild_terrain_layers import (  # noqa: E402
    AXIS_NAME,
    find_ti,
    fmt_float,
    fmt_vec,
    parse_layers,
)

REPO = Path(__file__).resolve().parents[2]
WORLD = REPO / "l2-unity/docs/interlude-ref/world"
ENGINE_UC = REPO / "l2-unity/docs/interlude-ref/scripts/Engine.u"
MAPS = REPO / "l2-unity/Assets/Resources/Data/Maps"
TEX = REPO / "l2-unity/Assets/Resources/Data/Textures"
SM = REPO / "l2-unity/Assets/Resources/Data/StaticMeshes"

TILE = "l2_lobby"
TILE_PKG = "Lobby_terrain_t"

_UC_CLASS_RE = re.compile(r"^class\s+(\w+)\s+extends\s+(\w+)", re.M)
_UC_DEFAULT_RE = re.compile(r"^defaultproperties\s*\{(.*?)^\}", re.M | re.S)
_UC_ENUM_RE = re.compile(r"^enum\s+(\w+)\s*\{(.*?)\}", re.M | re.S)


def load_uc(uc_dir: Path, class_name: str) -> tuple[str | None, dict[str, str], dict[str, list[str]]]:
    """(extends, defaultproperties scalars, enums) parsed from a UScriptDump .uc."""
    path = uc_dir / f"{class_name}.uc"
    if not path.is_file():
        raise SystemExit(f"ERROR {path} missing — run extract_login_lobby.sh step 4 (Engine.u dump)")
    text = path.read_text(encoding="utf-8", errors="replace")
    m = _UC_CLASS_RE.search(text)
    extends = m.group(2) if m else None
    defaults: dict[str, str] = {}
    dm = _UC_DEFAULT_RE.search(text)
    if dm:
        for line in dm.group(1).splitlines():
            line = line.split("//", 1)[0].strip()
            if "=" in line:
                k, _, v = line.partition("=")
                defaults[k.strip()] = v.strip()
    enums: dict[str, list[str]] = {}
    for em in _UC_ENUM_RE.finditer(text):
        # Strip // comments before splitting: they trail the comma, so a naive
        # split("//") would eat the following entry's name.
        body = re.sub(r"//[^\n]*", "", em.group(2))
        names = []
        for item in body.split(","):
            item = item.strip()
            if item:
                names.append(item.split("=")[0].strip())
        enums[em.group(1)] = names
    return extends, defaults, enums


def resolve_uc_default(uc_dir: Path, class_name: str, prop_name: str) -> str | None:
    """Walk the extends chain for a defaultproperties scalar (e.g. Camera->PlayerController FovAngle)."""
    seen = set()
    cur: str | None = class_name
    while cur and cur not in seen:
        seen.add(cur)
        extends, defaults, _ = load_uc(uc_dir, cur)
        if prop_name in defaults:
            return defaults[prop_name]
        cur = extends
    return None


def layer_ref(ly: dict, key: str) -> dict:
    """parse_layers keeps the ref string under <key> and the raw UE2 ref index
    under _<key>_index — resolution goes through the index (object names are
    not unique in the import table)."""
    return {"ref": ly.get(key), "index": ly.get("_" + key + "_index")}


def texture_ref(imps: dict, ref_or_field) -> str:
    full = full_name(imps, ref_or_field, "Texture")
    return "None" if not full else f"Texture'{full}'"


def format_terrain_layer(ly: dict, imps: dict) -> str:
    """One Layers(i)= line. Same field set as rebuild_terrain_layers.format_layer,
    but refs keep their official full names (lobby alphamaps sit at the package
    root, there is no synthetic 'Height' group)."""
    parts = [f"Texture={texture_ref(imps, layer_ref(ly, 'Texture'))}"]
    alpha = texture_ref(imps, layer_ref(ly, "AlphaMap"))
    if alpha != "None":
        parts.append(f"AlphaMap={alpha}")
    parts.append(f"UScale={fmt_float(ly.get('UScale', 0))}")
    parts.append(f"VScale={fmt_float(ly.get('VScale', 0))}")
    if ly.get("UPan") not in (None, 0, 0.0):
        parts.append(f"UPan={fmt_float(ly['UPan'])}")
    if ly.get("VPan") not in (None, 0, 0.0):
        parts.append(f"VPan={fmt_float(ly['VPan'])}")
    if ly.get("TextureRotation") is not None:
        parts.append(f"TextureRotation={fmt_float(ly['TextureRotation'])}")
    axis = ly.get("TextureMapAxis")
    if axis is not None:
        try:
            ai = int(axis)
        except (TypeError, ValueError):
            ai = axis
        parts.append(f"TextureMapAxis={AXIS_NAME.get(ai, axis) if isinstance(ai, int) else axis}")
    if "bUseAlpha" in ly:
        parts.append(f"bUseAlpha={'True' if ly['bUseAlpha'] else 'False'}")
    if ly.get("LayerScale"):
        parts.append(f"Scale={fmt_vec(ly['LayerScale'])}")
    rot = ly.get("LayerRotation")
    if rot and any(float(rot.get(k, 0) or 0) for k in ("pitch", "yaw", "roll")):
        parts.append(
            "LayerRotation=(Pitch={0},Yaw={1},Roll={2})".format(
                fmt_float(rot.get("pitch")), fmt_float(rot.get("yaw")), fmt_float(rot.get("roll"))
            )
        )
    for i in sorted(ly.get("ToWorld") or {}):
        parts.append(f"ToWorld({i})={fmt_vec(ly['ToWorld'][i])}")
    for i in sorted(ly.get("ToMaskmap") or {}):
        parts.append(f"ToMaskmap({i})={fmt_vec(ly['ToMaskmap'][i])}")
    if ly.get("KFriction") is not None:
        parts.append(f"KFriction={fmt_float(ly['KFriction'])}")
    if ly.get("KRestitution") is not None:
        parts.append(f"KRestitution={fmt_float(ly['KRestitution'])}")
    return f"Layers({ly['index']})=(" + ",".join(parts) + ")"


def prop(export: dict, name: str) -> dict | None:
    return next((p for p in export.get("properties") or [] if p.get("name") == name), None)


def vec_line(name: str, v: dict) -> str:
    return f"{name}=(X={fmt_float(v.get('x'))},Y={fmt_float(v.get('y'))},Z={fmt_float(v.get('z'))})"


def rot_str(rot: dict | None) -> str:
    rot = rot or {}
    parts = []
    for key, label in (("pitch", "Pitch"), ("yaw", "Yaw"), ("roll", "Roll")):
        val = int(rot.get(key) or 0)
        if val:
            parts.append(f"{label}={val}")
    return ",".join(parts) if parts else "Yaw=0"


def sma_block(export: dict, imps: dict) -> tuple[str, str | None]:
    name = export.get("name") or "StaticMeshActor"
    mesh = full_name(imps, prop(export, "StaticMesh"), "StaticMesh")
    lines = [f"Begin Actor Class=StaticMeshActor Name={name}"]
    if mesh:
        lines.append(f"    StaticMesh=StaticMesh'{mesh}'")
    loc = (prop(export, "Location") or {}).get("vector")
    if loc:
        lines.append("    " + vec_line("Location", loc))
    lines.append(f"    SwayRotationOrig=({rot_str((prop(export, 'Rotation') or {}).get('rotator'))})")
    draw = (prop(export, "DrawScale") or {}).get("value")
    draw = float(draw) if draw is not None else 1.0
    lines.append(f"    DrawScale={fmt_float(draw)}")
    ds3 = (prop(export, "DrawScale3D") or {}).get("vector")
    if ds3:
        sx, sy, sz = (float(ds3.get(k) or 1.0) for k in ("x", "y", "z"))
        if (sx, sy, sz) != (1.0, 1.0, 1.0) and not (sx == sy == sz == draw):
            lines.append("    " + vec_line("DrawScale3D", ds3))
    lines.append("End Actor")
    return "\n".join(lines), mesh


def camera_block(export: dict, default_fov: str) -> tuple[str, bool]:
    name = export.get("name") or "Camera"
    lines = [f"Begin Actor Class=Camera Name={name}"]
    loc = (prop(export, "Location") or {}).get("vector")
    if loc:
        lines.append("    " + vec_line("Location", loc))
    rot = (prop(export, "Rotation") or {}).get("rotator")
    if rot:
        lines.append(f"    Rotation=({rot_str(rot)})")
    tag = (prop(export, "Tag") or {}).get("value")
    if tag:
        lines.append(f"    Tag={tag}")
    fov = (prop(export, "FovAngle") or {}).get("value")
    used_default = fov is None
    if used_default:
        # Officially absent on the actor: the value is the Engine.u class
        # default (Camera.uc does not override; PlayerController.uc FovAngle=50.0).
        fov = float(default_fov)
    lines.append(f"    FovAngle={fmt_float(float(fov))}")
    lines.append("End Actor")
    return "\n".join(lines), used_default


def interpolation_block(export: dict) -> str:
    name = export.get("name") or "InterpolationPoint"
    lines = [f"Begin Actor Class=InterpolationPoint Name={name}"]
    loc = (prop(export, "Location") or {}).get("vector")
    if loc:
        lines.append("    " + vec_line("Location", loc))
    rot = (prop(export, "Rotation") or {}).get("rotator")
    if rot:
        lines.append(f"    Rotation=({rot_str(rot)})")
    lines.append("End Actor")
    return "\n".join(lines)


def ambient_block(export: dict, imps: dict, as_type_names: list[str]) -> str:
    name = export.get("name") or "AmbientSoundObject"
    lines = [f"Begin LevelObject Class=AmbientSoundObject Name={name}"]
    sound = full_name(imps, prop(export, "AmbientSound"), "Sound")
    if sound:
        lines.append(f"    AmbientSound=Sound'{sound}'")
    for key in ("SoundRadius", "AmbientSoundStartTime"):
        p = prop(export, key)
        if p and p.get("value") is not None:
            lines.append(f"    {key}={fmt_float(p['value'])}")
    for key in ("SoundVolume", "SoundPitch", "AmbientRandom"):
        p = prop(export, key)
        if p and p.get("value") is not None:
            lines.append(f"    {key}={int(p['value'])}")
    group = prop(export, "Group")
    if group and group.get("value"):
        lines.append(f'    Group="{group["value"]}"')
    as_type = prop(export, "AmbientSoundType")
    if as_type and as_type.get("value") is not None:
        idx = int(as_type["value"])
        if idx >= len(as_type_names):
            raise SystemExit(f"ERROR {name}: AmbientSoundType byte {idx} outside enum ASType1 ({as_type_names})")
        lines.append(f"    AmbientSoundType={as_type_names[idx]}")
    loc = (prop(export, "Location") or {}).get("vector")
    if loc:
        lines.append("    " + vec_line("Location", loc))
    lines.append("End LevelObject")
    return "\n".join(lines)


def find_case_insensitive(root: Path, name: str) -> Path | None:
    if not root.is_dir():
        return None
    want = name.lower()
    for child in root.iterdir():
        if child.is_dir() and child.name.lower() == want:
            return child
    return None


def collect_mesh_refs(doc: dict, imps: dict) -> set[str]:
    """Every StaticMesh full name the lobby references (StaticMeshActors +
    terrain deco layers), resolved through the raw UE2 ref indices."""
    out: set[str] = set()
    for export in doc.get("exports") or []:
        if export.get("class") != "StaticMeshActor":
            continue
        mesh = full_name(imps, prop(export, "StaticMesh"), "StaticMesh")
        if mesh:
            out.add(mesh)
    deco_prop = prop(find_ti(doc), "DecoLayers")
    for lay in (deco_prop.get("subproperties") or []) if deco_prop else []:
        for fld in lay if isinstance(lay, list) else [lay]:
            if isinstance(fld, dict) and fld.get("name") == "StaticMesh":
                mesh = full_name(imps, fld, "StaticMesh")
                if mesh:
                    out.add(mesh)
    return out


def mesh_on_disk(full: str) -> bool:
    parts = full.split(".")
    folder = find_case_insensitive(SM, parts[0])
    return bool(folder and any(p.stem.lower() == parts[-1].lower() for p in folder.glob("*.fbx")))


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--dump", type=Path, default=WORLD / "env/Lobby.unr.json.gz")
    ap.add_argument("--quadvisibility", type=Path, default=WORLD / "quadvisibility/Lobby.quadvisibility.txt")
    ap.add_argument("--brushes", type=Path, default=WORLD / "env/Lobby.brushes.t3d")
    ap.add_argument("--engine-uc", type=Path, default=ENGINE_UC)
    ap.add_argument("--out", type=Path, default=MAPS / TILE / "Meta")
    args = ap.parse_args()

    doc = load_json(args.dump)
    imps = import_index(doc)
    ti = find_ti(doc)

    # Engine.u class defaults (gap G1, committed dump): Camera FovAngle comes
    # from the extends chain (Camera.uc silent -> PlayerController.uc), the
    # AmbientSoundType byte enum from AmbientSoundObject.uc. Missing data is a
    # hard error, never a guessed value.
    default_fov = resolve_uc_default(args.engine_uc, "Camera", "FovAngle")
    if default_fov is None:
        raise SystemExit(
            f"ERROR no FovAngle default in the Camera extends chain under {args.engine_uc} "
            "— rerun extract_login_lobby.sh step 4"
        )
    _, _, as_enums = load_uc(args.engine_uc, "AmbientSoundObject")
    as_type_names = as_enums.get("ASType1")
    if not as_type_names:
        raise SystemExit(f"ERROR no enum ASType1 in {args.engine_uc}/AmbientSoundObject.uc")

    if not args.quadvisibility.is_file():
        raise SystemExit(f"ERROR missing {args.quadvisibility} — run extract_login_lobby.sh step 2")
    words = load_words(args.quadvisibility)

    # --- TerrainInfo0.txt ---
    terrain_map = full_name(imps, prop(ti, "TerrainMap"), "Texture")
    if not terrain_map:
        raise SystemExit("ERROR TerrainInfo has no TerrainMap ref")
    scale = (prop(ti, "TerrainScale") or {}).get("vector") or {}
    loc = (prop(ti, "Location") or {}).get("vector") or {}
    draw = (prop(ti, "DrawScale") or {}).get("value")
    sectors = (prop(ti, "GeneratedSectorCounter") or {}).get("value")

    layer_lines = [format_terrain_layer(ly, imps) for ly in parse_layers(ti)]
    deco_prop = prop(ti, "DecoLayers")
    deco_layers = list(deco_prop.get("subproperties") or []) if deco_prop else []
    deco_lines = [format_deco_layer(i, lay, imps) for i, lay in enumerate(deco_layers)]

    info0 = [f"TerrainMap=Texture'{terrain_map}'", vec_line("TerrainScale", scale)]
    info0 += layer_lines
    info0 += deco_lines
    info0 += [f"QuadVisibilityBitmap({i})={int(w)}" for i, w in enumerate(words)]
    if sectors is not None:
        info0.append(f"GeneratedSectorCounter={int(sectors)}")
    info0.append(vec_line("Location", loc))
    info0.append(f"DrawScale={fmt_float(draw if draw is not None else 1.0)}")
    info0_text = "\n".join(info0) + "\n"

    # --- t3d ---
    t3d = ["Begin Map", "Begin Actor Class=TerrainInfo Name=TerrainInfo0"]
    for line in info0:
        t3d.append("    " + line)
    t3d.append("End Actor")

    individuals: list[str] = []
    cameras_default_fov: list[str] = []
    for export in doc.get("exports") or []:
        cls = export.get("class")
        if cls == "StaticMeshActor":
            block, mesh = sma_block(export, imps)
            t3d.append(block)
            if mesh:
                individuals.append(mesh)
        elif cls == "Camera":
            block, used_default = camera_block(export, default_fov)
            t3d.append(block)
            if used_default:
                cameras_default_fov.append(export.get("name") or "?")
        elif cls == "InterpolationPoint":
            t3d.append(interpolation_block(export))
        elif cls == "AmbientSoundObject":
            t3d.append(ambient_block(export, imps, as_type_names))

    if args.brushes.is_file():
        t3d.append(args.brushes.read_text(encoding="utf-8").rstrip("\n"))
    else:
        print(f"ERROR no {args.brushes} — BSP skybox/walk brushes missing (extract step 3)", file=sys.stderr)
    t3d.append("End Map")
    t3d_text = "\n".join(t3d) + "\n"

    args.out.mkdir(parents=True, exist_ok=True)
    (args.out / "TerrainInfo0.txt").write_text(info0_text, encoding="utf-8")
    (args.out / f"{TILE}.t3d").write_text(t3d_text, encoding="utf-8")
    uniq = sorted(set(individuals))
    (args.out / "StaticMeshActorIndividuals.txt").write_text("\n".join(uniq) + "\n", encoding="utf-8")

    # --- verify referenced assets exist ---
    missing: list[str] = []
    refs_tex: set[str] = set()
    for ly in parse_layers(ti):
        for key in ("Texture", "AlphaMap"):
            full = full_name(imps, layer_ref(ly, key), "Texture")
            if full:
                refs_tex.add(full)
    for lay in deco_layers:
        for fld in lay if isinstance(lay, list) else [lay]:
            if not isinstance(fld, dict) or fld.get("name") not in ("DensityMap", "ColorMap", "ScaleMap"):
                continue
            full = full_name(imps, fld, "Texture")
            if full:
                refs_tex.add(full)
    deco_meshes: set[str] = set()
    for lay in deco_layers:
        for fld in lay if isinstance(lay, list) else [lay]:
            if not isinstance(fld, dict) or fld.get("name") != "StaticMesh":
                continue
            full = full_name(imps, fld, "StaticMesh")
            if full:
                deco_meshes.add(full)
    for full in sorted(refs_tex):
        parts = full.split(".")
        pkg, name = parts[0], parts[-1]
        if full == "T_texture.Texture.layer0":
            pkg, name = "T_texture", "layer0"
        folder = find_case_insensitive(TEX, pkg)
        hit = folder and any(p.stem.lower() == name.lower() for p in folder.glob("*.png"))
        if not hit:
            missing.append(f"texture {full} (want {TEX}/<case-insensitive {pkg}>/{name}.png)")
    # TerrainMap is the heightmap BMP: TextureUtils.GetHeightMapPath synthesizes
    # the Height "group", so the file is <pkg>/Height.<name>.bmp.
    tm_parts = terrain_map.split(".")
    tm_folder = find_case_insensitive(TEX, tm_parts[0])
    tm_stem = f"Height.{tm_parts[-1]}".lower()
    if not (tm_folder and any(p.stem.lower() == tm_stem for p in tm_folder.glob("*.bmp"))):
        missing.append(f"heightmap {terrain_map} (want {TEX}/<case-insensitive {tm_parts[0]}>/{tm_stem}.bmp)")
    for full in sorted(set(individuals) | deco_meshes):
        if not mesh_on_disk(full):
            parts = full.split(".")
            missing.append(f"mesh {full} (want {SM}/<case-insensitive {parts[0]}>/{parts[-1]}.fbx)")

    print(
        f"{TILE}: {len(layer_lines)} layers, {len(deco_lines)} deco layers, "
        f"{len(individuals)} SMAs ({len(uniq)} unique meshes)"
    )
    if cameras_default_fov:
        print(
            f"FOV: {len(cameras_default_fov)} cameras store no FovAngle; emitted the Engine.uc "
            f"class default FovAngle={fmt_float(float(default_fov))} "
            f"(Camera->PlayerController chain): {', '.join(cameras_default_fov)}"
        )
    if missing:
        print("ERROR referenced assets missing:", file=sys.stderr)
        for m in missing:
            print(f"  {m}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
