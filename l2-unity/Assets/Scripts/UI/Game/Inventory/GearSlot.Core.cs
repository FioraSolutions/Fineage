using UnityEngine;
using UnityEngine.UIElements;

public partial class GearSlot : InventorySlot
{
    public GearSlot(int position, VisualElement slotElement, InventoryGearTab tab, SlotType slotType) : base(position, slotElement, tab, slotType, false, true)
    {
        _blackElement = CreateBlack(_blackElementName);
    }

    protected override void HandleLeftClick()
    {
        // A completed drag is not a click: the drop already equipped/unequipped it.
        if (_slotDragManipulator != null && (_slotDragManipulator.dragged || _slotDragManipulator.DragCompleted))
            return;
        base.HandleLeftClick();
    }

    protected override void HandleRightClick()
    {
        if (TryGiveToEnchant())
            return;
        UnequipWorn();
    }

    public override void UseItem()
    {
        UnequipWorn();
    }
}
