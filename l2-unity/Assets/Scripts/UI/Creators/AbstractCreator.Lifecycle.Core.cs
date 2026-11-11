using System;
using System.Linq;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractCreator
{
    public void InitTradeTabs(string[] nameTabs)
    {
        _nameTabs = nameTabs;
        _tabsTradeTabs = new TradeTab[nameTabs.Length];
    }

    public void InitContentTabs(string[] nameTabs)
    {
        _nameTabs = nameTabs;
        _tabsContentTabs = new ContentTab[nameTabs.Length];
    }

    public int FindTabByName(string name)
    {
        for(int i=0; i < _tabsContentTabs.Length; i++)
        {
            ITab tab = _tabsContentTabs[i];
            if (tab.GetTabName() == name)
            {
                return i;
            }
        }
        return -1;
    }
}
