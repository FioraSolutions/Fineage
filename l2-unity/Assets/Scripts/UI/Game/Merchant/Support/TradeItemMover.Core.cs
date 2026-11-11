using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeItemMover
{
    public TradeItemMover(DealerWindow dealerWindow , ShopInventoryController shopCellCreator , ShopItemEvaluator itemEvaluator)
    {
        _shopCellCreator = shopCellCreator;
        _itemEvalutor = itemEvaluator;
        _dealerWindow = dealerWindow;
    }

    public void MovePanelPriceSell(int selectCount, Product source, bool _isModifySell, int position , List<Product> listServer)
    {
        if (selectCount == source.Count)
        {
           var _listSell = MovingSellWithRemoval(_isModifySell, _dealerWindow.GetListSell(), source, listServer, position);
           _dealerWindow.SetListSell(_listSell);
        }
        else if (selectCount < source.Count)
        {
            Product clone = source.Clone();
            source.SetCount(source.Count - selectCount);
            clone.SetCount(selectCount);

            MovingWithCopy(clone, listServer, position);
        }else if (source.GetConsumeCategory() != ConsumeCategory.Normal)
        {
            Product clone = source.Clone();
            clone.SetCount(selectCount);

            MovingWithCopy(clone, listServer, position);
        }
    }

    public void MovingBuyWithRemoval(bool isModifySell, Product source, int position)
    {
        if (isModifySell) ModifyBuy(source, _dealerWindow.GetListSell(), _dealerWindow.GetListBuy());

        ShiftBuy(_dealerWindow.GetTabSell(), position, _dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetInventorySlotsBuy());

        var listSell = _shopCellCreator.RemoveProduct(_dealerWindow.GetListSell(), source, true);
        _dealerWindow.SetListSell(listSell);

        UpdatePriceAndWeight(listSell, _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());
    }

    private void MovingBuyWithCopy(bool isModifySell, Product source, int position)
    {

        if (isModifySell) ModifyBuy(source , _dealerWindow.GetListSell() , _dealerWindow.GetListBuy());
        if (!isModifySell) ShiftBuy(_dealerWindow.GetTabSell(), position, _dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetInventorySlotsBuy());

        //_listSell = _shopCellCreator.RemoveProduct(_listSell, source, false);
        var listSell = _shopCellCreator.RemoveProduct(_dealerWindow.GetListSell(), source, false);
        _dealerWindow.SetListSell(listSell);

        UpdatePriceAndWeight(listSell , _dealerWindow.GetCurrentDataWeight() , _dealerWindow.GetMaxWeight());
    }

    public List<Product> MovingSellWithRemoval(bool isModifySell, List<Product> listSell, Product source, List<Product> listServer, int position)
    {
        // Buy-from-shop: keep merchant row; cart gets a clone (player-trade moves the real bag row).
        Product cartItem = isModifySell ? source : source.Clone();
        listSell = _shopCellCreator.AddProduct(listSell, cartItem);

        RefreshToolTips(listSell);

        if (listSell.Count != 0)
        {
            Move(_dealerWindow.GetInventorySlotsBuy(), _dealerWindow.GetTabBuy(), GetNewPosition(listSell, cartItem), cartItem);
        }

        ModifySell(isModifySell, position, listServer, source, true);

        UpdatePriceAndWeight(listSell, _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());

        return listSell;
    }

    private void MovingWithCopy(Product source, List<Product> listServer, int position)
    {
        Product cartItem = source.Clone();
        var listSell = _shopCellCreator.AddProduct(_dealerWindow.GetListSell(), cartItem);
        _dealerWindow.SetListSell(listSell);
        RefreshToolTips(listSell);

        if (listSell.Count != 0)
        {
            Move(_dealerWindow.GetInventorySlotsBuy(), _dealerWindow.GetTabBuy(), GetNewPosition(listSell, cartItem), cartItem);
        }

        // Buy mode: never strip the merchant grid (trade/sell-to-NPC only).
        ModifySell(false, position, listServer, source, false);

        UpdatePriceAndWeight(listSell , _dealerWindow.GetCurrentDataWeight() , _dealerWindow.GetMaxWeight());
    }

    private void ModifySell(bool isModify, int position, List<Product> listServer, Product source, bool isRemoval)
    {
        if (isModify)
        {
            ShiftSell(_dealerWindow.GetTabBuy() ,  position, _dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetInventorySlotsBuy());
            var _listBuy = _shopCellCreator.RemoveProduct(listServer, source, isRemoval);
            _dealerWindow.SetListBuy(_listBuy);
        }

    }
}
