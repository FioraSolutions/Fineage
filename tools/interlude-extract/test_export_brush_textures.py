#!/usr/bin/env python3
"""Brush/BSP texture want-list is derived from the lobby T3D and Brushes.json."""
from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
sys.path.insert(0, str(Path(__file__).resolve().parent))
import export_brush_textures as brush  # noqa: E402

JSON = ROOT / "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/Brushes.json"
T3D = ROOT / "l2-unity/docs/interlude-ref/world/env/Lobby.brushes.t3d"


class BrushTextureRefsTest(unittest.TestCase):
    def test_json_and_t3d_agree_on_png_textures(self):
        from_json = {ref.key() for ref in brush.collect([JSON])}
        from_t3d = {ref.key() for ref in brush.collect([T3D])}
        self.assertEqual(from_json, from_t3d)
        self.assertIn(("FX_E_T", "WaterShader01"), from_json)
        self.assertIn(("Castle_Kent_t", "Kent_capet_m"), from_json)
        self.assertTrue(all("Shaders" not in full for _, _, full in (
            (r.package, r.texture, r.full) for r in brush.collect([JSON])
        )))
        self.assertNotIn(("L2_Skies", "HazeRing_Final"), from_json)

    def test_missing_export_names_package_and_texture(self):
        refs = brush.collect([JSON])
        with tempfile.TemporaryDirectory() as tmp:
            lines = brush.missing_messages(refs, Path(tmp), lambda _package: None)
        text = "\n".join(lines)
        self.assertIn("package=FX_E_T texture=WaterShader01", text)
        self.assertIn("package=Castle_Kent_t texture=Kent_capet_m", text)
        self.assertIn("utx missing", text)

    def test_present_png_is_not_reported(self):
        ref = brush.BrushTexture("FX_E_T", "WaterShader01", "FX_E_T.WaterSurfaceShaderSet.WaterShader01")
        with tempfile.TemporaryDirectory() as tmp:
            png = brush.dest_png(Path(tmp), ref)
            png.parent.mkdir(parents=True)
            png.write_bytes(b"png")
            self.assertEqual(
                brush.missing_messages([ref], Path(tmp), lambda _p: Path("FX_E_T.utx")),
                [],
            )


if __name__ == "__main__":
    unittest.main()
