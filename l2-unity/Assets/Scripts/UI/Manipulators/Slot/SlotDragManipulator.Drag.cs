using UnityEngine;
using UnityEngine.UIElements;

public partial class SlotDragManipulator : PointerManipulator
{
    void PanelPointerMove(PointerMoveEvent evt)
    {
        HandleMove(evt.pointerId, evt.position, evt);
    }

    void HandleMove(int pointerId, Vector3 position, EventBase evt)
    {
        if (!enabled || _finishing || !_pointerDown)
            return;
        if (_activePointerId != pointerId)
            return;

        Vector2 pos = new Vector2(position.x, position.y);
        if (!dragged && (pos - _startMousePosition).sqrMagnitude < DragThresholdSqr)
            return;

        if (!dragged)
        {
            if (HudCorePacketServices.Slots == null)
                return;
            dragged = true;
            if (_root != null && !_root.HasPointerCapture(pointerId))
                _root.CapturePointer(pointerId);
            HudCorePacketServices.Slots.SetDraggedSlot(_slot);
        }

        HudCorePacketServices.Slots.DragSlotAt(pos, _grabOffset);
        evt.StopPropagation();
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        HandleUp(evt.pointerId, evt.position, evt);
    }

    void PanelPointerUp(PointerUpEvent evt)
    {
        HandleUp(evt.pointerId, evt.position, evt);
    }

    void HandleUp(int pointerId, Vector3 position, EventBase evt)
    {
        if (_activePointerId != pointerId)
            return;

        Vector2 pos = new Vector2(position.x, position.y);
        bool wasDragging = dragged;
        FinishPointerGesture(pos);
        if (wasDragging)
            evt.StopPropagation();
    }

    void PointerCancelHandler(PointerCancelEvent evt)
    {
        if (_activePointerId != evt.pointerId)
            return;
        FinishPointerGesture(null);
    }

    void PanelPointerCancel(PointerCancelEvent evt)
    {
        if (_activePointerId != evt.pointerId)
            return;
        FinishPointerGesture(null);
    }

    void PointerCaptureOutHandler(PointerCaptureOutEvent evt)
    {
        // Capture is taken only after drag starts; unexpected loss must drop the ghost icon.
        if (_finishing || !dragged)
            return;
        FinishPointerGesture(null);
    }

    void FinishPointerGesture(Vector2? panelPos)
    {
        if (_finishing)
            return;
        if (!_pointerDown && !dragged)
            return;

        _finishing = true;

        bool wasDragging = dragged;
        dragged = false;
        DragCompleted = wasDragging;
        _pointerDown = false;
        int pointerId = _activePointerId;
        _activePointerId = -1;
        UnregisterPanelFallback();

        if (pointerId >= 0 && _root != null && _root.HasPointerCapture(pointerId))
            _root.ReleasePointer(pointerId);

        L2SlotManager manager = HudCorePacketServices.Slots;
        if (manager == null)
        {
            _finishing = false;
            return;
        }

        if (wasDragging)
        {
            if (panelPos.HasValue && _root != null && _root.panel != null)
            {
                L2Slot drop = SlotHoverDetectManipulator.PickSlot(_root.panel, panelPos.Value);
                manager.SetHoverSlot(drop);
            }
            manager.ReleaseDrag();
        }
        else
        {
            manager.CancelDrag();
        }

        _finishing = false;
    }
}
