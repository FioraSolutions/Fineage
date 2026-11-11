using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class NetSession
{
    internal void ApplyNpcHtml(NpcHtmlNetMsg html)
    {
        HtmlWindow window = GameWindowPacketServices.Html;
        if (window == null)
            return;
        window.ShowNpcDialog(html.NpcObjectId, html.NpcId, html.Html ?? "");
    }

    internal void ApplyBuyList(BuyListNetMsg list)
    {
        BuyListItemNet[] src = list.Items ?? System.Array.Empty<BuyListItemNet>();
        var items = new NetBuyItem[src.Length];
        for (int i = 0; i < src.Length; i++)
            items[i] = new NetBuyItem { ItemId = src[i].ItemId, Price = src[i].Price, Count = src[i].Count };
        NetNpcWindows.ApplyBuyList(new NetBuyList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            BaseTaxPercent = list.BaseTaxPercent,
            CastleTaxPercent = list.CastleTaxPercent,
            Items = items
        });
    }

    internal void ApplySellList(SellListNetMsg list)
    {
        SellListItemNet[] src = list.Items ?? System.Array.Empty<SellListItemNet>();
        var items = new NetSellItem[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            items[i] = new NetSellItem
            {
                ObjectId = src[i].ObjectId,
                ItemId = src[i].ItemId,
                Count = src[i].Count,
                Price = src[i].Price,
                EnchantLevel = src[i].EnchantLevel
            };
        }

        NetNpcWindows.ApplySellList(new NetSellList { NpcObjectId = list.NpcObjectId, Items = items });
    }

    internal void ApplyMultiSellList(MultiSellListNetMsg list)
    {
        MultiSellEntryNet[] src = list.Entries ?? System.Array.Empty<MultiSellEntryNet>();
        var entries = new NetMultiSellEntry[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            entries[i] = new NetMultiSellEntry
            {
                EntryId = src[i].EntryId,
                Ingredients = MapMultiParts(src[i].Ingredients),
                Products = MapMultiParts(src[i].Products)
            };
        }

        NetNpcWindows.ApplyMultiSellList(new NetMultiSellList
        {
            NpcObjectId = list.NpcObjectId,
            ListId = list.ListId,
            MaintainEnchantment = list.MaintainEnchantment,
            InventoryOnly = list.InventoryOnly,
            CastleTaxPercent = list.CastleTaxPercent,
            Entries = entries
        });
    }

    static NetMultiPart[] MapMultiParts(MultiSellPartNet[] parts)
    {
        if (parts == null || parts.Length == 0)
            return System.Array.Empty<NetMultiPart>();
        var rows = new NetMultiPart[parts.Length];
        for (int i = 0; i < parts.Length; i++)
            rows[i] = new NetMultiPart
            {
                ItemId = parts[i].ItemId,
                Count = parts[i].Count,
                EnchantLevel = parts[i].EnchantLevel
            };
        return rows;
    }

    internal void ApplyShopBalance(ShopBalanceNetMsg balance)
    {
        NetNpcWindows.ApplyShopBalance(new NetShopBalance
        {
            Adena = balance.Adena,
            Weight = balance.Weight,
            MaxWeight = balance.MaxWeight,
            Slots = balance.Slots,
            MaxSlots = balance.MaxSlots,
            BaseTaxPercent = balance.BaseTaxPercent,
            CastleTaxPercent = balance.CastleTaxPercent
        });
    }
}
