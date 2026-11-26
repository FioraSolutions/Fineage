using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    void TryAddGearAnchor(ItemSlot slot, string elementName)
    {
        VisualElement ele = _windowEle.Q<VisualElement>(elementName);
        if (ele == null || _gearAnchors.ContainsKey(slot))
            return;
        _gearAnchors[slot] = new GearItem(ele, slot);
    }

    private void CreateGearSlot(Dictionary<ItemSlot, GearItem> gearAnchors)
    {
        var elementToSlot = new Dictionary<VisualElement, GearSlot>();
        int cellId = 0;
        foreach (KeyValuePair<ItemSlot, GearItem> entry in gearAnchors)
        {
            GearItem gearItem = entry.Value;
            VisualElement ele = gearItem.GetElement();
            if (ele == null)
                continue;

            if (!elementToSlot.TryGetValue(ele, out GearSlot slot))
            {
                _gearCellIds.Add(cellId, gearItem.GetSlotType());
                slot = new GearSlot(cellId, ele, this, SlotType.Gear);
                elementToSlot[ele] = slot;
                cellId++;
            }
            gearItem.SetGearSlot(slot);
        }
    }

    public void UpdateEquipList(List<ItemInstance> modified, ChangeInventoryData changeInventoryData , int userId)
    {
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] paperdoll update has no inventory model");
            return;
        }
        BindEquip(inventory.GetPlayerEquipToList());
    }

    public void SetEquipList(List<ItemInstance> equipItems, int userId)
    {
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] paperdoll set has no inventory model");
            return;
        }
        BindEquip(inventory.GetPlayerEquipToList());
    }
}
