using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;

public partial class SkillbarSlot
{
    void ClearInnerSlot()
    {
        if (_innerSlot == null)
            return;

        if (_innerSlot is L2DraggableSlot)
            _innerSlot.ClearManipulators();
        else if (_innerSlot is L2ClickableSlot clickable)
            clickable.ClearManipulators();
        else
            _innerSlot.ClearManipulators();

        _innerSlot = null;
    }

    public void AssignItem(int objectId, int itemIdHint = 0)
    {
        ClearInnerSlot();
        // The bar can restore shortcuts before the inventory is bound (skillbar init runs early).
        PlayerInventory inventory = SkillbarPacketServices.Inventory;
        ItemInstance item = null;
        string error = null;
        if (inventory != null && !inventory.TryResolveShortcut(objectId, itemIdHint, out item, out error))
            item = null;
        if (!string.IsNullOrEmpty(error))
            Debug.LogError("[Inventory] " + error);
        _itemId = item != null ? item.ItemId : itemIdHint;
        _innerSlot = new InventorySlot(_position, _slotElement, SlotType.SkillBar);
        ((InventorySlot)_innerSlot).AssignItem(item);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }

    public void AssignSkill(int objectId , int level)
    {
        ClearInnerSlot();
        _itemId = 0;
        _innerSlot = new SkillSlot(_slotElement, _position, SlotType.SkillBar);
        ((SkillSlot)_innerSlot).AssignSkill(objectId , level , false);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }

    public void AssignAction(int objectId)
    {
        ClearInnerSlot();
        _itemId = 0;
        _innerSlot = new ActionSlot(_slotElement, _position, SlotType.SkillBar);
        ((ActionSlot)_innerSlot).AssignAction((ActionType)objectId);
        ((L2ClickableSlot)_innerSlot).UnregisterClickableCallback();

        UpdateInputInfo();
        RefreshMarks();
    }
}
