using System;
using System.IO;

/// <summary>
/// Zone V2 NPC dialog, quest, shop, and service bodies.
/// Keys match Fineage-Backend <c>Messages.cs</c>. Extra keys are skipped.
/// </summary>
public static partial class ZoneV2Messages
{
    public static byte[] EncodeRequestQuestList()
    {
        return ZoneV2MsgPack.WriteArray(_ => { }, 0);
    }

    public static byte[] EncodeRequestQuestAbort(int questId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, questId), 1);
    }

    public static byte[] EncodeNpcTalk(int npcObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, npcObjectId), 1);
    }

    public static byte[] EncodeQuestEvent(int npcObjectId, int questId, string evt)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteInt(w, questId);
            ZoneV2MsgPack.WriteString(w, evt ?? "");
        }, 3);
    }

    public static byte[] EncodeBypassRequest(int npcObjectId, string command)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteString(w, command ?? "");
        }, 2);
    }

    public static byte[] EncodeRequestBuyItem(int npcObjectId, int listId, ZoneV2IdCount[] items)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteInt(w, listId);
            WriteIdCounts(w, items);
        }, 3);
    }

    public static byte[] EncodeRequestSellItem(int npcObjectId, ZoneV2IdCount[] items)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            WriteIdCounts(w, items);
        }, 2);
    }

    public static byte[] EncodeRequestMultiSellChoose(int npcObjectId, int listId, int entryId, int amount)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteInt(w, listId);
            ZoneV2MsgPack.WriteInt(w, entryId);
            ZoneV2MsgPack.WriteInt(w, amount);
        }, 4);
    }

    public static byte[] EncodeRequestPreviewItem(int npcObjectId, int listId, int itemId)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteInt(w, listId);
            ZoneV2MsgPack.WriteInt(w, itemId);
        }, 3);
    }

    public static byte[] EncodeTeleportSelect(int npcObjectId, string listName, int locationId)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            ZoneV2MsgPack.WriteString(w, listName ?? "");
            ZoneV2MsgPack.WriteInt(w, locationId);
        }, 3);
    }

    public static byte[] EncodeWarehouseDeposit(int npcObjectId, ZoneV2IdCount[] lines)
    {
        return EncodeWarehouse(npcObjectId, lines);
    }

    public static byte[] EncodeWarehouseWithdraw(int npcObjectId, ZoneV2IdCount[] lines)
    {
        return EncodeWarehouse(npcObjectId, lines);
    }

    public static byte[] EncodeSupportMagic(int npcObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, npcObjectId), 1);
    }

    public static byte[] EncodeGiveBlessing(int npcObjectId)
    {
        return ZoneV2MsgPack.WriteArray(w => ZoneV2MsgPack.WriteInt(w, npcObjectId), 1);
    }

    public static bool TryDecodeNpcHtml(byte[] body, out ZoneV2NpcHtml html)
    {
        html = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            html.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            html.NpcId = ZoneV2MsgPack.ReadInt(body, ref i);
            html.Html = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            int used = 3;
            if (n > used)
            {
                html.ItemId = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            SkipRest(body, ref i, n, used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeQuestList(byte[] body, out ZoneV2QuestList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            var active = new ZoneV2QuestJournal[count];
            for (int r = 0; r < count; r++)
            {
                if (!TryReadQuestJournal(body, ref i, out active[r]))
                    return false;
            }

            list.Active = active;
            int used = 1;
            if (n > used)
            {
                list.CompletedMask = ZoneV2MsgPack.ReadBytes(body, ref i) ?? Array.Empty<byte>();
                used++;
            }
            else
            {
                list.CompletedMask = Array.Empty<byte>();
            }

            if (n > used)
            {
                list.CompletedQuestIds = ZoneV2MsgPack.ReadIntArray(body, ref i) ?? Array.Empty<int>();
                used++;
            }
            else
            {
                list.CompletedQuestIds = Array.Empty<int>();
            }

            if (n > used)
            {
                list.MaxActive = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            SkipRest(body, ref i, n, used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeQuestDialog(byte[] body, out ZoneV2QuestDialog dialog)
    {
        dialog = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 6)
                return false;
            dialog.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            dialog.NpcId = ZoneV2MsgPack.ReadInt(body, ref i);
            dialog.QuestId = ZoneV2MsgPack.ReadInt(body, ref i);
            dialog.Hook = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            dialog.Event = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            dialog.Html = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            SkipRest(body, ref i, n, 6);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeQuestSound(byte[] body, out string name)
    {
        name = "";
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 1)
                return false;
            name = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            SkipRest(body, ref i, n, 1);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeBuyList(byte[] body, out ZoneV2BuyList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 5)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.ListId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.BaseTaxPercent = ZoneV2MsgPack.ReadInt(body, ref i);
            list.CastleTaxPercent = ZoneV2MsgPack.ReadInt(body, ref i);
            int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            var items = new ZoneV2BuyItem[count];
            for (int r = 0; r < count; r++)
            {
                int kn = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
                if (kn < 3)
                    return false;
                items[r].ItemId = ZoneV2MsgPack.ReadInt(body, ref i);
                items[r].Price = ZoneV2MsgPack.ReadInt(body, ref i);
                items[r].Count = ZoneV2MsgPack.ReadInt(body, ref i);
                SkipRest(body, ref i, kn, 3);
            }

            list.Items = items;
            SkipRest(body, ref i, n, 5);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeSellList(byte[] body, out ZoneV2SellList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 2)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.Items = ReadSellItems(body, ref i);
            SkipRest(body, ref i, n, 2);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeMultiSellList(byte[] body, out ZoneV2MultiSellList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 6)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.ListId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.MaintainEnchantment = ZoneV2MsgPack.ReadBool(body, ref i);
            list.InventoryOnly = ZoneV2MsgPack.ReadBool(body, ref i);
            list.CastleTaxPercent = ZoneV2MsgPack.ReadInt(body, ref i);
            int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            var entries = new ZoneV2MultiSellEntry[count];
            for (int r = 0; r < count; r++)
            {
                int kn = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
                if (kn < 3)
                    return false;
                entries[r].EntryId = ZoneV2MsgPack.ReadInt(body, ref i);
                entries[r].Ingredients = ReadMultiParts(body, ref i);
                entries[r].Products = ReadMultiParts(body, ref i);
                SkipRest(body, ref i, kn, 3);
            }

            list.Entries = entries;
            SkipRest(body, ref i, n, 6);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeShopBalance(byte[] body, out ZoneV2ShopBalance balance)
    {
        balance = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 5)
                return false;
            balance.Adena = ZoneV2MsgPack.ReadInt(body, ref i);
            balance.Weight = ZoneV2MsgPack.ReadInt(body, ref i);
            balance.MaxWeight = ZoneV2MsgPack.ReadInt(body, ref i);
            balance.Slots = ZoneV2MsgPack.ReadInt(body, ref i);
            balance.MaxSlots = ZoneV2MsgPack.ReadInt(body, ref i);
            int used = 5;
            if (n > used)
            {
                balance.BaseTaxPercent = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            if (n > used)
            {
                balance.CastleTaxPercent = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            SkipRest(body, ref i, n, used);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeTeleportList(byte[] body, out ZoneV2TeleportList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 4)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.ListName = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            list.ListType = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            var rows = new ZoneV2TeleportLocation[count];
            for (int r = 0; r < count; r++)
            {
                int kn = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
                if (kn < 7)
                    return false;
                rows[r].Id = ZoneV2MsgPack.ReadInt(body, ref i);
                rows[r].Name = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
                rows[r].X = ZoneV2MsgPack.ReadInt(body, ref i);
                rows[r].Y = ZoneV2MsgPack.ReadInt(body, ref i);
                rows[r].Z = ZoneV2MsgPack.ReadInt(body, ref i);
                rows[r].FeeItemId = ZoneV2MsgPack.ReadInt(body, ref i);
                rows[r].FeeCount = ZoneV2MsgPack.ReadInt(body, ref i);
                SkipRest(body, ref i, kn, 7);
            }

            list.Locations = rows;
            SkipRest(body, ref i, n, 4);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeWarehouseDepositList(byte[] body, out ZoneV2WarehouseDepositList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 6)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.FeePerLine = ZoneV2MsgPack.ReadInt(body, ref i);
            list.SlotsUsed = ZoneV2MsgPack.ReadInt(body, ref i);
            list.SlotsMax = ZoneV2MsgPack.ReadInt(body, ref i);
            list.Adena = ZoneV2MsgPack.ReadInt(body, ref i);
            if (!ZoneV2Messages.TryReadItemRows(body, ref i, out list.Items))
                return false;
            SkipRest(body, ref i, n, 6);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeWarehouseWithdrawList(byte[] body, out ZoneV2WarehouseWithdrawList list)
    {
        list = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 8)
                return false;
            list.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            list.SlotsUsed = ZoneV2MsgPack.ReadInt(body, ref i);
            list.SlotsMax = ZoneV2MsgPack.ReadInt(body, ref i);
            list.InventorySlotsUsed = ZoneV2MsgPack.ReadInt(body, ref i);
            list.InventorySlotsMax = ZoneV2MsgPack.ReadInt(body, ref i);
            list.CurrentWeight = ZoneV2MsgPack.ReadInt(body, ref i);
            list.MaxWeight = ZoneV2MsgPack.ReadInt(body, ref i);
            if (!TryReadItemRows(body, ref i, out list.Items))
                return false;
            SkipRest(body, ref i, n, 8);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeNewbieBuffResult(byte[] body, out ZoneV2NewbieBuffResult result)
    {
        result = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            result.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            result.SkillIds = ZoneV2MsgPack.ReadIntArray(body, ref i) ?? Array.Empty<int>();
            result.Refusal = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
            SkipRest(body, ref i, n, 3);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    public static bool TryDecodeBlessingResult(byte[] body, out ZoneV2BlessingResult result)
    {
        result = default;
        try
        {
            int i = 0;
            int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (n < 3)
                return false;
            result.NpcObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            result.SkillId = ZoneV2MsgPack.ReadInt(body, ref i);
            result.Accepted = ZoneV2MsgPack.ReadBool(body, ref i);
            SkipRest(body, ref i, n, 3);
            return true;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    static byte[] EncodeWarehouse(int npcObjectId, ZoneV2IdCount[] lines)
    {
        return ZoneV2MsgPack.WriteArray(w =>
        {
            ZoneV2MsgPack.WriteInt(w, npcObjectId);
            WriteIdCounts(w, lines);
        }, 2);
    }

    static void WriteIdCounts(BinaryWriter w, ZoneV2IdCount[] items)
    {
        if (items == null)
            items = Array.Empty<ZoneV2IdCount>();
        ZoneV2MsgPack.WriteArrayHeader(w, items.Length);
        for (int i = 0; i < items.Length; i++)
        {
            ZoneV2MsgPack.WriteArrayHeader(w, 2);
            ZoneV2MsgPack.WriteInt(w, items[i].Id);
            ZoneV2MsgPack.WriteInt(w, items[i].Count);
        }
    }

    static bool TryReadQuestJournal(byte[] body, ref int i, out ZoneV2QuestJournal row)
    {
        row = default;
        int n = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (n < 2)
            return false;
        row.QuestId = ZoneV2MsgPack.ReadInt(body, ref i);
        row.Name = ZoneV2MsgPack.ReadString(body, ref i) ?? "";
        int used = 2;
        if (n > used)
        {
            row.State = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n > used)
        {
            row.Cond = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n > used)
        {
            row.ProgressFlags = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n > used)
        {
            row.JournalWord = ZoneV2MsgPack.ReadInt(body, ref i);
            used++;
        }

        if (n > used)
        {
            row.Repeatable = ZoneV2MsgPack.ReadBool(body, ref i);
            used++;
        }

        SkipRest(body, ref i, n, used);
        return true;
    }

    static ZoneV2SellItem[] ReadSellItems(byte[] body, ref int i)
    {
        int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        var items = new ZoneV2SellItem[count];
        for (int r = 0; r < count; r++)
        {
            int kn = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (kn < 4)
                throw new InvalidDataException("short sell row");
            items[r].ObjectId = ZoneV2MsgPack.ReadInt(body, ref i);
            items[r].ItemId = ZoneV2MsgPack.ReadInt(body, ref i);
            items[r].Count = ZoneV2MsgPack.ReadInt(body, ref i);
            items[r].Price = ZoneV2MsgPack.ReadInt(body, ref i);
            int used = 4;
            if (kn > used)
            {
                items[r].EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            SkipRest(body, ref i, kn, used);
        }

        return items;
    }

    static ZoneV2MultiPart[] ReadMultiParts(byte[] body, ref int i)
    {
        int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        var parts = new ZoneV2MultiPart[count];
        for (int r = 0; r < count; r++)
        {
            int kn = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
            if (kn < 2)
                throw new InvalidDataException("short multisell part");
            parts[r].ItemId = ZoneV2MsgPack.ReadInt(body, ref i);
            parts[r].Count = ZoneV2MsgPack.ReadInt(body, ref i);
            int used = 2;
            if (kn > used)
            {
                parts[r].EnchantLevel = ZoneV2MsgPack.ReadInt(body, ref i);
                used++;
            }

            SkipRest(body, ref i, kn, used);
        }

        return parts;
    }

    /// <summary>Item rows share <see cref="TryReadItemEntry"/> with the bag list.</summary>
    static bool TryReadItemRows(byte[] body, ref int i, out ZoneV2ItemEntry[] items)
    {
        items = Array.Empty<ZoneV2ItemEntry>();
        int count = ZoneV2MsgPack.ReadArrayHeader(body, ref i);
        if (count <= 0)
            return true;
        var rows = new ZoneV2ItemEntry[count];
        for (int r = 0; r < count; r++)
        {
            if (!TryReadItemEntry(body, ref i, out rows[r]))
                return false;
        }

        items = rows;
        return true;
    }

    static void SkipRest(byte[] body, ref int i, int total, int used)
    {
        if (total > used)
            ZoneV2MsgPack.SkipValues(body, ref i, total - used);
    }
}

public struct ZoneV2IdCount
{
    public int Id;
    public int Count;
}

public struct ZoneV2NpcHtml
{
    public int NpcObjectId;
    public int NpcId;
    public string Html;
    public int ItemId;
}

public struct ZoneV2QuestJournal
{
    public int QuestId;
    public string Name;
    public int State;
    public int Cond;
    public int ProgressFlags;
    public int JournalWord;
    public bool Repeatable;
}

public struct ZoneV2QuestList
{
    public ZoneV2QuestJournal[] Active;
    public byte[] CompletedMask;
    public int[] CompletedQuestIds;
    public int MaxActive;
}

public struct ZoneV2QuestDialog
{
    public int NpcObjectId;
    public int NpcId;
    public int QuestId;
    public string Hook;
    public string Event;
    public string Html;
}

public struct ZoneV2BuyItem
{
    public int ItemId;
    public int Price;
    public int Count;
}

public struct ZoneV2BuyList
{
    public int NpcObjectId;
    public int ListId;
    public int BaseTaxPercent;
    public int CastleTaxPercent;
    public ZoneV2BuyItem[] Items;
}

public struct ZoneV2SellItem
{
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int Price;
    public int EnchantLevel;
}

public struct ZoneV2SellList
{
    public int NpcObjectId;
    public ZoneV2SellItem[] Items;
}

public struct ZoneV2MultiPart
{
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct ZoneV2MultiSellEntry
{
    public int EntryId;
    public ZoneV2MultiPart[] Ingredients;
    public ZoneV2MultiPart[] Products;
}

public struct ZoneV2MultiSellList
{
    public int NpcObjectId;
    public int ListId;
    public bool MaintainEnchantment;
    public bool InventoryOnly;
    public int CastleTaxPercent;
    public ZoneV2MultiSellEntry[] Entries;
}

public struct ZoneV2ShopBalance
{
    public int Adena;
    public int Weight;
    public int MaxWeight;
    public int Slots;
    public int MaxSlots;
    public int BaseTaxPercent;
    public int CastleTaxPercent;
}

public struct ZoneV2TeleportLocation
{
    public int Id;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public int FeeItemId;
    public int FeeCount;
}

public struct ZoneV2TeleportList
{
    public int NpcObjectId;
    public string ListName;
    public string ListType;
    public ZoneV2TeleportLocation[] Locations;
}

public struct ZoneV2WarehouseDepositList
{
    public int NpcObjectId;
    public int FeePerLine;
    public int SlotsUsed;
    public int SlotsMax;
    public int Adena;
    public ZoneV2ItemEntry[] Items;
}

public struct ZoneV2WarehouseWithdrawList
{
    public int NpcObjectId;
    public int SlotsUsed;
    public int SlotsMax;
    public int InventorySlotsUsed;
    public int InventorySlotsMax;
    public int CurrentWeight;
    public int MaxWeight;
    public ZoneV2ItemEntry[] Items;
}

public struct ZoneV2NewbieBuffResult
{
    public int NpcObjectId;
    public int[] SkillIds;
    public string Refusal;
}

public struct ZoneV2BlessingResult
{
    public int NpcObjectId;
    public int SkillId;
    public bool Accepted;
}
