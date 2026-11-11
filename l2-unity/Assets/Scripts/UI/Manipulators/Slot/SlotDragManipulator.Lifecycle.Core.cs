using UnityEngine;
using UnityEngine.UIElements;

public partial class SlotDragManipulator : PointerManipulator
{
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
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        target.RegisterCallback<PointerCancelEvent>(PointerCancelHandler, TrickleDown.TrickleDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        UnregisterPanelFallback();
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
        target.UnregisterCallback<PointerCancelEvent>(PointerCancelHandler, TrickleDown.TrickleDown);
    }
}
