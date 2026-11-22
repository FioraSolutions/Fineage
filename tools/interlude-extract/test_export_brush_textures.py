#!/usr/bin/env python3
"""Brush faces are classified by the package export's class, then by chain."""
from __future__ import annotations

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import brush_face_class as faces  # noqa: E402
from brush_face_class import KIND_MATERIAL, KIND_TEXTURE  # noqa: E402
import export_brush_textures as export  # noqa: E402

JSON = Path(__file__).resolve().parents[2] / (
    "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/Brushes.json"
)
PROPS = Path(__file__).resolve().parents[2] / (
    "l2-unity/Assets/Resources/Data/Textures/FX_E_T/Shader/WaterShader01.props.txt"
)


class BrushFaceClassTest(unittest.TestCase):
    def test_lobby_faces_match_the_export_table(self):
        water = faces.classify("FX_E_T.WaterSurfaceShaderSet.WaterShader01")
        self.assertEqual(KIND_MATERIAL, water.kind)
        self.assertEqual("Shader", water.ue_class)
        self.assertEqual("WaterSurfaceShaderSet", water.group)
        self.assertEqual("WaterShader01", water.name)

        carpet = faces.classify("Castle_Kent_t.Kent_capet_m")
        self.assertEqual(KIND_MATERIAL, carpet.kind)
        self.assertEqual("Shader", carpet.ue_class)
        self.assertEqual("", carpet.group)

        floor = faces.classify("Castle_Kent_t.GL_CA_floortile")
        self.assertEqual(KIND_TEXTURE, floor.kind)
        self.assertEqual("Texture", floor.ue_class)

        sky = faces.classify("L2_Skies.Shaders.HazeRing_Final")
        self.assertEqual(KIND_MATERIAL, sky.kind)
        self.assertEqual("ColorModifier", sky.ue_class)
        self.assertEqual("Shaders", sky.group)

        missing = faces.classify("Castle_Kent_t.NoSuchFace")
        self.assertEqual("absent", missing.kind)
        self.assertIn("class=absent", missing.format())
        self.assertIn("package=Castle_Kent_t", missing.format())
        self.assertIn("name=NoSuchFace", missing.format())

    def test_water_shader_props_chain_reaches_a_texture(self):
        root = faces.classify("FX_E_T.WaterSurfaceShaderSet.WaterShader01")
        text = PROPS.read_text(encoding="utf-8")
        tokens = faces.props_tokens(text)
        self.assertIn("WaterSurfaceShaderSet.reflection", tokens)
        self.assertIn("WaterSurfaceSet.base1", tokens)
        reflection = faces.resolve_props_token("WaterSurfaceShaderSet.reflection", "FX_E_T")
        base = faces.resolve_props_token("WaterSurfaceSet.base1", "FX_E_T")
        self.assertEqual(KIND_MATERIAL, reflection.kind)
        self.assertEqual("Combiner", reflection.ue_class)
        self.assertEqual(KIND_TEXTURE, base.kind)
        self.assertEqual("base1", base.name)

        def read_props(face):
            if face.name == "WaterShader01":
                return text
            if face.name == "reflection":
                return "Diffuse = Texture'WaterSurfaceSet.base1'\n"
            return None

        textures, materials, errors = export.expand_chain([root], read_props)
        self.assertEqual([], errors)
        self.assertEqual({"base1"}, {face.name for face in textures})
        self.assertEqual({"WaterShader01", "reflection"}, {face.name for face in materials})

    def test_live_package_must_agree_on_class_and_group(self):
        carpet = faces.classify("Castle_Kent_t.Kent_capet_m")
        self.assertIsNone(faces.confirm_live(carpet, [("Kent_capet_m", "Shader", "")]))
        mismatch = faces.confirm_live(carpet, [("Kent_capet_m", "Texture", "")])
        self.assertIn("class=Shader", mismatch)
        self.assertIn("class=Texture", mismatch)
        water = faces.classify("FX_E_T.WaterSurfaceShaderSet.WaterShader01")
        wrong_group = faces.confirm_live(water, [("WaterShader01", "Shader", "OtherGroup")])
        self.assertIn("not in the package", wrong_group)
        self.assertIn("group=WaterSurfaceShaderSet", wrong_group)

    def test_json_list_does_not_treat_shaders_as_textures(self):
        kinds = {}
        for ref in export.refs_in_file(JSON):
            face = faces.classify(ref)
            kinds.setdefault(face.kind, set()).add(face.name)
        self.assertIn("WaterShader01", kinds[KIND_MATERIAL])
        self.assertIn("Kent_capet_m", kinds[KIND_MATERIAL])
        self.assertIn("GL_CA_floortile", kinds[KIND_TEXTURE])
        self.assertNotIn("WaterShader01", kinds.get(KIND_TEXTURE, set()))


if __name__ == "__main__":
    unittest.main()
