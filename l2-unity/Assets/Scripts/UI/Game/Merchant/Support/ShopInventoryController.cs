using System.Collections.Generic;
using UnityEngine.UIElements;
using static L2Slot;

public class ShopInventoryController
{
    private readonly DealerWindow _dealerWindow;
    private readonly int _defaultCount;

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

    public SlotType DetectedClickPanel(int type)
    {
        if (type == (int)SlotType.PriceBuy)
            return SlotType.PriceBuy;
        if (type == (int)SlotType.PriceSell)
            return SlotType.PriceSell;
        return SlotType.Other;
    }

    public List<Product> AddProduct(List<Product> listSell, Product source)
    {
        if (listSell == null)
            return new List<Product> { source };

        bool isUpdate = false;
        if (source.GetTypeItem() == EnumType2.TYPE2_OTHER)
            isUpdate = UpdateList(listSell, source, plus: true);

        if (!isUpdate)
            listSell.Add(source);

        return listSell;
    }

    public List<Product> RemoveProduct(List<Product> listSell, Product source, bool isRemoval)
    {
        if (listSell == null)
            return new List<Product> { source };

        bool isUpdate = false;
        if (source.GetTypeItem() == EnumType2.TYPE2_OTHER && !isRemoval)
            isUpdate = UpdateList(listSell, source, plus: false);

        if (!isUpdate || isRemoval || source.Count == 0)
            listSell.Remove(source);

        return listSell;
    }

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
