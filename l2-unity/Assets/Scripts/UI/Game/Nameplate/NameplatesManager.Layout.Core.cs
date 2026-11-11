using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public partial class NameplatesManager : MonoBehaviour
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
}
