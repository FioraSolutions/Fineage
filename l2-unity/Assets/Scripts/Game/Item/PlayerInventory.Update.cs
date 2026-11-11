using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory
{
    public void UpdateInventory(Dictionary<int, ItemInstance> items , Dictionary<int, ItemInstance> equipitems)
    {
        lock (_lock)
        {

            _changeInventoryData.ClearData();

            List<ItemInstance> listEquip = equipitems.Values.ToList();

            var items_f = items.Values.ToArray();
            var _tempForRemoveAndAdd = FilterByRemove(items_f);
            var _tempForModified = FilterByModified(items_f);

            try
            {

                UpdatePlayerInventory(_tempForRemoveAndAdd, _tempForModified , listEquip);
                ModifiedEquip(_playerInventory, _playerEquipInventory, listEquip);

                int adenaCount = GetAdenaCount(_playerInventory.Values.ToList());
                int useSlot = _playerInventory.Count + _playerEquipInventory.Count;

                if(_currentUser == null) _currentUser =  StorageNpc.getInstance().GetFirstUser();
                int userId = _currentUser.PlayerInfoInterlude.Identity.Id;
                DispatcherPacketServices.Dispatcher.Enqueue(() => {
                    // Same reason as SetInventory: the window may not exist yet. The change data
                    // is applied to _playerInventory above, so only the redraw is skipped.
                    if (InventoryPacketServices.Window == null)
                        return;
                    InventoryPacketServices.Window.UpdateItemList(_tempForRemoveAndAdd, _tempForModified, listEquip, adenaCount, useSlot, _changeInventoryData, userId);
                    InventoryPacketServices.Window.RemoveOldEquipItemOrNoQuipItem(_changeInventoryData , userId);
                });

            }
            catch (Exception)
            {
                // Debug.LogError("PlayerInventory Errors: " + ex.StackTrace);
            }
        }

    }
    private int GetAdenaCount(List<ItemInstance> modified)
    {
        ItemInstance item =  modified.FirstOrDefault(o => o.Category == ItemCategory.Adena);
        return (item != null) ? item.Count : 0;
    }
    private void UpdatePlayerInventory(List<ItemInstance> removeAndAdd, List<ItemInstance> modified , List<ItemInstance> listEquip)
    {
        for (int i = 0; i < removeAndAdd.Count; i++)
        {
            ItemInstance item = removeAndAdd[i];
            AddAndRemove(_playerInventory, item);
        }

        for (int i = 0; i < modified.Count; i++)
        {
            ItemInstance item_m = modified[i];
            Modified(_playerInventory, _playerEquipInventory , listEquip , item_m);
        }
    }

    private void AddAndRemove(Dictionary<int, ItemInstance> _playerInventory , ItemInstance item)
    {
        //Debug.Log("Add new Object 1 NoFilter" + item.ItemId + " ObjectID " + item.ObjectId);
        if (item.LastChange == (int)InventoryChange.REMOVED)
        {
            if (_playerInventory.ContainsKey(item.ObjectId))
            {
                //Debug.Log("Remove object id " + item.ObjectId + " remove item id " + item.ItemId);
                _playerInventory.Remove(item.ObjectId);
                RefreshPosition(_playerInventory);
            }

            if (_playerEquipInventory != null && _playerEquipInventory.ContainsKey(item.ObjectId))
                RemoveGearInventory(_playerEquipInventory, _playerEquipInventory[item.ObjectId]);
        }
        else if(item.LastChange == (int)InventoryChange.ADDED)
        {
            
            if (!_playerInventory.ContainsKey(item.ObjectId))
            {
                AddInventory(item);
            }
            
        }
    }

    private void AddInventory(ItemInstance item)
    {
        int slot = InventoryPacketServices.Window != null
            ? InventoryPacketServices.Window.GetEmptySlot()
            : _playerInventory.Count;
        item.SetSlot(slot);
        _playerInventory.Add(item.ObjectId, item);

    }

    private void RefreshPosition(Dictionary<int, ItemInstance> playerInventory)
    {
     
    }

    private void Modified(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory, List<ItemInstance> listEquip , ItemInstance item)
    {
        //Debug.Log("Modified data count " + item.Count + " item obj id " + item.ObjectId + " itemid" + item.ItemId + " equiped " + item.Equipped);
        UpdateInventory(playerInventory, playerEquipInventory , listEquip , item);
        UpdateEquip(playerInventory , playerEquipInventory, item);
    }

    private void ModifiedEquip(Dictionary<int, ItemInstance> playerInventory, Dictionary<int, ItemInstance> playerEquipInventory , List<ItemInstance> modified)
    {
        for(int i =0; i < modified.Count; i++)
        {
            ItemInstance item = modified[i];
            UpdateInventoryEquip(playerInventory, playerEquipInventory, item);
        }
        
    }

    private void UpdateInventory(Dictionary<int, ItemInstance> playerInventory , Dictionary<int, ItemInstance> playerEquipInventory, List<ItemInstance> listEquip ,ItemInstance item)
    {
   
        if (playerEquipInventory.ContainsKey(item.ObjectId) & !item.Equipped)
        {
           
            ItemInstance del_item_replace = listEquip.FirstOrDefault(itemEquip => itemEquip.EqualsBodyPart(item.BodyPart , item.IsBow()));

            if (del_item_replace != null)
            {
                GearReplaceSlot(playerEquipInventory, del_item_replace, item);
            }
            else
            {
                ItemInstance del_item = playerEquipInventory[item.ObjectId];
                RemoveGearInventory(playerEquipInventory, del_item);
            }

        }

        //If we move from gear Inventory to items Inventory
        if (!playerInventory.ContainsKey(item.ObjectId))
        {
            if (!item.Equipped)
            {
                AddInventory(item); 
            }
        }
        else
        {
            
            ItemInstance oldItem = playerInventory[item.ObjectId];
            int count = item.Count - oldItem.Count;
            item.SetSlot(oldItem.Slot);
            
            //StorageVariable.getInstance().AddS1Items(new VariableItem(count.ToString(), item.ObjectId));
           // StorageVariable.getInstance().AddS2Items(new VariableItem(item.ItemData.ItemName.Name, item.ObjectId));
            oldItem.Update(item);
        }
    }

}
