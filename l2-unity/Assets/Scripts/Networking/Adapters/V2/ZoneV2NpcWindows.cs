using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies NPC HTML, quest, shop, and service packets onto the existing windows.
/// </summary>
public static class ZoneV2NpcWindows
{
    public static bool TryTake(ZoneV2MsgType type, byte[] body, Action<Action> post)
    {
        if (post == null)
            return false;
        switch (type)
        {
            case ZoneV2MsgType.NpcHtml:
                ZoneV2NpcHtml html;
                if (!ZoneV2Messages.TryDecodeNpcHtml(body, out html))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad NpcHtml body"));
                    return true;
                }

                ZoneV2NpcHtml htmlCopy = html;
                post(() => ShowHtml(htmlCopy.NpcObjectId, htmlCopy.NpcId, htmlCopy.Html));
                return true;
            case ZoneV2MsgType.QuestList:
                ZoneV2QuestList quests;
                if (!ZoneV2Messages.TryDecodeQuestList(body, out quests))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad QuestList body"));
                    return true;
                }

                ZoneV2QuestList questCopy = quests;
                post(() => ShowQuests(questCopy));
                return true;
            case ZoneV2MsgType.QuestDialog:
                ZoneV2QuestDialog dialog;
                if (!ZoneV2Messages.TryDecodeQuestDialog(body, out dialog))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad QuestDialog body"));
                    return true;
                }

                ZoneV2QuestDialog dialogCopy = dialog;
                post(() => ShowHtml(dialogCopy.NpcObjectId, dialogCopy.NpcId, dialogCopy.Html));
                return true;
            case ZoneV2MsgType.QuestSound:
                string sound;
                if (!ZoneV2Messages.TryDecodeQuestSound(body, out sound))
                    return true;
                post(() => Debug.Log("[ZoneV2] QuestSound " + sound));
                return true;
            case ZoneV2MsgType.BuyList:
                ZoneV2BuyList buy;
                if (!ZoneV2Messages.TryDecodeBuyList(body, out buy))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad BuyList body"));
                    return true;
                }

                ZoneV2BuyList buyCopy = buy;
                post(() => ShowBuy(buyCopy));
                return true;
            case ZoneV2MsgType.SellList:
                ZoneV2SellList sell;
                if (!ZoneV2Messages.TryDecodeSellList(body, out sell))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad SellList body"));
                    return true;
                }

                ZoneV2SellList sellCopy = sell;
                post(() => ShowSell(sellCopy));
                return true;
            case ZoneV2MsgType.MultiSellList:
                ZoneV2MultiSellList multi;
                if (!ZoneV2Messages.TryDecodeMultiSellList(body, out multi))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad MultiSellList body"));
                    return true;
                }

                ZoneV2MultiSellList multiCopy = multi;
                post(() => ShowMulti(multiCopy));
                return true;
            case ZoneV2MsgType.ShopBalance:
                ZoneV2ShopBalance balance;
                if (!ZoneV2Messages.TryDecodeShopBalance(body, out balance))
                    return true;
                ZoneV2ShopBalance balanceCopy = balance;
                post(() => ApplyBalance(balanceCopy));
                return true;
            case ZoneV2MsgType.TeleportList:
                ZoneV2TeleportList teleport;
                if (!ZoneV2Messages.TryDecodeTeleportList(body, out teleport))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad TeleportList body"));
                    return true;
                }

                ZoneV2TeleportList teleportCopy = teleport;
                post(() => ZoneV2NpcSession.OpenDialog(teleportCopy.NpcObjectId));
                return true;
            case ZoneV2MsgType.WarehouseDepositList:
                ZoneV2WarehouseDepositList deposit;
                if (!ZoneV2Messages.TryDecodeWarehouseDepositList(body, out deposit))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad WarehouseDepositList body"));
                    return true;
                }

                ZoneV2WarehouseDepositList depositCopy = deposit;
                post(() => ShowDeposit(depositCopy));
                return true;
            case ZoneV2MsgType.WarehouseWithdrawList:
                ZoneV2WarehouseWithdrawList withdraw;
                if (!ZoneV2Messages.TryDecodeWarehouseWithdrawList(body, out withdraw))
                {
                    post(() => Debug.LogWarning("[ZoneV2] bad WarehouseWithdrawList body"));
                    return true;
                }

                ZoneV2WarehouseWithdrawList withdrawCopy = withdraw;
                post(() => ShowWithdraw(withdrawCopy));
                return true;
            case ZoneV2MsgType.NewbieBuffResult:
                ZoneV2NewbieBuffResult buff;
                if (!ZoneV2Messages.TryDecodeNewbieBuffResult(body, out buff))
                    return true;
                ZoneV2NewbieBuffResult buffCopy = buff;
                post(() => ShowBuff(buffCopy));
                return true;
            case ZoneV2MsgType.BlessingResult:
                ZoneV2BlessingResult blessing;
                if (!ZoneV2Messages.TryDecodeBlessingResult(body, out blessing))
                    return true;
                if (!blessing.Accepted)
                    return true;
                post(() => ZoneV2ChatFeed.NoteSystem("The Blessing of Protection is upon you."));
                return true;
            default:
                return false;
        }
    }

    public static void Tick()
    {
        if (!ZoneV2NpcSession.WindowsOpen)
            return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
            return;
        }

        int npcId = ZoneV2NpcSession.ActiveNpcObjectId;
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
        float limit = ZoneV2NpcSession.InteractionDistance;
        return dx * dx + dy * dy + dz * dz <= limit * limit;
    }

    public static void Close()
    {
        ZoneV2NpcSession.CloseWindows();
        if (HtmlWindow.Instance != null)
            HtmlWindow.Instance.HideWindow();
        if (DealerWindow.Instance != null)
            DealerWindow.Instance.HideWindow();
        if (MultiSellWindow.Instance != null)
            MultiSellWindow.Instance.HideWindow();
    }

    static ZoneV2ShopBalance _balance;
    static bool _hasBalance;

    static void ShowHtml(int npcObjectId, int npcId, string html)
    {
        HtmlWindow window = HtmlWindow.Instance;
        if (window == null)
            return;
        window.ShowNpcDialog(npcObjectId, npcId, html);
    }

    static void ShowQuests(ZoneV2QuestList list)
    {
        QuestWindow window = QuestWindow.Instance;
        if (window == null || list.Active == null)
            return;
        var quests = new List<QuestInstance>(list.Active.Length);
        for (int i = 0; i < list.Active.Length; i++)
        {
            ZoneV2QuestJournal row = list.Active[i];
            int flags = row.JournalWord != 0 ? row.JournalWord : row.Cond;
            if (flags == 0)
                flags = 1;
            quests.Add(new QuestInstance(row.QuestId, flags, row.Repeatable ? 1 : 0, row.Name));
        }

        try
        {
            window.AddData(quests);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ZoneV2] quest list: " + ex.Message);
        }
    }

    static void ShowBuy(ZoneV2BuyList list)
    {
        ZoneV2NpcSession.OpenShop(list.NpcObjectId, list.ListId);
        var products = new List<Product>();
        if (list.Items != null)
        {
            for (int i = 0; i < list.Items.Length; i++)
            {
                ZoneV2BuyItem row = list.Items[i];
                int count = row.Count < 0 ? 10000 : row.Count;
                products.Add(new Product(0, 0, count, (int)EnumType2.TYPE2_OTHER, 0, 0, 0, row.Price, row.ItemId));
            }
        }

        OpenDealer("Shop", "Sell", "Buy", ProductType.BUY, products, false, list.ListId, _hasBalance ? _balance.Adena : 0);
    }

    static void ShowSell(ZoneV2SellList list)
    {
        ZoneV2NpcSession.OpenShop(list.NpcObjectId, 0);
        OpenDealer("Shop", "Inventory", "Sell", ProductType.SELL, SellProducts(list.Items), true, 0, _hasBalance ? _balance.Adena : 0);
    }

    static void ShowDeposit(ZoneV2WarehouseDepositList list)
    {
        ZoneV2NpcSession.OpenShop(list.NpcObjectId, 0);
        string title = "Private Warehouse (" + list.FeePerLine + " adena)";
        OpenDealer(title, "Inventory", "Deposit", ProductType.WHDepositList, BagProducts(list.Items), true, 0, list.Adena);
    }

    static void ShowWithdraw(ZoneV2WarehouseWithdrawList list)
    {
        ZoneV2NpcSession.OpenShop(list.NpcObjectId, 0);
        _balance.Weight = list.CurrentWeight;
        _balance.MaxWeight = list.MaxWeight;
        _hasBalance = true;
        OpenDealer("Private Warehouse", "Warehouse", "Withdraw", ProductType.WHWithdrawList, BagProducts(list.Items), true, 0, _hasBalance ? _balance.Adena : 0);
    }

    static void ShowMulti(ZoneV2MultiSellList list)
    {
        ZoneV2NpcSession.OpenShop(list.NpcObjectId, list.ListId);
        MultiSellWindow window = MultiSellWindow.Instance;
        if (window == null || list.Entries == null)
            return;
        var rows = new List<MultiSellData>(list.Entries.Length);
        var products = new List<ItemInstance>();
        for (int i = 0; i < list.Entries.Length; i++)
        {
            ZoneV2MultiSellEntry entry = list.Entries[i];
            var made = new List<ItemInstance>();
            if (entry.Products != null)
            {
                for (int p = 0; p < entry.Products.Length; p++)
                {
                    ZoneV2MultiPart part = entry.Products[p];
                    var item = new ItemInstance(0, part.ItemId, ItemLocation.Trade, products.Count, part.Count,
                        ItemsType.ParceCategory((int)EnumType2.TYPE2_OTHER), false, ItemSlot.none, part.EnchantLevel, 9999);
                    made.Add(item);
                    products.Add(item);
                }
            }

            var ingredients = new List<Ingredient>();
            if (entry.Ingredients != null)
            {
                for (int g = 0; g < entry.Ingredients.Length; g++)
                {
                    ZoneV2MultiPart part = entry.Ingredients[g];
                    ingredients.Add(new Ingredient(part.ItemId, (int)EnumType2.TYPE2_OTHER, part.Count, part.EnchantLevel));
                }
            }

            var row = new MultiSellData(made, ingredients);
            row.ZoneEntryId = entry.EntryId;
            rows.Add(row);
        }

        var dto = new MultiSellListDto();
        dto.LoadZoneV2(list.ListId, rows, products);
        try
        {
            window.AddData(products, dto);
            window.ShowWindow();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ZoneV2] multisell: " + ex.Message);
        }
    }

    static void ApplyBalance(ZoneV2ShopBalance balance)
    {
        _balance = balance;
        _hasBalance = true;
        DealerWindow dealer = DealerWindow.Instance;
        if (dealer == null)
            return;
        double percent = balance.MaxWeight > 0 ? (balance.Weight * 100.0) / balance.MaxWeight : 0;
        try
        {
            dealer.UpdateDataForm(balance.Adena, percent, balance.Weight, balance.MaxWeight);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[ZoneV2] shop balance: " + ex.Message);
        }
    }

    static void ShowBuff(ZoneV2NewbieBuffResult result)
    {
        ZoneV2NpcSession.OpenDialog(result.NpcObjectId);
        if (!string.IsNullOrEmpty(result.Refusal))
        {
            string line = ZoneV2SystemLines.BuffRefusal(result.Refusal);
            if (!string.IsNullOrEmpty(line))
                ZoneV2ChatFeed.NoteSystem(line);
            return;
        }

        if (result.SkillIds != null && result.SkillIds.Length > 0)
            ZoneV2ChatFeed.NoteSystem("Supplemental magic has been granted.");
    }

    static void OpenDealer(string title, string sellHeader, string buyHeader, ProductType type, List<Product> products, bool modifySell, int listId, int adena)
    {
        DealerWindow dealer = DealerWindow.Instance;
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
            Debug.LogWarning("[ZoneV2] shop window: " + ex.Message);
        }
    }

    static List<Product> SellProducts(ZoneV2SellItem[] items)
    {
        var products = new List<Product>();
        if (items == null)
            return products;
        for (int i = 0; i < items.Length; i++)
        {
            ZoneV2SellItem row = items[i];
            products.Add(new Product(0, row.ObjectId, row.Count, (int)EnumType2.TYPE2_OTHER, 0, 0, row.EnchantLevel, row.Price, row.ItemId));
        }

        return products;
    }

    static List<Product> BagProducts(ZoneV2ItemEntry[] items)
    {
        var products = new List<Product>();
        if (items == null)
            return products;
        for (int i = 0; i < items.Length; i++)
        {
            ZoneV2ItemEntry row = items[i];
            if (row.EquippedSlot >= 0)
                continue;
            products.Add(new Product(0, row.ObjectId, row.Count, (int)EnumType2.TYPE2_OTHER, 0, 0, row.EnchantLevel, 0, row.ItemId));
        }

        return products;
    }
}

public static class ZoneV2NpcOrders
{
    public static ZoneV2IdCount[] ItemCounts(List<Product> products)
    {
        return Counts(products, true);
    }

    public static ZoneV2IdCount[] ObjectCounts(List<Product> products)
    {
        return Counts(products, false);
    }

    static ZoneV2IdCount[] Counts(List<Product> products, bool itemId)
    {
        if (products == null || products.Count == 0)
            return Array.Empty<ZoneV2IdCount>();
        var rows = new List<ZoneV2IdCount>(products.Count);
        for (int i = 0; i < products.Count; i++)
        {
            Product product = products[i];
            if (product == null || product.Count <= 0)
                continue;
            int id = itemId ? product.ItemId : product.ObjId;
            if (id <= 0)
                continue;
            rows.Add(new ZoneV2IdCount { Id = id, Count = product.Count });
        }

        return rows.ToArray();
    }
}
