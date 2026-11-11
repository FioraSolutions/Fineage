using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{
    public List<ItemInstance> FilterInventory(List<ItemCategory> filteredCategories)
    {
        if(_playerInventory != null && filteredCategories != null)
        {
                var filteredItemsInventory = _playerInventory
               .Where(kvp => filteredCategories.Contains(kvp.Value.Category))
               .Where(kvp => !kvp.Value.Equipped)
               .Select(kvp => kvp.Value)
               .ToList();

            return filteredItemsInventory;
        }

        return new List<ItemInstance>();
    }

    public List<ItemInstance> GetPlayerInvetoryToList()
    {
        return _playerInventory.Values.ToList();
    }

    public List<ItemInstance> GetPlayerEquipToList()
    {
        return _playerEquipInventory.Values.ToList();
    }

    public ItemInstance GetItemEquip(int objId)
    {
        if (_playerEquipInventory.ContainsKey(objId))
        {
            return _playerEquipInventory[objId];
        }
        return null;
    }

    public ItemInstance FindByAllInventory(int objId)
    {
        ItemInstance item = GetItem(objId);
        if (item != null)
        {
            return item;
        }

        return GetItemEquip(objId);
    }

    public ItemInstance GetItem(int objId)
    {
        if (_playerInventory.ContainsKey(objId))
        {
            return _playerInventory[objId];
        }
        return null;
    }

    public ItemInstance GetItemByItemId(int itemId)
    {
        if (_playerInventory == null) return null;

        return _playerInventory.Values.FirstOrDefault(item => item.ItemId == itemId);
    }
}
