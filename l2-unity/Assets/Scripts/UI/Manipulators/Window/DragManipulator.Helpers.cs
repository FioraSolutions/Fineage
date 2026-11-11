using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class DragManipulator : PointerManipulator
{
    void OnPointerMove(PointerMoveEvent evt)
    {
        if (!target.HasPointerCapture(evt.pointerId))
            return;

        dragged = true;
        Vector2 delta = (Vector2)evt.position - _startMousePosition;
        _root.style.left = _startPosition.x + delta.x;
        _root.style.top = _startPosition.y + delta.y;
        _root.style.right = StyleKeyword.Null;
        _root.style.bottom = StyleKeyword.Null;
        evt.StopPropagation();
    }

    void OnPointerUp(PointerUpEvent evt)
    {
        if (!target.HasPointerCapture(evt.pointerId) || !CanStopManipulation(evt))
            return;

        bool endedDrag = dragged;
        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
        if (endedDrag)
            _onDragEnded?.Invoke();
    }

    void OnPointerCaptureOut(PointerCaptureOutEvent _)
    {
        dragged = false;
    }

    bool IsUnderButton(VisualElement ve)
    {
        for (; ve != null && ve != target; ve = ve.parent)
        {
            if (ve is Button)
                return true;
        }
        return false;
    }
}
