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

    private void BuildPaintList(Camera cam)
    {
        _paintList.Clear();

        CharacterSelector selector = LoginPacketServices.Characters;
        if (selector == null)
        {
            return;
        }

        IReadOnlyList<GameObject> pawns = selector.CharacterPawns;
        if (pawns == null)
        {
            return;
        }

        int count = Mathf.Min(pawns.Count, MaxSlots);
        for (int i = 0; i < count; i++)
        {
            GameObject pawn = pawns[i];
            if (pawn == null)
            {
                continue;
            }

            SelectableCharacterEntity entity = pawn.GetComponent<SelectableCharacterEntity>();
            if (entity == null || entity.CharacterInfoInterlude == null)
            {
                continue;
            }

            Transform t = entity.transform;
            if (!IsNameplateVisible(cam, t))
            {
                continue;
            }

            CharSelectInfoPackage info = entity.CharacterInfoInterlude;
            if (string.IsNullOrEmpty(info.Name))
            {
                continue;
            }

            _paintList.Add(new PaintItem
            {
                Slot = i,
                World = L2OverheadAnchor.GetWorldPos(t),
                Name = info.Name,
                Color = ResolveNameColor(info.Karma)
            });
        }
    }

    private Color ResolveNameColor(int karma)
    {
        if (karma <= 0)
        {
            return _defaultNameColor;
        }

        float t = Mathf.Clamp01(karma / 1000f);
        return Color.Lerp(Color.white, new Color(1f, 0.25f, 0.25f, 1f), t);
    }

    private bool IsNameplateVisible(Camera cam, Transform target)
    {
        if (target == null || cam == null)
        {
            return false;
        }

        return Vector3.Distance(cam.transform.position, target.position) <= _nameplateViewDistance;
    }
}
