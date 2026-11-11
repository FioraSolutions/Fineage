using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory
{
    private void UpdateInventoryEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
        if (item.LastChange == (int)InventoryChange.REMOVED)
        {
            if (playerInventory != null && playerInventory.ContainsKey(item.ObjectId))
                RemoveInventory(playerInventory, playerInventory[item.ObjectId]);
            if (playerEquipInventory != null && playerEquipInventory.ContainsKey(item.ObjectId))
                RemoveGearInventory(playerEquipInventory, playerEquipInventory[item.ObjectId]);
            return;
        }

        if(playerInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            ItemInstance del_item = playerInventory[item.ObjectId];
            RemoveInventory(playerInventory, del_item);

        }
        
        if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == false)
        {
            ItemInstance del_item = playerEquipInventory[item.ObjectId];
            RemoveGearInventory(playerEquipInventory, del_item);
        }
        else if (!playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            playerEquipInventory.Add(item.ObjectId, item);
        }
        else if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == true)
        {
            playerEquipInventory[item.ObjectId].Update(item);
        }
    }

    private void RemoveInventory(Dictionary<int, ItemInstance> playerInventory , ItemInstance del_item)
    {
        
        playerInventory.Remove(del_item.ObjectId);
        RefreshPosition(playerInventory);
        _changeInventoryData.AddRemoveInventory(del_item);
    }

    private void RemoveGearInventory(Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance del_item)
    {

        _changeInventoryData.AddRemoveGear(del_item);

        if (playerEquipInventory.ContainsKey(del_item.ObjectId))
        {
            playerEquipInventory.Remove(del_item.ObjectId);
        }

    }

    private void GearReplaceSlot(Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance sourceItem , ItemInstance gearItem)
    {
        if(playerEquipInventory.ContainsKey(gearItem.ObjectId)) playerEquipInventory.Remove(gearItem.ObjectId);
        _changeInventoryData.AddReplaceGear(sourceItem, gearItem);
    }

    private void UpdateEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, ItemInstance item)
    {
        ItemsDetectedToGear(playerInventory, item);
        GearDetectedToItems(playerEquipInventory, item);
    }

    private void ItemsDetectedToGear(Dictionary<int, ItemInstance> playerInventory, ItemInstance item)
    {
        if (playerInventory.ContainsKey(item.ObjectId) & item.Equipped)
        {
            RemoveInventory(playerInventory, item);
        }

        if (!playerInventory.ContainsKey(item.ObjectId) & !item.Equipped)
        {
            playerInventory.Add(item.ObjectId, item);
        }

    }
    private void GearDetectedToItems(Dictionary<int, ItemInstance> playerEquipInventory , ItemInstance item)
    {
        if (!playerEquipInventory.ContainsKey(item.ObjectId))
        {
            if (item.Equipped)
            {
                playerEquipInventory.Add(item.ObjectId, item);
            }
        }

        //if we move from gear Invetory to items Inventory we need to delete from Gear so that there is no duplication
        else if (playerEquipInventory.ContainsKey(item.ObjectId) & item.Equipped == false)
        {
            RemoveGearInventory( playerEquipInventory, item);
        }
        else
        {
            ItemInstance oldItem = playerEquipInventory[item.ObjectId];
            item.SetSlot(oldItem.Slot);
            oldItem.Update(item);
        }
    }

    private List<ItemInstance> FilterByRemove(ItemInstance[] items)
    {
        return items.Where(item =>
        item.LastChange == (int)InventoryChange.REMOVED ||
        item.LastChange == (int)InventoryChange.ADDED).ToList();
    }

    private List<ItemInstance> FilterByModified(ItemInstance[] items)
    {
        return items.Where(item =>
        item.LastChange == (int)InventoryChange.MODIFIED).ToList();
    }

}
