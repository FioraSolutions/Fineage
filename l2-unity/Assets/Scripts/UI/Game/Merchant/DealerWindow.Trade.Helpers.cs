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
}
