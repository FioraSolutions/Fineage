using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{
    public List<ItemInstance> FilterInventory(List<ItemCategory> filteredCategories)
    {
        var list = new List<ItemInstance>();
        if (filteredCategories == null)
            return list;
        List<ItemInstance> bag = GetPlayerInvetoryToList();
        for (int i = 0; i < bag.Count; i++)
        {
            ItemInstance item = bag[i];
            if (item != null && !item.Equipped && filteredCategories.Contains(item.Category))
                list.Add(item);
        }
        return list;
    }

    public Dictionary<int, ItemInstance> GetPlayerInventory()
    {
        return _playerInventory;
    }

    public List<ItemInstance> GetPlayerInvetoryToList()
    {
        var list = new List<ItemInstance>();
        CharacterInventory.Row[] rows = Book.BagRows();
        for (int i = 0; i < rows.Length; i++)
        {
            ItemInstance item;
            if (_playerInventory != null && _playerInventory.TryGetValue(rows[i].ObjectId, out item))
                list.Add(item);
            else
                Debug.LogError("[Inventory] bag objectId " + rows[i].ObjectId + " has no view");
        }
        return list;
    }

    public List<ItemInstance> GetPlayerEquipToList()
    {
        var list = new List<ItemInstance>();
        CharacterInventory.Row[] rows = Book.PaperdollRows();
        for (int i = 0; i < rows.Length; i++)
        {
            ItemInstance item;
            if (_playerEquipInventory != null && _playerEquipInventory.TryGetValue(rows[i].ObjectId, out item))
                list.Add(item);
            else
                Debug.LogError("[Inventory] paperdoll objectId " + rows[i].ObjectId + " has no view");
        }
        return list;
    }

    public Dictionary<int, ItemInstance> GetPlayerEquipInventory()
    {
        return _playerEquipInventory;
    }

    public ItemInstance GetItemEquip(int objId)
    {
        ItemInstance item;
        if (_playerEquipInventory != null && _playerEquipInventory.TryGetValue(objId, out item))
            return item;
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
        ItemInstance item;
        if (_playerInventory != null && _playerInventory.TryGetValue(objId, out item))
            return item;
        return null;
    }

    public ItemInstance GetItemByItemId(int itemId)
    {
        ItemInstance found = null;
        int matches = 0;
        CountItemId(_playerInventory, itemId, ref found, ref matches);
        CountItemId(_playerEquipInventory, itemId, ref found, ref matches);
        if (matches > 1)
        {
            Debug.LogError("[Inventory] itemId " + itemId + " is on " + matches + " objectIds; not picking one");
            return null;
        }
        return found;
    }

    public bool TryResolveShortcut(int objectId, int itemIdHint, out ItemInstance item, out string error)
    {
        item = null;
        CharacterInventory.Row row;
        if (!Book.TryResolveShortcut(objectId, itemIdHint, out row, out error))
            return false;
        item = FindByAllInventory(row.ObjectId);
        if (item != null)
            return true;
        error = "shortcut objectId " + row.ObjectId + " is in the inventory but has no view";
        return false;
    }

    static void CountItemId(Dictionary<int, ItemInstance> source, int itemId, ref ItemInstance found, ref int matches)
    {
        if (source == null)
            return;
        foreach (ItemInstance item in source.Values)
        {
            if (item == null || item.ItemId != itemId)
                continue;
            matches++;
            if (found == null)
                found = item;
        }
    }
}
