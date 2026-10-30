using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Drags a window in UI Toolkit panel space. Uses left/top only so the grab point
/// stays under the cursor (no Screen.width/height mix, no mid-drag anchor flips).
/// </summary>
public class DragManipulator : PointerManipulator
{
    private VisualElement _root;
    private Vector2 _startMousePosition;
    private Vector2 _startPosition;
    public bool dragged = false;

    public DragManipulator(VisualElement target, VisualElement root)
    {
        this.target = target;
        _root = root;
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
        if (evt.button != 0)
            return;

        dragged = false;
        _startMousePosition = evt.position;
        // layout.x/y are parent-local; pointer events are panel coords. Fineage roots
        // sit at the panel origin, so the delta matches 1:1 for absolute children.
        _startPosition = new Vector2(_root.layout.x, _root.layout.y);
        target.CapturePointer(evt.pointerId);
        evt.StopPropagation();
    }

    public void PointerMoveHandler(PointerMoveEvent evt)
    {
        if (!target.HasPointerCapture(evt.pointerId))
        {
            evt.StopPropagation();
            return;
        }

        dragged = true;
        Vector2 delta = new Vector2(evt.position.x, evt.position.y) - _startMousePosition;
        _root.style.left = _startPosition.x + delta.x;
        _root.style.top = _startPosition.y + delta.y;
        _root.style.right = StyleKeyword.Null;
        _root.style.bottom = StyleKeyword.Null;
        evt.StopPropagation();
    }

    public void PointerUpHandler(PointerUpEvent evt)
    {
        if (target.HasPointerCapture(evt.pointerId))
            target.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
    }
}
