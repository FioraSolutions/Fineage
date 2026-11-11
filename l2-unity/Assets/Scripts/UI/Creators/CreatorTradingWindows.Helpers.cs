using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class CreatorTradingWindows : AbstractCreator , ICreatorTradeTab
{
    public void SetClickActiveTab(int position)
    {
        if(_activeTab != null)
        {
            _activeTab.OnClickLeftEvent(position);
        }
    }

    public TradeTab GetMainTab()
    {
        foreach (TradeTab tab in _tabsTradeTabs)
        {
            if(tab != null & tab.GetMainTab() == true)
            {
                return tab;
            }
        }

        return null;
    }

    public void AddData(List<ItemInstance> allItems)
    {
        List<ItemInstance> allItemsInstance =  allItems.OfType<ItemInstance>().ToList();

        if (_activeTab != null)
        {
            _activeTab.AddDataTrade(allItemsInstance);
        }
    }

    public ItemInstance GetActiveByPosition(int position)
    {
        if(_activeTab != null)
        {
           return _activeTab.GetSlotByPosition(position);
        }

        return null;
    }
}
