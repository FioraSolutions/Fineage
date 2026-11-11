using System;

public partial class LobbyNameplatesManager
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            NameplatePacketServices.Bind(lobbyNameplates: this);
        }
        else
        {
            Destroy(this);
            return;
        }

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
        _pixelSnap.ClearAll();
        _batch.Dispose();

        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void EnsureResources()
    {
        if (_font == null)
        {
            _font = L2BitmapFont.LoadFromResources(_atlasResourcePath, _csvResourcePath);
            if (_font != null && !_loggedReady)
            {
                _loggedReady = true;
                // Debug.Log($"[LobbyNameplates] Font ready atlas={_font.AtlasWidth}x{_font.AtlasHeight} (screen-space batch)");
            }
        }

        if (_font != null)
        {
            _batch.EnsureMaterial(_font.Atlas);
        }
    }
}
