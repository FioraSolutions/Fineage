using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ShopInventoryController
{
    private InventoryTab EnsureTab(
        InventoryTab tab,
        VisualElement content,
        VisualElement windowEle,
        bool isSell)
    {
        if (tab != null)
            return tab;

        tab = new InventoryTab();
        if (isSell)
            _dealerWindow.SetTabSell(tab);
        else
            _dealerWindow.SetTabBuy(tab);

        tab.Initialize(windowEle, content, content, false, false);
        return tab;
    }

    private InventorySlot[] BuildSlots(
        int count,
        InventoryTab tab,
        VisualElement content,
        VisualTreeAsset template,
        SlotType type)
    {
        var slots = new InventorySlot[count];
        for (int i = 0; i < count; i++)
        {
            VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(template, content);
            slots[i] = new InventorySlot(i, slotElement, tab, type, false);
        }

        return slots;
    }

    private int CapacityForProducts(List<Product> products)
    {
        int itemCount = products?.Count ?? 0;
        return L2SlotGridLayout.CapacityForItemCount(itemCount, _defaultCount);
    }

    private static bool UpdateList(List<Product> listSell, Product source, bool plus)
    {
        bool isUpdate = false;
        foreach (Product product in listSell)
        {
            if (product.ItemId != source.ItemId)
                continue;

            if (plus)
                product.SetCount(product.Count + source.Count);

            isUpdate = true;
        }

        return isUpdate;
    }
}
