using System;
using System.Linq;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract partial class AbstractCreator 
{
    protected ITab[] _tabsTradeTabs;
    protected ITab[] _tabsContentTabs;
    protected string[] _nameTabs;
    protected ITab _activeTab;
    public event Action<int, ItemCategory, int> EventLeftClick;
    public event Action<ITab , bool> EventSwitchOut;

    
}
