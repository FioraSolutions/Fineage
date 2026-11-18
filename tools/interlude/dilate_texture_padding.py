#!/usr/bin/env python3
"""Fill the RGB under alpha=0 with the nearest visible colour (alpha unchanged).

Why: an L2 texture stores the coverage mask in alpha and leaves the padding black.
At mip level 2 and below the black padding averages into the silhouette, so every
weapon edge darkens with distance. Measured on soul_separator_t00_wp: 50% of edge
texels lose more than 15 luma at mip2, 78% at mip5. Character textures already
ship dilated (FDwarf_m002_t42_u is 77% filled); the weapon atlas never was.

Alpha is copied through untouched, so an alpha-clipped material cuts the same
pixels before and after. Only the invisible RGB changes.

    python3 tools/interlude/dilate_texture_padding.py <png> [<png> ...]
    python3 tools/interlude/dilate_texture_padding.py --check <png> [...]
"""
import sys
from collections import deque

try:
    import numpy as np
    from PIL import Image
except ImportError:  # pragma: no cover - the tools venv has both
    sys.exit("needs numpy and pillow: python3 -m venv /tmp/texvenv && /tmp/texvenv/bin/pip install numpy pillow")


def fill(rgba: np.ndarray) -> tuple[np.ndarray, int]:
    """Multi-source BFS outward from visible texels into the padding."""
    out = rgba.copy()
    alpha = rgba[:, :, 3]
    visible = alpha > 0
    if not visible.any() or visible.all():
        return out, 0

    h, w = alpha.shape
    filled = visible.copy()
    queue = deque()
    ys, xs = np.nonzero(visible)
    for y, x in zip(ys.tolist(), xs.tolist()):
        queue.append((y, x))
    painted = 0
    while queue:
        y, x = queue.popleft()
        src = out[y, x, :3]
        for dy, dx in ((1, 0), (-1, 0), (0, 1), (0, -1)):
            ny, nx = y + dy, x + dx
            if 0 <= ny < h and 0 <= nx < w and not filled[ny, nx]:
                out[ny, nx, :3] = src
                filled[ny, nx] = True
                painted += 1
                queue.append((ny, nx))
    return out, painted


def is_filled(rgba: np.ndarray) -> bool:
    alpha = rgba[:, :, 3]
    pad = alpha == 0
    if not pad.any():
        return True
    return bool(rgba[:, :, :3][pad].max() > 0)


def main(argv: list[str]) -> int:
    check = "--check" in argv
    paths = [a for a in argv if not a.startswith("--")]
    if not paths:
        print(__doc__)
        return 2

    dirty = 0
    for path in paths:
        with Image.open(path) as image:
            if image.mode != "RGBA":
                print(f"skip {path} ({image.mode}, no coverage alpha)")
                continue
            rgba = np.array(image)
            if check:
                if not is_filled(rgba):
                    print(f"BLACK PADDING {path}")
                    dirty += 1
                continue
            filled, painted = fill(rgba)
            if painted == 0:
                continue
            Image.fromarray(filled, mode="RGBA").save(path)
            print(f"filled {painted:7d} padding texels  {path}")

    if check:
        print(f"\n{dirty} texture(s) still have black padding under alpha=0")
        return 1 if dirty else 0
    return 0


if __name__ == "__main__":
    raise SystemExit(main(sys.argv[1:]))
