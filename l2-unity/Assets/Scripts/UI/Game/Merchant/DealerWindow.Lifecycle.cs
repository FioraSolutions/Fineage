using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using Debug = UnityEngine.Debug;
using VContainer;

public partial class DealerWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _itemEvalutor = new ShopItemEvaluator(_instance);
            _shopCellCreator = new ShopInventoryController(_instance , _defaultCount);
            _tradeItemMover = new TradeItemMover(this , _shopCellCreator, _itemEvalutor);
            WireMerchantPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireMerchantPacketServices();
    }

    private void WireMerchantPacketServices()
    {
        MerchantPacketServices.Bind(dealer: this);
    }

    private void OnDestroy()
    {
        _instance = null;
        _itemEvalutor = null;
        _inventorySlotsBuy = null;
        _inventorySlotsSell = null;
        _tradeItemMover = null;
    }
}
