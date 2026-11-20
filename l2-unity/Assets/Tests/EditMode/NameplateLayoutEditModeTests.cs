using NUnit.Framework;
using UnityEngine;

public sealed class NameplateLayoutEditModeTests
{
    [Test]
    public void Name_line_top_uses_anchor_minus_one_glyph_line()
    {
        const float screenH = 1080f;
        const float anchorY = 620f;
        const float lineH = 14f;

        float yNameTop = L2NameplateScreenLayout.ComputeYNameTop(screenH, anchorY, lineH);
        Assert.AreEqual(446f, yNameTop, 0.001f);
    }

    [Test]
    public void Say_bottom_aligns_with_name_line_row()
    {
        const float screenH = 1080f;
        const float anchorX = 640f;
        const float anchorY = 620f;
        const float lineH = 14f;

        Vector2 bottomLeft = L2NameplateScreenLayout.ResolveSayBottomLeft(
            screenH, anchorX, anchorY, lineH);

        Assert.AreEqual(anchorX, bottomLeft.x, 0.001f);
        Assert.AreEqual(anchorY + lineH, bottomLeft.y, 0.001f);
    }

    [Test]
    public void Title_sits_one_line_above_name_with_gap()
    {
        const float lineH = 14f;
        const float gap = 2f;
        float yNameTop = 400f;

        float yTitleTop = L2NameplateScreenLayout.ComputeYTitleTop(yNameTop, lineH, gap);
        Assert.AreEqual(384f, yTitleTop, 0.001f);
    }
}