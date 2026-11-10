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

    /// <summary>
    /// Unlimited GM/shop stackables are listed with Count=1 for tooltip display.
    /// Qty "All" still caps at the zone grant limit (10000).
    /// </summary>
    const int ShopStackableQuantityMax = 10000;

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

    private void OnCloseQuantity()
    {
        QuantityInputPacketServices.Window.OnButtonClose -= OnCloseQuantity;
        UseCloseInParentWindow();
    }

    private void UseCloseInParentWindow()
    {
       RefreshOpacity(1);
    }

    private void UseSuccessInParentWindow(DealerWindow _dealerWindow, string value, List<Product> listServer)
    {
         _dealerWindow.RefreshOpacity(1);
        // Debug.Log("Move value 1 " + value);
         _dealerWindow.MoveРЎellElseQuantitySelected(_type, listServer, _position, int.Parse(value));
    }

    public void MoveРЎellElseQuantitySelected(SlotType type, List<Product> listServer, int position , int selectCount)
    {
        if (selectCount <= 0)
            return;

        if (type == SlotType.PriceBuy)
        {
            if (_listSell == null) return;
            if (!IsIndexValid(_listSell, position)) return;

            Product source = _listSell[position];

            _tradeItemMover.MovePanelPriceBuy(selectCount, source, _isModifySell, position);

        }
        else if (type == SlotType.PriceSell)
        {
            if (!IsIndexValid(listServer, position)) return;
            Product source = listServer[position];
            if (source == null) return;

            _tradeItemMover.MovePanelPriceSell(selectCount, source, _isModifySell, position, listServer);

        }
    }





  

    public bool IsIndexValid(List<Product> _listSell, int index)
    {
        return index >= 0 && index < _listSell.Count;
    }

 


    
 

   

    //_weiBar = GetElementById("WeightBg");
    //_weiBarBg = GetElementById("WeightGauge");

}
