#!/usr/bin/env python3
"""umodel argv: one object per invocation, class explicit, group not positional."""
from __future__ import annotations

import shlex
import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import umodel_invoke as umodel  # noqa: E402


def positionals(argv: list[str]) -> list[str]:
    return [arg for arg in argv if arg != "/umodel" and not arg.startswith("-")]


class UmodelArgvTest(unittest.TestCase):
    def test_multi_object_is_one_invocation_each_with_its_class(self):
        # The Mac failure: Cloud_Final, HazeRing_Final, SkybackgroundColor were
        # one argv, so umodel read object=Cloud_Final class=HazeRing_Final.
        argvs = umodel.material_argvs("textures/l2_skies.utx", [
            ("Shaders", "Cloud_Final", "ColorModifier"),
            ("Shaders", "HazeRing_Final", "ColorModifier"),
            ("Shaders", "SkybackgroundColor", "ColorModifier"),
        ])
        self.assertEqual(3, len(argvs))
        self.assertEqual(
            ["textures/l2_skies.utx", "Cloud_Final", "ColorModifier"],
            positionals(argvs[0]),
        )
        self.assertEqual(
            ["textures/l2_skies.utx", "HazeRing_Final", "ColorModifier"],
            positionals(argvs[1]),
        )
        self.assertEqual(
            ["textures/l2_skies.utx", "SkybackgroundColor", "ColorModifier"],
            positionals(argvs[2]),
        )
        inner = umodel.docker_inner(argvs)
        self.assertEqual(3, inner.count("/umodel"))
        self.assertNotIn("Cloud_Final HazeRing_Final", inner)
        self.assertNotIn("Cloud_Final' 'HazeRing_Final", inner)
        # Group is not an object or a class slot.
        for argv in argvs:
            self.assertNotIn("Shaders", positionals(argv))
            self.assertEqual(3, len(positionals(argv)))

    def test_grouped_name_is_leaf_object_plus_class(self):
        argvs = umodel.material_argvs("textures/FX_E_T.utx", [
            ("WaterSurfaceShaderSet", "WaterShader01", "Shader"),
        ])
        self.assertEqual(
            ["textures/FX_E_T.utx", "WaterShader01", "Shader"],
            positionals(argvs[0]),
        )
        self.assertNotIn("WaterSurfaceShaderSet", argvs[0])
        with self.assertRaises(ValueError):
            umodel.material_argvs("textures/FX_E_T.utx", [
                ("WaterSurfaceShaderSet", "WaterSurfaceShaderSet.WaterShader01", "Shader"),
            ])

    def test_ungrouped_shader_still_passes_class(self):
        argvs = umodel.material_argvs("textures/castle_kent_t.utx", [
            ("", "Kent_capet_m", "Shader"),
        ])
        self.assertEqual(
            ["textures/castle_kent_t.utx", "Kent_capet_m", "Shader"],
            positionals(argvs[0]),
        )

    def test_whole_package_has_no_object_or_class(self):
        argv = umodel.package_argv("textures/l2_skies.utx", out="/export/l2_skies")
        self.assertEqual(["textures/l2_skies.utx"], positionals(argv))
        self.assertIn("-out=/export/l2_skies", argv)
        script = umodel.main([
            "--package", "textures/l2_skies.utx",
            "--out", "/export/l2_skies",
        ])
        self.assertEqual(0, script)

    def test_cli_pairs_object_with_class_and_refuses_a_mismatch(self):
        from io import StringIO
        from contextlib import redirect_stdout
        buf = StringIO()
        with redirect_stdout(buf):
            rc = umodel.main([
                "--package", "textures/l2_skies.utx",
                "--object", "Cloud_Final", "--class", "ColorModifier",
                "--object", "HazeRing_Final", "--class", "ColorModifier",
            ])
        self.assertEqual(0, rc)
        lines = buf.getvalue().splitlines()
        self.assertEqual(2, len(lines))
        self.assertEqual(
            ["textures/l2_skies.utx", "Cloud_Final", "ColorModifier"],
            positionals(shlex.split(lines[0])),
        )
        self.assertEqual(
            ["textures/l2_skies.utx", "HazeRing_Final", "ColorModifier"],
            positionals(shlex.split(lines[1])),
        )
        with self.assertRaises(SystemExit):
            umodel.main([
                "--package", "textures/l2_skies.utx",
                "--object", "Cloud_Final", "--object", "HazeRing_Final",
                "--class", "ColorModifier",
            ])

    def test_mesh_objects_are_one_each_without_a_class_slot(self):
        argvs = umodel.object_argvs("staticmeshes/rionsctgart_tree_s.usx", [
            "Dwarftree01", "Orctree01",
        ])
        self.assertEqual(
            ["staticmeshes/rionsctgart_tree_s.usx", "Dwarftree01"],
            positionals(argvs[0]),
        )
        self.assertEqual(
            ["staticmeshes/rionsctgart_tree_s.usx", "Orctree01"],
            positionals(argvs[1]),
        )
        with self.assertRaises(ValueError):
            umodel.object_argvs("staticmeshes/rionsctgart_tree_s.usx", ["tree.Dwarftree01"])


class LobbyStep6DryRunTest(unittest.TestCase):
    def test_lobby_materials_come_from_the_property_dump(self):
        import export_brush_textures as export
        brushes = Path(__file__).resolve().parents[2] / (
            "l2-unity/Assets/Resources/Data/Maps/l2_lobby/Meta/Brushes.json"
        )
        t3d = Path(__file__).resolve().parents[2] / (
            "l2-unity/docs/interlude-ref/world/env/Lobby.brushes.t3d"
        )
        roots = [brushes]
        if t3d.is_file():
            roots.append(t3d)
        refs = export.collect_refs(roots)
        rows = export.material_plan(refs, [export.TEX_ASSETS])
        by_object = {face.name: (face, status) for face, status in rows}
        text = export.format_plan(rows)
        self.assertNotIn("umodel", text)
        self.assertNotIn("/umodel", text)
        # Props derived from l2_skies.json are committed, so they are not re-exported.
        for name in ("Cloud_Final", "HazeRing_Final", "SkybackgroundColor", "WaterShader01"):
            face, status = by_object[name]
            self.assertEqual("satisfied", status, face.format())
        # No dump is committed for these packages. Step 6 dumps the utx on the Mac.
        for name in ("Kent_capet_m", "reflection"):
            face, status = by_object[name]
            self.assertEqual("needs-property-dump", status, face.format())
            self.assertNotEqual("Texture", face.ue_class)


if __name__ == "__main__":
    unittest.main()
