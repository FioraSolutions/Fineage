using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeTab : AbstractTab, ITab
{
    public TradeTab(string tabName , int countSlot , VisualElement tabContainer, VisualElement tabHeader, bool initEmpty , SlotType slotType , bool isDragged)
    {
        _createScroller = new CreateScroller();
        _tabName = tabName;
        _defaultCountSlot = countSlot;
        _tabHeader = tabHeader;
        _tabContainer = tabContainer;
        _contentContainer = tabContainer.Q<VisualElement>("Content");
        _createScroller.Start(tabContainer);
        CreateEmptyInventory(initEmpty , slotType , isDragged);
        OnRegisterClickTab(tabHeader);
    }

    public void OnRegisterClickTab(VisualElement tabHeader)
    {

            tabHeader.RegisterCallback<MouseDownEvent>(evt => {
                //EventSwitch?.Invoke(this);
                OnEventSwitch(this , true);
            }, TrickleDown.TrickleDown);
    }

    public void CreateEmptyInventory(bool initEmty , SlotType slotType , bool isDragged)
    {
        if (_contentContainer != null && initEmty)
        {
            _contentContainer.Clear();
            _tradeSlots = new TradingSlot[_defaultCountSlot];
            CreateSlots(_tradeSlots, _contentContainer , slotType , isDragged);
        }

    }

    private void CreateSlots(TradingSlot[] tradeSlots , VisualElement contentContainer , SlotType slotType , bool isDragged)
    {
        VisualTreeAsset template = InventoryPacketServices.Window.InventorySlotTemplate;
        for (int i = 0; i < _tradeSlots.Length; i++)
        {
            VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(template, contentContainer);
            TradingSlot slot = CreateTradeSlot(new TradingSlotModel(i, isDragged, slotElement, slotType));
            slot.EventLeftClick += OnClickLeftEvent;
            tradeSlots[i] = slot;
        }
    }

    public void AddDataTrade(List<ItemInstance> allItems , bool checkInventory = false)
    {
        for (int i = 0; i < allItems.Count; i++)
        {
            ItemInstance item = allItems[i];
            item.SetSlot(i);
            Assign(item, checkInventory, i);
        }
    }

    private void Assign(ItemInstance item , bool checkInventory,  int position)
    {

        if (!checkInventory)
        {
            _tradeSlots[position].AssignItem(item);
            return;
        }

        ItemInstance inventoryItem = InventoryPacketServices.Inventory.GetItemByItemId(item.ItemId);

        if (inventoryItem != null && inventoryItem.Count >= item.Count)
        {
            _tradeSlots[position].AssignItem(item , false);
        }
        else
        {
            _tradeSlots[position].AssignItem(item, true);
        }
    }

    public void ClearSlots(List<ItemInstance> oldListItems)
    {
        for (int i = 0; i < oldListItems.Count; i++)
        {
            ItemInstance item = oldListItems[i];
            item.SetSlot(i);
            _tradeSlots[i].AssignEmpty();
        }
    }
}
