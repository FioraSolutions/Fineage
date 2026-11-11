using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using Debug = UnityEngine.Debug;
using VContainer;

public partial class DealerWindow
{
    private void MoveРЎellElsePriceType(SlotType type , List<Product> listServer, int position)
    {

        if (type == SlotType.PriceBuy)
        {

            if (_listSell == null) return;
            if (!IsIndexValid(_listSell, position)) return;

            Product source = _listSell[position];

            if (source.Count > 1 || source.GetConsumeCategory() != ConsumeCategory.Normal)
            {
                RefreshOpacity(0.7f);
                RefreshSelectData(listServer, position, type);
                QuantityInputPacketServices.Window.ShowWindow(QuantityMaxFor(source));
                QuantityInputPacketServices.Window.OnButtonOk += OnSelectedQuantity;
                QuantityInputPacketServices.Window.OnButtonClose += OnCloseQuantity;
            }
            else
            {
                source.SetCount(1);
                _tradeItemMover.MovingBuyWithRemoval(_isModifySell  ,source, position);
            }


        }
        else if (type == SlotType.PriceSell)
        {
            if (!IsIndexValid(listServer, position)) return;
            Product source = listServer[position];

            if (source == null) return;

            if (source.Count > 1 || source.GetConsumeCategory() != ConsumeCategory.Normal)
            {
                RefreshOpacity(0.7f);
                RefreshSelectData(listServer, position, type);
                QuantityInputPacketServices.Window.ShowWindow(QuantityMaxFor(source));
                QuantityInputPacketServices.Window.OnButtonOk += OnSelectedQuantity;
                QuantityInputPacketServices.Window.OnButtonClose += OnCloseQuantity;
            }
            else
            {
                source.SetCount(1);
                _listSell = _tradeItemMover.MovingSellWithRemoval(_isModifySell , _listSell , source, listServer, position);
            }
        }
    }

    static int QuantityMaxFor(Product source)
    {
        if (source == null)
            return 1;
        if (source.Count > 1)
            return source.Count;
        if (source.GetConsumeCategory() != ConsumeCategory.Normal)
            return ShopStackableQuantityMax;
        return 1;
    }

    private void RefreshSelectData(List<Product> listServer , int position , SlotType type)
    {
        _listServer = listServer;
        _position = position;
        _type = type;
    }

    private void OnSelectedQuantity(string value)
    {
        QuantityInputPacketServices.Window.OnButtonOk -= OnSelectedQuantity;
        UseSuccessInParentWindow(this, value, _listServer);
    }
}
