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

        if (!L2MapLocalPlayer.TryMapPlaneFromLocalPlayer(out Vector2 l2XY))
        {
            Debug.LogError("[MapPanner] Local player Identity missing; cannot project world-map marker.");
            return;
        }

        Vector2 mapPx = DetailedMapProjection.ProjectWorldMapPixel(l2XY.x, l2XY.y);

        MapUtils.PlaceMarkerOnMap(ref offset, _map, _marker, mapPx, MapPixelSize);
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
