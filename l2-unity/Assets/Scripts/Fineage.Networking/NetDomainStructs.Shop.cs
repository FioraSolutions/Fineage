using System;


public struct NetBuyItem
{
    public int ItemId;
    public int Price;
    public int Count;
}

public struct NetBuyList
{
    public int NpcObjectId;
    public int ListId;
    public int BaseTaxPercent;
    public int CastleTaxPercent;
    public NetBuyItem[] Items;
}

public struct NetSellItem
{
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int Price;
    public int EnchantLevel;
}

public struct NetSellList
{
    public int NpcObjectId;
    public NetSellItem[] Items;
}

public struct NetMultiPart
{
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct NetMultiSellEntry
{
    public int EntryId;
    public NetMultiPart[] Ingredients;
    public NetMultiPart[] Products;
}

public struct NetMultiSellList
{
    public int NpcObjectId;
    public int ListId;
    public bool MaintainEnchantment;
    public bool InventoryOnly;
    public int CastleTaxPercent;
    public NetMultiSellEntry[] Entries;
}

public struct NetShopBalance
{
    public int Adena;
    public int Weight;
    public int MaxWeight;
    public int Slots;
    public int MaxSlots;
    public int BaseTaxPercent;
    public int CastleTaxPercent;
}

public struct NetTeleportLocation
{
    public int Id;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public int FeeItemId;
    public int FeeCount;
}

public struct NetTeleportList
{
    public int NpcObjectId;
    public string ListName;
    public string ListType;
    public NetTeleportLocation[] Locations;
}

public struct NetWarehouseDepositList
{
    public int NpcObjectId;
    public int FeePerLine;
    public int SlotsUsed;
    public int SlotsMax;
    public int Adena;
    public NetItemEntry[] Items;
}

public struct NetWarehouseWithdrawList
{
    public int NpcObjectId;
    public int SlotsUsed;
    public int SlotsMax;
    public int InventorySlotsUsed;
    public int InventorySlotsMax;
    public int CurrentWeight;
    public int MaxWeight;
    public NetItemEntry[] Items;
}

public struct NetNewbieBuffResult
{
    public int NpcObjectId;
    public int[] SkillIds;
    public string Refusal;
}

public struct NetBlessingResult
{
    public int NpcObjectId;
    public int SkillId;
    public bool Accepted;
}
