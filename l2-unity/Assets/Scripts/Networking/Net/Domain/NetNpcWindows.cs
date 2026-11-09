using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies NPC HTML, quest, shop, and service packets onto the existing windows.
/// </summary>
public static class NetNpcWindows
{
    public static void Tick()
    {
        if (!NetNpcSession.WindowsOpen)
            return;
        if (PollInput.EscapeDown)
        {
            Close();
            return;
        }

        int npcId = NetNpcSession.ActiveNpcObjectId;
        if (npcId <= 0)
            return;
        World world = WorldApplyIncomingPacketServices.GameWorld;
        if (world == null)
            return;
        Entity npc = world.GetEntityNoLockSync(npcId);
        if (npc == null || !InRange(npc))
            Close();
    }

    public static bool InRange(Entity npc)
    {
        PlayerEntity player = PlayerEntity.Instance;
        if (player == null || npc == null || npc.Identity == null)
            return true;
        Vector3 from = player.Identity.GetL2jPos(player.transform.position);
        Vector3 to = npc.Identity.GetL2jPos(npc.transform.position);
        float dx = from.x - to.x;
        float dy = from.y - to.y;
        float dz = from.z - to.z;
        float limit = NetNpcSession.InteractionDistance;
        return dx * dx + dy * dy + dz * dz <= limit * limit;
    }

    public static void Close()
    {
        NetNpcSession.CloseWindows();
        if (GameWindowPacketServices.Html != null)
            GameWindowPacketServices.Html.HideWindow();
        if (MerchantPacketServices.Dealer != null)
            MerchantPacketServices.Dealer.HideWindow();
        if (MultiSellPacketServices.MultiSell != null)
            MultiSellPacketServices.MultiSell.HideWindow();
    }

    static NetShopBalance _balance;
    static bool _hasBalance;

    static void ShowQuests(NetQuestList list)
    {
        QuestWindow window = QuestPacketServices.Quest;
        if (window == null || list.Active == null)
            return;
        var quests = new List<QuestInstance>(list.Active.Length);
        for (int i = 0; i < list.Active.Length; i++)
        {
            NetQuestJournal row = list.Active[i];
            int flags = row.JournalWord != 0 ? row.JournalWord : row.Cond;
            if (flags == 0)
                flags = 1;
            quests.Add(new QuestInstance(row.QuestId, flags, row.Repeatable ? 1 : 0, row.Name));
        }

        try
        {
            // Enter-world also pushes QuestList to prefill the journal — never auto-open here.
            // MenuWindow toggles the window before RequestQuestList; AddData fills whatever is visible.
            window.AddData(quests);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] quest list: " + ex.Message);
        }
    }

    public static void ApplyBuyList(NetBuyList list) => ShowBuy(list);
    public static void ApplySellList(NetSellList list) => ShowSell(list);
    public static void ApplyMultiSellList(NetMultiSellList list) => ShowMulti(list);
    public static void ApplyShopBalance(NetShopBalance balance) => ApplyBalance(balance);
    public static void ApplyWarehouseDepositList(NetWarehouseDepositList list) => ShowDeposit(list);
    public static void ApplyWarehouseWithdrawList(NetWarehouseWithdrawList list) => ShowWithdraw(list);
    public static void ApplyQuestList(NetQuestList list) => ShowQuests(list);

    /// <summary>
    /// Net <c>AcquireSkillList</c>: fill the trainer window (Mobius <c>Folk.showSkillList</c> answer).
    /// The window's own learn flow then sends <c>RequestAcquireSkillInfo</c> / <c>RequestAcquireSkill</c>.
    /// </summary>
    public static void ApplyAcquireSkillList(AcquireSkillListDto list)
    {
        SkillLearnWindow window = SkillLearnPacketServices.SkillLearn;
        if (window == null || list == null || list.AcquireList == null)
            return;
        Close();
        try
        {
            window.ShowWindow();
            window.AddData(list.AcquireList);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] acquire skill list: " + ex.Message);
        }
    }

    /// <summary>Net <c>AcquireSkillInfo</c>: the confirmation page (Mobius <c>AcquireSkillInfo</c>).</summary>
    public static void ApplyAcquireSkillInfo(AcquireSkillInfoDto info)
    {
        DescriptionSkillWindow window = SkillLearnPacketServices.SkillDesc;
        if (window == null || info == null)
            return;
        try
        {
            window.ShowWindow();
            window.AddData(info);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] acquire skill info: " + ex.Message);
        }
    }

    /// <summary>
    /// Net <c>EnchantResult</c>: hand the finished roll to the enchant window, then let the
    /// following <c>InventoryUpdate</c> land the level and crystal rows.
    /// </summary>
    public static void ApplyEnchantResult(NetEnchantResult result)
    {
        EnchantWindow window = ItemEnchantPacketServices.Enchant;
        try
        {
            window?.ApplyZoneResult(result);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Net] enchant result: " + ex.Message);
        }
    }

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

    static void ShowDeposit(NetWarehouseDepositList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, 0);
        string title = "Private Warehouse (" + list.FeePerLine + " adena)";
        OpenDealer(title, "Inventory", "Deposit", ProductType.WHDepositList, BagProducts(list.Items), true, 0, list.Adena);
    }

    static void ShowWithdraw(NetWarehouseWithdrawList list)
    {
        NetNpcSession.OpenShop(list.NpcObjectId, 0);
        _balance.Weight = list.CurrentWeight;
        _balance.MaxWeight = list.MaxWeight;
        _hasBalance = true;
        OpenDealer("Private Warehouse", "Warehouse", "Withdraw", ProductType.WHWithdrawList, BagProducts(list.Items), true, 0, _hasBalance ? _balance.Adena : 0);
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

    /// <summary>
    /// Interlude TYPE2 from client grp tables (BuyList/SellList/warehouse rows have no type on the wire).
    /// </summary>
    static int Type2FromItemId(int itemId)
    {
        if (itemId == 57)
            return (int)EnumType2.TYPE2_MONEY;

        if (WeapongrpTable.Instance != null && WeapongrpTable.Instance.GetWeapon(itemId) != null)
            return (int)EnumType2.TYPE2_WEAPON;

        Armorgrp armor = ArmorgrpTable.Instance != null ? ArmorgrpTable.Instance.GetArmor(itemId) : null;
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

public static class NetNpcOrders
{
    public static NetIdCount[] ItemCounts(List<Product> products)
    {
        return Counts(products, true);
    }

    public static NetIdCount[] ObjectCounts(List<Product> products)
    {
        return Counts(products, false);
    }

    static NetIdCount[] Counts(List<Product> products, bool itemId)
    {
        if (products == null || products.Count == 0)
            return Array.Empty<NetIdCount>();
        var rows = new List<NetIdCount>(products.Count);
        for (int i = 0; i < products.Count; i++)
        {
            Product product = products[i];
            if (product == null || product.Count <= 0)
                continue;
            int id = itemId ? product.ItemId : product.ObjId;
            if (id <= 0)
                continue;
            rows.Add(new NetIdCount { Id = id, Count = product.Count });
        }

        return rows.ToArray();
    }
}
