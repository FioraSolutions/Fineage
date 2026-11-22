using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using System.Collections;
using UnityEngine;
using VContainer;

public partial class InventoryWindow
{
    public void SetItemList(List<ItemInstance> allItems , List<ItemInstance> quipAllItems, int adenaCount, int usedSlots , int userId)
    {
        RebindFromModel();
    }

    public void RebindFromModel()
    {
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] window has no inventory model");
            return;
        }
        if (_adenaCountLabel == null || _inventoryCountLabel == null || _tabs == null || _tabs.Count == 0)
        {
            Debug.LogError("[Inventory] window is not built; rows stay in the model");
            return;
        }

        SetAdenaCountLabel(inventory.AdenaCount);
        _slotCount = PLAYER_INVENTORY_SIZE;
        _inventoryCountLabel.text = "(" + inventory.UsedSlots + "/" + _slotCount + ")";
        if (_activeTab != null)
            SwitchItemList(_activeTab);
        else
            SwitchItemList(_tabs[0]);
        if (_gearTab != null)
            _gearTab.BindEquip(inventory.GetPlayerEquipToList());
        RefreshWeightFromPlayer();
    }

    public void UpdateItemList(List<ItemInstance> removeAndAdd , List<ItemInstance> modified , List<ItemInstance> listEquipModified,  int adenaCount , int usedSlots , ChangeInventoryData changeInventoryData , int userId)
    {
        RebindFromModel();
    }

    public void RemoveOldEquipItemOrNoQuipItem(ChangeInventoryData changeInventoryData , int userId)
    {
        RebindFromModel();
    }


    private void SetAdenaCountLabel(int count )
    {
        _adenaCountLabel.text = ToolTipsUtils.ConvertToPrice(count);
    }

 
    public void ToggleHideWindowManual()
    {
        if (InventoryPacketServices.Inventory == null)
        {
            Debug.LogError("[Inventory] toggle has no inventory model");
            return;
        }

        if (_isWindowHidden)
        {
            InventoryPacketServices.Inventory.RefreshWindow();
            ShowWindow();
        }
        else
        {
            HideWindow();
        }
    }

    public ItemInstance GetItemByPosition(int position)
    {
        if(_activeTab != null)
        {
            return _activeTab.GetInventorySlotByPosition(position);
        }

        return null;
    }

    public GearItem GetGearPosition(int position)
    {
       return _gearTab.ConvertIdToGearType(position);
    }
}
