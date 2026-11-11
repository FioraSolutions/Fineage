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

    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetInventory(Dictionary<int, ItemInstance> items, Dictionary<int, ItemInstance> equipItems , bool openInventory , int adenaCount , int usedSlot)
    {
        _playerInventory = items;
        _playerEquipInventory = equipItems;

        List<ItemInstance> items_collect =  _playerInventory.Values.ToList();
        List<ItemInstance> equip_collect = _playerEquipInventory.Values.ToList();

        //if (_playerEquipInventory != null)
        //{
        //    equip_collect = _playerEquipInventory.Values.ToList();
        // }
        if (_currentUser == null) _currentUser = StorageNpc.getInstance().GetFirstUser();

        DispatcherPacketServices.Dispatcher.Enqueue(() =>
        {
            // The window may not be built yet (it is created on first UI load). The data above
            // is already stored, and RefreshWindow pushes it once the window exists, so a
            // missing window must not throw away the snapshot.
            InventoryWindow window = InventoryPacketServices.Window;
            if (window == null)
                return;

            int userId = _currentUser != null && _currentUser.PlayerInfoInterlude.Identity != null
                ? _currentUser.PlayerInfoInterlude.Identity.Id
                : 0;
            window.SetItemList(items_collect, equip_collect, adenaCount , usedSlot , userId);

            if (openInventory)
            {
                window.ShowWindow();
            }
        });
    }

    public void RefreshWindow()
    {
        InventoryWindow window = InventoryPacketServices.Window;
        if (window == null || !window.IsItemSurfaceReady)
            return;
        if (_playerInventory == null)
            return;

        if (_currentUser == null) _currentUser = StorageNpc.getInstance().GetFirstUser();
        if (_currentUser == null || _currentUser.PlayerInfoInterlude.Identity == null)
            return;

        List<ItemInstance> items = _playerInventory.Values.ToList();
        List<ItemInstance> equip = _playerEquipInventory != null
            ? _playerEquipInventory.Values.ToList()
            : new List<ItemInstance>();

        window.SetItemList(
            items,
            equip,
            GetAdenaCount(items),
            _playerInventory.Count + equip.Count,
            _currentUser.PlayerInfoInterlude.Identity.Id);
    }

    public void ChangeItemOrder(int fromSlot, int toSlot)
    {
        //ItemInstance fromItem = GetItemBySlot(fromSlot);
        //ItemInstance toItem = GetItemBySlot(toSlot);

        //if (fromItem == null)
        //{
         //   Debug.LogError($"Can't change item order, item not found at slot {fromSlot}.");
        //    return;
        //}

        //List<InventoryOrder> orders = new List<InventoryOrder>();
        //orders.Add(new InventoryOrder(fromItem.ObjectId, toSlot));

        //if (toItem != null)
        //{
        //    orders.Add(new InventoryOrder(toItem.ObjectId, fromSlot));
        //}

        //InventoryPacketServices.Window.SelectSlot(toSlot);
        // Debug.Log("пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ");
        //GameClient.Instance.ClientPacketHandler.UpdateInventoryOrder(orders);
    }
}
