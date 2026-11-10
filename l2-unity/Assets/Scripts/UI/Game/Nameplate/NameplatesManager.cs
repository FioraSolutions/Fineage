using System;

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// World HeadDisplay orchestrator — L2 canvas path: Project → screen-pixel glyphs/quads →
/// <see cref="L2NameplateScreenBatch"/>. Stack: title → name → cast gauge → bubbles.
/// World Y from <see cref="L2OverheadAnchor"/> (one-shot height, locked to root — no live bone).
/// </summary>
public class NameplatesManager : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _nameplateViewDistance = 50f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private Color _defaultNameColor = Color.white;
    [SerializeField] private float _pixelScale = 1f;
    [Tooltip("L2 canvas: lock plate to whole screen pixels (hysteresis).")]
    [SerializeField] private bool _snapAnchorToPixels = true;
    [SerializeField] private float _snapHysteresisPx = 0.75f;
    [SerializeField] private bool _snapDiscardSubpixelFrac = true;
    [SerializeField] private bool _drawTitles = true;
    [SerializeField] private float _titleGapPixels = 2f;
    [SerializeField] private bool _drawBubbles = true;
    [SerializeField] private string _atlasResourcePath = "Data/UI/Font/L2Lobby/l2_lobby_font_atlas";
    [SerializeField] private string _csvResourcePath = "Data/UI/Font/L2Lobby/ul2font_ascii";

    [SerializeField] public Collider[] _entitiesInRange;

    private readonly List<NameplatePaintItem> _paintList = new List<NameplatePaintItem>(64);

    private NameplateBubbleResolver _bubbleResolver;
    private NameplateEntryStore _entryStore;
    private NameplatePixelSnap _pixelSnap;
    private NameplateTextDrawer _textDrawer;
    private NameplateBubbleDrawer _bubbleDrawer;
    private NameplateCastBarDrawer _castBarDrawer;

    private Transform _playerTransform;
    private bool _subscribed;

    private static NameplatesManager _instance;

    [System.Obsolete("Use NameplatePacketServices.Nameplates or [Inject] NameplatesManager instead")]
    public static NameplatesManager Instance => _instance;

    public Camera Camera
    {
        get => _camera;
        set => _camera = value;
    }

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

    private Camera ResolveCamera()
    {
        return _camera != null ? _camera : Camera.main;
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            if (DomainPacketServices.LocalPlayer != null && DomainPacketServices.LocalPlayer.transform != null)
            {
                _playerTransform = DomainPacketServices.LocalPlayer.transform;
            }
            else
            {
                return;
            }
        }

        if (_entryStore == null || _pixelSnap == null)
            return;

        _entitiesInRange = Physics.OverlapSphere(
            _playerTransform.position,
            _nameplateViewDistance,
            _entityMask);

        _entryStore.Discover(_entitiesInRange, _defaultNameColor);

        // Remote users (CharInfo/UserEntity) move via L2PawnWalk with their
        // CharacterController disabled, so OverlapSphere cannot find them. Upsert
        // every registered user directly from World so their nameplates stay visible
        // even when not hovered. Distance cull still happens in RefreshVisibility.
        World world = WorldApplyPacketServices.GameWorld;
        if (world != null)
        {
            world.ForEachUser(user => _entryStore.UpsertEntry(user, _defaultNameColor));
            // NPCs/monsters whose click_area is outside OverlapSphere (or missing) still need plates (#930).
            world.ForEachNpc(npc => _entryStore.UpsertEntry(npc, _defaultNameColor));
        }

        _entryStore.EnsureHoverAndTarget(_entityMask, _defaultNameColor);

        if (DomainPacketServices.LocalPlayer != null)
        {
            _entryStore.UpsertEntry(DomainPacketServices.LocalPlayer, _defaultNameColor);
        }

        _entryStore.RefreshVisibility(_playerTransform, _nameplateViewDistance, _pixelSnap);
    }

    private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
    {
        Camera targetCam = ResolveCamera();
        if (cam == null || targetCam == null || cam != targetCam)
        {
            return;
        }

        EnsureResources();
        if (!_textDrawer.IsReady)
        {
            return;
        }

        _entryStore.BuildPaintList(_paintList, _drawTitles);
        if (_paintList.Count == 0)
        {
            return;
        }

        _textDrawer.Draw(cam, _paintList, _pixelSnap);
        _castBarDrawer?.Draw(cam, _paintList);
        _bubbleDrawer.Draw(cam, _paintList);
    }
}
