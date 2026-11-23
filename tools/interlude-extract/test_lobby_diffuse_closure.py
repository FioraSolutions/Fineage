#!/usr/bin/env python3
"""A qualified Diffuse (Texture'Deco_T.X') is satisfied only from Deco_T's folder."""
from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import lobby_diffuse_closure as closure  # noqa: E402


def write(root: Path, rel: str, text: str = "") -> None:
    path = root / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


class DiffuseClosureTest(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.tex = Path(self.tmp.name) / "tex"
        self.tex.mkdir()
        self.env = Path(self.tmp.name) / "env"
        self.env.mkdir()
        closure._MATERIAL_LEAVES.clear()
        closure._PNG_LEAVES.clear()
        closure._PACKAGE_FOLDERS.clear()
        closure._ENV_GROUPS.clear()

    def tearDown(self):
        self.tmp.cleanup()

    def rows(self, packages=("primitive_fieldobj_t",)):
        return closure.missing(set(packages), tex=self.tex, skins=set(), env=self.env)

    def dump(self, package: str, exports: list[tuple[str, str]]) -> None:
        body = {
            "exports": [
                {"name": name, "outer": f"export:{package.lower()}:0:Package:{group}" if group else ""}
                for group, name in exports
            ]
        }
        write(self.env, f"{package}.json", json.dumps(body))

    def test_qualified_ref_ignores_same_named_png_in_own_package(self):
        write(self.tex, "Primitive_FieldOBJ_T/Shader/BigLeaf.props.txt",
              "Diffuse = Texture'Deco_T.Primitive_BigLeaf_001'\n")
        write(self.tex, "Primitive_FieldOBJ_T/Primitive_BigLeaf_001.png")
        self.assertEqual(
            [("Deco_T", "Primitive_FieldOBJ_T", "Primitive_BigLeaf_001")],
            self.rows(),
        )

    def test_qualified_ref_satisfied_by_png_in_the_named_package(self):
        write(self.tex, "Primitive_FieldOBJ_T/Shader/BigLeaf.props.txt",
              "Diffuse = Texture'Deco_T.Primitive_BigLeaf_001'\n")
        write(self.tex, "Deco_T/Primitive_BigLeaf_001.png")
        self.assertEqual([], self.rows())

    def test_package_match_is_case_insensitive(self):
        write(self.tex, "Primitive_FieldOBJ_T/Shader/BigLeaf.props.txt",
              "Diffuse = Texture'Deco_T.Primitive_BigLeaf_001'\n")
        write(self.tex, "deco_t/primitive_bigleaf_001.png")
        self.assertEqual([], self.rows())

    def test_group_qualified_ref_names_the_first_segment(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'PackB.Group.Leaf_002'\n")
        self.assertEqual([("PackB", "PackA", "Leaf_002")], self.rows(packages=("packa",)))

    def test_unqualified_ref_keeps_leaf_anywhere(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'Anywhere_003'\n")
        write(self.tex, "PackB/Anywhere_003.png")
        self.assertEqual([], self.rows(packages=("packa",)))

    def test_unqualified_ref_missing_names_own_package(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'Nowhere_004'\n")
        self.assertEqual([("PackA", "PackA", "Nowhere_004")], self.rows(packages=("packa",)))

    def test_material_leaf_is_not_a_texture_row(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'Deco_T.Chain_005'\n")
        write(self.tex, "Deco_T/Shader/Chain_005.props.txt", "Diffuse = None\n")
        self.assertEqual([], self.rows(packages=("packa", "deco_t")))

    def test_two_qualifiers_for_one_leaf_are_two_rows(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'PackB.Leaf_006'\n")
        write(self.tex, "PackA/Shader/Leaf2.props.txt",
              "Diffuse = Texture'PackC.Leaf_006'\n")
        self.assertEqual(
            [("PackB", "PackA", "Leaf_006"), ("PackC", "PackA", "Leaf_006")],
            self.rows(packages=("packa",)),
        )

    def test_none_diffuse_is_skipped(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt", "Diffuse = None\n")
        self.assertEqual([], self.rows(packages=("packa",)))

    def test_dump_proven_group_ref_loads_from_own_package(self):
        # l2_skies.Textures.WhiteRing prints as Textures.WhiteRing: an export
        # of l2_skies under group Textures, not a package named Textures.
        self.dump("l2_skies", [("Textures", "WhiteRing")])
        write(self.tex, "l2_skies/Shader/HazeRing.props.txt",
              "Diffuse = Texture'Textures.WhiteRing'\n")
        self.assertEqual([("l2_skies", "l2_skies", "WhiteRing")], self.rows(packages=("l2_skies",)))

    def test_dump_proven_group_ref_satisfied_by_own_package_png(self):
        self.dump("l2_skies", [("Textures", "WhiteRing")])
        write(self.tex, "l2_skies/Shader/HazeRing.props.txt",
              "Diffuse = Texture'Textures.WhiteRing'\n")
        write(self.tex, "l2_skies/WhiteRing.png")
        self.assertEqual([], self.rows(packages=("l2_skies",)))

    def test_dump_rules_out_group_ref(self):
        # The dump lists every export; WhiteRing under another group means
        # Textures.WhiteRing is not same-package — it is package Textures.
        self.dump("l2_skies", [("Skies", "WhiteRing")])
        write(self.tex, "l2_skies/Shader/HazeRing.props.txt",
              "Diffuse = Texture'Textures.WhiteRing'\n")
        self.assertEqual(
            [("Textures", "l2_skies", "WhiteRing")],
            self.rows(packages=("l2_skies",)),
        )

    def test_ref_naming_own_package_loads_from_own_package(self):
        write(self.tex, "PackA/Shader/Leaf.props.txt",
              "Diffuse = Texture'PackA.Leaf_007'\n")
        write(self.tex, "PackA/Leaf_007.png")
        self.assertEqual([], self.rows(packages=("packa",)))


if __name__ == "__main__":
    unittest.main()
