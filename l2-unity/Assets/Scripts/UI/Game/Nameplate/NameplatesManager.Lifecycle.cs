using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public partial class NameplatesManager : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            NameplatePacketServices.Bind(nameplates: this);
        }
        else
        {
            Destroy(this);
            return;
        }

        _bubbleResolver = new NameplateBubbleResolver();
        _entryStore = new NameplateEntryStore(_bubbleResolver);
        _pixelSnap = new NameplatePixelSnap();
        _textDrawer = new NameplateTextDrawer();
        _bubbleDrawer = new NameplateBubbleDrawer();
        _castBarDrawer = new NameplateCastBarDrawer();

        ApplySettingsToSubsystems();
        EnsureResources();
    }

    private void OnEnable()
    {
        if (!_subscribed)
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            _subscribed = true;
        }
    }

    private void OnDisable()
    {
        if (_subscribed)
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            _subscribed = false;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
        _entryStore?.Clear();
        _pixelSnap?.ClearAll();
        _textDrawer?.Dispose();
        _bubbleDrawer?.Dispose();
        _castBarDrawer?.Dispose();

        if (_instance == this)
        {
            _instance = null;
        }
    }

    public void SetMask(LayerMask mask)
    {
        _entityMask = mask;
    }
}
