using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateCastBarDrawer
{
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
