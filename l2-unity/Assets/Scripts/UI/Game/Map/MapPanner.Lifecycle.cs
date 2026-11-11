using UnityEngine.UIElements;
using UnityEngine;

public partial class MapPanner
{
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

    void MoveMarkerToPoint(MouseUpEvent up)
    {
        if (_isDisabled) return;
        MapUtils.PanViewportToMarker(ref offset, _map, _viewport, _marker);
    }
}
