#!/usr/bin/env python3
"""TerrainMap is the package the level names, and a vertex grid beats a stale G16."""
from __future__ import annotations

import struct
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import terrain_map_ref as ref  # noqa: E402

REPO = Path(__file__).resolve().parents[2]
DUMPS = REPO / "l2-unity/docs/interlude-ref/world"


class ResolveTests(unittest.TestCase):
    def test_lobby_imports_utx_height_and_has_no_level_texture(self) -> None:
        doc = ref.load_doc(DUMPS / "env/Lobby.unr.json.gz")
        got = ref.one_terrain_map(doc)
        self.assertEqual(got.qualified, "Lobby_terrain_t.Height")
        self.assertEqual(got.package, "Lobby_terrain_t")
        self.assertEqual(got.group, "")
        self.assertEqual(got.name, "Height")
        self.assertEqual(got.kind, "import")
        self.assertEqual(got.index, -346)
        self.assertFalse(got.in_level)
        textures = [e for e in doc["exports"] if e.get("class") == "Texture"]
        self.assertEqual(textures, [])

    def test_15_21_refuses_its_own_utx(self) -> None:
        doc = ref.load_doc(DUMPS / "maps/15_21.unr.json.gz")
        got = ref.one_terrain_map(doc)
        self.assertEqual(got.qualified, "T_15_20.Height.15_20")
        self.assertEqual(got.index, -41)
        with self.assertRaises(SystemExit) as caught:
            ref.assert_height_package(got, "T_15_21", "15_21_heightmap.raw")
        self.assertIn("T_15_20", str(caught.exception))
        self.assertIn("T_15_21", str(caught.exception))
        ref.assert_height_package(got, "T_15_20", "the referenced package")

    def test_object_name_is_not_the_tile_id_when_the_dump_says_so(self) -> None:
        a = ref.one_terrain_map(ref.load_doc(DUMPS / "maps/21_25.unr.json.gz"))
        self.assertEqual(a.qualified, "T_21_25.Height.21_17")
        self.assertEqual(a.name, "21_17")
        b = ref.one_terrain_map(ref.load_doc(DUMPS / "maps/21_16.unr.json.gz"))
        self.assertEqual(b.qualified, "T_21_16.21_16")
        self.assertEqual(b.group, "")


class VertexGridTests(unittest.TestCase):
    def test_finds_a_grid_past_a_header_and_recovers_samples(self) -> None:
        n = 4
        scale = 64.0
        loc_z = 160.0
        scale_z = 128.0
        ox, oy = -256.0, 1024.0
        samples = [30000 + x + 10 * y for y in range(n) for x in range(n)]
        body = bytearray(b"\xab" * 10)
        for y in range(n):
            for x in range(n):
                sample = samples[y * n + x]
                z = loc_z + (sample - 32768) * (scale_z / 256.0)
                body += struct.pack("<fff", ox + x * scale, oy + y * scale, z)
        found = ref.find_vertex_grid(bytes(body), scale, scale, n)
        self.assertIsNotNone(found)
        align, start, sign = found
        self.assertEqual(sign, 1)
        got = ref.grid_samples(bytes(body), align, start, n, sign, loc_z, scale_z, True)
        self.assertEqual(got, samples)
        origin = ref.grid_origin(bytes(body), align, start, n, sign)
        self.assertEqual(origin, (ox, oy))

    def test_bmp_roundtrip_keeps_top_origin_samples(self) -> None:
        samples = [1000 + i for i in range(16)]
        with tempfile.TemporaryDirectory() as tmp:
            path = Path(tmp) / "Height.Height.bmp"
            ref.write_bmp(path, 4, samples)
            n, got = ref.read_bmp_samples(path)
        self.assertEqual(n, 4)
        self.assertEqual(got, samples)


if __name__ == "__main__":
    unittest.main()
