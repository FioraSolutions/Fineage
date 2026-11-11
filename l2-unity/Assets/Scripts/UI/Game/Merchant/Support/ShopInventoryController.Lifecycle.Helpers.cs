using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;

public partial class ShopInventoryController
{
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
