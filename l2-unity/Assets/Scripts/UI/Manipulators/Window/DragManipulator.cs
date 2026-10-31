using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Window drag in panel space (left/top). Skips <see cref="Button"/> picks under the
/// handle so title-bar close keeps <c>Clickable</c> (UITK pointer capture rules).
/// </summary>
public class DragManipulator : PointerManipulator
{
    readonly VisualElement _root;
    Vector2 _startMousePosition;
    Vector2 _startPosition;
    public bool dragged;

    public DragManipulator(VisualElement handle, VisualElement windowRoot)
    {
        target = handle;
        _root = windowRoot;
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

        target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
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
