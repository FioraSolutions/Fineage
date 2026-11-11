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
}
