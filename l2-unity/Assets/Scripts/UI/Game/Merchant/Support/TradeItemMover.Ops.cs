using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class TradeItemMover
{
    private void ModifyBuy(Product source , List<Product> listSell ,  List<Product> listBuy)
    {
        if (listSell.Count != 0)
        {
            listBuy = _shopCellCreator.AddProduct(listBuy, source);
            Move(_dealerWindow.GetInventorySlotsSell(), _dealerWindow.GetTabSell(), GetNewPosition(listBuy, source), source);
        }
    }

    private void UpdatePriceAndWeight(List<Product> listSell , int _currentDataWeight, int _maxWeight)
    {
        _itemEvalutor.UpdateAllPrice(listSell);
        _itemEvalutor.UpdateWeight(listSell, _currentDataWeight, _maxWeight);
    }

    private void RefreshToolTips(List<Product> listSell)
    {
        ToolTipManager.GetInstance().SetBuyData(listSell);
    }

    public int GetNewPosition(List<Product> list, Product moveItem)
    {
        //example: stem/cord - count 6 pcs
        if (moveItem.GetTypeItem() == EnumType2.TYPE2_OTHER)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Product product = list[i];
                if (product.ItemId == moveItem.ItemId)
                {
                    return i;
                }
            }
        }
        return list.Count - 1;
    }

    private void Move(InventorySlot[] inventorySlots, InventoryTab tab, int newPosition, Product product)
    {
        if (inventorySlots == null || newPosition < 0 || newPosition >= inventorySlots.Length)
            return;
        inventorySlots[newPosition].AssignProduct(product);
        tab.UpdateInventorySlots(inventorySlots);
    }

    private void ShiftSell(InventoryTab tabBuy , int newPosition , InventorySlot[] inventorySlotsSell, InventorySlot[] inventorySlotsBuy)
    {
        ShiftElements.ShiftElementsLeft(inventorySlotsSell, newPosition);
        tabBuy.UpdateInventorySlots(inventorySlotsBuy);
    }

    public void ShiftBuy(InventoryTab tabSell, int newPosition, InventorySlot[] inventorySlotsSell, InventorySlot[] inventorySlotsBuy)
    {
        ShiftElements.ShiftElementsLeft(inventorySlotsBuy, newPosition);
        tabSell.UpdateInventorySlots(inventorySlotsSell);
    }
}
