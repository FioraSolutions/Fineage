#!/usr/bin/env python3
"""Resolve a map's TerrainMap to the package it names, and prefer the level's vertex grid.

UE2 TerrainInfo.TerrainMap is an object reference. A negative index is an import
(a texture in another package). A positive index is an export in the map itself.
The height file is loaded from that package only. A same-named texture in a
different package is an error, not a fallback.

The map's TerrainInfo export also carries a native vertex grid (256*256 vectors
when the heightmap is 256). When that grid is present and its heights are not
the imported G16, those vertices are the surface the level saved.
"""
from __future__ import annotations

import argparse
import array
import gzip
import hashlib
import json
import struct
import sys
from dataclasses import dataclass
from pathlib import Path


@dataclass(frozen=True)
class TerrainMapRef:
    package: str
    group: str
    name: str
    kind: str
    index: int
    qualified: str

    @property
    def in_level(self) -> bool:
        return self.kind == "export"


def _walk_import(doc: dict, index: int) -> list[str]:
    """UE2 import index is negative. -1 is imports[0]."""
    seen: set[int] = set()
    parts: list[str] = []
    cur = index
    while cur and cur not in seen:
        seen.add(cur)
        if cur > 0:
            exp = doc["exports"][cur - 1]
            parts.append(exp.get("name") or "")
            cur = exp.get("outer_index") or 0
            continue
        imp = doc["imports"][-cur - 1]
        parts.append(imp.get("object_name") or "")
        cur = imp.get("package_index") or 0
    parts.reverse()
    return parts


def _object_path(doc: dict, index: int) -> tuple[str, str]:
    if not index:
        raise SystemExit("ERROR TerrainMap index is 0 (None)")
    if index > 0:
        exp = doc["exports"][index - 1]
        parts = [doc.get("package") or "", exp.get("name") or ""]
        outer = exp.get("outer_index") or 0
        group_parts: list[str] = []
        seen: set[int] = set()
        while outer and outer not in seen:
            seen.add(outer)
            if outer > 0:
                parent = doc["exports"][outer - 1]
                group_parts.append(parent.get("name") or "")
                outer = parent.get("outer_index") or 0
            else:
                group_parts.extend(reversed(_walk_import(doc, outer)))
                break
        group_parts.reverse()
        qualified_parts = [p for p in [parts[0], *group_parts, parts[1]] if p]
        return "export", ".".join(qualified_parts)
    return "import", ".".join(_walk_import(doc, index))


def _split_qualified(qualified: str) -> tuple[str, str, str]:
    parts = qualified.split(".")
    if len(parts) < 2:
        raise SystemExit(f"ERROR TerrainMap path {qualified!r} has no package.name")
    package, name = parts[0], parts[-1]
    group = ".".join(parts[1:-1])
    return package, group, name


def terrain_maps(doc: dict) -> list[TerrainMapRef]:
    out: list[TerrainMapRef] = []
    for exp in doc.get("exports") or []:
        if exp.get("class") != "TerrainInfo":
            continue
        prop = next((p for p in exp.get("properties") or [] if p.get("name") == "TerrainMap"), None)
        if prop is None:
            raise SystemExit(
                f"ERROR TerrainInfo {exp.get('name')} has no TerrainMap property"
            )
        index = int(prop.get("index") or 0)
        kind, qualified = _object_path(doc, index)
        package, group, name = _split_qualified(qualified)
        out.append(TerrainMapRef(package, group, name, kind, index, qualified))
    return out


def one_terrain_map(doc: dict) -> TerrainMapRef:
    refs = terrain_maps(doc)
    if len(refs) != 1:
        names = ", ".join(r.qualified for r in refs) or "(none)"
        raise SystemExit(f"ERROR expected one TerrainMap, found {len(refs)}: {names}")
    return refs[0]


def load_doc(path: Path) -> dict:
    raw = path.read_bytes()
    if path.suffix == ".gz" or raw[:2] == b"\x1f\x8b":
        raw = gzip.decompress(raw)
    return json.loads(raw)


def assert_height_package(ref: TerrainMapRef, opened_package: str, where: str) -> None:
    """The height file's package must be the package TerrainMap names."""
    if ref.package.lower() != (opened_package or "").lower():
        raise SystemExit(
            f"ERROR TerrainMap {ref.qualified} index {ref.index} ({ref.kind}) "
            f"is package {ref.package}, resolved to {opened_package} at {where}"
        )


def _floats(data: bytes, align: int) -> array.array:
    usable = data[align:]
    usable = usable[: len(usable) // 4 * 4]
    out = array.array("f")
    out.frombytes(usable)
    return out


def find_vertex_grid(data: bytes, scale_x: float, scale_y: float, n: int = 256) -> tuple[int, int, int] | None:
    """Return (byte_align, float_index, y_sign) of an n*n X-major vector grid.

    y_sign is +1 when the next row's Y is scale_y higher, -1 when it is lower.
    """
    need = 3 * n * n
    for align in range(4):
        floats = _floats(data, align)
        limit = len(floats) - need
        if limit < 0:
            continue
        i = 0
        while i <= limit:
            x0 = floats[i]
            y0 = floats[i + 1]
            x1 = floats[i + 3]
            y1 = floats[i + 4]
            if abs((x1 - x0) - scale_x) > 0.05 or abs(y1 - y0) > 0.05:
                i += 1
                continue
            if abs((floats[i + 6] - x1) - scale_x) > 0.05:
                i += 1
                continue
            row_ok = True
            for k in (1, n // 2, n - 1):
                base = i + 3 * k
                if abs(floats[base] - (x0 + k * scale_x)) > 0.05 or abs(floats[base + 1] - y0) > 0.05:
                    row_ok = False
                    break
            if not row_ok:
                i += 1
                continue
            row2 = i + 3 * n
            dy = floats[row2 + 1] - y0
            if abs(dy - scale_y) <= 0.05:
                sign = 1
            elif abs(dy + scale_y) <= 0.05:
                sign = -1
            else:
                i += 1
                continue
            last = i + 3 * (n - 1 + (n - 1) * n)
            expect_y = y0 + sign * (n - 1) * scale_y
            if abs(floats[last + 1] - expect_y) > 0.05:
                i += 1
                continue
            return align, i, sign
    return None


def grid_samples(
    data: bytes,
    align: int,
    start: int,
    n: int,
    y_sign: int,
    loc_z: float,
    scale_z: float,
    world_z: bool,
) -> list[int]:
    """uint16 height samples, row 0 = the row whose Y is the minimum.

    world_z: vertex Z is a world coordinate (subtract the actor Location.Z).
    Otherwise vertex Z is already relative to that location.
    """
    floats = _floats(data, align)
    z_per = scale_z / 256.0
    rows: list[list[int]] = []
    for y in range(n):
        row: list[int] = []
        for x in range(n):
            z = floats[start + 3 * (x + y * n) + 2]
            if world_z:
                z -= loc_z
            sample = z / z_per + 32768.0
            row.append(int(round(sample)))
        rows.append(row)
    if y_sign < 0:
        rows.reverse()
    flat: list[int] = []
    for row in rows:
        flat.extend(row)
    return flat


def grid_origin(data: bytes, align: int, start: int, n: int, y_sign: int) -> tuple[float, float]:
    floats = _floats(data, align)
    if y_sign > 0:
        return float(floats[start]), float(floats[start + 1])
    last = start + 3 * (n - 1) * n
    return float(floats[last]), float(floats[last + 1])


def read_bmp_samples(path: Path) -> tuple[int, list[int]]:
    """Top-origin row-major uint16. File row 0 of a bottom-up BMP is the last image row."""
    blob = path.read_bytes()
    if blob[:2] != b"BM":
        raise SystemExit(f"ERROR {path} is not a BMP")
    off = struct.unpack_from("<I", blob, 10)[0]
    w, h = struct.unpack_from("<ii", blob, 18)
    bpp = struct.unpack_from("<H", blob, 28)[0]
    if bpp != 16 or w != h or w <= 0:
        raise SystemExit(f"ERROR {path} is {w}x{h} bpp {bpp}, want a square 16-bit BMP")
    stride = ((w * bpp + 31) // 32) * 4
    samples = [0] * (w * h)
    for file_row in range(h):
        image_row = h - 1 - file_row
        base = off + file_row * stride
        for x in range(w):
            samples[image_row * w + x] = struct.unpack_from("<H", blob, base + x * 2)[0]
    return w, samples


def write_bmp(path: Path, n: int, samples: list[int]) -> None:
    if len(samples) != n * n:
        raise SystemExit(f"ERROR height sample count {len(samples)} != {n}*{n}")
    row_bytes = n * 2
    padded = (row_bytes + 3) & ~3
    header = bytearray(54)
    header[0:2] = b"BM"
    struct.pack_into("<I", header, 2, 54 + padded * n)
    struct.pack_into("<I", header, 10, 54)
    struct.pack_into("<I", header, 14, 40)
    struct.pack_into("<i", header, 18, n)
    struct.pack_into("<i", header, 22, n)
    struct.pack_into("<H", header, 26, 1)
    struct.pack_into("<H", header, 28, 16)
    out = bytearray(header)
    pad = b"\x00" * (padded - row_bytes)
    for image_row in range(n - 1, -1, -1):
        for x in range(n):
            out += struct.pack("<H", samples[image_row * n + x] & 0xFFFF)
        out += pad
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_bytes(out)


def _median(values: list[float]) -> float:
    if not values:
        return 0.0
    ordered = sorted(values)
    mid = len(ordered) // 2
    if len(ordered) % 2:
        return ordered[mid]
    return 0.5 * (ordered[mid - 1] + ordered[mid])


def compare_samples(current: list[int], proposed: list[int], z_per: float) -> tuple[float, int]:
    """Median |dZ| in UU, and how many texels differ by more than 30 UU."""
    diffs = [abs(a - b) * z_per for a, b in zip(current, proposed)]
    outliers = sum(1 for d in diffs if d > 30.0)
    return _median(diffs), outliers


def g16_as_top_origin(raw: bytes, n: int, flip_y: bool) -> list[int]:
    if len(raw) != n * n * 2:
        raise SystemExit(f"ERROR G16 raw is {len(raw)} bytes, want {n * n * 2}")
    samples = list(struct.unpack("<" + "H" * (n * n), raw))
    if not flip_y:
        return samples
    rows = [samples[y * n : (y + 1) * n] for y in range(n)]
    rows.reverse()
    flat: list[int] = []
    for row in rows:
        flat.extend(row)
    return flat


def actor_location(doc: dict, name: str) -> tuple[float, float, float] | None:
    for exp in doc.get("exports") or []:
        if exp.get("name") != name:
            continue
        for prop in exp.get("properties") or []:
            if prop.get("name") == "Location" and prop.get("vector"):
                v = prop["vector"]
                return float(v["x"]), float(v["y"]), float(v["z"])
    return None


def probe_z(samples: list[int], n: int, origin_x: float, origin_y: float, scale_x: float, scale_y: float, loc_z: float, scale_z: float, ue_x: float, ue_y: float) -> str:
    fx = (ue_x - origin_x) / scale_x
    fy = (ue_y - origin_y) / scale_y
    ix = int(round(fx))
    iy = int(round(fy))
    if not (0 <= ix < n and 0 <= iy < n):
        return f"probe UE ({ue_x:.2f},{ue_y:.2f}) is off the sheet (fx {fx:.2f}, fy {fy:.2f})"
    sample = samples[iy * n + ix]
    z = loc_z + (sample - 32768) * (scale_z / 256.0)
    return (
        f"probe UE ({ue_x:.2f},{ue_y:.2f}) texel ({ix},{iy}) sample {sample} sheet Z {z:.1f}"
    )


def apply_owner(owner_path: Path, bmp_path: Path, dump_path: Path | None, actor: str | None) -> int:
    owner = json.loads(owner_path.read_text(encoding="utf-8"))
    ref = TerrainMapRef(
        owner["package"], owner.get("group") or "", owner["name"],
        owner["kind"], int(owner["index"]), owner["reference"],
    )
    utx_name = Path(owner["utx"]).name
    stem = utx_name[: -len(utx_name.split(".")[-1]) - 1] if "." in utx_name else utx_name
    if ref.kind == "import":
        assert_height_package(ref, stem, owner["utx"])
    else:
        print(
            f"[TerrainMap] {ref.qualified} index {ref.index} is an export in the level "
            f"(export_index {owner.get('utx_export_index')})"
        )
    n = int(owner["width"])
    if int(owner["height"]) != n:
        raise SystemExit(f"ERROR heightmap {owner['width']}x{owner['height']} is not square")
    loc = owner["location"]
    scale = owner["scale"]
    loc_z = float(loc[2])
    scale_z = float(scale[2])
    z_per = scale_z / 256.0
    root = owner_path.parent
    g16 = (root / owner["g16"]).read_bytes()
    info = (root / owner["terrain_info"]).read_bytes()
    _, current = read_bmp_samples(bmp_path)
    if len(current) != n * n:
        raise SystemExit(f"ERROR {bmp_path} is {len(current)} samples, G16 is {n}x{n}")

    print(
        f"[TerrainMap] {ref.qualified} index {ref.index} ({ref.kind}) "
        f"group={ref.group or '(none)'} -> {owner['utx']} export {owner.get('utx_export_index')} "
        f"sha256 {owner.get('g16_sha256')}"
    )

    best_g16: tuple[float, list[int], str] | None = None
    for flip, label in ((False, "mip order"), (True, "mip flip-Y")):
        proposed = g16_as_top_origin(g16, n, flip)
        median, outliers = compare_samples(current, proposed, z_per)
        print(f"[TerrainMap] G16 {label}: median |dZ| {median:.2f} UU, texels >30 UU: {outliers}")
        if best_g16 is None or median < best_g16[0]:
            best_g16 = (median, proposed, label)

    grid = find_vertex_grid(info, float(scale[0]), float(scale[1]), n)
    chosen = best_g16[1]
    chosen_why = f"referenced G16 ({best_g16[2]})"
    origin_x = float(loc[0]) - (n / 2.0) * float(scale[0])
    origin_y = float(loc[1]) - (n / 2.0) * float(scale[1])
    if grid is None:
        print("[TerrainMap] no n*n vector grid in the TerrainInfo export; using the referenced G16")
    else:
        align, start, sign = grid
        origin_x, origin_y = grid_origin(info, align, start, n, sign)
        print(
            f"[TerrainMap] vertex grid at align {align} float {start} y_sign {sign} "
            f"origin ({origin_x:.3f},{origin_y:.3f})"
        )
        fitted: tuple[float, list[int], str] | None = None
        for world_z, label in ((True, "world Z"), (False, "local Z")):
            samples = grid_samples(info, align, start, n, sign, loc_z, scale_z, world_z)
            in_range = sum(1 for s in samples if 0 <= s <= 65535)
            if in_range != n * n:
                print(f"[TerrainMap] vertices {label}: {in_range}/{n * n} samples inside uint16")
                continue
            median, outliers = compare_samples(best_g16[1], samples, z_per)
            print(f"[TerrainMap] vertices {label} vs G16: median |dZ| {median:.2f} UU, texels >30 UU: {outliers}")
            if fitted is None or median < fitted[0]:
                fitted = (median, samples, label)
        if fitted is None:
            print("[TerrainMap] vertex Z is not a height sample; using the referenced G16")
        else:
            median, samples, label = fitted
            _, outliers = compare_samples(best_g16[1], samples, z_per)
            bmp_median, bmp_out = compare_samples(current, samples, z_per)
            print(f"[TerrainMap] vertices {label} vs bmp: median |dZ| {bmp_median:.2f} UU, texels >30 UU: {bmp_out}")
            if outliers > 50:
                chosen = samples
                chosen_why = f"TerrainInfo vertex grid ({label}, level-owned heights)"

    if dump_path is not None and actor:
        doc = load_doc(dump_path)
        loc_a = actor_location(doc, actor)
        if loc_a is None:
            print(f"[TerrainMap] actor {actor} has no Location in {dump_path}")
        else:
            print("[TerrainMap] " + probe_z(
                chosen, n, origin_x, origin_y, float(scale[0]), float(scale[1]),
                loc_z, scale_z, loc_a[0], loc_a[1],
            ) + f" actor Z {loc_a[2]:.1f} ({chosen_why})")

    if chosen == current:
        print(f"[TerrainMap] {bmp_path.name} already matches {chosen_why}")
        return 0
    # chosen is a new list even when values match; compare values
    if chosen == list(current):
        print(f"[TerrainMap] {bmp_path.name} already matches {chosen_why}")
        return 0
    write_bmp(bmp_path, n, chosen)
    digest = hashlib.sha256(bmp_path.read_bytes()).hexdigest()
    print(f"[TerrainMap] wrote {bmp_path} from {chosen_why} sha256 {digest}")
    return 0


def main(argv: list[str] | None = None) -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    sub = ap.add_subparsers(dest="cmd", required=True)
    show = sub.add_parser("resolve", help="print TerrainMap package.group.name and index")
    show.add_argument("dump", type=Path)
    check = sub.add_parser("check", help="error when a height package is not the TerrainMap package")
    check.add_argument("dump", type=Path)
    check.add_argument("opened_package")
    check.add_argument("where")
    apply_p = sub.add_parser("apply", help="write the level-owned height BMP when it differs")
    apply_p.add_argument("--owner", type=Path, required=True)
    apply_p.add_argument("--bmp", type=Path, required=True)
    apply_p.add_argument("--dump", type=Path)
    apply_p.add_argument("--actor")
    args = ap.parse_args(argv)
    if args.cmd == "resolve":
        ref = one_terrain_map(load_doc(args.dump))
        print(
            f"{ref.qualified} kind={ref.kind} index={ref.index} "
            f"package={ref.package} group={ref.group or '(none)'} name={ref.name}"
        )
        return 0
    if args.cmd == "check":
        ref = one_terrain_map(load_doc(args.dump))
        assert_height_package(ref, args.opened_package, args.where)
        print(f"[TerrainMap] {ref.qualified} index {ref.index} package {ref.package} ({args.where})")
        return 0
    return apply_owner(args.owner, args.bmp, args.dump, args.actor)


if __name__ == "__main__":
    sys.exit(main())
