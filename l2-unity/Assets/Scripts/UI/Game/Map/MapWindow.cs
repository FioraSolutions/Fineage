using System;

﻿using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class MapWindow : L2PopupWindow
{
    private static MapWindow _instance;
    private const string _mapTemplateName = "Data/UI/_Elements/Game/Map/MapAdenTemplate";
    private VisualTreeAsset _mapTemplate;
    private MapPanner _mapPanner;
    private MapMarkerFollowCamera _mapMarkerFollowCamera;
    private DetailedMapOverlay _detailedMap;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MapWindow Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _mapPanner = new MapPanner();
            _mapMarkerFollowCamera = new MapMarkerFollowCamera();
            _instance = this;
            WireHudCorePacketServices();

        }
        else
        {
            Destroy(this);
        }
    }

    
    private void Start()
    {
        WireHudCorePacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(map: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Map/MapWindow");
        _mapTemplate = LoadAsset(_mapTemplateName);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        InterludeWindowLayout.Apply(_windowEle, "MinimapWnd");
        _windowEle.pickingMode = PickingMode.Position;

        VisualElement minimap = GetElementById("Minimap");
        VisualElement host = GetElementById("map-host");
        if (minimap == null || host == null)
        {
            Debug.LogError("[MapWindow] Minimap or map-host is missing from the extracted layout.");
            yield break;
        }

        minimap.pickingMode = PickingMode.Position;
        minimap.Add(host);
        host.style.position = Position.Absolute;
        host.style.left = 0f;
        host.style.top = 0f;
        host.style.right = 0f;
        host.style.bottom = 0f;
        if (_mapTemplate != null)
            host.Add(_mapTemplate.Instantiate());

        VisualElement viewport = host.Q("viewport");
        if (viewport != null)
        {
            viewport.style.width = Length.Percent(100f);
            viewport.style.height = Length.Percent(100f);
        }

        Button button = GetElementById("MyLocButton") as Button;
        VisualElement map = host.Q("map");
        VisualElement marker = host.Q("minimapPos");
        if (viewport == null || map == null || marker == null || button == null)
        {
            Debug.LogError("[MapWindow] map viewport, marker, or MyLocButton is missing.");
        }
        else
        {
            _mapPanner.SetElements(viewport, map, marker, button);
            _mapPanner.RegisterCallback();
            _detailedMap = new DetailedMapOverlay(map, host.Q("town-map-layer"));
            _mapMarkerFollowCamera.SetElement(marker);
        }

        VisualElement dragArea = GetElementByClass("drag-area");
        if (dragArea == null)
            dragArea = _windowEle;
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        RegisterClickWindowEvent(_windowEle, dragArea);
        L2InterludeHudSkinPipeline.ValidateTextures(
            InterludeHudWindowCatalog.MinimapWnd,
            nameof(MapWindow));

        yield return new WaitForEndOfFrame();

  
    }

    public void LateUpdate()
    {
        if (_mapPanner == null)
            return;
        _mapPanner.UpdateMarker(_isWindowHidden);
        _detailedMap?.UpdateMarkers();
    }

    public void TurnMarker(Camera targetCamera)
    {
        _mapMarkerFollowCamera.TurnUpdate(targetCamera , _isWindowHidden);
    }

    public override void ShowWindow()
    {
        _mapPanner.SetDisabled(false);
        _mapPanner.SetManualUpdate(true);
        _mapMarkerFollowCamera.ManualTurnUpdate();
        _detailedMap?.TryAutoOpenForPlayer();
        base.ShowWindow();
    }

    public override void HideWindow()
    {
        _mapPanner.SetDisabled(true);
        if (_detailedMap != null)
            _mapPanner.MoveMarkerToOrigin();
        _detailedMap?.Close();
        base.HideWindow();
    }
}

