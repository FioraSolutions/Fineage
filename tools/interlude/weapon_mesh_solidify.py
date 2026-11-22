#!/usr/bin/env python3
"""Solidify single-sided LineageWeapons FBX geometry (must match WeaponMeshTopology in Unity).

Usage:
  python3 tools/interlude/weapon_mesh_solidify.py --audit
  python3 tools/interlude/weapon_mesh_solidify.py --fix [--limit N]
  python3 tools/interlude/weapon_mesh_solidify.py --fix path/to/mesh.fbx

Re-run is idempotent: meshes that already satisfy the opposite-normal-pair ratio are skipped.
"""
from __future__ import annotations

import argparse
import math
import os
import sys
from pathlib import Path
from typing import List, Sequence, Tuple

REPO = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(REPO / "tools" / "interlude"))
from fbx_meter_patch import Node, parse, serialize  # noqa: E402

MIN_OPP_RATIO = 0.84
SOLIDIFY_FACTOR = 0.035
MIN_SHELL = 0.0015
INTENTIONALLY_FLAT = ("fishing_rod",)


def intentionally_flat(name: str) -> bool:
    stem = Path(name).stem
    if stem.endswith("_et") or stem.endswith("_sh"):
        return True
    return any(token in stem for token in INTENTIONALLY_FLAT)


def triangulate(indices: Sequence[int]) -> List[Tuple[int, int, int]]:
    tris: List[Tuple[int, int, int]] = []
    poly: List[int] = []
    for idx in indices:
        if idx < 0:
            poly.append(~idx)
            if len(poly) >= 3:
                for i in range(1, len(poly) - 1):
                    tris.append((poly[0], poly[i], poly[i + 1]))
            poly = []
        else:
            poly.append(idx)
    return tris


def compute_triangle_normal(verts: Sequence[float], a: int, b: int, c: int) -> Tuple[float, float, float]:
    ax, ay, az = verts[a * 3 : a * 3 + 3]
    bx, by, bz = verts[b * 3 : b * 3 + 3]
    cx, cy, cz = verts[c * 3 : c * 3 + 3]
    ux, uy, uz = bx - ax, by - ay, bz - az
    vx, vy, vz = cx - ax, cy - ay, cz - az
    nx, ny, nz = uy * vz - uz * vy, uz * vx - ux * vz, ux * vy - uy * vx
    length = math.sqrt(nx * nx + ny * ny + nz * nz)
    if length < 1e-12:
        return (0.0, 0.0, 0.0)
    return (nx / length, ny / length, nz / length)


def opposite_normal_pair_ratio(verts: Sequence[float], tris: Sequence[Tuple[int, int, int]]) -> float:
    normals = [compute_triangle_normal(verts, *t) for t in tris]
    normals = [n for n in normals if n != (0.0, 0.0, 0.0)]
    if not normals:
        return 0.0
    unmatched = 0
    used = [False] * len(normals)
    for i, n in enumerate(normals):
        if used[i]:
            continue
        found = False
        for j in range(i + 1, len(normals)):
            if used[j]:
                continue
            m = normals[j]
            dot = n[0] * m[0] + n[1] * m[1] + n[2] * m[2]
            if dot < -0.85:
                used[i] = used[j] = True
                found = True
                break
        if not found:
            unmatched += 1
    return 1.0 - unmatched / len(normals)


def average_normals(verts: Sequence[float], tris: Sequence[Tuple[int, int, int]]) -> List[Tuple[float, float, float]]:
    count = len(verts) // 3
    sums = [[0.0, 0.0, 0.0] for _ in range(count)]
    weight = [0] * count
    for a, b, c in tris:
        n = compute_triangle_normal(verts, a, b, c)
        if n == (0.0, 0.0, 0.0):
            continue
        for vtx in (a, b, c):
            sums[vtx][0] += n[0]
            sums[vtx][1] += n[1]
            sums[vtx][2] += n[2]
            weight[vtx] += 1
    avgs: List[Tuple[float, float, float]] = []
    for i in range(count):
        if weight[i] == 0:
            avgs.append((0.0, 0.0, 1.0))
            continue
        x, y, z = (sums[i][0] / weight[i], sums[i][1] / weight[i], sums[i][2] / weight[i])
        length = math.sqrt(x * x + y * y + z * z)
        if length < 1e-12:
            avgs.append((0.0, 0.0, 1.0))
        else:
            avgs.append((x / length, y / length, z / length))
    return avgs


def bounds_size(verts: Sequence[float]) -> Tuple[float, float, float]:
    xs = [verts[i] for i in range(0, len(verts), 3)]
    ys = [verts[i] for i in range(1, len(verts), 3)]
    zs = [verts[i] for i in range(2, len(verts), 3)]
    return (max(xs) - min(xs), max(ys) - min(ys), max(zs) - min(zs))


def solidify(verts: Sequence[float], tris: Sequence[Tuple[int, int, int]]) -> Tuple[List[float], List[int]]:
    base = len(verts) // 3
    avgs = average_normals(verts, tris)
    longest = max(bounds_size(verts))
    shell = max(longest * SOLIDIFY_FACTOR, MIN_SHELL)
    new_verts = list(verts)
    for i in range(base):
        nx, ny, nz = avgs[i]
        ix = i * 3
        new_verts.extend(
            (
                verts[ix] + nx * shell,
                verts[ix + 1] + ny * shell,
                verts[ix + 2] + nz * shell,
            )
        )
    new_polys: List[List[int]] = []
    for a, b, c in tris:
        new_polys.append([a, b, c])
        new_polys.append([a + base, c + base, b + base])
    flat_indices: List[int] = []
    for poly in new_polys:
        for v in poly:
            flat_indices.append(v)
        flat_indices.append(~flat_indices[-1])
    return new_verts, flat_indices


def read_geometry(objects: Node) -> Tuple[Node | None, Node | None, Node | None]:
    for obj in objects.children:
        if obj.name != "Geometry":
            continue
        verts = obj.first("Vertices")
        indices = obj.first("PolygonVertexIndex")
        if verts and indices:
            return obj, verts, indices
    return None, None, None


def audit_file(path: Path) -> dict | None:
    version, nodes, original, footer = parse(str(path))
    root = Node("", [], nodes, wide=version >= 7500)
    objects = root.first("Objects")
    if not objects:
        return None
    _, verts_node, idx_node = read_geometry(objects)
    if not verts_node or not idx_node or not verts_node.props or not idx_node.props:
        return None
    verts = verts_node.props[0].value
    indices = idx_node.props[0].value
    tris = triangulate(indices)
    ratio = opposite_normal_pair_ratio(verts, tris)
    size = bounds_size(verts)
    longest = max(size)
    shortest = min(size)
    thick = shortest / longest if longest > 1e-6 else 0.0
    degenerate = not intentionally_flat(path.name) and ratio < MIN_OPP_RATIO
    return {
        "path": str(path),
        "name": path.stem,
        "verts": len(verts) // 3,
        "tris": len(tris),
        "opp_ratio": ratio,
        "thick_ratio": thick,
        "degenerate": degenerate,
        "version": version,
        "nodes": nodes,
        "footer": original[footer:],
    }


def fix_file(path: Path) -> bool:
    info = audit_file(path)
    if info is None:
        print(f"SKIP(no geometry): {path}")
        return False
    if not info["degenerate"]:
        return False
    verts_node = None
    idx_node = None
    objects = Node("", [], info["nodes"], wide=info["version"] >= 7500).first("Objects")
    for obj in (objects.children if objects else []):
        if obj.name != "Geometry":
            continue
        verts_node = obj.first("Vertices")
        idx_node = obj.first("PolygonVertexIndex")
        break
    if not verts_node or not idx_node:
        raise SystemExit(f"{path}: geometry nodes missing")
    verts = verts_node.props[0].value
    tris = triangulate(idx_node.props[0].value)
    new_verts, new_indices = solidify(verts, tris)
    after_ratio = opposite_normal_pair_ratio(new_verts, triangulate(new_indices))
    if after_ratio < MIN_OPP_RATIO:
        raise SystemExit(
            f"{path}: solidify failed (opp {info['opp_ratio']:.3f} -> {after_ratio:.3f})"
        )
    verts_node.props[0].value = new_verts
    idx_node.props[0].value = new_indices
    tmp = str(path) + ".tmp"
    with open(tmp, "wb") as fh:
        fh.write(serialize(info["version"], info["nodes"], info["footer"]))
    os.replace(tmp, path)
    print(
        f"FIXED {path.name}: verts {info['verts']}->{len(new_verts)//3} "
        f"opp {info['opp_ratio']:.3f}->{after_ratio:.3f}"
    )
    return True


def default_models_dir() -> Path:
    return REPO / "l2-unity/Assets/Resources/Data/Animations/LineageWeapons/Models"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("paths", nargs="*")
    parser.add_argument("--audit", action="store_true")
    parser.add_argument("--fix", action="store_true")
    parser.add_argument("--limit", type=int, default=0)
    args = parser.parse_args()

    if args.paths:
        files = [Path(p) for p in args.paths]
    else:
        files = sorted(default_models_dir().glob("*.fbx"))

    if args.audit or not args.fix:
        flat = []
        for path in files:
            info = audit_file(path)
            if not info:
                continue
            if info["degenerate"]:
                flat.append(info)
        print(f"degenerate={len(flat)} total={len(files)} threshold_opp={MIN_OPP_RATIO}")
        for row in flat[:50]:
            print(
                f"  {row['name']}: opp={row['opp_ratio']:.3f} thick={row['thick_ratio']:.3f} "
                f"verts={row['verts']} tris={row['tris']}"
            )
        if len(flat) > 50:
            print(f"  ... and {len(flat) - 50} more")
        return 1 if flat and not args.fix else 0

    fixed = 0
    for path in files:
        if args.limit and fixed >= args.limit:
            break
        if fix_file(path):
            fixed += 1
    print(f"fixed={fixed}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())