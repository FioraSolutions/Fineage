using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class TradeWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _exchangeSlots = new List<ExchangeSlot>();
            _partnerExchangeSlots = new List<ExchangeSlot>();
            _exchangeItems = new List<ItemInstance>();
            WireTradePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireTradePacketServices();
    }

    private void WireTradePacketServices()
    {
        TradePacketServices.Bind(trade: this);
    }
}
