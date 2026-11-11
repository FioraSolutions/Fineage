using System.Collections.Generic;
using UnityEngine;

public sealed partial class NameplateTextDrawer
{
    public void Draw(Camera cam, List<NameplatePaintItem> paintList, NameplatePixelSnap snap)
    {
        if (cam == null || paintList == null || _font == null || !_batch.IsReady)
        {
            return;
        }

        _batch.BeginFrame();

        float scale = PixelScale > 0f ? PixelScale : 1f;
        float screenH = cam.pixelHeight;
        float lineH = _font.MeasureHeight(scale);
        float titleGap = Mathf.Max(0f, TitleGapPixels);
        bool discardFrac = SnapAnchorToPixels && SnapDiscardSubpixelFrac;

        float orbitDist = 0f;
        CameraController cameraController = CameraPacketServices.Controller;
        if (cameraController != null)
        {
            orbitDist = cameraController.CurrentDistance;
        }

        for (int i = 0; i < paintList.Count; i++)
        {
            NameplatePaintItem item = paintList[i];
            item.ScreenValid = false;

            Vector3 screen = cam.WorldToScreenPoint(item.World);
            if (screen.z <= 0f)
            {
                paintList[i] = item;
                continue;
            }

            float depth = screen.z;
            float ax = screen.x;
            float ay = screen.y;
            if (SnapAnchorToPixels && snap != null)
            {
                float snapDist = (item.IsLocalPlayer && orbitDist > 0.05f) ? orbitDist : depth;
                ax = snap.Snap(item.Id, screen.x, true, snapDist);
                ay = snap.Snap(item.Id, screen.y, false, snapDist);
            }

            float yNameTop = screenH - ay - lineH;
            if (!string.IsNullOrEmpty(item.Title))
            {
                float titleW = _font.MeasureWidth(item.Title, scale);
                float xTitle = ax - titleW * 0.5f;
                float yTitleTop = yNameTop - lineH - titleGap;
                _batch.AppendLine(
                    _font, item.Title, xTitle, yTitleTop, scale, item.TitleColor,
                    depth, screenH, discardFrac);
            }

            float nameW = _font.MeasureWidth(item.Name, scale);
            float xName = ax - nameW * 0.5f;
            _batch.AppendLine(
                _font, item.Name, xName, yNameTop, scale, item.NameColor,
                depth, screenH, discardFrac);

            item.ScreenValid = true;
            item.Depth = depth;
            item.AnchorX = ax;
            item.NameW = nameW;
            item.XName = xName;
            item.YNameTop = yNameTop;
            item.YNameBottom = yNameTop + lineH;
            paintList[i] = item;
        }

        _batch.UploadAndDraw(cam);
    }

    public void Dispose()
    {
        _batch.Dispose();
    }
}
