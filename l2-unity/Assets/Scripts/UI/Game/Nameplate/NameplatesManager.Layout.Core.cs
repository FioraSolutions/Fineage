using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public partial class NameplatesManager : MonoBehaviour, INameplateRoster
{
    public void Remove(int id)
    {
        _entryStore?.Remove(id, _pixelSnap);
    }

    private void ApplySettingsToSubsystems()
    {
        if (_pixelSnap == null || _textDrawer == null || _bubbleDrawer == null)
            return;

        _pixelSnap.HysteresisPx = _snapHysteresisPx;
        _textDrawer.ConfigurePaths(_atlasResourcePath, _csvResourcePath);
        _textDrawer.PixelScale = _pixelScale;
        _textDrawer.TitleGapPixels = _titleGapPixels;
        _textDrawer.SnapAnchorToPixels = _snapAnchorToPixels;
        _textDrawer.SnapDiscardSubpixelFrac = _snapDiscardSubpixelFrac;
        _bubbleDrawer.Enabled = _drawBubbles;
    }

    private void EnsureResources()
    {
        if (_pixelSnap == null || _textDrawer == null || _bubbleDrawer == null || _entryStore == null)
            return;

        ApplySettingsToSubsystems();
        _textDrawer.EnsureResources();
        _bubbleDrawer.EnsureResources();
        _castBarDrawer?.EnsureResources();
    }

    /// <summary>
    /// Projects the shared overhead anchor to the name-line row for screen-space say UI.
    /// </summary>
    public bool TryResolveOverheadSayScreenPos(
        Camera cam,
        Entity entity,
        Transform root,
        out Vector2 screenBottomLeft)
    {
        screenBottomLeft = default;
        if (cam == null || entity == null || root == null)
            return false;

        Vector3 world = L2OverheadAnchor.GetWorldPos(entity, root);
        Vector3 screen = cam.WorldToScreenPoint(world);
        if (screen.z <= 0f)
            return false;

        float lineH = _textDrawer != null
            ? _textDrawer.MeasureNameLineHeightPx()
            : L2NameplateScreenLayout.FallbackLineHeightPx * (_pixelScale > 0f ? _pixelScale : 1f);

        float ax = screen.x;
        float ay = screen.y;
        if (_snapAnchorToPixels && _pixelSnap != null && entity.Identity != null)
        {
            float orbitDist = 0f;
            CameraController cameraController = CameraPacketServices.Controller;
            if (cameraController != null)
                orbitDist = cameraController.CurrentDistance;

            bool isLocal = DomainPacketServices.LocalPlayer != null && entity == DomainPacketServices.LocalPlayer;
            float snapDist = (isLocal && orbitDist > 0.05f) ? orbitDist : screen.z;
            int id = entity.Identity.Id;
            ax = _pixelSnap.Snap(id, screen.x, true, snapDist);
            ay = _pixelSnap.Snap(id, screen.y, false, snapDist);
        }

        screenBottomLeft = L2NameplateScreenLayout.ResolveSayBottomLeft(
            cam.pixelHeight, ax, ay, lineH);
        return true;
    }
}
