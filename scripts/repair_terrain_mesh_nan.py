#!/usr/bin/env python3
"""Rebuild the NaN terrain meshes of tiles baked with a zero-size TerrainData.

Tiles whose TerrainInfo0.txt had no GeneratedSectorCounter were baked with terrain size (0,0,0), so
every vertex of NN_NN_Mesh.asset has y/normal = NaN. TerrainToMesh samples the terrain at vertices
that sit exactly on the heightmap grid, so the height is the stored int16 height * heightmap scale.y
/ 32766 (verified against the healthy tile 17_25: identical y, area-weighted normals dot 1.0).
The heightmap scale.y comes from a healthy tile because every tile has the same TerrainScale and
GeneratedSectorCounter (256).

Usage: repair_terrain_mesh_nan.py <Maps dir> <reference tile> <tile> [<tile> ...]
Needs: pip install UnityPy numpy
"""
import re
import struct
import sys

import numpy as np
import UnityPy


def terrain_heightmap(path):
    env = UnityPy.load(path)
    for obj in env.objects:
        if obj.type.name == "TerrainData":
            return obj.read_typetree()["m_Heightmap"]
    raise SystemExit(f"{path}: no TerrainData object")


def find_terrain_asset(maps, tile):
    import os
    for sub in ("UnityTerrain", "TerrainData"):
        p = os.path.join(maps, tile, sub, f"{tile}.asset")
        if os.path.exists(p):
            return p
    raise SystemExit(f"{tile}: terrain asset not found")


def normals(pos, tri):
    p0, p1, p2 = pos[tri[:, 0]], pos[tri[:, 1]], pos[tri[:, 2]]
    fn = np.cross(p1 - p0, p2 - p0)
    n = np.zeros_like(pos)
    for k in range(3):
        np.add.at(n, tri[:, k], fn)
    return n / np.maximum(np.linalg.norm(n, axis=1, keepdims=True), 1e-30)


def tangents(pos, nrm, uv, tri):
    p0, p1, p2 = pos[tri[:, 0]], pos[tri[:, 1]], pos[tri[:, 2]]
    u0, u1, u2 = uv[tri[:, 0]], uv[tri[:, 1]], uv[tri[:, 2]]
    e1, e2 = p1 - p0, p2 - p0
    du1, dv1 = (u1 - u0)[:, 0], (u1 - u0)[:, 1]
    du2, dv2 = (u2 - u0)[:, 0], (u2 - u0)[:, 1]
    det = du1 * dv2 - du2 * dv1
    r = np.where(np.abs(det) > 1e-30, 1.0 / np.where(det == 0, 1, det), 0.0)[:, None]
    sdir = (e1 * dv2[:, None] - e2 * dv1[:, None]) * r
    tdir = (e2 * du1[:, None] - e1 * du2[:, None]) * r
    t1 = np.zeros_like(pos)
    t2 = np.zeros_like(pos)
    for k in range(3):
        np.add.at(t1, tri[:, k], sdir)
        np.add.at(t2, tri[:, k], tdir)
    t = t1 - nrm * (nrm * t1).sum(1, keepdims=True)
    t = t / np.maximum(np.linalg.norm(t, axis=1, keepdims=True), 1e-30)
    w = np.where((np.cross(nrm, t) * t2).sum(1) < 0, -1.0, 1.0)
    return t, w


def fmt(v):
    return ("%.8g" % v)


def repair(maps, tile, scale_y):
    import os
    mesh_path = os.path.join(maps, tile, f"{tile}_Mesh.asset")
    text = open(mesh_path).read()
    raw = np.frombuffer(bytes.fromhex(re.search(r"_typelessdata: ([0-9a-f]+)", text).group(1)), dtype=np.uint8)
    vb = raw.view("<f4").reshape(-1, 12).copy()
    if not np.isnan(vb[:, 1]).all():
        raise SystemExit(f"{tile}: mesh is not NaN-only; refusing to touch it")
    idx = np.frombuffer(bytes.fromhex(re.search(r"m_IndexBuffer: ([0-9a-f]+)", text).group(1)), dtype="<u2").astype(np.int64)
    hm = terrain_heightmap(find_terrain_asset(maps, tile))
    res = hm["m_Resolution"]
    heights = np.array(hm["m_Heights"], dtype=np.float64).reshape(res, res)  # [z][x]
    if res != 257 or heights.min() <= 0:
        raise SystemExit(f"{tile}: unexpected heightmap resolution {res} / min {heights.min()}")
    x = vb[:, 0].astype(np.float64)
    z = vb[:, 2].astype(np.float64)
    gx, gz = x * (res - 1), z * (res - 1)
    ix, iz = np.rint(gx).astype(int), np.rint(gz).astype(int)
    if np.abs(gx - ix).max() > 1e-3 or np.abs(gz - iz).max() > 1e-3:
        raise SystemExit(f"{tile}: vertices are not on the heightmap grid")
    y = heights[np.minimum(iz, res - 1), np.minimum(ix, res - 1)] * scale_y / 32766.0
    pos = np.stack([x, y, z], axis=1)
    tri = idx.reshape(-1, 3)
    nrm = normals(pos, tri)
    uv = vb[:, 10:12].astype(np.float64)
    tan, w = tangents(pos, nrm, uv, tri)
    vb[:, 0:3] = pos
    vb[:, 3:6] = nrm
    vb[:, 6:9] = tan
    vb[:, 9] = w
    new_hex = vb.astype("<f4").tobytes().hex()
    text = re.sub(r"(_typelessdata: )[0-9a-f]+", lambda m: m.group(1) + new_hex, text, count=1)
    lo = pos.min(axis=0)
    hi = pos.max(axis=0)
    center, ext = (lo + hi) / 2, (hi - lo) / 2

    def aabb(m):
        return (f"{m.group(1)}m_Center: {{x: {fmt(center[0])}, y: {fmt(center[1])}, z: {fmt(center[2])}}}\n"
                f"{m.group(1)}m_Extent: {{x: {fmt(ext[0])}, y: {fmt(ext[1])}, z: {fmt(ext[2])}}}\n")

    text, n = re.subn(r"( +)m_Center: \{[^\n]*\}\n +m_Extent: \{[^\n]*\}\n", aabb, text)
    if n != 2:
        raise SystemExit(f"{tile}: expected 2 AABB blocks, found {n}")
    open(mesh_path, "w").write(text)
    print(f"{tile}: y {y.min():.3f}..{y.max():.3f}, normals/tangents/AABB rebuilt")


def main():
    maps, ref, tiles = sys.argv[1], sys.argv[2], sys.argv[3:]
    import os
    scale_y = terrain_heightmap(find_terrain_asset(maps, ref))["m_Scale"]["y"]
    for tile in tiles:
        repair(maps, tile, scale_y)


if __name__ == "__main__":
    main()
