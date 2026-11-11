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
                    InventorySlot i_slot = _inventorySlots[item.Slot];
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
                        InventorySlot i_slot = _inventorySlots[currentItem.Slot];

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
