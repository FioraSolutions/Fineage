using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class MultiSellWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
            _dataProvider = new TooltipDataProvider();
            _toolTips = new MultiSellToolTips(_dataProvider);
            WireMultiSellPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireMultiSellPacketServices();
    }
}
