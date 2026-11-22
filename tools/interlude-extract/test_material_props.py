#!/usr/bin/env python3
"""Material props are rendered from the property dump, not from umodel."""
from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import export_brush_textures as export  # noqa: E402
import material_props as props  # noqa: E402

REPO = Path(__file__).resolve().parents[2]
SKIES = REPO / "l2-unity/docs/interlude-ref/world/env/l2_skies.json"
BRUSHES = REPO / "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/Brushes.json"

# Minimal ExportPackageProperties document. Color bytes are signed, the way
# the Java writer stores them (r=-1 is 255). Groups come from outer Package
# exports, which is what turns an object ref into Class'Group.Name'.
FIXTURE = {
    "package": "l2_skies",
    "exports": [
        {
            "export_index": 46, "class": "Package", "name": "Shaders",
            "outer": "none", "properties": [],
        },
        {
            "export_index": 59, "class": "Package", "name": "Textures",
            "outer": "none", "properties": [],
        },
        {
            "export_index": 7, "class": "Shader", "name": "Cloud",
            "outer": "export:l2_skies:46:Package:Shaders", "properties": [],
        },
        {
            "export_index": 6, "class": "Shader", "name": "HazeRing",
            "outer": "export:l2_skies:46:Package:Shaders", "properties": [],
        },
        {
            "export_index": 8, "class": "Texture", "name": "WhiteChip",
            "outer": "export:l2_skies:59:Package:Textures", "properties": [],
        },
        {
            "export_index": 48, "class": "ColorModifier", "name": "Cloud_Final",
            "outer": "export:l2_skies:46:Package:Shaders",
            "properties": [
                {
                    "name": "Color", "type": "Struct", "struct": "Color",
                    "color": {"r": -1, "g": -64, "b": -105, "a": -1},
                },
                {"name": "RenderTwoSided", "type": "Bool", "value": False},
                {
                    "name": "Material", "type": "Object", "index": 8,
                    "ref": "export:l2_skies:7:Shader:Cloud",
                },
            ],
        },
        {
            "export_index": 49, "class": "ColorModifier", "name": "HazeRing_Final",
            "outer": "export:l2_skies:46:Package:Shaders",
            "properties": [
                {
                    "name": "Color", "type": "Struct", "struct": "Color",
                    "color": {"r": -1, "g": -28, "b": -107, "a": -1},
                },
                {"name": "RenderTwoSided", "type": "Bool", "value": False},
                {
                    "name": "Material", "type": "Object", "index": 7,
                    "ref": "export:l2_skies:6:Shader:HazeRing",
                },
            ],
        },
        {
            "export_index": 47, "class": "ColorModifier", "name": "SkybackgroundColor",
            "outer": "export:l2_skies:46:Package:Shaders",
            "properties": [
                {
                    "name": "Color", "type": "Struct", "struct": "Color",
                    "color": {"r": 0, "g": -106, "b": -50, "a": -1},
                },
                {"name": "RenderTwoSided", "type": "Bool", "value": False},
                {"name": "AlphaBlend", "type": "Bool", "value": False},
                {
                    "name": "Material", "type": "Object", "index": 9,
                    "ref": "export:l2_skies:8:Texture:WhiteChip",
                },
            ],
        },
    ],
}


class ColorModifierFixtureTest(unittest.TestCase):
    def test_fixture_renders_umodel_style_props(self):
        dump = props.Dump(FIXTURE, Path("fixture.json"))
        cloud = dump.render(dump.find("Cloud_Final", "ColorModifier", "Shaders"))
        self.assertIn("Material = Shader'Shaders.Cloud'", cloud)
        self.assertIn("Color = (B=151,G=192,R=255,A=255)", cloud)
        self.assertIn("RenderTwoSided = false", cloud)

        haze = dump.render(dump.find("HazeRing_Final", "ColorModifier", "Shaders"))
        self.assertIn("Material = Shader'Shaders.HazeRing'", haze)
        self.assertIn("Color = (B=149,G=228,R=255,A=255)", haze)

        sky = dump.render(dump.find("SkybackgroundColor", "ColorModifier", "Shaders"))
        self.assertIn("Material = Texture'Textures.WhiteChip'", sky)
        self.assertIn("AlphaBlend = false", sky)
        self.assertIn("Color = (B=206,G=150,R=0,A=255)", sky)

    def test_committed_l2_skies_dump_matches_the_fixture(self):
        dump = props.Dump.load(SKIES)
        fixture = props.Dump(FIXTURE, Path("fixture.json"))
        for name in ("Cloud_Final", "HazeRing_Final", "SkybackgroundColor"):
            live = dump.render(dump.find(name, "ColorModifier", "Shaders"))
            expected = fixture.render(fixture.find(name, "ColorModifier", "Shaders"))
            self.assertEqual(expected, live, name)

    def test_duplicate_texpanner_is_chosen_by_group(self):
        dump = props.Dump.load(SKIES)
        shaders = dump.render(dump.find("TexPanner0", "TexPanner", "Shaders"))
        moon = dump.render(dump.find("TexPanner0", "TexPanner", "moon"))
        self.assertIn("Material = Texture'Textures.WhiteCloud'", shaders)
        self.assertIn("PanRate = 0.002", shaders)
        self.assertIn("Material = Texture'moon.OpenEyeMoon_myst_bitmap4'", moon)
        self.assertIn("PanRate = -0.05", moon)
        self.assertIsNone(dump.find("TexPanner0", "TexPanner", ""))


class DumpPlanTest(unittest.TestCase):
    def test_missing_props_are_from_dump_and_present_props_are_satisfied(self):
        refs = export.collect_refs([BRUSHES])
        empty = export.material_plan(refs, [])
        by_empty = {face.name: status for face, status in empty}
        self.assertEqual("from-dump", by_empty["Cloud_Final"])
        self.assertEqual("from-dump", by_empty["HazeRing_Final"])
        self.assertEqual("from-dump", by_empty["SkybackgroundColor"])
        self.assertEqual("needs-property-dump", by_empty["Kent_capet_m"])
        self.assertNotIn("umodel", export.format_plan(empty))
        # WaterShader01's committed props pull Combiner reflection into the chain.
        committed = {face.name: status for face, status in export.material_plan(refs, [props.TEX_ASSETS])}
        self.assertEqual("satisfied", committed["WaterShader01"])
        self.assertEqual("needs-property-dump", committed["reflection"])

        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            dump = props.Dump.load(SKIES)
            for name in ("Cloud_Final", "HazeRing_Final", "SkybackgroundColor"):
                text = dump.render(dump.find(name, "ColorModifier", "Shaders"))
                props.write_rendered(
                    props.props_dest(root, "l2_skies", "ColorModifier", name), text,
                )
            # Shaders that already have committed props are not rewritten.
            with self.assertRaises(SystemExit) as raised:
                props.write_package(dump, root, [props.TEX_ASSETS])
            self.assertIn("Blend3Textures", str(raised.exception))
            self.assertIn("no elements", str(raised.exception))
            self.assertFalse((root / "l2_skies" / "Shader" / "Cloud.props.txt").is_file())
            again = export.material_plan(refs, [root])
            by_written = {face.name: status for face, status in again}
            self.assertEqual("satisfied", by_written["Cloud_Final"])
            self.assertEqual("satisfied", by_written["HazeRing_Final"])
            self.assertEqual("satisfied", by_written["SkybackgroundColor"])


class UmodelClassAuditTest(unittest.TestCase):
    def test_colormodifier_is_a_lobby_brush_class_umodel_does_not_export(self):
        self.assertNotIn("ColorModifier", props.UMODEL_SUPPORTED_MATERIAL_CLASSES)
        brushes = props.lobby_brush_classes()
        self.assertIn("ColorModifier", brushes)
        self.assertIn("Shader", brushes)
        self.assertIn("Texture", brushes)
        text = "\n".join(props.audit_lines())
        self.assertIn("brush ColorModifier:", text)
        self.assertIn("umodel-unsupported", text)
        mesh = props.committed_mesh_skin_classes()
        self.assertTrue(mesh)
        unsupported = set(mesh) - props.UMODEL_SUPPORTED_MATERIAL_CLASSES
        self.assertEqual(set(), unsupported)
        # The skin scan must actually look at lobby mesh props, not return silently empty
        # when the StaticMeshes tree exists.
        _skins, missing = props.lobby_skin_classes()
        self.assertTrue(missing or _skins)


if __name__ == "__main__":
    unittest.main()
