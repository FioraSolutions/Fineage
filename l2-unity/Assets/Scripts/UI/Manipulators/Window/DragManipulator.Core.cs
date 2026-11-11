using System;
using UnityEngine;
using UnityEngine.UIElements;

public partial class DragManipulator : PointerManipulator
{
    public DragManipulator(VisualElement handle, VisualElement windowRoot, Action onDragEnded = null)
    {
        target = handle;
        _root = windowRoot;
        _onDragEnded = onDragEnded;
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown, TrickleDown.TrickleDown);
        target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
        target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
    }

    void OnPointerDown(PointerDownEvent evt)
    {
        if (!CanStartManipulation(evt) || IsUnderButton(evt.target as VisualElement))
            return;

        dragged = false;
        _startMousePosition = evt.position;
        _startPosition = new Vector2(_root.layout.x, _root.layout.y);
        target.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }
}
