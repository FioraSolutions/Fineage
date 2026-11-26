#!/usr/bin/env python3
"""Side-by-side MinimapWnd: ui-original layout render vs Fineage WindowLayouts.json."""

from __future__ import annotations

import json
import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(Path(__file__).resolve().parent))

from interlude_layout import compile_docs, resolve_tree  # noqa: E402
from render_window_layout import render_window  # noqa: E402

DOCS = ROOT / "l2-unity/docs/ui-original"
RESOURCES = ROOT / "l2-unity/Assets/Resources"
RUNTIME_LAYOUT = RESOURCES / "Data/UI/Interlude/WindowLayouts.json"
BOUNDS_JSON = ROOT / "l2-unity/Assets/StreamingAssets/Data/Map/world_map_interlude.json"

# Elven Village gatekeeper band (EditMode anchor).
SAMPLE_L2_X = 46890.0
SAMPLE_L2_Y = 51531.0


def _font(size: int) -> ImageFont.ImageFont:
    for path in (
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
    ):
        if Path(path).is_file():
            return ImageFont.truetype(path, size)
    return ImageFont.load_default()


def load_bounds() -> dict:
    return json.loads(BOUNDS_JSON.read_text(encoding="utf-8"))


def project_map_pixel(l2_x: float, l2_y: float, bounds: dict) -> tuple[float, float]:
    nx = (l2_x - bounds["l2MinX"]) / (bounds["l2MaxX"] - bounds["l2MinX"])
    ny = (l2_y - bounds["l2MinY"]) / (bounds["l2MaxY"] - bounds["l2MinY"])
    return (
        nx * bounds["pixelWidth"],
        ny * bounds["pixelHeight"],
    )


def composite_map_chrome(
    chrome: Image.Image,
    window: dict,
    bounds: dict,
    l2_x: float,
    l2_y: float,
) -> Image.Image:
    rects = resolve_tree(window)
    minimap = rects["Minimap"]
    window_rect = rects[window["name"]]

    def to_window_space(local: tuple[float, float, float | None, float | None]):
        parent = window["name"]

        def abs_rect(name: str):
            if name == parent:
                return rects[name]
            control = next(c for c in window["controls"] if c["name"] == name)
            local_r = rects[name]
            if not control["parent"]:
                return local_r
            p = abs_rect(control["parent"])
            return (p[0] + local_r[0], p[1] + local_r[1], local_r[2], local_r[3])

        return abs_rect("Minimap")

    hole = to_window_space(minimap)
    left, top, w, h = (int(hole[0]), int(hole[1]), int(hole[2] or 1), int(hole[3] or 1))

    world_path = RESOURCES / "Data/UI/Map/WorldMap_Black.png"
    pin_path = RESOURCES / "Data/UI/Map/MinimapPos.png"
    if not world_path.is_file():
        raise SystemExit(f"missing {world_path}")
    if not pin_path.is_file():
        raise SystemExit(f"missing {pin_path}")

    world = Image.open(world_path).convert("RGBA")
    pin = Image.open(pin_path).convert("RGBA")

    map_px, map_py = project_map_pixel(l2_x, l2_y, bounds)
    scale_x = w / bounds["pixelWidth"]
    scale_y = h / bounds["pixelHeight"]
    marker_x = map_px * scale_x
    marker_y = map_py * scale_y

    view_w, view_h = w, h
    pan_x = max(0.0, min(marker_x - view_w / 2, world.width * scale_x - view_w))
    pan_y = max(0.0, min(marker_y - view_h / 2, world.height * scale_y - view_h))

    crop_left = int(pan_x / scale_x)
    crop_top = int(pan_y / scale_y)
    crop_right = int((pan_x + view_w) / scale_x)
    crop_bottom = int((pan_y + view_h) / scale_y)
    cropped = world.crop((crop_left, crop_top, crop_right, crop_bottom))
    viewport = cropped.resize((w, h), Image.Resampling.BILINEAR)

    out = chrome.copy()
    out.alpha_composite(viewport, (left, top))

    pin_w, pin_h = pin.size
    pin_left = int(left + marker_x - pan_x - pin_w / 2)
    pin_top = int(top + marker_y - pan_y - pin_h / 2)
    out.alpha_composite(pin, (pin_left, pin_top))
    return out


def window_dict_from_runtime(name: str) -> dict:
    document = json.loads(RUNTIME_LAYOUT.read_text(encoding="utf-8"))
    for window in document["windows"]:
        if window["name"] == name:
            return window
    raise SystemExit(f"{name} missing from WindowLayouts.json")


def main() -> int:
    import argparse

    parser = argparse.ArgumentParser()
    parser.add_argument(
        "-o",
        type=Path,
        default=ROOT / "l2-unity/docs/research/world-map-1638-original-vs-fineage.png",
    )
    args = parser.parse_args()

    bounds = load_bounds()
    original_doc = compile_docs(DOCS, DOCS / "textures-map.json")
    orig_window = next(w for w in original_doc["windows"] if w["name"] == "MinimapWnd")
    fine_window = window_dict_from_runtime("MinimapWnd")

    orig_chrome = render_window(original_doc, "MinimapWnd")
    fine_chrome = render_window({"windows": [fine_window]}, "MinimapWnd")

    orig = composite_map_chrome(orig_chrome, orig_window, bounds, SAMPLE_L2_X, SAMPLE_L2_Y)
    fine = composite_map_chrome(fine_chrome, fine_window, bounds, SAMPLE_L2_X, SAMPLE_L2_Y)

    pad = 12
    label_h = 28
    out_w = orig.width + fine.width + pad * 3
    out_h = max(orig.height, fine.height) + label_h + pad * 2
    sheet = Image.new("RGB", (out_w, out_h), (20, 20, 24))
    draw = ImageDraw.Draw(sheet)
    font = _font(15)

    draw.text(
        (pad, 4),
        "Interlude (ui-original + textures-map)",
        fill=(220, 220, 220),
        font=font,
    )
    draw.text(
        (orig.width + pad * 2, 4),
        "Fineage (WindowLayouts.json runtime asset)",
        fill=(220, 220, 220),
        font=font,
    )
    sheet.paste(orig.convert("RGB"), (pad, label_h + pad), orig if orig.mode == "RGBA" else None)
    sheet.paste(fine.convert("RGB"), (pad * 2 + orig.width, label_h + pad), fine if fine.mode == "RGBA" else None)

    caption = (
        f"Player pin @ L2 ({SAMPLE_L2_X:.0f}, {SAMPLE_L2_Y:.0f}) "
        f"→ map px ({project_map_pixel(SAMPLE_L2_X, SAMPLE_L2_Y, bounds)[0]:.0f}, "
        f"{project_map_pixel(SAMPLE_L2_X, SAMPLE_L2_Y, bounds)[1]:.0f})"
    )
    draw.text((pad, out_h - 22), caption, fill=(160, 160, 160), font=_font(12))

    args.o.parent.mkdir(parents=True, exist_ok=True)
    sheet.save(args.o)
    print(args.o, sheet.size)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())