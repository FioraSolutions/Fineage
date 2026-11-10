using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

public partial class InventoryWindow
{
    public void SetItemList(List<ItemInstance> allItems , List<ItemInstance> quipAllItems, int adenaCount, int usedSlots , int userId)
    {
        //_adenaCountLabel.text = adenaCount.ToString();
        SetAdenaCountLabel(adenaCount);
        //_tabs[0].ForEach((tab) =>
        //{
        _tabs[0].SetItemList(FilterOutQuestItems(allItems));
        _gearTab.SetEquipList(quipAllItems , userId);
        _slotCount = PLAYER_INVENTORY_SIZE;
        _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
        // });
        SwitchTab(_tabs[0]);
        RefreshWeightFromPlayer();
}
    public void UpdateItemList(List<ItemInstance> removeAndAdd , List<ItemInstance> modified , List<ItemInstance> listEquipModified,  int adenaCount , int usedSlots , ChangeInventoryData changeInventoryData , int userId)
    {
        if (_activeTab.MainTab)
        {

            // Slot count
            _slotCount = PLAYER_INVENTORY_SIZE;
            _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
            SetAdenaCountLabel(adenaCount);
            _tabs[0].UpdateItemList(
                FilterOutQuestItems(removeAndAdd),
                FilterOutQuestItems(modified),
                changeInventoryData);
            _gearTab.UpdateEquipList(listEquipModified , changeInventoryData , userId);
        }
        else
        {

            _slotCount = PLAYER_INVENTORY_SIZE;
            _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
            SetAdenaCountLabel(adenaCount);
            _activeTab.SetItemList(InventoryPacketServices.Inventory.FilterInventory(_activeTab.GetFilterCategories));
            _gearTab.SetEquipList(InventoryPacketServices.Inventory.GetPlayerEquipToList(), userId);
        }

    }

    public void RemoveOldEquipItemOrNoQuipItem(ChangeInventoryData changeInventoryData , int userId)
    {
        List<ItemInstance> obsoleteItemsInventory = changeInventoryData.ItemsInventory();
        List<ItemInstance> obsoleteItemsGear = changeInventoryData.ItemsGears();
        List<ReplaceGearData> obsoleteItemsGearReplace = changeInventoryData.ItemsReplace();

        foreach (ItemInstance item in obsoleteItemsInventory)
        {
            if (!obsoleteItemsGearReplace.Any(replaceData => replaceData.GetGear().EqualsBodyPart(item.BodyPart , item.IsBow())))
            {
                _tabs[0].ModifiedRemove(item);
            }
        }

        obsoleteItemsGear.ForEach(item =>
        {
            _gearTab.ModifiedRemove(item.BodyPart, item);
            GamePacketServices.Bus?.UnEquipped(item , userId);
        });


        foreach (ReplaceGearData item in obsoleteItemsGearReplace)
        {

            ItemInstance source = item.GetSource();
            ItemInstance gearItem = item.GetGear();

            InventorySlot source_item_slot = _tabs[0].GetInventorySlot(source.ObjectId);

            if (source_item_slot != null && source_item_slot.ItemInstance != null)
            {
                _gearTab.ModifiedReplace(source_item_slot.ItemInstance , userId);
            }



            if(gearItem != null & source_item_slot != null)
            {
                GamePacketServices.Bus?.UnEquipped(gearItem , userId);
                source_item_slot.AssignItem(gearItem);
            }
            //if there are 2 items in LHand and Rhand, we move 1 to the position of the one who is trying to change, and insert the second item into the inventory
            else if (gearItem != null & source_item_slot == null)
            {
                GamePacketServices.Bus?.UnEquipped(gearItem, userId);
                _tabs[0].AddItem(gearItem);
            }

        }


    }


    private void SetAdenaCountLabel(int count )
    {
        _adenaCountLabel.text = ToolTipsUtils.ConvertToPrice(count);
    }

 
    public void ToggleHideWindowManual()
    {
        if (InventoryPacketServices.Inventory == null)
            return;

        Dictionary<int, ItemInstance> allItems = GetInventoryItems();
        Dictionary<int, ItemInstance> equipItems = GetInventoryEquipItems();
        // Empty is fine; only a null dictionary means the snapshot never arrived. Both start
        // empty in PlayerInventory.Awake now, so an empty bag simply opens empty.
        if (allItems == null)
            allItems = new Dictionary<int, ItemInstance>();
        if (equipItems == null)
            equipItems = new Dictionary<int, ItemInstance>();

        int adenaCount = GetAdenaCount(allItems.Values.ToList());

        if (_isWindowHidden)
        {
             InventoryPacketServices.Inventory.SetInventory(allItems, equipItems , true, adenaCount , allItems.Count + equipItems.Count);
        }
        else
        {
            HideWindow();
        }
    }

    private Dictionary<int, ItemInstance> GetInventoryItems()
    {
       return  InventoryPacketServices.Inventory.GetPlayerInventory();
    }

    private Dictionary<int, ItemInstance> GetInventoryEquipItems()
    {
        return InventoryPacketServices.Inventory.GetPlayerEquipInventory();
    }

    private int GetAdenaCount(List<ItemInstance> modified)
    {
        ItemInstance item = modified.FirstOrDefault(o => o.Category == ItemCategory.Adena);
        return (item != null) ? item.Count : 0;
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
