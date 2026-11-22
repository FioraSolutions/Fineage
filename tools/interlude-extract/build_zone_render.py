#!/usr/bin/env python3
"""Derived ZoneInfo / SkyZoneInfo / NSun view over the lossless map dumps.

Reads l2-unity/docs/interlude-ref/world/maps/*.unr.json.gz (tiles) and
l2-unity/docs/interlude-ref/world/env/*.unr.json.gz (Lobby, entry, skylevel,
ship_position). Does not read .unr. An empty cell means the property was not
authored.
"""
from __future__ import annotations

import gzip
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
WORLD = ROOT / "l2-unity" / "docs" / "interlude-ref" / "world"
MAPS = WORLD / "maps"
ENV = WORLD / "env"
DOCS = ROOT / "l2-unity" / "docs" / "interlude-ref" / "world" / "zone-render.tsv"
RUNTIME = ROOT / "l2-unity" / "Assets" / "StreamingAssets" / "Data" / "Meta" / "World" / "zone-render.tsv"
NOTE = ROOT / "l2-unity" / "docs" / "interlude-ref" / "world" / "zone-render.md"

CLASSES = ("ZoneInfo", "LevelInfo", "SkyZoneInfo", "NSun", "NMoon", "NMovableSunLight")
HEADER = [
    "map", "class", "name", "zone_tag", "b_distance_fog", "b_terrain_zone",
    "fog_start", "fog_end", "fog_r", "fog_g", "fog_b", "fog_a",
    "tex_u_pan", "tex_v_pan", "lens_flares", "lens_offsets", "lens_scales",
    "radius", "limit_max_radius", "b_directional", "draw_scale", "light_brightness",
    "rot_pitch", "rot_yaw", "rot_roll",
]


def cell_float(value) -> str:
    return str(value)


def unsigned_byte(value: int) -> str:
    return str(int(value) & 255)


def first(props: dict, name: str):
    items = props.get(name)
    return items[0] if items else None


def collect(export: dict) -> dict:
    grouped: dict[str, list] = {}
    for prop in export.get("properties") or []:
        grouped.setdefault(prop.get("name"), []).append(prop)
    for items in grouped.values():
        items.sort(key=lambda p: int(p.get("array_index") or 0))
    return grouped


def bool_cell(props: dict, name: str) -> str:
    prop = first(props, name)
    if prop is None or "value" not in prop:
        return ""
    return "1" if prop["value"] else "0"


def float_cell(props: dict, name: str) -> str:
    prop = first(props, name)
    if prop is None or "value" not in prop:
        return ""
    return cell_float(prop["value"])


def name_cell(props: dict, name: str) -> str:
    prop = first(props, name)
    if prop is None:
        return ""
    return str(prop.get("value") or "")


def color_cells(props: dict) -> tuple[str, str, str, str]:
    prop = first(props, "DistanceFogColor")
    if prop is None or not isinstance(prop.get("color"), dict):
        return "", "", "", ""
    color = prop["color"]
    return tuple(unsigned_byte(color.get(ch, 0)) for ch in ("r", "g", "b", "a"))


def flare_names(props: dict) -> str:
    names = []
    for prop in props.get("LensFlare") or []:
        ref = str(prop.get("ref") or "")
        parts = ref.split(":")
        if len(parts) >= 4 and parts[1] and parts[-1]:
            names.append(parts[1] + "." + parts[-1])
        elif ref:
            names.append(ref)
    return "|".join(names)


def float_list(props: dict, name: str) -> str:
    values = []
    for prop in props.get(name) or []:
        if "value" in prop:
            values.append(cell_float(prop["value"]))
    return "|".join(values)


def rotator_cells(props: dict) -> tuple[str, str, str]:
    prop = first(props, "Rotation")
    if prop is None or not isinstance(prop.get("rotator"), dict):
        return "", "", ""
    rot = prop["rotator"]
    return tuple(str(int(rot.get(ch, 0))) for ch in ("pitch", "yaw", "roll"))


def row_for(map_name: str, export: dict) -> list[str]:
    props = collect(export)
    r, g, b, a = color_cells(props)
    pitch, yaw, roll = rotator_cells(props)
    return [
        map_name,
        export.get("class") or "",
        export.get("name") or "",
        name_cell(props, "ZoneTag"),
        bool_cell(props, "bDistanceFog"),
        bool_cell(props, "bTerrainZone"),
        float_cell(props, "DistanceFogStart"),
        float_cell(props, "DistanceFogEnd"),
        r, g, b, a,
        float_cell(props, "TexUPanSpeed"),
        float_cell(props, "TexVPanSpeed"),
        flare_names(props),
        float_list(props, "LensFlareOffset"),
        float_list(props, "LensFlareScale"),
        float_cell(props, "Radius"),
        float_cell(props, "LimitMaxRadius"),
        bool_cell(props, "bDirectional"),
        float_cell(props, "DrawScale"),
        float_cell(props, "LightBrightness"),
        pitch, yaw, roll,
    ]


def main() -> None:
    rows = []
    for folder in (MAPS, ENV):
        for path in sorted(folder.glob("*.unr.json.gz")):
            map_name = path.name[: -len(".unr.json.gz")]
            with gzip.open(path, "rt", encoding="utf-8") as handle:
                data = json.load(handle, parse_float=str)
            for export in data.get("exports") or []:
                if export.get("class") not in CLASSES:
                    continue
                rows.append(row_for(map_name, export))
    rows.sort(key=lambda r: (r[0], r[1], r[2]))
    body = "\t".join(HEADER) + "\n" + "".join("\t".join(r) + "\n" for r in rows)
    DOCS.parent.mkdir(parents=True, exist_ok=True)
    DOCS.write_text(body, encoding="utf-8")
    RUNTIME.parent.mkdir(parents=True, exist_ok=True)
    RUNTIME.write_text(body, encoding="utf-8")
    NOTE.write_text(
        "\n".join([
            "# Zone render catalog",
            "",
            "Derived from `world/maps/*.unr.json.gz` and `world/env/*.unr.json.gz`.",
            "The gzip dumps stay the lossless source.",
            "An empty cell means the property was not authored on that actor.",
            "Engine.u `ZoneInfo` defaultproperties are not serialized in the map, so they are",
            "not copied into this catalog. When `fog_start` is empty the runtime applies the",
            "class default `DistanceFogStart=3000`.",
            "",
            "Columns: " + ", ".join(f"`{name}`" for name in HEADER) + ".",
            "",
            "`NSun` rotation and `NMovableSunLight` brightness stay in the table.",
            "Disc direction and color stay on the day/night LUT. `Radius` is a light radius;",
            "the runtime scales the captured disc by `tileRadius / catalogMode`.",
            "`Engine.Flare*` textures are not in Resources. A missing flare is logged by name.",
            "",
        ]),
        encoding="utf-8",
    )
    print(f"rows {len(rows)}")


if __name__ == "__main__":
    main()
