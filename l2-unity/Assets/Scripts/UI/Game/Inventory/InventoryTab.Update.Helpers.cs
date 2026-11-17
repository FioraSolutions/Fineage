using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using static L2Slot;
using static PlayerInventory;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryTab
{
    private void AddAndRemove(List<ItemInstance> removeAndAdd)
    {

        for (int i = 0; i < removeAndAdd.Count; i++)
        {
            ItemInstance item = removeAndAdd[i];
            if (item.LastChange == (int)InventoryChange.ADDED)
            {
                // Model already contains the row (PlayerInventory applies before UI).
                // Gate on UI presence so open Items tab still paints loot pickups.
                if (GetInventorySlot(item.ObjectId) == null)
                {
                    // Several rows can be added by one packet (a multi-item purchase). Assign by the
                    // slot the model gave the row: indexing blindly would land every row on the same
                    // cell, so only the last one stayed visible.
                    int slotIndex = item.GetPosition();
                    if (slotIndex < 0
                        || slotIndex >= _inventorySlots.Length
                        || (_inventorySlots[slotIndex] != null && !_inventorySlots[slotIndex].IsEmpty))
                        slotIndex = FindEmptySlotIndex();
                    if (slotIndex < 0)
                        continue;

                    InventorySlot i_slot = _inventorySlots[slotIndex];
                    if (i_slot != null)
                        i_slot.AssignItem(item);
                }
            }
            else if(item.LastChange == (int)InventoryChange.REMOVED)
            {
                InventorySlot slot = GetInventorySlot(item.ObjectId);
                if (slot != null)
                {
                    _inventorySlots[slot.Position].AssignEmpty();
                    ShiftElements.ShiftElementsLeft(_inventorySlots, slot.Position);
                }
            }
        }

    }

    private int FindEmptySlotIndex()
    {
        if (_inventorySlots == null)
            return -1;
        for (int i = 0; i < _inventorySlots.Length; i++)
        {
            InventorySlot slot = _inventorySlots[i];
            if (slot == null || slot.IsEmpty)
                return i;
        }
        return -1;
    }

    private void Modified(List<ItemInstance> modified , ChangeInventoryData changeInventoryData)
    {
        for (int i = 0; i < modified.Count; i++)
        {
            ItemInstance currentItem = modified[i];
            if (currentItem.LastChange == (int)InventoryChange.MODIFIED)
            {
                InventorySlot oldSlot = GetInventorySlot(currentItem.ObjectId);
                //Replace Inventory->Equip and Equip->Inventory elsewhere in the code, we skip it here
                if(!changeInventoryData.IsReplaceSourceItem(currentItem.ObjectId))
                {
                    if (oldSlot != null)
                    {
                        UpdateSlot(oldSlot, currentItem);

                    }
                    else
                    {
                        int slotIndex = currentItem.GetPosition();
                        InventorySlot i_slot = slotIndex >= 0 && slotIndex < _inventorySlots.Length
                            ? _inventorySlots[slotIndex]
                            : null;

                        if (i_slot != null)
                        {
                            i_slot.AssignItem(currentItem);
                        }
                    }
                }

              

            }
        }
    }

    private void UpdateSlot(InventorySlot slot , ItemInstance currentItem)
    {

        if (slot != null)
        {
            _inventorySlots[slot.Position].AssignItem(currentItem);
        }
    }
}
