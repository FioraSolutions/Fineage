using System;

public partial class LobbyNameplatesManager
{
    private Camera ResolveCamera()
    {
        if (_camera != null)
        {
            return _camera;
        }

        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector != null && selector.Camera != null)
        {
            return selector.Camera;
        }

        return null;
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        Camera targetCam = ResolveCamera();
        if (cam == null || targetCam == null || cam != targetCam || !cam.isActiveAndEnabled)
        {
            return;
        }

        EnsureResources();
        if (_font == null || !_batch.IsReady)
        {
            return;
        }

        BuildPaintList(cam);
        if (_paintList.Count == 0)
        {
            return;
        }

        if (!RebuildAndDraw(cam))
        {
            return;
        }
    }

    private bool RebuildAndDraw(Camera cam)
    {
        _batch.BeginFrame();

        float scale = _pixelScale > 0f ? _pixelScale : 1f;
        float screenH = cam.pixelHeight;
        float lineH = _font.MeasureHeight(scale);
        bool discardFrac = _snapAnchorToPixels && _snapDiscardSubpixelFrac;

        for (int i = 0; i < _paintList.Count; i++)
        {
            PaintItem item = _paintList[i];
            Vector3 screen = cam.WorldToScreenPoint(item.World);
            if (screen.z <= 0f)
            {
                continue;
            }

            float ax = screen.x;
            float ay = screen.y;
            if (_snapAnchorToPixels)
            {
                _pixelSnap.HysteresisPx = _snapHysteresisPx;
                ax = _pixelSnap.Snap(item.Slot, screen.x, true, screen.z);
                ay = _pixelSnap.Snap(item.Slot, screen.y, false, screen.z);
            }

            float textW = _font.MeasureWidth(item.Name, scale);
            float x = ax - textW * 0.5f;
            // GUI/L2 Y-down top of string; projected point = bottom of text.
            float yTop = screenH - ay - lineH;

            _batch.AppendLine(
                _font, item.Name, x, yTop, scale, item.Color, screen.z, screenH, discardFrac);
        }

        return _batch.UploadAndDraw(cam);
    }
}
