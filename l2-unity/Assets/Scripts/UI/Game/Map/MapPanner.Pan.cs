using UnityEngine;
using UnityEngine.UIElements;

public partial class MapPanner
{
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
