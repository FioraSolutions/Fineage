using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateTextDrawer
{
    private readonly L2NameplateScreenBatch _batch = new L2NameplateScreenBatch();
    private L2BitmapFont _font;
    private string _atlasResourcePath;
    private string _csvResourcePath;

    public float PixelScale { get; set; } = 1f;
    public float TitleGapPixels { get; set; } = 2f;
    public bool SnapAnchorToPixels { get; set; } = true;
    public bool SnapDiscardSubpixelFrac { get; set; } = true;

    public bool IsReady => _font != null && _batch.IsReady;

    public float MeasureNameLineHeightPx()
    {
        float scale = PixelScale > 0f ? PixelScale : 1f;
        if (_font == null)
            return L2NameplateScreenLayout.FallbackLineHeightPx * scale;

        return _font.MeasureHeight(scale);
    }




}
