using UnityEngine;
using UnityEngine.UIElements;

public partial class MouseOverDetectionManipulator : PointerManipulator
{
    private bool _enabled = true;
    private bool _overThisManipulator = false;
    private L2UI _ui;
    private VisualElement _subElement;
    private bool _isSelectDropfield = false;
    public bool MouseOver => _overThisManipulator;





    //Deprecated not working Flexbox VisualElement
    //private bool IsVisibleAndContains(Vector2 pos, VisualElement el) =>
    //    el != null && el.resolvedStyle.display != DisplayStyle.None && el.worldBound.Contains(pos);











}
