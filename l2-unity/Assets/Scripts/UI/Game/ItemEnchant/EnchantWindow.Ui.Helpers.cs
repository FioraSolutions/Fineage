using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{
    private void InitSlotElement2(VisualElement slotBox)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotChoiceTemplate, slotBox);
        _enchant2 = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        _enchant2.AssignEmpty();
    }

    public void AddEnchantItem(int objectId)
    {
        if (_zoneWire && (objectId <= 0 || objectId == _scrollObjectId))
            return;

        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
            return;
        ItemInstance itemInstance = inventory.GetItem(objectId);
        if (itemInstance == null)
        {
            Dictionary<int, ItemInstance> worn = inventory.GetPlayerEquipInventory();
            if (worn != null)
                worn.TryGetValue(objectId, out itemInstance);
        }

        if (itemInstance != null)
        {
            _selectItem = itemInstance;
            _slot2.Clear();
            _enchant2 = ChangeSlotBoxType(_slot2, _enchant2);
            _enchant2.AssignItem(itemInstance);
            if (_textLabel != null)
                _textLabel.text = NetEnchant.WithLevel(itemInstance.GetName(), itemInstance.EnchantLevel);
        }
        else
        {
            // Debug.LogWarning("EnchantWindows error not add dropItems");
        }
    }

    private EnchantSlot CreateEnchantSlot(int i, VisualElement slotElement, SlotType slotType)
    {
        return new EnchantSlot(i, slotElement, null, slotType, false);
    }

    private void OnClick(ClickEvent evt)
    {
        if (_awaitingRoll)
            return;
        if (_selectItem == null || (_zoneWire && _selectItem.ObjectId == _scrollObjectId))
        {
            if (_textLabel != null)
                _textLabel.text = "Select an item to enchant.";
            return;
        }

        if (_zoneWire)
            _awaitingRoll = true;
        RequestEnchantItem(_selectItem.ObjectId);
    }

    private void OnCancel(ClickEvent evt)
    {
        if (_zoneWire)
        {
            HideWindow();
            return;
        }

        RequestEnchantItem(-1);
    }
}
