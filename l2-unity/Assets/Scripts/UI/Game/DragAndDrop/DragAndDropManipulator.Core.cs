using UnityEngine;
using UnityEngine.UIElements;

public partial class DragAndDropManipulator : PointerManipulator
{
    public DragAndDropManipulator(VisualElement target , DragAndDropManager manager)
    {
        this.target = target;
        this.manager = manager;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        // Register the four callbacks on target.
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
        target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        // Un-register the four callbacks from target.
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
        target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
    }

    private void PointerDownHandler(PointerDownEvent evt)
    {
        targetStartPosition = target.transform.position;
        pointerStartPosition = evt.position;
        target.CapturePointer(evt.pointerId);
        var vector = new Vector2(target.worldBound.position.x , target.worldBound.position.y);
        SetActiveToManager((VisualElement)evt.currentTarget);
        DragIconPacketServices.Icon.SetBackground(manager.GetActiveBackground());
        DragIconPacketServices.Icon.NewPosition(vector);

        enabled = true;
    }
}
