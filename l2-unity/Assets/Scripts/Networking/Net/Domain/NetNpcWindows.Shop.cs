using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class NetNpcWindows
{
    public static void ApplyBuyList(NetBuyList list) => ShowBuy(list);

    public static void ApplySellList(NetSellList list) => ShowSell(list);

    public static void ApplyMultiSellList(NetMultiSellList list) => ShowMulti(list);

    public static void ApplyShopBalance(NetShopBalance balance) => ApplyBalance(balance);

    static void ShowBuy(NetBuyList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, list.ListId);
        var products = new List<Product>();
        if (list.Items != null)
        {
            for (int i = 0; i < list.Items.Length; i++)
            {
                NetBuyItem row = list.Items[i];
                int type2 = Type2FromItemId(row.ItemId);
                // Unlimited shop rows arrive as Count=-1. Keep display Count=1 so tooltips do not
                // show a fake "(10000)" next to soulshots; stackables still open the qty dialog via
                // ConsumeCategory, and the dialog max is raised in DealerWindow.
                int count = row.Count < 0 ? 1 : row.Count;
                // Net BuyList carries itemId/price only — TYPE2 must come from local weapongrp/armorgrp
                // (hardcoding OTHER made RB jewels hit EtcItem tooltip → NRE).
                products.Add(new Product(0, 0, count, type2, 0, 0, 0, row.Price, row.ItemId));
            }
        }

        // Interlude BuyList: top = merchant goods, bottom = purchase cart (not player-trade).
        OpenDealer("Shop", "Item List", "Buy List", ProductType.BUY, products, false, list.ListId, _hasBalance ? _balance.Adena : 0);
    }

    static void ShowSell(NetSellList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, 0);
        OpenDealer("Shop", "Inventory", "Sell", ProductType.SELL, SellProducts(list.Items), true, 0, _hasBalance ? _balance.Adena : 0);
    }

    static void ShowMulti(NetMultiSellList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, list.ListId);
        MultiSellWindow window = MultiSellPacketServices.MultiSell;
        if (window == null || list.Entries == null)
            return;
        var rows = new List<MultiSellData>(list.Entries.Length);
        var products = new List<ItemInstance>();
        for (int i = 0; i < list.Entries.Length; i++)
        {
            NetMultiSellEntry entry = list.Entries[i];
            var made = new List<ItemInstance>();
            if (entry.Products != null)
            {
                for (int p = 0; p < entry.Products.Length; p++)
                {
                    NetMultiPart part = entry.Products[p];
                    int type2 = Type2FromItemId(part.ItemId);
                    var item = new ItemInstance(0, part.ItemId, ItemLocation.Trade, products.Count, part.Count,
                        ItemsType.ParceCategory(type2), false, ItemSlot.none, part.EnchantLevel, 9999);
                    made.Add(item);
                    products.Add(item);
                }
            }

            var ingredients = new List<Ingredient>();
            if (entry.Ingredients != null)
            {
                for (int g = 0; g < entry.Ingredients.Length; g++)
                {
                    NetMultiPart part = entry.Ingredients[g];
                    ingredients.Add(new Ingredient(part.ItemId, Type2FromItemId(part.ItemId), part.Count, part.EnchantLevel));
                }
            }

            var row = new MultiSellData(made, ingredients);
            row.ZoneEntryId = entry.EntryId;
            rows.Add(row);
        }

        var dto = new MultiSellListDto();
        dto.LoadNet(list.ListId, rows, products);
        try
        {
            window.AddData(products, dto);
            window.ShowWindow();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] multisell: " + ex.Message);
        }
    }

    static void ApplyBalance(NetShopBalance balance)
    {
        _balance = balance;
        _hasBalance = true;
        DealerWindow dealer = MerchantPacketServices.Dealer;
        if (dealer == null)
            return;
        double percent = balance.MaxWeight > 0 ? (balance.Weight * 100.0) / balance.MaxWeight : 0;
        try
        {
            dealer.UpdateDataForm(balance.Adena, percent, balance.Weight, balance.MaxWeight);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] shop balance: " + ex.Message);
        }
    }

    static void OpenDealer(string title, string sellHeader, string buyHeader, ProductType type, List<Product> products, bool modifySell, int listId, int adena)
    {
        DealerWindow dealer = MerchantPacketServices.Dealer;
        if (dealer == null)
            return;
        int weight = _hasBalance ? _balance.Weight : 0;
        int max = _hasBalance ? _balance.MaxWeight : 0;
        double percent = max > 0 ? (weight * 100.0) / max : 0;
        try
        {
            dealer.SetWindowName(title);
            dealer.SetHeaderNameSellPanel(sellHeader);
            dealer.SetHeaderNameBuyPanel(buyHeader);
            dealer.SetProductType(type);
            dealer.UpdateBuyData(products, modifySell, listId);
            dealer.UpdateDataForm(adena, percent, weight, max);
            dealer.ShowWindowToCenter();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] shop window: " + ex.Message);
        }
    }

    static List<Product> SellProducts(NetSellItem[] items)
    {
        var products = new List<Product>();
        if (items == null)
            return products;
        for (int i = 0; i < items.Length; i++)
        {
            NetSellItem row = items[i];
            products.Add(new Product(0, row.ObjectId, row.Count, Type2FromItemId(row.ItemId), 0, 0, row.EnchantLevel, row.Price, row.ItemId));
        }

        return products;
    }

    static List<Product> BagProducts(NetItemEntry[] items)
    {
        var products = new List<Product>();
        if (items == null)
            return products;
        for (int i = 0; i < items.Length; i++)
        {
            NetItemEntry row = items[i];
            if (row.EquippedSlot >= 0)
                continue;
            products.Add(new Product(0, row.ObjectId, row.Count, Type2FromItemId(row.ItemId), 0, 0, row.EnchantLevel, 0, row.ItemId));
        }

        return products;
    }

    static int Type2FromItemId(int itemId)
    {
        if (itemId == 57)
            return (int)EnumType2.TYPE2_MONEY;

        WeapongrpTable weapons = TablePacketServices.WeaponGrps;
        if (weapons != null && weapons.GetWeapon(itemId) != null)
            return (int)EnumType2.TYPE2_WEAPON;

        ArmorgrpTable armors = TablePacketServices.ArmorGrps;
        Armorgrp armor = armors != null ? armors.GetArmor(itemId) : null;
        if (armor != null)
        {
            if (IsJewelSlot(armor.BodyPart))
                return (int)EnumType2.TYPE2_ACCESSORY;
            return (int)EnumType2.TYPE2_SHIELD_ARMOR;
        }

        return (int)EnumType2.TYPE2_OTHER;
    }

    static bool IsEquipmentType2(int type2) =>
        type2 == (int)EnumType2.TYPE2_WEAPON
        || type2 == (int)EnumType2.TYPE2_SHIELD_ARMOR
        || type2 == (int)EnumType2.TYPE2_ACCESSORY;

    static bool IsJewelSlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.neck:
            case ItemSlot.lear:
            case ItemSlot.rear:
            case ItemSlot.lfinger:
            case ItemSlot.rfinger:
                return true;
            default:
                return false;
        }
    }

}
