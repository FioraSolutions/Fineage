using System.IO;
using System.Linq;
using Fineage.TerrainBsp;
using NUnit.Framework;
using UnityEngine;

/// <summary>Elven Village brush dumps: CSG_Subtract must carve, not render; portal caps collected.</summary>
public sealed class ElvenVillageBspCsgEditModeTests
{
    static string FixtureRoot =>
        Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "tools", "interlude", "fixtures", "elven_village"));

    [Test]
    public void Tile_20_20_has_many_subtract_brushes_and_portal_caps()
    {
        string path = Path.Combine(FixtureRoot, "20_20", "brushes.t3d");
        Assert.IsTrue(File.Exists(path), "missing fixture " + path);
        var brushes = L2T3DBrushParser.ParseFile(path);
        var analysis = L2BspCsgCarver.Analyze(brushes);
        Assert.Greater(analysis.subtractBrushCount, 400, "20_20 fixture should include CSG_Subtract brushes");
        Assert.Greater(analysis.portalCapCount, 10, "AntiPortal doorway caps expected on add brushes");
        int antiPortalPolys = brushes.SelectMany(b => b.polygons ?? System.Array.Empty<L2BspPoly>())
            .Count(pd => L2BspPolyFlags.IsPortalCapTexture(pd?.texture));
        Assert.Greater(antiPortalPolys, 10);
        foreach (var pd in brushes.SelectMany(b => b.polygons ?? System.Array.Empty<L2BspPoly>()))
        {
            if (pd == null) continue;
            if (L2BspPolyFlags.Contains(pd.polyFlags, "PF_Invisible")
                || L2BspPolyFlags.Contains(pd.polyFlags, "PF_Portal"))
                Assert.IsFalse(L2BspPolyFlags.ShouldRenderPolygon(pd.polyFlags, pd.texture));
        }
    }

    [Test]
    public void Subtract_volume_intersects_known_doorway_wall_region()
    {
        string path = Path.Combine(FixtureRoot, "20_20", "brushes.t3d");
        var brushes = L2T3DBrushParser.ParseFile(path);
        var subtracts = L2BspCsgCarver.CollectSubtractVolumes(brushes);
        Assert.Greater(subtracts.Count, 400);
        // AntiPortal brush609 region (~ shop row Y 95k X 11k)
        bool hit = subtracts.Any(b =>
            b.Y0 < 96000f && b.Y1 > 94000f && b.X0 < 12000f && b.X1 > 9000f);
        Assert.IsTrue(hit, "expected subtract volume near doorway AntiPortal cluster");
    }

    [Test]
    public void CarveFacePieces_can_open_axis_aligned_wall_quad()
    {
        var add = new L2BspBrush
        {
            name = "Wall",
            csgOper = L2BspCsgCarver.CsgAdd,
            position = Vector3.zero,
            prePivot = Vector3.zero,
            polyFlags = new[] { "PF_Default" },
            polygons = new[]
            {
                new L2BspPoly
                {
                    texture = "Oren_HED_T2.hed_wall01",
                    polyFlags = new[] { "PF_Default" },
                    origin = Vector3.zero,
                    textureU = Vector3.right * 128f,
                    textureV = Vector3.forward * 128f,
                    vertices = new[]
                    {
                        new Vector3(0, 0, 0),
                        new Vector3(512, 0, 0),
                        new Vector3(512, 0, 256),
                        new Vector3(0, 0, 256),
                    }
                }
            }
        };
        var subtracts = new System.Collections.Generic.List<L2BspAabb>
        {
            new L2BspAabb { X0 = 150, X1 = 350, Y0 = -64, Y1 = 64, Z0 = 50, Z1 = 200 }
        };
        var pieces = L2BspCsgCarver.CarveFacePieces(add, add.polygons[0], Vector3.zero,
            new System.Collections.Generic.List<L2BspPortalRect>(), subtracts);
        Assert.IsNotNull(pieces);
        Assert.Greater(pieces.Count, 1, "subtract AABB should split the wall into frame pieces");
    }

}