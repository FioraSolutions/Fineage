using Fineage.TerrainBsp;
using NUnit.Framework;

public class L2BspPolyFlagsEditModeTests
{
    [Test]
    public void Sky_textures_are_excluded_by_default_for_world_tiles()
    {
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("L2_Skies.L2_Lobby_sky"));
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("l2_skies.cloud01"));
    }

    [Test]
    public void Lobby_bake_can_opt_into_sky_brushes()
    {
        Assert.IsTrue(L2BspPolyFlags.IsBrushTextureRenderable("L2_Skies.L2_Lobby_sky", includeSky: true));
    }

    [Test]
    public void Other_exclusions_stay_regardless_of_sky_opt_in()
    {
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("Engine.SomeTexture", includeSky: true));
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("L2_ETC_Textures.Portal", includeSky: true));
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("WaterShader01", includeSky: true));
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable(null, includeSky: true));
        Assert.IsFalse(L2BspPolyFlags.IsBrushTextureRenderable("", includeSky: true));
    }

    [Test]
    public void Ordinary_brush_textures_render_in_both_modes()
    {
        Assert.IsTrue(L2BspPolyFlags.IsBrushTextureRenderable("T_Rune.rock01"));
        Assert.IsTrue(L2BspPolyFlags.IsBrushTextureRenderable("T_Rune.rock01", includeSky: true));
    }
}
