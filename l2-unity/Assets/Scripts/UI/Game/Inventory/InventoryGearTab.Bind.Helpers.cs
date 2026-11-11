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
        if(InventoryPacketServices.Inventory.GetPlayerEquipInventory().Count == 0)
        {
            EquipEmptyAll();
        }
        try
        {
            for (int i = 0; i < modified.Count; i++)
            {
                ItemInstance currentSlotEquip = modified[i];

                if (currentSlotEquip.Equipped == true)
                {
                    if (currentSlotEquip == null) continue;
                    bool isReplace = changeInventoryData.IsReplaceSourceItem(currentSlotEquip.ObjectId);
                    if (changeInventoryData.IsReplaceSourceItem(currentSlotEquip.ObjectId)) continue;
                    EquipItem(currentSlotEquip);
                    GamePacketServices.Bus?.Equipped(currentSlotEquip, userId);
                    // GearFlowLog.Info("InventoryGearTab Equipped userId=" + userId +
                        // " itemId=" + currentSlotEquip.ItemId +
                        // " body=" + currentSlotEquip.BodyPart +
                        // " busInstance=" + (EventBus.Instance != null));
                }
            }
        }
        catch (Exception)
        {
            // Debug.LogError($"InventoryGearTab> UpdateEquipList : {ex.Message}");
        }

    }

    public void SetEquipList(List<ItemInstance> equipItems , int userId)
    {
        if (equipItems == null) return;
        EquipList(equipItems , userId);

        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }
}
