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
}
