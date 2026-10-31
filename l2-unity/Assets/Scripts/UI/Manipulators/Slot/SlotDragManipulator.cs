using UnityEngine;
using UnityEngine.UIElements;

public class SlotDragManipulator : PointerManipulator
{
    // ~8px — clicks stay clicks; intentional drags still start quickly.
    const float DragThresholdSqr = 64f;

    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _grabOffset;
    private L2Slot _slot;
    private int _activePointerId = -1;
    private bool _pointerDown;
    private bool _finishing;
    private bool _panelListening;
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
            if (L2SlotManager.Instance == null)
                return;
            dragged = true;
            if (_root != null && !_root.HasPointerCapture(pointerId))
                _root.CapturePointer(pointerId);
            L2SlotManager.Instance.SetDraggedSlot(_slot);
        }

        L2SlotManager.Instance.DragSlotAt(pos, _grabOffset);
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
        _pointerDown = false;
        int pointerId = _activePointerId;
        _activePointerId = -1;
        UnregisterPanelFallback();

        if (pointerId >= 0 && _root != null && _root.HasPointerCapture(pointerId))
            _root.ReleasePointer(pointerId);

        L2SlotManager manager = L2SlotManager.Instance;
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
