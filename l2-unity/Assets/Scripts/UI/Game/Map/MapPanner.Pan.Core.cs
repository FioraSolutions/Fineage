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

        if (!L2MapCoordinates.TryMapPlaneFromLocalPlayer(out Vector2 l2XY))
        {
            PlayerController player = ControllerPacketServices.Player;
            l2XY = player != null
                ? L2MapCoordinates.MapPlaneFromUnity(player.GetPlayerPosition())
                : Vector2.zero;
        }

        Vector2 mapPx = DetailedMapProjection.ProjectWorldMapPixel(l2XY.x, l2XY.y);

        MapUtils.PlaceMarkerOnMap(ref offset, _map, _marker, mapPx, mapSizeD);
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
}
