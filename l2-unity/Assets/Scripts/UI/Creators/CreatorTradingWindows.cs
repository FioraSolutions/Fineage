using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class CreatorTradingWindows : AbstractCreator , ICreatorTradeTab
{
    public event Action<int> EventSwitchTabByIndexOfTab;
    private VisualTreeAsset[] _templates;
    //tabTemplate 0 , tabHeaderTemplate 1
    private readonly string[] loadTemplate = new string[2]
    {
        L2InventoryUiAssets.Tab,
        L2InventoryUiAssets.TabHeader,
    };









}
