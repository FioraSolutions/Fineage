public partial class InventoryTab
{
    public void UpdateItemList(List<ItemInstance> removeAndAdd , List<ItemInstance> modified , ChangeInventoryData changeInventoryData)
    {
        AddAndRemove(removeAndAdd);
        Modified(modified , changeInventoryData);

        if (_selectedSlot != -1)
        {
            SelectSlot(_selectedSlot);
        }
    }

    public void ModifiedRemove(ItemInstance item)
    {
        InventorySlot i_slot1 = GetInventorySlot(item.ObjectId);

        if (i_slot1 != null)
        {
            i_slot1.AssignEmpty();
            ShiftElements.ShiftElementsLeft(_inventorySlots, i_slot1.Position);
        }
        else
        {
            // Debug.Log("Shift element number null " + i_slot1);
        }
    }

    public void AddItem(ItemInstance item)
    {
        InventorySlot i_slot1 = GetEmptyInventorySlot();
        i_slot1.AssignItem(item);
    }
}
