#!/usr/bin/env python3
"""Render parity + static perf tables from the #1670 lossless map dumps vs the Fineage tree.

No Unity. Frame-time measurements that need the editor are documented, not run.
"""
from __future__ import annotations

import csv
import gzip
import json
import re
import sys
from collections import Counter, defaultdict
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref"
WORLD = DOCS / "world"
MAPS = WORLD / "maps"
ASSETS = ROOT / "l2-unity" / "Assets"
FINE_MAPS = ASSETS / "Resources" / "Data" / "Maps"

# Play-first tiles from #1673 (TI, Gludio, Elven Village / Mother Tree, Dion, Giran harbour).
ZONES = [
    ("Talking Island", ("16_24", "16_25", "16_26", "17_24", "17_25", "17_26", "18_25", "18_26")),
    ("Town of Gludio", ("19_21", "19_19")),
    ("Elven Village", ("21_19", "21_18", "21_20", "20_19")),
    ("Town of Dion", ("20_22",)),
    ("Giran Castle Town / Harbour", ("22_22", "22_21", "22_23", "21_21", "21_22", "21_23")),
]

# UE2: 1 world unit / 52.5 = metres (Fineage VectorUtils).
UU_TO_M = 1.0 / 52.5


def open_json(path: Path):
    if str(path).endswith(".gz"):
        with gzip.open(path, "rt", encoding="utf-8") as f:
            return json.load(f)
    with path.open(encoding="utf-8") as f:
        return json.load(f)


def dump_path(tile: str) -> Path | None:
    for p in (MAPS / f"{tile}.unr.json.gz", MAPS / f"{tile}.unr.json"):
        if p.exists():
            return p
    return None


def ref_name(ref: str) -> str:
    if not ref or ref in {"none", "None"}:
        return ""
    return str(ref).split(":")[-1]


def prop_map(export: dict) -> dict:
    out = defaultdict(list)
    for p in export.get("properties") or []:
        out[p.get("name") or ""].append(p)
    return out


def first(pmap, name, key="value"):
    if name not in pmap or not pmap[name]:
        return None
    p = pmap[name][0]
    if key in p and p[key] is not None:
        return p[key]
    return p.get("ref") or p.get("vector") or p.get("rotator") or p.get("color") or p.get("int_array")


def struct_fields(prop: dict) -> dict:
    """Flatten one struct's subproperties (Range Min/Max, etc.)."""
    out = {}
    for block in prop.get("subproperties") or []:
        items = block if isinstance(block, list) and block and isinstance(block[0], dict) else [block]
        for sp in items:
            if not isinstance(sp, dict):
                continue
            n = sp.get("name")
            if not n:
                continue
            if sp.get("subproperties"):
                out[n] = struct_fields(sp)
            else:
                out[n] = sp.get("value") if "value" in sp else (sp.get("ref") or sp.get("vector") or sp.get("rotator"))
    return out


def parse_layers(ti_export: dict) -> list[dict]:
    layers = []
    for p in ti_export.get("properties") or []:
        if p.get("name") != "Layers" or p.get("struct") != "TerrainLayer":
            continue
        idx = p.get("array_index") or 0
        fields = struct_fields(p)
        # also grab direct children if flatten missed array-index vectors
        for sp_block in p.get("subproperties") or []:
            items = sp_block if isinstance(sp_block, list) else [sp_block]
            for sp in items:
                if not isinstance(sp, dict):
                    continue
                n = sp.get("name")
                if n in ("Texture", "AlphaMap", "LayerWeightMap") and sp.get("ref"):
                    fields[n] = sp["ref"]
                elif n in ("UScale", "VScale", "UPan", "VPan", "TextureRotation", "bUseAlpha", "TextureMapAxis") and "value" in sp:
                    fields[n] = sp["value"]
                elif n == "LayerRotation" and sp.get("rotator"):
                    fields[n] = sp["rotator"]
                elif n == "Scale" and sp.get("vector"):
                    fields["LayerScale"] = sp["vector"]
        layers.append({"index": idx, **fields})
    layers.sort(key=lambda r: r["index"])
    return layers


def parse_deco(ti_export: dict) -> list[dict]:
    deco = []
    for p in ti_export.get("properties") or []:
        if p.get("name") != "DecoLayers":
            continue
        for block in p.get("subproperties") or []:
            row = {}
            items = block if isinstance(block, list) else [block]
            for sp in items:
                if not isinstance(sp, dict):
                    continue
                n = sp.get("name")
                if sp.get("subproperties"):
                    row[n] = struct_fields(sp)
                else:
                    row[n] = sp.get("value") if "value" in sp else (sp.get("ref") or sp.get("vector"))
            if row:
                deco.append(row)
    return deco


def bitmap_stats(ti_export: dict, name: str) -> dict:
    for p in ti_export.get("properties") or []:
        if p.get("name") != name:
            continue
        arr = p.get("int_array") or []
        zeros = sum(1 for x in arr if x == 0)
        return {
            "words": len(arr),
            "zeros": zeros,
            "nonzero": len(arr) - zeros,
            "array_size": p.get("array_size"),
            "type": p.get("type"),
            "data_len": p.get("data_len"),
        }
    return {}


def parse_official_tile(tile: str) -> dict | None:
    path = dump_path(tile)
    if not path:
        return None
    data = open_json(path)
    exports = data.get("exports") or []
    by = defaultdict(list)
    for e in exports:
        by[e.get("class") or ""].append(e)

    out = {
        "tile": tile,
        "export_count": data.get("export_count") or len(exports),
        "classes": {k: len(v) for k, v in by.items()},
        "layers": [],
        "deco": [],
        "terrain_scale": None,
        "terrain_map": "",
        "terrain_location": None,
        "map_x": None,
        "map_y": None,
        "quad_vis": {},
        "edge_turn": {},
        "sma": {"count": 0},
        "water": 0,
        "zone_fog": [],
        "sky": 0,
        "lights": 0,
        "speedtree": 0,
    }
    if by.get("TerrainInfo"):
        ti = by["TerrainInfo"][0]
        pmap = prop_map(ti)
        out["terrain_scale"] = first(pmap, "TerrainScale")
        out["terrain_map"] = ref_name(first(pmap, "TerrainMap") or "")
        out["terrain_location"] = first(pmap, "Location")
        out["map_x"] = first(pmap, "MapX")
        out["map_y"] = first(pmap, "MapY")
        out["layers"] = parse_layers(ti)
        out["deco"] = parse_deco(ti)
        out["quad_vis"] = bitmap_stats(ti, "QuadVisibilityBitmap")
        out["edge_turn"] = bitmap_stats(ti, "EdgeTurnBitmap")

    sma = by.get("StaticMeshActor") or []
    meshes = Counter()
    ds = 0
    ds3d = 0
    unlit = 0
    shadow = 0
    static_lit = 0
    lod = 0
    ignored = 0
    inst = 0
    steps = 0
    mirrors = 0
    unique_ds = []
    for e in sma:
        pmap = prop_map(e)
        sm = ref_name(first(pmap, "StaticMesh") or "")
        if sm:
            meshes[sm] += 1
        if "DrawScale" in pmap:
            ds += 1
            unique_ds.append(first(pmap, "DrawScale"))
        if "DrawScale3D" in pmap:
            ds3d += 1
            v = first(pmap, "DrawScale3D") or {}
            if isinstance(v, dict) and any(float(v.get(k, 1) or 1) < 0 for k in ("x", "y", "z")):
                mirrors += 1
        if first(pmap, "bUnlit") is True:
            unlit += 1
        if first(pmap, "bShadowCast") is True:
            shadow += 1
        if first(pmap, "bStaticLighting") is True:
            static_lit += 1
        if "L2LodViewDuration" in pmap or "L2CurrentLod" in pmap:
            lod += 1
        if first(pmap, "bIgnoredRange") is True:
            ignored += 1
        if "StaticMeshInstance" in pmap:
            inst += 1
        if "StepSound_1" in pmap:
            steps += 1
    out["sma"] = {
        "count": len(sma),
        "unique_meshes": len(meshes),
        "drawscale": ds,
        "drawscale3d": ds3d,
        "mirrors": mirrors,
        "unlit": unlit,
        "shadow": shadow,
        "static_lighting": static_lit,
        "l2_lod": lod,
        "ignored_range": ignored,
        "instances": inst,
        "step_sounds": steps,
        "top_meshes": meshes.most_common(8),
    }
    out["water"] = len(by.get("WaterVolume") or [])
    out["lights"] = len(by.get("Light") or []) + len(by.get("Spotlight") or [])
    out["sky"] = len(by.get("SkyZoneInfo") or [])
    out["nsun"] = len(by.get("NSun") or [])
    out["nmoon"] = len(by.get("NMoon") or [])
    out["speedtree"] = sum(len(v) for k, v in by.items() if "speedtree" in k.lower() or k.lower() in {"tree", "l2tree"})
    for z in (by.get("ZoneInfo") or []) + (by.get("LevelInfo") or []):
        pmap = prop_map(z)
        fog = {
            "class": z.get("class"),
            "name": z.get("name"),
            "distance_fog": first(pmap, "bDistanceFog"),
            "fog_color": first(pmap, "DistanceFogColor"),
            "fog_start": first(pmap, "DistanceFogStart"),
            "fog_end": first(pmap, "DistanceFogEnd"),
            "ambient_brightness": first(pmap, "AmbientBrightness"),
            "ambient_saturation": first(pmap, "AmbientSaturation"),
        }
        out["zone_fog"].append(fog)
    return out


def parse_fineage_terrain_txt(tile: str) -> dict:
    p = FINE_MAPS / tile / "Meta" / "TerrainInfo0.txt"
    if not p.exists():
        return {"present": False}
    text = p.read_text(encoding="utf-8", errors="replace")
    layers = len(re.findall(r"^Layers\((\d+)\)=", text, re.M))
    deco = len(re.findall(r"^DecoLayers\((\d+)\)=", text, re.M))
    scale = None
    m = re.search(r"TerrainScale=\(X=([^,]+),Y=([^,]+),Z=([^)]+)\)", text)
    if m:
        scale = {"x": float(m.group(1)), "y": float(m.group(2)), "z": float(m.group(3))}
    rot = len(re.findall(r"LayerRotation=\([^)]*[1-9]", text))
    texrot = len(re.findall(r"TextureRotation=([^,\)]+)", text))
    texrot_nonzero = 0
    for v in re.findall(r"TextureRotation=([^,\)]+)", text):
        try:
            if abs(float(v)) > 1e-6:
                texrot_nonzero += 1
        except ValueError:
            pass
    # Official / Meta text uses QuadVisibilityBitmap(N)=word, not a parenthesized list.
    qwords = qhidden = q_not_full = 0
    has_quad = False
    for m in re.finditer(r"^QuadVisibilityBitmap\((\d+)\)=(-?\d+)", text, re.M):
        has_quad = True
        qwords += 1
        word = int(m.group(2)) & 0xFFFFFFFF
        bits = word.bit_count()
        hidden = 32 - bits
        qhidden += hidden
        if word != 0xFFFFFFFF:
            q_not_full += 1
    return {
        "present": True,
        "layers": layers,
        "deco": deco,
        "scale": scale,
        "layer_rotation_nonzero_lines": rot,
        "texture_rotation_fields": texrot,
        "texture_rotation_nonzero": texrot_nonzero,
        "has_quad_vis": has_quad,
        "quad_words": qwords,
        "quad_hidden": qhidden,
        "quad_not_full_words": q_not_full,
    }


def parse_fineage_sma(tile: str) -> dict:
    p = FINE_MAPS / tile / "Meta" / "StaticMeshActor.json"
    t3d = FINE_MAPS / tile / "Meta" / f"{tile}.t3d"
    out = {"json_present": p.exists(), "json_count": 0, "json_scale": 0, "json_scale_xyz": 0,
           "t3d_present": t3d.exists(), "t3d_sma": 0, "t3d_drawscale": 0, "t3d_ds3d": 0}
    if p.exists():
        data = json.loads(p.read_text(encoding="utf-8"))
        meshes = data.get("staticMeshes") or []
        out["json_count"] = len(meshes)
        out["json_scale"] = sum(1 for m in meshes if m.get("scale") not in (None, 0, 1, 1.0))
        out["json_scale_xyz"] = sum(1 for m in meshes if any(m.get(k) not in (None, 0, 1, 1.0) for k in ("scaleX", "scaleY", "scaleZ")))
        out["json_has_euler"] = sum(1 for m in meshes if "eulerAngles" in m)
        out["json_has_yaw"] = sum(1 for m in meshes if "yaw" in m)
    if t3d.exists():
        text = t3d.read_text(encoding="utf-8", errors="replace")
        parts = re.split(r"Begin Actor ", text)
        for part in parts:
            if part.startswith("Class=StaticMeshActor"):
                out["t3d_sma"] += 1
                if re.search(r"^DrawScale=", part, re.M):
                    out["t3d_drawscale"] += 1
                if re.search(r"^DrawScale3D=", part, re.M):
                    out["t3d_ds3d"] += 1
    return out


def fineage_flags(tile: str) -> dict:
    d = FINE_MAPS / tile
    return {
        "tile": d.is_dir(),
        "terrain_txt": (d / "Meta" / "TerrainInfo0.txt").exists(),
        "t3d": (d / "Meta" / f"{tile}.t3d").exists(),
        "sma_json": (d / "Meta" / "StaticMeshActor.json").exists(),
        "sun_masks": (d / "Meta" / "StaticMeshSunMasks.json").exists() or (d / "Meta" / "StaticMeshSunMasksRemapped.json").exists(),
        "vertices": (d / "Meta" / "StaticMeshVertices.json").exists(),
        "deco_prefab": (d / f"{tile}_DecoLayer.prefab").exists(),
        "deco_instances": (d / f"{tile}_DecoInstances.asset").exists(),
        "water_asset": (d / f"{tile}_Water.asset").exists(),
        "water_mat": (d / f"{tile}_Mat_Water.mat").exists(),
        "water_any": (d / f"{tile}_Water.asset").exists() or (d / f"{tile}_Mat_Water.mat").exists(),
        "static_meshes_prefab": (d / "StaticMeshes.prefab").exists() or (d / f"{tile}_StaticMeshes.prefab").exists(),
    }


def vec_eq(a, b, tol=1e-3):
    if not isinstance(a, dict) or not isinstance(b, dict):
        return a == b
    return all(abs(float(a.get(k, 0)) - float(b.get(k, 0))) <= tol for k in ("x", "y", "z"))


def load_holes_tsv() -> dict:
    """Official hidden-quad counts from Cruma UTerrainInfoActor.getQuadVisibilityBitmap().

    The #1670 JSON dump only stores QuadVisibilityBitmap array_size (no payload);
    typed Cruma reads the 256x256 bitset. See DumpTerrainHoles.java.
    """
    path = WORLD / "render-parity-holes.tsv"
    out = {}
    if not path.exists():
        return out
    with path.open(encoding="utf-8") as f:
        r = csv.DictReader(f, delimiter="\t")
        for row in r:
            out[row["tile"]] = row
    return out


def load_all():
    official = {}
    fine_t = {}
    fine_s = {}
    flags = {}
    holes = load_holes_tsv()
    for _, tiles in ZONES:
        for tile in tiles:
            print(f"parse {tile}", file=sys.stderr)
            official[tile] = parse_official_tile(tile)
            if official[tile] and tile in holes:
                official[tile]["official_hidden_quads"] = int(holes[tile].get("quad_hidden") or 0)
                official[tile]["official_quad_bits"] = int(holes[tile].get("quad_bits") or 0)
                official[tile]["official_edge_true"] = int(holes[tile].get("edge_true") or 0)
                official[tile]["hm"] = f"{holes[tile].get('hm_x')}x{holes[tile].get('hm_y')}"
            fine_t[tile] = parse_fineage_terrain_txt(tile)
            fine_s[tile] = parse_fineage_sma(tile)
            flags[tile] = fineage_flags(tile)
    return official, fine_t, fine_s, flags


def fmt_vec(v):
    if not isinstance(v, dict):
        return str(v)
    return f"({v.get('x')}, {v.get('y')}, {v.get('z')})"


def write_tables(official, fine_t, fine_s, flags):
    # per-tile TSV
    rows = []
    for zone, tiles in ZONES:
        for tile in tiles:
            o = official[tile]
            t = fine_t[tile]
            s = fine_s[tile]
            f = flags[tile]
            if not o:
                rows.append({"zone": zone, "tile": tile, "official_unr": "no"})
                continue
            sma = o["sma"]
            rows.append({
                "zone": zone,
                "tile": tile,
                "official_unr": "yes",
                "fineage_tile": "yes" if f["tile"] else "no",
                "terrain_scale": fmt_vec(o["terrain_scale"]) if o["terrain_scale"] else "",
                "fineage_terrain_scale": fmt_vec(t.get("scale")) if t.get("scale") else "",
                "scale_match": "yes" if t.get("scale") and o["terrain_scale"] and vec_eq(o["terrain_scale"], t["scale"]) else ("n/a" if not t.get("present") else "no"),
                "official_layers": len(o["layers"]),
                "fineage_layers_txt": t.get("layers", ""),
                "official_deco": len(o["deco"]),
                "fineage_deco_txt": t.get("deco", ""),
                "official_hidden_quads": o.get("official_hidden_quads", ""),
                "official_quad_bits": o.get("official_quad_bits", ""),
                "official_heightmap": o.get("hm", ""),
                "fineage_meta_quad_words": t.get("quad_words", ""),
                "fineage_meta_hidden_quads": t.get("quad_hidden", ""),
                "fineage_parser_reads_holes": "no",
                "official_sma": sma["count"],
                "fineage_sma_json": s.get("json_count", ""),
                "fineage_sma_t3d": s.get("t3d_sma", ""),
                "sma_delta_json": (s.get("json_count") or 0) - sma["count"] if s.get("json_present") else "",
                "official_drawscale": sma["drawscale"],
                "official_ds3d": sma["drawscale3d"],
                "official_mirrors": sma["mirrors"],
                "official_unlit": sma["unlit"],
                "official_l2_lod": sma["l2_lod"],
                "official_mesh_instances": sma["instances"],
                "official_unique_meshes": sma["unique_meshes"],
                "official_water": o["water"],
                "fineage_water": ("asset" if f["water_asset"] else ("mat" if f["water_mat"] else "no")),
                "fineage_deco_prefab": "yes" if f["deco_prefab"] else "no",
                "fineage_deco_instances": "yes" if f["deco_instances"] else "no",
                "fineage_sun_masks": "yes" if f["sun_masks"] else "no",
                "official_lights": o["lights"],
                "official_fog_zones": sum(1 for z in o["zone_fog"] if z.get("distance_fog") is True or z.get("fog_end")),
            })
    path = WORLD / "render-parity-tiles.tsv"
    fields = list(rows[0].keys())
    with path.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=fields, delimiter="\t", lineterminator="\n")
        w.writeheader()
        w.writerows(rows)
    print("wrote", path, "rows", len(rows), file=sys.stderr)

    # perf TSV
    perf = []
    for zone, tiles in ZONES:
        for tile in tiles:
            o = official[tile]
            f = flags[tile]
            if not o:
                continue
            sma = o["sma"]
            deco_cap = 0
            fade_max_m = []
            for d in o["deco"]:
                mpq = d.get("MaxPerQuad") or 0
                # density map is typically heightmap-sized; 256x256 quads is the Interlude default sector grid
                try:
                    deco_cap += int(mpq) * 256 * 256
                except (TypeError, ValueError):
                    pass
                fr = d.get("FadeoutRadius") or {}
                if isinstance(fr, dict) and fr.get("Max") is not None:
                    fade_max_m.append(float(fr["Max"]) * UU_TO_M)
            # Static estimate: one draw call per unique mesh if instanced, else per actor, plus terrain, plus water, plus deco batches
            uninstanced_sma = sma["count"]
            instanced_sma = sma["unique_meshes"]
            perf.append({
                "zone": zone,
                "tile": tile,
                "in_fineage": "yes" if f["tile"] else "no",
                "official_sma": sma["count"],
                "unique_static_meshes": sma["unique_meshes"],
                "sma_with_baked_instance": sma["instances"],
                "sma_with_l2_lod": sma["l2_lod"],
                "sma_ignored_range": sma["ignored_range"],
                "terrain_layers": len(o["layers"]),
                "deco_layers": len(o["deco"]),
                "deco_maxperquad_x_256sq_cap": deco_cap,
                "deco_fadeout_max_m": ",".join(f"{x:.1f}" for x in fade_max_m),
                "water_volumes": o["water"],
                "lights": o["lights"],
                "est_draw_calls_if_sma_uninstanced": uninstanced_sma + len(o["layers"]) + max(len(o["deco"]), 0) + o["water"] + 1,
                "est_draw_calls_if_sma_instanced": instanced_sma + len(o["layers"]) + max(len(o["deco"]), 0) + o["water"] + 1,
                "fineage_deco_gpu_instances": "yes" if f["deco_instances"] else ("prefab_only" if f["deco_prefab"] else "no"),
                "fineage_sun_masks": "yes" if f["sun_masks"] else "no",
                "official_cull_distance_authored": 0,
            })
    ppath = WORLD / "render-perf.tsv"
    pf = list(perf[0].keys())
    with ppath.open("w", encoding="utf-8", newline="") as f:
        w = csv.DictWriter(f, fieldnames=pf, delimiter="\t", lineterminator="\n")
        w.writeheader()
        w.writerows(perf)
    print("wrote", ppath, file=sys.stderr)
    return rows, perf


def write_report(official, fine_t, fine_s, flags, rows, perf):
    lines = []
    w = lines.append
    w("# Interlude world render parity and static performance")
    w("")
    w("Issue: [#1673](https://github.com/FioraSolutions/Fineage/issues/1673).")
    w("Built on the lossless map dump from #1670 (`world/maps/*.unr.json.gz`).")
    w("Compared against Fineage Meta (`TerrainInfo0.txt`, `StaticMeshActor.json`, `.t3d`),")
    w("importers/bakers under `Assets/Scripts/Tools/Terrain` and `Assets/Editor/Terrain`,")
    w("and shipped tile assets. **No Unity session and no batchmode were run.**")
    w("UE2 property semantics come from the official actors and from Cruma's typed")
    w("TerrainInfo/DecorationLayer/StaticMeshActor readers — not from guessed lists.")
    w("")
    w("Derived tables: `render-parity-tiles.tsv`, `render-perf.tsv`, `render-parity-holes.tsv`.")
    w("")
    # totals
    def tiles_all():
        for z, ts in ZONES:
            for t in ts:
                yield z, t

    w("## Gap table")
    w("")
    w("| Category | What the original does | What Fineage does | Verdict | Fix |")
    w("|---|---|---|---|---|")
    w("| Terrain heightmap scale | `TerrainInfo.TerrainScale` tagged value is the UU size of one heightmap vertex (all play-first tiles: `(128, 128, 76)`). Heightmap is 256×256 (`GeneratedSectorCounter`=256). Fineage sizes the Unity terrain as `counter * TerrainScale` (height range 256×76 UU ≈ 370.6 m). Cruma's typed `getScale().z` is `76/256=0.296875` (internal per-unit). | Parser reads `TerrainMap`, `TerrainScale`, and `Location`. On shipped tiles the tagged scale vectors match. `GeneratedSectorCounter` is hard-coded to 256 (correct here, not parsed). | **Scale matches on shipped tiles.** Confirm imported height amplitude in a later Unity pass. | Keep dump-backed scale/location. Parse `GeneratedSectorCounter`. |")
    w("| Terrain layers | Each `TerrainLayer` has Texture, AlphaMap, U/VScale, U/VPan, `TextureRotation`, `LayerRotation`, `TextureMapAxis`, `bUseAlpha`, layer `Scale`, and `TerrainMatrix`. Play-first tiles: LayerRotation is 0; one non-zero TextureRotation on `22_23` (65066 UU); TextureMapAxis ≠ 0 on `22_22` and `22_21`. | Parser keeps Texture, AlphaMap, UScale, VScale, and WPlane X/Y as UV offset; drops rotation, axis, `bUseAlpha`, and layer Scale. **Meta drops real textures** vs the official actor (e.g. Gludio `19_21` missing Obase_1; Giran `22_22` missing INNS_05/INNS_06; `22_21` missing GUS05, GIS01, 22_21_map; Dion `20_22` missing DI_S4). Empty `none` slots are also omitted. | **Partial, and Meta is incomplete outside TI.** UV scale/offset imported; missing splat textures and dropped axis/rotation are visible bugs. | Rebuild layers from the official dump (every slot, including axis/rotation), not the thinned Meta text. |")
    w("| Terrain holes | Official `QuadVisibilityBitmap` is a 256×256 (65536-bit) mask of hidden quads. Cruma typed read (payload missing from the #1670 JSON, which only has `array_size=2048`): `19_21` 1191 hidden, `22_22` 3814, `20_22` 261, `21_22` 118, `16_25` 7, `21_21`/`21_23` 49, `19_19` 25. `EdgeTurnBitmap` flips triangulation (thousands of true bits; not holes). | Parser never reads either bitmap. TI Meta has the full 2048 words (and matches official hidden counts). **Meta outside TI only stores 64 words and 0 hidden** — the bitmaps were truncated. Unity terrain is a continuous surface. | **Missing, and Meta is unusable for holes outside TI.** Giran town and Gludio should show large cutouts (moat/water/buildings) and do not. | Import the official 65536-bit mask (Cruma `UBitSet` / re-dump) and apply Unity terrain holes; do not trust the 64-word Meta. |")
    w("| DecoLayers (grass/bushes) | Each layer has StaticMesh, DensityMap, optional ColorMap/ScaleMap, `ShowOnTerrain`, `FadeoutRadius` Min/Max (UU), `DensityMultiplier` Min/Max, `MaxPerQuad`, `Seed`, `AlignToTerrain`, `RandomYaw`, `ScaleMultiplier`, lighting flags. Example on `17_25`: Grass009 fade 300–1500 UU (5.7–28.6 m), MaxPerQuad=20, density 10–50. | Parser keeps ShowOnTerrain, DensityMap, StaticMesh, and ScaleMultiplier only. `GenerateDecoLayers` writes Unity detail layers with **raw density-texture R (0–255) and no MaxPerQuad / DensityMultiplier / FadeoutRadius**. GPU instancing is used for Unity terrain details; baked `*_DecoInstances.asset` exists only on `17_25` (and the lobby). Other shipped tiles have a `*_DecoLayer.prefab` only. | **Wrong density and draw distance.** Fade and per-quad caps are ignored; density mapping is not the official multiplier. | Drive deco placement and fade from the dump; cap at MaxPerQuad; instance every shipped tile the same way. |")
    w("| Trees / SpeedTree | No `SpeedTree` (or similar) actors in these tiles. Trees/grass are DecoLayers plus StaticMeshActors (e.g. Mother Tree). | No SpeedTree importer (correct). Tree-scale meshes go through the SMA path. | **N/A for SpeedTree.** Tree meshes follow the SMA gaps below. | — |")
    w("| StaticMeshActors — counts | Every `.unr` has a concrete SMA count (see the per-tile table). `17_25` official **986**. | `StaticMeshActor.json` **984** (−2). The `.t3d` has **981**. Other tiles similarly drift. | **Count mismatch** on shipped tiles. Missing actors are a visibility bug. | Rebuild the placement table from the #1670 dump (every SMA, no t3d/JSON thinning). |")
    w("| StaticMeshActors — transform | Official: `Location`, `Rotation`/`SwayRotationOrig`, `DrawScale` (only when ≠ 1; 258 on `17_25`), `DrawScale3D` (19 on `17_25`, including X=−1 mirrors). | Interlude bake reads `.t3d` (`SwayRotationOrig`, `DrawScale`, `DrawScale3D`). JSON used by the older builder has `yaw/pitch/roll` but `L2StaticMesh` deserializes `eulerAngles` — those fields do not bind. JSON also lacks `hasDrawScale3D`, so mirrors would take the wrong axis path if that importer is used. | **Transforms mostly present in t3d Meta, fragile in JSON.** Mirrors and any actor without `SwayRotationOrig` are at risk. | One dump-backed placement path; keep DrawScale / DrawScale3D / Rotation together. |")
    w("| StaticMeshActors — lighting & materials | 835/986 SMA on `17_25` reference a `StaticMeshInstance` (baked vertex / light color). `bUnlit`, `bShadowCast`, `bStaticLighting` are authored. Skins/CullDistance are rarely overridden on these starting tiles (0 on `17_25`). 207 actors have `StepSound_*`. | `StaticMeshSunMasks.json` exists only for `16_24`, `16_25`, `17_25`. Other starting-zone tiles have no sun-mask data in the tree. SMA materials are bound by `L2InterludeMaterialBinder` to URP Lit via a shortened UE2 shader chain. Unlit / shadow / static-lighting flags are not first-class on `L2StaticMesh`. | **Vertex lighting incomplete outside three TI tiles.** Material flags are flattened. | Import `StaticMeshInstance` color streams (or sun masks) for every tile; honour bUnlit / shadow flags from the dump. |")
    w("| StaticMeshActors — LOD / cull | Lineage-specific `L2LodViewDuration` + `L2CurrentLod` on 149 actors on `17_25`; `bIgnoredRange` on 15; `LODBias` rare. `CullDistance` is not authored on these tiles (engine default). | Those properties are not parsed or applied. Visibility work (#1591, #1609, #1652) is a separate frustum/far-object path, not the official per-actor LOD. | **Official LOD is unused.** | One visibility/LOD system driven by L2Lod* / bIgnoredRange / any future CullDistance, per #1652. |")
    w("| Water | `WaterVolume` brushes (TI village 1, Gludio `19_21` 9, Elven `21_19` 3, Dion `20_22` 2, Giran town `22_22` 8, Giran south `22_23` 13, harbour `21_23` 5). Materials, panning, envmap, and shorelines live on the volume — not as StaticMeshes. | `L2InterludeWaterVolumes` builds **one flat quad at max Z** per volume, shared `L2WaterSurface.mat` (URP Lit transparent, hard-coded tint, UV / 6 m). TI tiles ship `*_Mat_Water.mat`; some towns ship `*_Water.asset`. **No water asset on Giran harbour `21_23` (5 volumes), `22_23` (13), Dion `20_22` (2), `21_21` (9), `21_22` (3), `19_19` (4).** | **Approximated where present; missing on the harbour and several town tiles.** | Build water from dump bounds and official materials; ship a volume for every official WaterVolume. |")
    w("| Sky / fog / zone | `ZoneInfo.bDistanceFog` is on. Fog end is usually 15000 UU (285.7 m); Dion `20_22` is 1500–10000; Giran `22_22` has a 500–16000 zone; `22_21` ends at 20000; `21_21` at 10000. `SkyZoneInfo` has TexU/VPanSpeed and LensFlare. `NSun`/`NMoon` are present. | Runtime `DayNightCycle`, `L2CelestialDiscs`, and `L2WorldAmbientLUT` are global. Per-tile fog start/end/color and SkyZoneInfo panner/flare are not applied from the dump. | **Global LUT, not per-zone fog.** | Apply dump-backed ZoneInfo fog and SkyZoneInfo per loaded tile (with #1671). |")
    w("| Materials | UE2 `Shader` / `Combiner` / `FinalBlend` / `TexPanner` / `TexOscillator` / `TexEnvMap` / `TexScaler` chains. 748 committed `*.props.txt` under `Resources/Data/Textures` mention TexPanner 14, TexEnvMap 4, Combiner 2, TexOscillator 1 (plus many Shader slots). | `L2Ue2ShaderProps` classifies a chain as Opaque, MaskedCutout, TranslucentWater, or HiddenCollisionOnly, then `L2InterludeMaterialBinder` assigns a URP Lit `_BaseMap`. TexPanner/Combiner/EnvMap are not executed (except a one-off `L2VillageStatuePanner`). | **Diffuse + alpha only.** Animated and combined materials are static approximations. | A single material path that implements the UE2 ops from the props/dump, not per-mesh hacks. |")
    w("")
    w("## Per-tile comparison (play-first zones)")
    w("")
    w("Columns: official SMA / Fineage JSON (Δ) / t3d; terrain layers official→Meta; deco official→Meta; official hidden quads → Meta bitmap words; WaterVolume → shipped asset/mat; sun masks.")
    w("")
    for zone, tiles in ZONES:
        w(f"### {zone}")
        w("")
        w("| Tile | In Fineage | TerrainScale match | Layers (off→Meta) | Deco (off→Meta) | Hidden quads off→Meta words | SMA off→JSON (Δ) / t3d | Water off→shipped | Sun masks | L2 LOD / unlit / instances |")
        w("|---|---|---|---|---|---|---|---|---|---|")
        for tile in tiles:
            o, t, s, f = official[tile], fine_t[tile], fine_s[tile], flags[tile]
            if not o:
                w(f"| `{tile}` | no official `.unr` | — | — | — | — | — | — | — | — |")
                continue
            scale = "yes" if t.get("scale") and o["terrain_scale"] and vec_eq(o["terrain_scale"], t["scale"]) else ("n/a" if not t.get("present") else "no")
            layers = f"{len(o['layers'])}→{t.get('layers') if t.get('present') else '—'}"
            deco = f"{len(o['deco'])}→{t.get('deco') if t.get('present') else '—'}"
            hid = o.get("official_hidden_quads")
            holes = f"{hid if hid is not None else '—'}→{t.get('quad_words') or 0} words ({t.get('quad_hidden') or 0} hidden)"
            if s.get("json_present"):
                delta = (s.get("json_count") or 0) - o["sma"]["count"]
                delta_s = f"{delta:+d}"
            else:
                delta_s = "n/a"
            json_n = s.get("json_count") if s.get("json_present") else "—"
            t3d_n = s.get("t3d_sma") if s.get("t3d_present") else "—"
            if f["tile"] and (s.get("json_present") or s.get("t3d_present")):
                sma = f"{o['sma']['count']}→{json_n} ({delta_s}) / {t3d_n}"
            elif f["tile"]:
                sma = f"{o['sma']['count']}→no SMA meta"
            else:
                sma = f"{o['sma']['count']}→**no tile**"
            water_ship = "asset" if f.get("water_asset") else ("mat" if f.get("water_mat") else "no")
            water = f"{o['water']}→{water_ship}"
            sun = "yes" if f["sun_masks"] else "no"
            lod = f"{o['sma']['l2_lod']} / {o['sma']['unlit']} / {o['sma']['instances']}"
            inf = "yes" if f["tile"] else "**no**"
            w(f"| `{tile}` | {inf} | {scale} | {layers} | {deco} | {holes} | {sma} | {water} | {sun} | {lod} |")
        # zone notes
        notes = []
        for tile in tiles:
            o = official[tile]
            if not o:
                continue
            rots = sum(1 for ly in o["layers"] if isinstance(ly.get("LayerRotation"), dict) and any(int(ly["LayerRotation"].get(k, 0) or 0) for k in ("pitch", "yaw", "roll")))
            trot = sum(1 for ly in o["layers"] if isinstance(ly.get("TextureRotation"), (int, float)) and abs(float(ly["TextureRotation"])) > 1e-6)
            if rots or trot:
                notes.append(f"`{tile}` has {rots} layers with nonzero LayerRotation and {trot} with nonzero TextureRotation (dropped by the parser).")
            if o["deco"]:
                fades = []
                for d in o["deco"]:
                    fr = d.get("FadeoutRadius") or {}
                    if isinstance(fr, dict) and fr.get("Max") is not None:
                        fades.append(f"{ref_name(str(d.get('StaticMesh') or ''))} fade {fr.get('Min')}–{fr.get('Max')} UU ({float(fr['Max'])*UU_TO_M:.1f} m), MaxPerQuad={d.get('MaxPerQuad')}")
                if fades:
                    notes.append(f"`{tile}` deco: " + "; ".join(fades[:4]) + ("…" if len(fades) > 4 else ""))
            fogs = [z for z in o["zone_fog"] if z.get("distance_fog") is True or z.get("fog_end")]
            if fogs:
                bits = []
                for z in fogs[:3]:
                    bits.append(f"{z['class']} fogEnd={z.get('fog_end')} color={z.get('fog_color')}")
                notes.append(f"`{tile}` " + "; ".join(bits))
        if notes:
            w("")
            for n in notes:
                w(f"- {n}")
        w("")
    w("## Static performance estimates (no Unity timings)")
    w("")
    w("These are **inventory estimates** from official actor counts and Fineage shipped assets.")
    w("They are not p50/p95 frame times. Draw-call columns assume one call per uninstanced SMA")
    w("plus one per terrain layer, deco layer, water volume, and the terrain surface.")
    w("UE `CullDistance` is not authored on these starting tiles; Lineage `L2LodViewDuration` is.")
    w("")
    w("| Zone | Tile | SMA (unique meshes) | Est. draw calls uninstanced → instanced SMA | Deco layers (fade max m) | Fineage deco instancing | Sun masks | Lights |")
    w("|---|---|---|---|---|---|---|---:|")
    for p in perf:
        fade = p["deco_fadeout_max_m"] or "—"
        w(f"| {p['zone']} | `{p['tile']}` | {p['official_sma']} ({p['unique_static_meshes']}) | {p['est_draw_calls_if_sma_uninstanced']} → {p['est_draw_calls_if_sma_instanced']} | {p['deco_layers']} ({fade}) | {p['fineage_deco_gpu_instances']} | {p['fineage_sun_masks']} | {p['lights']} |")
    w("")
    w("### Bottlenecks (architectural, from the data)")
    w("")
    w("1. **Uninstanced StaticMeshActors** dominate the estimate. Giran `21_23` / `21_21` and Elven `21_19` have thousands of SMA. Fineage ships a `StaticMeshes.prefab` of individual objects; only deco on `17_25` is GPU-instanced via `DecoInstances`. One instancing path for SMA (grouped by mesh+material) is the fix, not per-tile hacks.")
    w("2. **Official L2 LOD is unused.** Hundreds of actors carry `L2LodViewDuration` / `L2CurrentLod` / `bIgnoredRange`. Without them, far meshes stay in the same bucket as the village street (see #1652, #1609).")
    w("3. **Deco overdraw / wrong fade.** Official fade radii are a few tens of metres; Unity detail distance or an unfaded prefab will either clip grass too early or keep it to the horizon. Density without MaxPerQuad can explode instance counts (cap is `MaxPerQuad × 256²` per layer — an upper bound, not the painted count).")
    w("4. **Lights.** Giran harbour `21_23` has 2378 official Light actors and `21_21` has 1749. Baking/sun masks exist only on three TI tiles (`16_24`, `16_25`, `17_25`).")
    w("5. **Water and materials** are cheap but wrong (one shared transparent quad / URP Lit). Not the main CPU cost; they are a parity cost.")
    w("")
    w("## Unity frame-time measurements (not run)")
    w("")
    w("Nikita may have the editor open on `~/Projects/Fineage`. This pass did **not** start Unity or batchmode.")
    w("When the Mac is free, measure p50/p95 frame time, draw calls, SetPass, tris, batches, CPU/GPU, and memory on this reference route, with the same quality settings, after a 10 s settle:")
    w("")
    w("1. Open the worktree (or a copy), not a dirty playtest checkout you care about, in Unity 6000.3.x.")
    w("2. Enable the Stats / Profiler (CPU, GPU, Memory, Rendering) and the Frame Debugger.")
    w("3. Walk or fly the route, 20+ seconds each, recording Profiler markers:")
    w("   - Talking Island village, tile `17_25` (Cedric / harbour approach).")
    w("   - Town of Gludio, tile `19_21`.")
    w("   - Elven Village / Mother Tree, tile `21_19`.")
    w("   - Dion fields / town, tile `20_22`.")
    w("   - Giran harbour, tile `21_23`, and Giran town `22_22`.")
    w("4. Capture a screenshot of the Rendering profiler at each stop and append numbers to `render-perf.tsv` (p50/p95 ms, draw calls, SetPass, tris, used-memory).")
    w("5. Note whether GPU instancing, terrain pixel error, shadow distance, and texture streaming are on, and whether SMA are instanced or individual MeshRenderers.")
    w("")
    w("## Files")
    w("")
    w("- `render-parity.md` (this file)")
    w("- `render-parity-tiles.tsv` — per-tile official vs Fineage counts")
    w("- `render-perf.tsv` — static draw-call / mesh / deco estimates")
    w("- `render-parity-holes.tsv` — official 256×256 QuadVisibility hidden-quad counts (Cruma typed `UBitSet`; the #1670 JSON omits the array payload)")
    w("- Regenerator for holes: `tools/interlude-extract/DumpTerrainHoles.java`")
    w("- Official actor dumps remain in `world/maps/` from #1670")
    w("- Regenerator: `tools/interlude-extract/build_render_parity.py`")
    w("")
    path = WORLD / "render-parity.md"
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")
    print("wrote", path, "lines", len(lines), file=sys.stderr)


def main() -> int:
    official, fine_t, fine_s, flags = load_all()
    rows, perf = write_tables(official, fine_t, fine_s, flags)
    write_report(official, fine_t, fine_s, flags, rows, perf)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
