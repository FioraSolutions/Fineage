using System;


public struct NetItemEntry
{
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int EquippedSlot;
    public string BodySlot;
    public string Kind;
    public string Icon;
    public string WeaponType;
    public string ArmorType;
    public int EnchantLevel;
}



public struct NetInventoryChange
{
    public int Change;
    public int ObjectId;
    public int ItemId;
    public int Count;
    public int EquippedSlot;
    public string BodySlot;
    public string Kind;
    public string Icon;
    public string WeaponType;
    public string ArmorType;
    public int EnchantLevel;
}

public struct NetEnchantResult
{
    public int Result;
    public int EnchantLevel;
    public int ObjectId;
    public int CrystalItemId;
    public int CrystalCount;
}
