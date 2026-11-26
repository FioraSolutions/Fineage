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
        // Enter/Leave include descendants. Over/Out fire when the pointer moves
        // onto a child icon, which cleared Hovering and dropped mouse-up clicks
        // on ActionWnd slots whose icon covers the slot.
        _root.RegisterCallback<PointerEnterEvent>(PointerEnterHandler);
        _root.RegisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        if (_byElement.TryGetValue(_root, out L2Slot cur) && cur == _slot)
            _byElement.Remove(_root);
        _root.UnregisterCallback<PointerEnterEvent>(PointerEnterHandler);
        _root.UnregisterCallback<PointerLeaveEvent>(PointerLeaveHandler);
    }


    public void PointerEnterHandler(PointerEnterEvent evt)
    {
        Hovering = true;
        HudCorePacketServices.Slots.SetHoverSlot(_slot);
    }

    public void PointerLeaveHandler(PointerLeaveEvent evt)
    {
        Hovering = false;
        HudCorePacketServices.Slots.SetHoverSlot(null);
    }
}