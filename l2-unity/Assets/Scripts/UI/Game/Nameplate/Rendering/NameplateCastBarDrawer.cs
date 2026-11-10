using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interlude SetupGauge BLUE under the local player's name — same screen-pixel path as
/// name glyphs / HeadDisplay bubbles (no OnGUI, no bone follow).
/// Stack (top → bottom): title → name → cast gauge → head.
/// </summary>
public sealed class NameplateCastBarDrawer
{
    // Same Target/ tree as HeadDisplay bubbles.
    const string FillResourcePath = "Data/UI/Target/HeadDisplay_DF_Gauge_Cast/HeadDisplay_DF_Gauge_Cast";
    const string BgResourcePath = "Data/UI/Target/HeadDisplay_DF_Gauge_Cast/HeadDisplay_DF_Gauge_Cast_Bg";

    /// <summary>Source atlas width. Opaque fill ink spans <see cref="ContentU0"/>..<see cref="ContentU1"/>.</summary>
    const float AtlasWidth = 128f;
    /// <summary>Inclusive-exclusive opaque columns in the fill PNG (left-packed ink).</summary>
    const float ContentU0 = 2f;
    const float ContentU1 = 78f;

    /// <summary>Drawn size — stretched from the cropped ink to Interlude-like proportions.</summary>
    const float GaugeWidth = 104f;
    const float GaugeHeight = 4f;
    const float GapBelowNamePx = 6f;

    readonly L2NameplateScreenBatch _batchBg = new L2NameplateScreenBatch();
    readonly L2NameplateScreenBatch _batchFill = new L2NameplateScreenBatch();

    Texture2D _texBg;
    Texture2D _texFill;

    public void EnsureResources()
    {
        if (_texBg == null)
            _texBg = L2NameplateBubbles.LoadTexture(BgResourcePath);
        if (_texFill == null)
            _texFill = L2NameplateBubbles.LoadTexture(FillResourcePath);

        if (_texBg != null)
            _batchBg.EnsureMaterial(_texBg);
        if (_texFill != null)
            _batchFill.EnsureMaterial(_texFill);
    }

    public void Draw(Camera cam, List<NameplatePaintItem> paintList)
    {
        if (cam == null || paintList == null)
            return;
        if (!_batchBg.IsReady || !_batchFill.IsReady)
            return;

        _batchBg.BeginFrame();
        _batchFill.BeginFrame();

        float screenH = cam.pixelHeight;
        float u0 = ContentU0 / AtlasWidth;
        float u1 = ContentU1 / AtlasWidth;
        bool any = false;

        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            if (!item.ScreenValid || !item.IsLocalPlayer)
                continue;
            if (item.CastRemaining01 < 0f)
                continue;

            float remaining = Mathf.Clamp01(item.CastRemaining01);
            float yTop = item.YNameBottom + GapBelowNamePx;
            float yBot = yTop + GaugeHeight;

            // Same screen axis as the name glyphs (character overhead anchor).
            float centerX = item.AnchorX;
            float x1 = centerX - GaugeWidth * 0.5f;
            float x2 = x1 + GaugeWidth;

            _batchBg.AppendQuad(
                x1, yTop, x2, yBot, item.Depth, screenH, Color.white,
                snapPixels: true, u0: u0, v0: 0f, u1: u1, v1: 1f);

            float fillW = GaugeWidth * remaining;
            if (fillW >= 1f)
            {
                float xFill2 = x1 + fillW;
                float uFill1 = u0 + (u1 - u0) * remaining;
                _batchFill.AppendQuad(
                    x1, yTop, xFill2, yBot, item.Depth, screenH, Color.white,
                    snapPixels: true, u0: u0, v0: 0f, u1: uFill1, v1: 1f);
            }

            any = true;
        }

        if (!any)
            return;

        _batchBg.UploadAndDraw(cam);
        _batchFill.UploadAndDraw(cam);
    }

    public void Dispose()
    {
        _batchBg.Dispose();
        _batchFill.Dispose();
    }
}
