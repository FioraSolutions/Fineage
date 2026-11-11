using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeItemMover 
{
    private ShopInventoryController _shopCellCreator;
    private ShopItemEvaluator _itemEvalutor;
    private DealerWindow _dealerWindow;
   public void MovePanelPriceBuy(int selectCount , Product source , bool _isModifySell , int position)
    {
        if (selectCount == source.Count)
        {
            MovingBuyWithRemoval(_isModifySell, source, position);
        }
        else if (selectCount < source.Count)
        {
            Product clone = source.Clone();
            source.SetCount(source.Count - selectCount);
            clone.SetCount(selectCount);

            MovingBuyWithCopy(true, clone, position);
        }
    }




    //If we receive a signal that modification of the SELL panel is allowed, then we transfer the product to another panel but do not touch our panel until the quantity is > 0











    //We do not move the target from sell -> buy because we believe that there is already something there and re-updating the cell is not required.We simply clear it
    //and shift all positions to the left if we delete an element

}
