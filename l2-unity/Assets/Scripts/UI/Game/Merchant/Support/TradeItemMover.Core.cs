using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeItemMover
{
    public TradeItemMover(DealerWindow dealerWindow, ShopInventoryController shopCellCreator, ShopItemEvaluator itemEvaluator)
    {
        _shopCellCreator = shopCellCreator;
        _itemEvalutor = itemEvaluator;
        _dealerWindow = dealerWindow;
    }

    public void MovePanelPriceSell(int selectCount, Product source, bool isModifySell, int position, List<Product> listServer)
    {
        ShopTransferMode mode = ShopPanelTransfer.FromModifySellFlag(isModifySell);
        if (selectCount == source.Count)
        {
            var listSell = MovingSellWithRemoval(mode, _dealerWindow.GetListSell(), source, listServer, position);
            _dealerWindow.SetListSell(listSell);
            return;
        }

        if (selectCount < source.Count)
        {
            ShopPanelTransfer.ApplyCatalogQuantityTaken(mode, source, selectCount);
            Product cartLine = ShopPanelTransfer.CreateCartLine(mode, source, selectCount);
            if (cartLine == null)
                return;

            if (mode == ShopTransferMode.NpcMerchantBuy)
            {
                MovingWithCopy(cartLine, listServer, position, mode);
                return;
            }

            Product staged = cartLine.Clone();
            staged.SetCount(selectCount);
            MovingWithCopy(staged, listServer, position, mode);
            return;
        }

        if (source.GetConsumeCategory() != ConsumeCategory.Normal)
        {
            Product cartLine = ShopPanelTransfer.CreateCartLine(mode, source, selectCount);
            if (cartLine == null)
                return;
            MovingWithCopy(cartLine, listServer, position, mode);
        }
    }

    public void MovingBuyWithRemoval(ShopTransferMode mode, Product source, int position)
    {
        if (ShopPanelTransfer.ReturnsToCatalog(mode))
            ModifyBuy(source, _dealerWindow.GetListSell(), _dealerWindow.GetListBuy());

        ShiftBuy(_dealerWindow.GetTabSell(), position, _dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetInventorySlotsBuy());

        var listSell = _shopCellCreator.RemoveProduct(_dealerWindow.GetListSell(), source, true);
        _dealerWindow.SetListSell(listSell);

        UpdatePriceAndWeight(listSell, _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());
    }

    void ReduceCartQuantity(ShopTransferMode mode, Product cartRow, int quantityToRemove, int position)
    {
        if (cartRow == null)
        {
            Debug.LogError("[Shop] ReduceCartQuantity cartRow=null position=" + position + " mode=" + mode);
            return;
        }

        if (quantityToRemove <= 0 || quantityToRemove > cartRow.Count)
        {
            Debug.LogError("[Shop] ReduceCartQuantity itemId=" + cartRow.ItemId + " remove=" + quantityToRemove
                + " have=" + cartRow.Count + " mode=" + mode);
            return;
        }

        if (ShopPanelTransfer.ReturnsToCatalog(mode))
        {
            Product returned = cartRow.Clone();
            returned.SetCount(quantityToRemove);
            ModifyBuy(returned, _dealerWindow.GetListSell(), _dealerWindow.GetListBuy());
        }

        if (quantityToRemove == cartRow.Count)
        {
            MovingBuyWithRemoval(mode, cartRow, position);
            return;
        }

        cartRow.SetCount(cartRow.Count - quantityToRemove);
        Move(_dealerWindow.GetInventorySlotsBuy(), _dealerWindow.GetTabBuy(), position, cartRow);
        UpdatePriceAndWeight(_dealerWindow.GetListSell(), _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());
    }

    public List<Product> MovingSellWithRemoval(ShopTransferMode mode, List<Product> listSell, Product source, List<Product> listServer, int position)
    {
        Product cartItem = ShopPanelTransfer.CreateCartLine(mode, source, source.Count);
        if (cartItem == null)
            return listSell;

        listSell = _shopCellCreator.AddProduct(listSell, cartItem);

        RefreshToolTips(listSell);

        if (listSell.Count != 0)
            Move(_dealerWindow.GetInventorySlotsBuy(), _dealerWindow.GetTabBuy(), GetNewPosition(listSell, cartItem), cartItem);

        if (mode == ShopTransferMode.PlayerBagCommit)
            ModifySell(position, listServer, source, true);

        UpdatePriceAndWeight(listSell, _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());

        return listSell;
    }

    void MovingWithCopy(Product cartLine, List<Product> listServer, int position, ShopTransferMode mode)
    {
        var listSell = _shopCellCreator.AddProduct(_dealerWindow.GetListSell(), cartLine);
        _dealerWindow.SetListSell(listSell);
        RefreshToolTips(listSell);

        if (listSell.Count != 0)
            Move(_dealerWindow.GetInventorySlotsBuy(), _dealerWindow.GetTabBuy(), GetNewPosition(listSell, cartLine), cartLine);

        if (mode == ShopTransferMode.PlayerBagCommit)
            ModifySell(position, listServer, cartLine, false);

        UpdatePriceAndWeight(listSell, _dealerWindow.GetCurrentDataWeight(), _dealerWindow.GetMaxWeight());
    }

    void ModifySell(int position, List<Product> listServer, Product source, bool isRemoval)
    {
        ShiftSell(_dealerWindow.GetTabBuy(), position, _dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetInventorySlotsBuy());
        var listBuy = _shopCellCreator.RemoveProduct(listServer, source, isRemoval);
        _dealerWindow.SetListBuy(listBuy);
    }
}