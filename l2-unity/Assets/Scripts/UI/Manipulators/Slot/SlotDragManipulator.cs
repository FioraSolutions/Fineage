using UnityEngine;
using UnityEngine.UIElements;

public partial class SlotDragManipulator : PointerManipulator
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

    /// <summary>
    /// True from the moment a real drag completes until the next press. The click callbacks fire after
    /// <see cref="PointerUpHandler"/> has already reset <see cref="dragged"/>, so slot click handlers use
    /// this to tell "the user dragged me somewhere" from "the user clicked me".
    /// </summary>
    public bool DragCompleted { get; private set; }

}
