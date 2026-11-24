#!/usr/bin/env python3
"""Composite a window from the generated layout and the extracted textures."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(Path(__file__).resolve().parent))

from interlude_layout import compile_docs, resolve_tree  # noqa: E402

DOCS = ROOT / "l2-unity/docs/ui-original"
RESOURCES = ROOT / "l2-unity/Assets/Resources"


def _font(size: int) -> ImageFont.ImageFont:
    for path in (
        str(RESOURCES / "Data/UI/Font/tahoma.ttf"),
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
    ):
        if Path(path).is_file():
            return ImageFont.truetype(path, size)
    return ImageFont.load_default()


def render_window(document: dict, name: str) -> Image.Image:
    windows = {window["name"]: window for window in document["windows"]}
    if name not in windows:
        raise SystemExit(f"window '{name}' is not in the layout")
    window = windows[name]
    rects = resolve_tree(window)
    root = rects[window["name"]]
    width = int(root[2] or 1)
    height = int(root[3] or 1)
    image = Image.new("RGBA", (width, height), (0, 0, 0, 0))
    draw = ImageDraw.Draw(image)
    font = _font(9)

    def paste(control_name: str, slot: str, local: tuple[float, float, float | None, float | None]) -> None:
        control = next(item for item in window["controls"] if item["name"] == control_name)
        path = control.get(slot) or ""
        if not path:
            return
        file_path = RESOURCES / f"{path}.png"
        if not file_path.is_file():
            raise SystemExit(f"{name}/{control_name} missing texture file {file_path}")
        left, top, rect_w, rect_h = local
        if rect_w is None or rect_h is None or rect_w <= 0 or rect_h <= 0:
            return
        sprite = Image.open(file_path).convert("RGBA")
        sprite = sprite.resize((max(1, int(rect_w)), max(1, int(rect_h))), Image.Resampling.NEAREST)
        image.alpha_composite(sprite, (int(left), int(top)))

    # Parent-space rects are converted to window space by walking parents.
    absolute: dict[str, tuple[float, float, float | None, float | None]] = {}

    def abs_rect(control_name: str) -> tuple[float, float, float | None, float | None]:
        if control_name in absolute:
            return absolute[control_name]
        control = next(item for item in window["controls"] if item["name"] == control_name)
        local = rects[control_name]
        if not control["parent"]:
            absolute[control_name] = local
            return local
        parent = abs_rect(control["parent"])
        placed = (parent[0] + local[0], parent[1] + local[1], local[2], local[3])
        absolute[control_name] = placed
        return placed

    for control in window["controls"]:
        placed = abs_rect(control["name"])
        paste(control["name"], "textureBack", placed)
        paste(control["name"], "texture", placed)
        if control["text"] and placed[2] and placed[3] and placed[2] > 0 and placed[3] > 0:
            draw.text((placed[0] + 2, placed[1] + 1), control["text"], fill=(255, 255, 255, 255), font=font)
    return image


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("windows", nargs="+")
    parser.add_argument("-o", type=Path, required=True)
    args = parser.parse_args()
    document = compile_docs(DOCS, DOCS / "textures-map.json")
    args.o.mkdir(parents=True, exist_ok=True)
    for name in args.windows:
        image = render_window(document, name)
        out = args.o / f"{name}.png"
        image.save(out)
        print(out, image.size)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
