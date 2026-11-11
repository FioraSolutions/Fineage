using System;
using System.Linq;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractCreator
{
    public void SwitchTab(ITab switchTo , bool isTrade , bool useEvent)
    {
        if (_activeTab != switchTo)
        {
            if (_activeTab != null) _activeTab.UnselectTabContainerClass();
            _activeTab = switchTo;
            if (_activeTab != null) _activeTab.SelectTabContainerClass();
            if(useEvent == true) EventSwitchOut?.Invoke(switchTo, isTrade);

        }
    }

    public void SetMainTab(int index, bool isMain , bool isTrade)
    {
        if (isTrade)
        {
            var tab = _tabsTradeTabs[index];
            tab.SetMainTab(isMain);
            EventSwitchOut?.Invoke(tab, isTrade);
        }
        else
        {
            var tab = _tabsContentTabs[index];
            tab.SetMainTab(isMain);
            EventSwitchOut?.Invoke(tab, isTrade);
        }

    }

    public void SwitchTab(int idTab , bool isTrade , bool useEvent)
    {
        if (isTrade)
        {
            if (_tabsTradeTabs == null) return;

            if (ArrayUtils.IsValidIndexArray(_tabsTradeTabs , idTab))
            {
                SwitchTab(_tabsTradeTabs[idTab], true , useEvent);
            }
        }
        else
        {
            if (_tabsContentTabs == null) return;

            if (ArrayUtils.IsValidIndexArray(_tabsContentTabs, idTab))
            {
                SwitchTab(_tabsContentTabs[idTab], false , useEvent);
            }
        }
    }
}
