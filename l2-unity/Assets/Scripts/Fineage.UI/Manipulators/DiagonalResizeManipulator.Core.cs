using UnityEngine.UIElements;
using UnityEngine;

public partial class DiagonalResizeManipulator : PointerManipulator
{
    public DiagonalResizeManipulator(
        VisualElement target, 
        VisualElement root, 
        float minWidth, 
        float maxWidth, 
        float minHeight, 
        float maxHeight, 
        float snapSize, 
        float snapOffset) {
        this.target = target;
        _root = root;
        _minWidth = minWidth;
        _maxWidth = maxWidth;
        _minHeight = minHeight;
        _maxHeight = maxHeight;
        _snapSize = snapSize;
        _snapOffset = snapOffset;
        _snap = true;
    }

    protected override void RegisterCallbacksOnTarget() {
        target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
        target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
    }

    protected override void UnregisterCallbacksFromTarget() {
        target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
        target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
        target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
    }
}
