using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;

public partial class TradeItemMover
{
    private ShopInventoryController _shopCellCreator;
    private ShopItemEvaluator _itemEvalutor;
    private DealerWindow _dealerWindow;

    public void MovePanelPriceBuy(int selectCount, Product source, bool isModifySell, int position)
    {
        ShopTransferMode mode = ShopPanelTransfer.FromModifySellFlag(isModifySell);
        if (source == null)
        {
            Debug.LogError("[Shop] MovePanelPriceBuy source=null position=" + position + " mode=" + mode);
            return;
        }

        if (selectCount <= 0)
        {
            Debug.LogError("[Shop] MovePanelPriceBuy itemId=" + source.ItemId + " selectCount=" + selectCount + " mode=" + mode);
            return;
        }

        if (selectCount == source.Count)
        {
            MovingBuyWithRemoval(mode, source, position);
            return;
        }

        if (selectCount < source.Count)
        {
            ReduceCartQuantity(mode, source, selectCount, position);
            return;
        }

        Debug.LogError("[Shop] MovePanelPriceBuy itemId=" + source.ItemId + " selectCount=" + selectCount
            + " cartCount=" + source.Count + " mode=" + mode);
    }
}