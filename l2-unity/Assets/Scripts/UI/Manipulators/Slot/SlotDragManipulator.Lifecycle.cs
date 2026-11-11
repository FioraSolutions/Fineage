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

    void RegisterPanelFallback()
    {
        if (_panelListening)
            return;
        VisualElement panelRoot = _root?.panel?.visualTree;
        if (panelRoot == null)
            return;
        panelRoot.RegisterCallback<PointerMoveEvent>(PanelPointerMove, TrickleDown.TrickleDown);
        panelRoot.RegisterCallback<PointerUpEvent>(PanelPointerUp, TrickleDown.TrickleDown);
        panelRoot.RegisterCallback<PointerCancelEvent>(PanelPointerCancel, TrickleDown.TrickleDown);
        _panelListening = true;
    }

    void UnregisterPanelFallback()
    {
        if (!_panelListening)
            return;
        VisualElement panelRoot = _root?.panel?.visualTree;
        if (panelRoot != null)
        {
            panelRoot.UnregisterCallback<PointerMoveEvent>(PanelPointerMove, TrickleDown.TrickleDown);
            panelRoot.UnregisterCallback<PointerUpEvent>(PanelPointerUp, TrickleDown.TrickleDown);
            panelRoot.UnregisterCallback<PointerCancelEvent>(PanelPointerCancel, TrickleDown.TrickleDown);
        }
        _panelListening = false;
    }

    public void PointerDownHandler(PointerDownEvent evt)
    {
        if (!enabled || evt.button != 0)
            return;

        dragged = false;
        _finishing = false;
        _pointerDown = true;
        DragCompleted = false;
        _activePointerId = evt.pointerId;
        _startMousePosition = evt.position;
        _grabOffset = evt.localPosition;
        // Do NOT capture yet — Button.Clickable / click handlers must own the click.
        // Capture only after the drag threshold so the floating icon cannot stick on click.
        RegisterPanelFallback();
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        HandleMove(evt.pointerId, evt.position, evt);
    }
}
