using UnityEngine;
using UnityEngine.UIElements;

public partial class MapPanner
{
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
