using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SlotHoverDetectManipulator : PointerManipulator
{
    private L2Slot _slot;
    private VisualElement _root;
    public bool Hovering { get; private set; }

    public SlotHoverDetectManipulator(VisualElement target, L2Slot slot)
    {
        _slot = slot;
        _root = target;
        this.target = target;
    }

    static readonly Dictionary<VisualElement, L2Slot> _byElement = new Dictionary<VisualElement, L2Slot>();

    /// <summary>Slot under a panel position (pointer capture during drag suppresses PointerOver).</summary>
    public static L2Slot PickSlot(IPanel panel, Vector2 panelPos)
    {
        if (panel == null)
            return null;
        VisualElement ve = panel.Pick(panelPos);
        while (ve != null)
        {
            if (_byElement.TryGetValue(ve, out L2Slot slot))
                return slot;
            ve = ve.parent;
        }
        return null;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        _byElement[_root] = _slot;
        _root.RegisterCallback<PointerOverEvent>(PointerOverHandler);
        _root.RegisterCallback<PointerOutEvent>(PointerOutHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        if (_byElement.TryGetValue(_root, out L2Slot cur) && cur == _slot)
            _byElement.Remove(_root);
        _root.UnregisterCallback<PointerOverEvent>(PointerOverHandler);
        _root.UnregisterCallback<PointerOutEvent>(PointerOutHandler);
    }


    public void PointerOverHandler(PointerOverEvent evt)
    {
        Hovering = true;
        L2SlotManager.Instance.SetHoverSlot(_slot);
    }

    public void PointerOutHandler(PointerOutEvent evt)
    {
        Hovering = false;
        L2SlotManager.Instance.SetHoverSlot(null);
    }
}