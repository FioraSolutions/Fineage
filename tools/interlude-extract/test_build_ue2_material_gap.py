#!/usr/bin/env python3
"""The texture index is one sorted row per resource, not one row per file."""
from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import build_ue2_material_gap as gap  # noqa: E402


def write(root: Path, rel: str, text: str = "") -> None:
    path = root / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


class IndexRowsTest(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.tex = Path(self.tmp.name)

    def tearDown(self):
        self.tmp.cleanup()

    def test_png_and_tga_of_one_stem_are_one_row(self):
        write(self.tex, "Primitive_FieldOBJ_T/Leaf.png")
        write(self.tex, "Primitive_FieldOBJ_T/Leaf.tga")
        self.assertEqual(
            ["Leaf\tPrimitive_FieldOBJ_T\tData/Textures/Primitive_FieldOBJ_T/Leaf"],
            gap.index_rows(self.tex),
        )

    def test_same_name_in_two_packages_is_two_rows(self):
        write(self.tex, "PackA/shared.png")
        write(self.tex, "PackB/shared.png")
        self.assertEqual(
            [
                "shared\tPackA\tData/Textures/PackA/shared",
                "shared\tPackB\tData/Textures/PackB/shared",
            ],
            gap.index_rows(self.tex),
        )

    def test_rows_are_sorted_and_header_is_added_by_render(self):
        write(self.tex, "PackB/b.png")
        write(self.tex, "PackA/a.png")
        write(self.tex, "PackA/c.png")
        rendered = gap.render_index(self.tex)
        lines = rendered.splitlines()
        self.assertEqual("name\tpackage\tresource_path", lines[0])
        self.assertEqual(sorted(lines[1:]), lines[1:])
        self.assertTrue(rendered.endswith("\n"))

    def test_non_image_files_are_skipped(self):
        write(self.tex, "PackA/a.png")
        write(self.tex, "PackA/a.png.meta")
        write(self.tex, "PackA/Shader/a.props.txt")
        self.assertEqual(1, len(gap.index_rows(self.tex)))

    def test_group_folder_is_part_of_the_path(self):
        write(self.tex, "PackA/Grp/a.png")
        self.assertEqual(
            ["a\tPackA\tData/Textures/PackA/Grp/a"],
            gap.index_rows(self.tex),
        )


class GapScanTest(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.tex = Path(self.tmp.name)

    def tearDown(self):
        self.tmp.cleanup()

    def test_every_props_file_counts_even_with_a_shared_stem(self):
        write(self.tex, "PackA/Shader/z_water01.props.txt",
              "Material1 = TexPanner'PackA.statue_pan'\n")
        write(self.tex, "PackB/Shader/z_water01.props.txt",
              "Material1 = TexPanner'PackB.statue_pan'\n")
        missing, present = gap.gap_scan(self.tex)
        self.assertEqual(2, missing[("TexPanner", "statue_pan")])
        self.assertEqual(2, present["TexPanner"])

    def test_modifier_with_props_is_not_missing(self):
        write(self.tex, "PackA/Shader/a.props.txt", "Material1 = Combiner'PackA.combo'\n")
        write(self.tex, "PackA/combo.props.txt", "CombineOperation = CO_Add\n")
        missing, _ = gap.gap_scan(self.tex)
        self.assertEqual(0, len(missing))

    def test_non_modifier_refs_are_ignored(self):
        write(self.tex, "PackA/Shader/a.props.txt",
              "Diffuse = Texture'PackB.leaf'\nFallbackMaterial = Shader'PackB.sh'\n")
        _, present = gap.gap_scan(self.tex)
        self.assertEqual(0, len(present))


if __name__ == "__main__":
    unittest.main()
