using System;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class AbstractCreator
{
    public void OnSwitchEvent(ITab tab , bool isTrade)
    {
        if (isTrade)
        {
            if (_tabsTradeTabs.Length > 1)
            {
                if (tab != null)
                {
                    SwitchTab(tab , isTrade, true);
                }
            }
        }
        else
        {
            if (_tabsContentTabs.Length > 1)
            {
                if (tab != null)
                {
                    SwitchTab(tab , isTrade , true);
                }
            }
        }

    }

    public void OnLeftClick(int itemId, ItemCategory category, int position)
    {
        EventLeftClick?.Invoke(itemId, category, position);
    }

    public VisualElement GetActiveContent()
    {
       return  _activeTab.GetContentElement();
    }

    public string GetNameActiveTab()
    {
        return _activeTab.GetTabName();
    }
}
