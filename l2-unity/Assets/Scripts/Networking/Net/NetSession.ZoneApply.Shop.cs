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


    internal void ApplyWarehouseDepositList(WarehouseDepositListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseDepositList(new NetWarehouseDepositList
        {
            NpcObjectId = list.NpcObjectId,
            FeePerLine = list.FeePerLine,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            Adena = list.Adena,
            Items = MapWarehouseItems(list.Items)
        });
    }


    internal void ApplyWarehouseWithdrawList(WarehouseWithdrawListNetMsg list)
    {
        NetNpcWindows.ApplyWarehouseWithdrawList(new NetWarehouseWithdrawList
        {
            NpcObjectId = list.NpcObjectId,
            SlotsUsed = list.SlotsUsed,
            SlotsMax = list.SlotsMax,
            InventorySlotsUsed = list.InventorySlotsUsed,
            InventorySlotsMax = list.InventorySlotsMax,
            CurrentWeight = list.CurrentWeight,
            MaxWeight = list.MaxWeight,
            Items = MapWarehouseItems(list.Items)
        });
    }


    internal void ApplyQuestList(QuestListNetMsg list)
    {
        QuestJournalEntryNet[] active = list.Active ?? System.Array.Empty<QuestJournalEntryNet>();
        var rows = new NetQuestJournal[active.Length];
        for (int i = 0; i < active.Length; i++)
        {
            QuestJournalEntryNet row = active[i] ?? new QuestJournalEntryNet();
            rows[i] = new NetQuestJournal
            {
                QuestId = row.QuestId,
                Name = row.Name ?? "",
                State = row.State,
                Cond = row.Cond,
                ProgressFlags = row.ProgressFlags,
                JournalWord = row.JournalWord,
                Repeatable = row.Repeatable
            };
        }

        NetNpcWindows.ApplyQuestList(new NetQuestList
        {
            Active = rows,
            CompletedMask = list.CompletedMask ?? System.Array.Empty<byte>(),
            CompletedQuestIds = list.CompletedQuestIds ?? System.Array.Empty<int>(),
            MaxActive = list.MaxActive
        });
    }


    internal void ApplyAcquireSkillList(AcquireSkillListNetMsg list)
    {
        AcquireSkillEntryNet[] rows = list.Skills ?? System.Array.Empty<AcquireSkillEntryNet>();
        var ids = new int[rows.Length];
        var next = new int[rows.Length];
        var max = new int[rows.Length];
        var sp = new int[rows.Length];
        var requirements = new int[rows.Length];
        for (int i = 0; i < rows.Length; i++)
        {
            AcquireSkillEntryNet row = rows[i] ?? new AcquireSkillEntryNet();
            ids[i] = row.SkillId;
            next[i] = row.NextLevel;
            max[i] = row.MaxLevel;
            sp[i] = row.SpCost;
            requirements[i] = row.Requirements;
        }

        NetNpcWindows.ApplyAcquireSkillList(
            AcquireSkillListDto.FromNet(list.SkillType, ids, next, max, sp, requirements));
    }


    internal void ApplyAcquireSkillInfo(AcquireSkillInfoNetMsg info)
    {
        AcquireSkillRequirementNet[] reqs = info.Requirements ?? System.Array.Empty<AcquireSkillRequirementNet>();
        var types = new int[reqs.Length];
        var itemIds = new int[reqs.Length];
        var counts = new long[reqs.Length];
        var unks = new int[reqs.Length];
        for (int i = 0; i < reqs.Length; i++)
        {
            AcquireSkillRequirementNet req = reqs[i] ?? new AcquireSkillRequirementNet();
            types[i] = req.Type;
            itemIds[i] = req.ItemId;
            counts[i] = req.Count;
            unks[i] = req.Unk;
        }

        NetNpcWindows.ApplyAcquireSkillInfo(
            AcquireSkillInfoDto.FromNet(info.SkillId, info.Level, info.SpCost, info.SkillType, types, itemIds, counts, unks));
    }


    internal void ApplyEnchantResult(EnchantResultNetMsg result)
    {
        NetNpcWindows.ApplyEnchantResult(new NetEnchantResult
        {
            Result = result.Result,
            EnchantLevel = result.EnchantLevel,
            ObjectId = result.ObjectId,
            CrystalItemId = result.CrystalItemId,
            CrystalCount = result.CrystalCount
        });
    }


    static NetItemEntry[] MapWarehouseItems(ItemListEntryNet[] items)
    {
        if (items == null || items.Length == 0)
            return System.Array.Empty<NetItemEntry>();
        var rows = new NetItemEntry[items.Length];
        for (int i = 0; i < items.Length; i++)
        {
            rows[i] = new NetItemEntry
            {
                ObjectId = items[i].ObjectId,
                ItemId = items[i].ItemId,
                Count = items[i].Count,
                EquippedSlot = items[i].EquippedSlot,
                EnchantLevel = items[i].EnchantLevel,
                BodySlot = "none",
                Kind = "etc",
                Icon = "",
                WeaponType = "",
                ArmorType = ""
            };
        }

        return rows;
    }


}
