using UnityEngine;
using UnityEngine.UIElements;

public class SlotDragManipulator : PointerManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _grabOffset;
    private L2Slot _slot;
    public bool dragged = false;
    public bool enabled = true;

    public SlotDragManipulator(VisualElement target, L2Slot slot)
    {
        _slot = slot;
        _root = target;
        this.target = target;
    }

    public SlotDragManipulator(VisualElement target, L2Slot slot, bool isEnabled)
    {
        _slot = slot;
        _root = target;
        this.target = target;
        enabled = isEnabled;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    public void PointerDownHandler(PointerDownEvent evt)
    {
        if (!enabled)
            return;

        if (evt.button == 0)
        {
            UpdateFlotatingSlotAppearance();
            dragged = false;
            _startMousePosition = evt.position;
            // Keep the click point inside the slot under the cursor while dragging.
            _grabOffset = evt.localPosition;
            _root.CapturePointer(evt.pointerId);
        }
        evt.StopPropagation();
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (!enabled)
            return;

        if (_root.HasPointerCapture(evt.pointerId))
        {
            Vector2 pos = new Vector2(evt.position.x, evt.position.y);
            if (!dragged && (pos - _startMousePosition).sqrMagnitude < 9f)
            {
                evt.StopPropagation();
                return;
            }
            dragged = true;
            L2SlotManager.Instance.DragSlotAt(pos, _grabOffset);
        }
        evt.StopPropagation();
    }

    private void UpdateFlotatingSlotAppearance()
    {
        L2SlotManager.Instance.SetDraggedSlot(_slot);
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (_root.HasPointerCapture(evt.pointerId))
        {
            _root.ReleasePointer(evt.pointerId);
            if (dragged)
            {
                dragged = false;
                L2Slot drop = SlotHoverDetectManipulator.PickSlot(_root.panel, new Vector2(evt.position.x, evt.position.y));
                L2SlotManager.Instance.SetHoverSlot(drop);
                L2SlotManager.Instance.ReleaseDrag();
            }
        }
        evt.StopPropagation();
    }
}
