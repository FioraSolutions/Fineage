using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ShopInventoryController
{
    public ShopInventoryController(DealerWindow dealerWindow, int defaultCount)
    {
        _dealerWindow = dealerWindow;
        _defaultCount = defaultCount;
    }

    public void CreateSell(
        List<Product> listBuy,
        InventoryTab tabSell,
        VisualElement contentSell,
        VisualElement windowEle)
    {
        tabSell = EnsureTab(tabSell, contentSell, windowEle, isSell: true);
        contentSell.Clear();
        ToolTipManager.GetInstance().SetSellData(listBuy);

        VisualTreeAsset template = _dealerWindow.GetInventorySlotTemplate();
        InventorySlot[] slots = BuildSlots(
            CapacityForProducts(listBuy),
            tabSell,
            contentSell,
            template,
            SlotType.PriceSell);

        if (listBuy != null)
        {
            for (int i = 0; i < listBuy.Count; i++)
                slots[i].AssignProduct(listBuy[i]);
        }

        tabSell.UpdateInventorySlots(slots);
        _dealerWindow.SetInventorySlotsSell(slots);
    }

    public void CreateBuy(
        List<Product> listBuy,
        InventoryTab tabBuy,
        VisualElement contentBuy,
        VisualElement windowEle)
    {
        tabBuy = EnsureTab(tabBuy, contentBuy, windowEle, isSell: false);
        contentBuy.Clear();
        ToolTipManager.GetInstance().SetBuyData(new List<Product>());

        VisualTreeAsset template = _dealerWindow.GetInventorySlotTemplate();
        InventorySlot[] slots = BuildSlots(
            CapacityForProducts(listBuy),
            tabBuy,
            contentBuy,
            template,
            SlotType.PriceBuy);

        // Empty cart cells must not start a drag ghost (default InventorySlot drag is for bag/trade).
        for (int i = 0; i < slots.Length; i++)
            slots[i].AssignEmpty();

        tabBuy.UpdateInventorySlots(slots);
        _dealerWindow.SetInventorySlotsBuy(slots);
    }

    public InventorySlot[] CreateEmptyBuy(
        InventoryTab tabBuy,
        int defaultCount,
        VisualElement contentBuy,
        VisualTreeAsset inventorySlotTemplate)
    {
        InventorySlot[] slots = BuildSlots(
            defaultCount,
            tabBuy,
            contentBuy,
            inventorySlotTemplate,
            SlotType.PriceBuy);
        tabBuy.UpdateInventorySlots(slots);
        return slots;
    }

    public void ClearSell(InventoryTab tabSell)
    {
        if (tabSell == null)
            return;

        tabSell.UnSelectSlot();
        tabSell.ResetSelectSlot();
    }

    public void ClearBuy(InventorySlot[] inventorySlotsBuy, InventoryTab tabBuy)
    {
        if (inventorySlotsBuy != null)
        {
            for (int i = 0; i < inventorySlotsBuy.Length; i++)
            {
                if (inventorySlotsBuy[i] != null)
                    inventorySlotsBuy[i].AssignEmpty();
            }
        }

        if (tabBuy == null)
            return;

        tabBuy.UnSelectSlot();
        tabBuy.ResetSelectSlot();
        tabBuy.UpdateInventorySlots(inventorySlotsBuy);
    }
}
