using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static void NormalizeEquippedIdentity(ref ItemCategory category, ref ItemSlot slot)
    {
        if (slot != ItemSlot.lhand_shield)
            return;
        slot = ItemSlot.lhand;
        category = ItemCategory.ShieldArmor;
    }

    static bool IsJewelItemSlot(ItemSlot slot)
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

    static ItemCategory CategoryFromZone(NetItemEntry row)
    {
        if (row.ItemId == 57)
            return ItemCategory.Adena;
        if (IsJewelBody(row.BodySlot))
            return ItemCategory.Jewel;
        string kind = row.Kind ?? "";
        if (kind == "weapon")
            return ItemCategory.Weapon;
        if (kind == "armor")
            return ItemCategory.ShieldArmor;
        return ItemCategory.Item;
    }

    static ItemSlot SlotFromBodyToken(string bodySlot)
    {
        if (string.IsNullOrEmpty(bodySlot) || bodySlot == "none")
            return ItemSlot.none;
        switch (bodySlot)
        {
            case "neck":
                return ItemSlot.neck;
            case "head":
                return ItemSlot.head;
            case "lhand":
                return ItemSlot.lhand;
            case "gloves":
                return ItemSlot.gloves;
            case "chest":
                return ItemSlot.chest;
            case "rhand":
                return ItemSlot.rhand;
            case "lear":
                return ItemSlot.lear;
            case "feet":
                return ItemSlot.feet;
            case "legs":
                return ItemSlot.legs;
            case "lrhand":
                return ItemSlot.lrhand;
            case "fullarmor":
                return ItemSlot.fullarmor;
            case "rear":
                return ItemSlot.rear;
            case "lfinger":
                return ItemSlot.lfinger;
            case "rfinger":
                return ItemSlot.rfinger;
            case "underwear":
                return ItemSlot.underwear;
            case "both_ears":
                return ItemSlot.lear;
            case "both_fingers":
                return ItemSlot.lfinger;
            case "hair":
            case "hairall":
                return ItemSlot.hat1;
            case "hair2":
                return ItemSlot.hat2;
            case "alldress":
                return ItemSlot.chest;
            default:
                return ItemSlot.none;
        }
    }

    static bool IsJewelBody(string bodySlot)
    {
        switch (bodySlot)
        {
            case "neck":
            case "lear":
            case "rear":
            case "both_ears":
            case "lfinger":
            case "rfinger":
            case "both_fingers":
                return true;
            default:
                return false;
        }
    }

    static void ApplyUserPaperdoll(PlayerAppearance appearance, NetPaperdollSlot[] slots) =>
        NetPaperdollApply.ApplyUserPaperdoll(appearance, slots);
}
