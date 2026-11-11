using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class TradeRequestWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
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
        TradePacketServices.Bind(tradeRequest: this);
    }

    public void AddData(SendTradeRequestDto request)
    {
        _senderId = request.SenderId;

        var target = Worlds.GetEntityName(_senderId);

        _requesterName = target;
    }
}
