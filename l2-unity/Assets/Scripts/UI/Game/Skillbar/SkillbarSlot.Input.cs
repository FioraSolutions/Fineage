using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class SkillbarSlot
{
    public SkillbarSlot(VisualElement slotElement, int position, int skillbarId, int slot) : base(slotElement, position, SlotType.SkillBar, true, false)
    {
        _slotElement = slotElement;
        _position = position;
        _skillbarId = skillbarId;
        _slot = slot;
        _keyElement = _slotElement.Q<VisualElement>("Key");
        _reuseElement = _slotElement.Q<VisualElement>("ReuseBg");
        _rechargeMaskBg = _slotElement.Q<VisualElement>("RechargeMaskBg");
        // UITK Button.Clickable steals pointer capture from SlotDragManipulator and leaves the
        // floating skill icon stuck to the cursor. Clicks go through L2ClickableSlot callbacks.
        if (_slotElement is Button button)
            button.clickable = null;
    }

    public VisualElement GetReuseElement()
    {
        return _reuseElement;
    }

    public VisualElement GetRechargeMaskElement()
    {
        return _rechargeMaskBg;
    }

    public void AssignShortcut(Shortcut shortcut)
    {
        ClearInnerSlot();
        // Do not ClearManipulators() here — that nulls SkillbarSlot._hoverManipulator while
        // MouseUp handlers stay registered → NRE in HandleSlotClickUp and dead clicks.
        if (_buttonClickSoundManipulator != null)
        {
            _slotElement.RemoveManipulator(_buttonClickSoundManipulator);
            _buttonClickSoundManipulator = null;
        }

        _shortcut = shortcut;
        _itemId = 0;
        // Re-assert: UITK may reattach Clickable on Button targets.
        if (_slotElement is Button button)
            button.clickable = null;
        _buttonClickSoundManipulator = new ButtonClickSoundManipulator(_slotElement);
        _slotElement.AddManipulator(_buttonClickSoundManipulator);

        switch (shortcut.Type)
        {
            case Shortcut.TYPE_ACTION:
                AssignAction(shortcut.Id);
                break;
            case Shortcut.TYPE_ITEM:
                // Zone stores template itemId in Level so the bar can rebind after stack objectId churn.
                AssignItem(shortcut.Id, shortcut.Level);
                break;
            case Shortcut.TYPE_MACRO:
                break;
            case Shortcut.TYPE_RECIPE:
                break;
            case Shortcut.TYPE_SKILL:
                AssignSkill(shortcut.Id , shortcut.Level);
                break;
        }
    }

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
        ItemInstance item = inventory != null ? inventory.FindByAllInventory(objectId) : null;
        // Soulshots etc.: objectId changes when the stack is emptied/restocked; Level carries itemId.
        if (item == null && inventory != null && itemIdHint > 0)
            item = inventory.GetItemByItemId(itemIdHint);
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
