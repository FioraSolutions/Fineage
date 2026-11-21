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

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);


        VisualElement host = (VisualElement)GetElementById("map-host");
        if (_mapTemplate != null && host != null)
            host.Add(_mapTemplate.Instantiate());

        Button button = (Button)GetElementById("MyLocation");
        VisualElement viewport = host.Q("viewport");
        VisualElement map = host.Q("map");
        VisualElement marker = host.Q("minimapPos");
        _mapPanner.SetElements(viewport, map, marker, button);
        _mapPanner.RegisterCallback();

        _detailedMap = new DetailedMapOverlay(map, host.Q("town-map-layer"));

        _mapMarkerFollowCamera.SetElement(marker);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);
        L2InterludeHudSkinPipeline.ApplyMinimapLayout(_windowEle);
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
        _mapPanner.MoveMarkerToOrigin();
        _detailedMap?.Close();
        base.HideWindow();
    }
}

