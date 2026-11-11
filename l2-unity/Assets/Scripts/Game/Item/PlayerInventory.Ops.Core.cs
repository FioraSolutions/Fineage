using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{
    public bool IsItemEquipByItemId(int itemId)
    {
        foreach (var item in _playerEquipInventory)
        {
            if(itemId == item.Value.ItemId)
            {
                return true;
            }
            
        }

        return false;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _changeInventoryData = new ChangeInventoryData();
            WireItemPacketServices();
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
        //_tempForRemoveAndAdd = new List<ItemInstance>();
        //_tempForModified = new List<ItemInstance>();
        _playerInventory = new Dictionary<int, ItemInstance>();
        // Equip starts empty, not null: the inventory window reads both dictionaries as soon
        // as it is built, which happens before the zone sends ItemList. Leaving this null
        // made ToggleHideWindowManual bail out and the window could never open at all.
        _playerEquipInventory = new Dictionary<int, ItemInstance>();
    }

    private void Start()
    {
        WireItemPacketServices();
        _playerInventory.Clear();
    }

    private void WireItemPacketServices()
    {
        ItemPacketServices.Bind();
        InventoryPacketServices.Bind(inventory: this);
    }
}
