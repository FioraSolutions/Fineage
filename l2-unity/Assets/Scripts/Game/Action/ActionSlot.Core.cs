using System;
using UnityEngine.UIElements;

public partial class ActionSlot : L2DraggableSlot
{
    public ActionSlot(VisualElement slotElement, int position, SlotType slotType) : base(position, slotElement, slotType, true, false)
    {
        _slotElement = slotElement;
        _position = position;
    }

    protected override void HandleLeftClick()
    {
        ShortcutPacketServices.Actions.UseAction((ActionType)ActionId);
    }

    protected override void HandleRightClick()
    {
    }
}
