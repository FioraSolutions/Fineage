#!/usr/bin/env python3
"""The gap closer resolves rows by which package actually holds the object."""
from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path
from types import SimpleNamespace

sys.path.insert(0, str(Path(__file__).resolve().parent))
import brush_face_class as faces  # noqa: E402
from brush_face_class import KIND_MATERIAL, KIND_TEXTURE  # noqa: E402
import close_diffuse_gaps as cdg  # noqa: E402
import export_brush_textures as export  # noqa: E402


class LiveIndexTest(unittest.TestCase):
    """Packages the committed table never dumped classify from live rows."""

    def test_add_live_classifies_an_undumped_package(self):
        idx = faces.ExportIndex()
        self.assertEqual("absent", faces.classify("Flameset.de_fire_glow_s", idx).kind)
        idx.add_live("Flameset", [("de_fire_glow_s", "Combiner", "Shader")])
        face = faces.classify("Flameset.de_fire_glow_s", idx)
        self.assertEqual(KIND_MATERIAL, face.kind)
        self.assertEqual("Combiner", face.ue_class)
        self.assertTrue(idx.has_package("Flameset"))

    def test_add_live_texture_kind(self):
        idx = faces.ExportIndex()
        idx.add_live("Deco_T", [("Primitive_BigLeaf_001", "Texture", "")])
        face = faces.classify("Deco_T.Primitive_BigLeaf_001", idx)
        self.assertEqual(KIND_TEXTURE, face.kind)

    def test_add_dump_classifies_from_a_property_dump(self):
        idx = faces.ExportIndex()
        dump = SimpleNamespace(
            package="Flameset",
            exports=[{"name": "de_fire_0000", "class": "Texture", "outer": ""}],
        )
        idx.add_dump(dump)
        face = faces.classify("Flameset.de_fire_0000", idx)
        self.assertEqual(KIND_TEXTURE, face.kind)
        self.assertTrue(idx.has_package("flameset"))

    def test_add_live_does_not_relabel_committed_rows(self):
        idx = faces.ExportIndex()
        before = faces.classify("Castle_Kent_t.Kent_capet_m", idx)
        idx.add_live("Castle_Kent_t", [("Kent_capet_m", "Shader", "")])
        after = faces.classify("Castle_Kent_t.Kent_capet_m", idx)
        self.assertEqual(before.ue_class, after.ue_class)
        self.assertEqual(before.kind, after.kind)


class HydrateGroupTest(unittest.TestCase):
    def _face(self, group=""):
        return faces.Face("Flameset", group, "de_fire_glow_s", "Flameset.de_fire_glow_s", "Combiner", KIND_MATERIAL)

    def test_single_live_group_fills_an_omitted_group(self):
        face = export._hydrate_group(self._face(), [("de_fire_glow_s", "Combiner", "Shader")])
        self.assertEqual("Shader", face.group)

    def test_two_live_groups_leave_the_ref_alone(self):
        rows = [("de_fire_glow_s", "Combiner", "Shader"), ("de_fire_glow_s", "Combiner", "Other")]
        self.assertEqual("", export._hydrate_group(self._face(), rows).group)

    def test_ref_group_wins(self):
        face = export._hydrate_group(self._face("Mine"), [("de_fire_glow_s", "Combiner", "Shader")])
        self.assertEqual("Mine", face.group)

    def test_top_level_export_stays_group_less(self):
        face = export._hydrate_group(self._face(), [("de_fire_glow_s", "Combiner", "")])
        self.assertEqual("", face.group)


class OpsRowsTest(unittest.TestCase):
    def test_missing_ops_resolve_like_the_gap_scan(self):
        with tempfile.TemporaryDirectory() as tmp:
            tex = Path(tmp)
            (tex / "FX_E_T").mkdir()
            (tex / "FX_E_T" / "a.props.txt").write_text(
                "Diffuse = TexPanner'fallwater.evil_fallwater01_pan'\n"
                "Specular = TexPanner'Orc_Dgn_light002_pan'\n"
                "Opacity = Texture'some_tex'\n"
                "SelfIllumination = TexPanner'satisfied_pan'\n",
                encoding="utf-8",
            )
            (tex / "FX_E_T" / "satisfied_pan.props.txt").write_text(
                "PanRate = 1.0\n", encoding="utf-8"
            )
            rows = cdg.ops_rows(tex)
        by_leaf = {row[2]: row for row in rows}
        self.assertIn("evil_fallwater01_pan", by_leaf)
        want, own, _leaf, cls = by_leaf["evil_fallwater01_pan"]
        self.assertEqual(("fallwater", "FX_E_T", "TexPanner"), (want, own, cls))
        self.assertIn("Orc_Dgn_light002_pan", by_leaf)
        self.assertEqual("FX_E_T", by_leaf["Orc_Dgn_light002_pan"][0])
        self.assertNotIn("satisfied_pan", by_leaf)
        self.assertNotIn("some_tex", by_leaf)


class ResolveRefsTest(unittest.TestCase):
    def test_named_package_wins_when_it_holds_the_object(self):
        refs, errors = cdg.resolve_refs(
            [("Deco_T", "field_deco_S", "Primitive_BigLeaf_001", "Texture")],
            lambda pkg: {"primitive_bigleaf_001"},
        )
        self.assertEqual(["Deco_T.Primitive_BigLeaf_001"], refs)
        self.assertEqual([], errors)

    def test_own_package_is_the_fallback_for_group_references(self):
        def live(pkg):
            return None if pkg == "WaterSurfaceShaderSet" else {"ssq_water_01_tex"}

        refs, errors = cdg.resolve_refs(
            [("WaterSurfaceShaderSet", "FX_E_T", "ssq_water_01_tex", "TexOscillator")], live
        )
        self.assertEqual(["FX_E_T.ssq_water_01_tex"], refs)
        self.assertEqual([], errors)

    def test_object_absent_from_the_named_utx_falls_back_to_own(self):
        def live(pkg):
            return {"other"} if pkg == "fallwater" else {"evil_fallwater01_pan"}

        refs, errors = cdg.resolve_refs(
            [("fallwater", "FX_E_T", "evil_fallwater01_pan", "TexPanner")], live
        )
        self.assertEqual(["FX_E_T.evil_fallwater01_pan"], refs)
        self.assertEqual([], errors)

    def test_object_in_neither_is_an_error_not_a_guess(self):
        refs, errors = cdg.resolve_refs(
            [("NoPkg", "OwnPkg", "ghost", "Texture")], lambda pkg: set()
        )
        self.assertEqual([], refs)
        self.assertEqual(1, len(errors))
        self.assertIn("package=NoPkg", errors[0])
        self.assertIn("own=OwnPkg", errors[0])
        self.assertIn("ghost", errors[0])

    def test_missing_utx_says_so(self):
        refs, errors = cdg.resolve_refs(
            [("NoPkg", "NoPkg", "ghost", "Texture")], lambda pkg: None
        )
        self.assertEqual([], refs)
        self.assertIn("utx missing", errors[0])


class GatherRowsTest(unittest.TestCase):
    def test_live_tree_rows_cover_diffuse_and_ops(self):
        import lobby_diffuse_closure as closure

        rows = cdg.gather_rows(None, closure.TEX)
        leaves = {row[2] for row in rows}
        # A diffuse-only leaf.
        self.assertIn("Primitive_BigLeaf_001", leaves)
        # Ops from the material-ops gap.
        self.assertIn("Orc_Dgn_light002_pan", leaves)
        self.assertIn("de_fire_glow_s", leaves)
        # An op named in a Diffuse slot keeps one row, not two.
        self.assertEqual(1, len([row for row in rows if row[2] == "ssq_water_01_tex"]))

    def test_live_tree_names_the_lobby_lens_flares(self):
        import lobby_diffuse_closure as closure

        rows = cdg.gather_rows(None, closure.TEX, cdg.ZONE_CATALOG, "Lobby")
        flares = sorted(row[2] for row in rows if row[3] == "LensFlare")
        self.assertEqual(["Flare01", "Flare02", "Flare03", "Flare04", "Flare06"], flares)
        for row in rows:
            if row[3] == "LensFlare":
                self.assertEqual(("Engine", "Engine"), (row[0], row[1]))


class FlareRowsTest(unittest.TestCase):
    CATALOG = (
        "map\tclass\tname\tzone_tag\tb_distance_fog\tb_terrain_zone\tfog_start\tfog_end\t"
        "fog_r\tfog_g\tfog_b\tfog_a\ttex_u_pan\ttex_v_pan\tlens_flares\n"
        "Lobby\tSkyZoneInfo\tSkyZoneInfo0\t\t\t\t\t\t\t\t\t\t\t\tEngine.Flare01|Engine.Flare02|Engine.Flare01\n"
        "Other\tSkyZoneInfo\tSkyZoneInfo9\t\t\t\t\t\t\t\t\t\t\t\tEngine.Flare09\n"
    )

    def test_distinct_flares_for_the_named_map_only(self):
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            catalog = root / "zone-render.tsv"
            catalog.write_text(self.CATALOG, encoding="utf-8")
            tex = root / "Textures"
            rows = cdg.flare_rows(catalog, "Lobby", tex)
        self.assertEqual(
            [("Engine", "Engine", "Flare01", "LensFlare"), ("Engine", "Engine", "Flare02", "LensFlare")],
            rows,
        )

    def test_committed_png_is_satisfied(self):
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            catalog = root / "zone-render.tsv"
            catalog.write_text(self.CATALOG, encoding="utf-8")
            tex = root / "Textures"
            (tex / "Engine").mkdir(parents=True)
            (tex / "Engine" / "Flare01.png").write_bytes(b"png")
            rows = cdg.flare_rows(catalog, "Lobby", tex)
        self.assertEqual([("Engine", "Engine", "Flare02", "LensFlare")], rows)

    def test_missing_catalog_is_no_rows(self):
        rows = cdg.flare_rows(Path("/no/such/catalog.tsv"), "Lobby", Path("/no/tex"))
        self.assertEqual([], rows)


if __name__ == "__main__":
    unittest.main()
