using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager
{
    private void AddActionToSkillbar()
    {
        int actionId = ((ActionSlot)_draggedSlot).ActionId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add action ID {actionId} into skillbar slot {slot}.");

        ShortcutPacketServices.Shortcuts.AddShortcut(slot, actionId, Shortcut.TYPE_ACTION , 0);
    }

    private void ChangeItemOrder()
    {
        int fromSlot = _draggedSlot.Position;
        int toSlot = _hoverSlot.Position;

        // Debug.Log($"Moving item from slot {fromSlot} to slot {toSlot}.");

        InventoryPacketServices.Inventory.ChangeItemOrder(fromSlot, toSlot);
    }

    private void AddItemToSkillbar()
    {
        int itemId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add item {itemId} to skillbar slot {slot}.");

        ShortcutPacketServices.Shortcuts.AddShortcut(slot, itemId, Shortcut.TYPE_ITEM, 0);
    }


    private void AddItemToEnchant()
    {
        int _objectId = ((InventorySlot)_draggedSlot).ObjectId;
        int slot = _hoverSlot.Position;
        // Debug.LogWarning($"Add _objectId {_objectId} to EnchantWindow slot {slot}.");
        ItemEnchantPacketServices.Enchant.AddEnchantItem(_objectId);
    }

    private void MoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        int newSlot = _hoverSlot.Position;
        // Debug.LogWarning($"Moving skillbar shortcut from slot {oldSlot} to slot {newSlot}.");
        ShortcutPacketServices.Shortcuts.MoveShortcut(oldSlot, newSlot);
    }

    private void RemoveSkillbarSlot()
    {
        int oldSlot = _draggedSlot.Position;
        // Debug.LogWarning($"Renoving skillbar shortcut from slot {oldSlot}.");
        ShortcutPacketServices.Shortcuts.DeleteShortcut(oldSlot , true);
    }

    #endregion

    private void ResetSlots()
    {
        _draggedSlot = null;
        _hoverSlot = null;
    }

    public override void ShowWindow()
    {
        if (_windowEle == null)
            return;
        _windowEle.style.display = DisplayStyle.Flex;
        _windowEle.style.opacity = 1;
    }

    public override void HideWindow()
    {
        if (_windowEle == null)
            return;
        _windowEle.style.opacity = 0;
        _windowEle.style.display = DisplayStyle.None;
        _windowEle.style.left = -10000;
        _windowEle.style.top = -10000;
    }
}

