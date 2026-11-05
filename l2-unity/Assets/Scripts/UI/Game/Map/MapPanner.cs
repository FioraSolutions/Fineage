using UnityEngine;
using UnityEngine.UIElements;

public class MapPanner 
{
    private VisualElement _viewport;
    private VisualElement _map;
    private VisualElement _marker;

    private Vector2 lastPointer;
    private Vector2 offset; 

    private bool dragging;

    private const float VIEW_W = 294f;
    private const float VIEW_H = 300f;
    private const float MAP_W = 1812f;
    private const float MAP_H = 2620f;
    public const float MapWidthPixels = MAP_W;
    public const float MapHeightPixels = MAP_H;
    private Button _buttonLocation;
    private bool _isDisabled = false;
    private bool _isManualUpdate = false;

    private Vector2 _mapPos = Vector2.zero;
    // Pixel size of the map image, used verbatim by PlaceMarkerOnMap.
    Vector2 mapSizeD = new Vector2(MAP_W, MAP_H);

    // L2 world rectangle covered by the 1812x2620 world map art. Calibrated against the town icon
    // positions on the art using the real teleporter coordinates of Aden, Giran, Oren, Dion,
    // Goddard, Rune, Orc Village, Dwarven Village and Talking Island (least squares: residual
    // ~9 px on x, ~12 px on y). The art's x axis follows L2 x and its y axis follows L2 y.
    public const float MapL2MinX = -125577f;
    public const float MapL2MaxX = 196500f;
    public const float MapL2MinY = -264300f;
    public const float MapL2MaxY = 267788f;


    public void SetElements(VisualElement viewport, VisualElement map, VisualElement minmapElement , Button buttonLocation)
    {
        _viewport = viewport;
        _viewport.pickingMode = PickingMode.Position;
        _marker = minmapElement;
        _map = map;
        _buttonLocation = buttonLocation;
        _buttonLocation.RegisterCallback<MouseUpEvent>(MoveMarkerToPoint);
        _marker.RegisterCallback<GeometryChangedEvent>(OnMarkerGeometryChanged);
        _viewport.RegisterCallback<GeometryChangedEvent>(MoveMarkerToPoint);

        viewport.RegisterCallback<PointerDownEvent>(evt => {
            dragging = true;
            lastPointer = evt.localPosition; // инициализируем, чтобы избежать прыжка
            viewport.CapturePointer(evt.pointerId);
        });

    }

    public void SetManualUpdate(bool isManualUpdate)
    {
        _isManualUpdate = isManualUpdate;
    }

    public void SetDisabled(bool disabled)
    {
        _isDisabled = disabled;
    }
    public void RegisterCallback()
    {

        _viewport.RegisterCallback<PointerDownEvent>(OnPointerDown);
        _viewport.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        _viewport.RegisterCallback<PointerUpEvent>(OnPointerUp);
        _viewport.RegisterCallback<PointerCancelEvent>(OnPointerCancel);

    }

    void UnRegisterCallback()
    {
        if (_viewport != null)
        {
            _viewport.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            _viewport.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            _viewport.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            _viewport.UnregisterCallback<PointerCancelEvent>(OnPointerCancel);
        }
    }



    void MoveMarkerToPoint(GeometryChangedEvent up)
    {
        MoveMarker(false);

    }

    //event click button Location
    void MoveMarkerToPoint(MouseUpEvent up)
    {
        if (_isDisabled) return;
        MapUtils.PanViewportToMarker(ref offset, _map, _viewport, _marker);
    }

    public void UpdateMarker(bool isHideWindow)
    {
        MoveMarker(isHideWindow);
    }

    private void MoveMarker(bool isHideWindow)
    {
        if (_map == null || _marker == null || _isDisabled || isHideWindow) return;

        Vector3 worldPos = ControllerPacketServices.Player != null
            ? ControllerPacketServices.Player.GetPlayerPosition()
            : Vector3.zero;

        // ConvertPosToUnity packs L2 (x, y, z) into Unity (z, x, y) / 52.5, so the L2 pair the
        // world map is drawn on comes back as Unity (z, x).
        float l2X = worldPos.z * 52.5f;
        float l2Y = worldPos.x * 52.5f;

        // The world map art spans the L2 world rectangle below. Map x grows with L2 x and map y
        // grows with L2 y (Sea of Spores at y≈20k sits above Elven Village at y≈51k), so only a
        // plain normalisation is needed. The previous code additionally subtracted 10 px on both
        // axes and relied on the marker's 0.5 pivot, which parked the icon off its true spot.
        float mapX = Mathf.InverseLerp(MapL2MinX, MapL2MaxX, l2X) * MAP_W;
        float mapY = Mathf.InverseLerp(MapL2MinY, MapL2MaxY, l2Y) * MAP_H;

        MapUtils.PlaceMarkerOnMap(ref offset, _map, _marker, new Vector2(mapX, mapY), mapSizeD);
    }

    private void OnMarkerGeometryChanged(GeometryChangedEvent evt)
    {
        if(_isDisabled) return;

        if(evt.newRect.x == 0) return;

        if (_isManualUpdate == true)
        {
            _isManualUpdate = false;
            MapUtils.PanViewportToMarker(ref offset, _map, _viewport, _marker);
        }

    }

    void OnPointerDown(PointerDownEvent evt)
    {

       
        _viewport.CapturePointer(evt.pointerId);
        dragging = true;
        lastPointer = evt.localPosition;
        evt.StopPropagation();
    }

    void OnPointerMove(PointerMoveEvent evt)
    {
       
        if (!dragging) return;

        Vector2 local = evt.localPosition;
        Vector2 delta = local - lastPointer;


        offset -= delta;


        float maxX = Mathf.Max(0f, MAP_W - VIEW_W);
        float maxY = Mathf.Max(0f, MAP_H - VIEW_H);
        offset.x = Mathf.Clamp(offset.x, 0f, maxX);
        offset.y = Mathf.Clamp(offset.y, 0f, maxY);


        _map.style.left = -offset.x;
        _map.style.top = -offset.y;

        lastPointer = local;
        evt.StopPropagation();
    }



void OnPointerUp(PointerUpEvent evt)
    {
       
        if (dragging)
        {
            _viewport.ReleasePointer(evt.pointerId);
            dragging = false;
            evt.StopPropagation();
        }
    }

    void OnPointerCancel(PointerCancelEvent evt)
    {
        if (dragging)
        {
            
            _viewport.ReleasePointer(evt.pointerId);
            dragging = false;
            evt.StopPropagation();
        }
    }

    public void MoveMarkerToOrigin()
    {
        _marker.style.left = 0;
        _marker.style.top = 0;

        _viewport.style.left = 0;
        _viewport.style.top = 0;
    }

}