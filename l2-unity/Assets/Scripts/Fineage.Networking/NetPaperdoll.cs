using System;

/// <summary>
/// Paperdoll indexes from Fineage-Backend. Right hand is 7 (Squire's Sword 2369, Apprentice's Wand 6).
/// Missing indexes read as empty.
/// </summary>
public static class NetPaperdoll
{
    public const int Count = 17;
    public const int Under = 0;
    public const int LeftEar = 1;
    public const int RightEar = 2;
    public const int Neck = 3;
    public const int LeftFinger = 4;
    public const int RightFinger = 5;
    public const int Head = 6;
    public const int RightHand = 7;
    public const int LeftHand = 8;
    public const int Gloves = 9;
    public const int Chest = 10;
    public const int Legs = 11;
    public const int Feet = 12;
    public const int Cloak = 13;
    public const int Face = 14;
    public const int Hair = 15;
    public const int Hair2 = 16;

    public static int ItemIdAt(int[] itemIds, int slot)
    {
        if (itemIds == null || slot < 0 || slot >= itemIds.Length)
            return 0;
        return itemIds[slot];
    }

    public static int RightHandItemId(NetPaperdollSlot[] slots)
    {
        if (slots == null || slots.Length <= RightHand)
            return 0;
        return slots[RightHand].ItemId;
    }

    public static int[] ItemIds(NetPaperdollSlot[] slots)
    {
        if (slots == null || slots.Length == 0)
            return Array.Empty<int>();
        var ids = new int[slots.Length];
        for (int i = 0; i < slots.Length; i++)
            ids[i] = slots[i].ItemId;
        return ids;
    }

    /// <summary>Inventory paperdoll cell name (ItemSlot) to the zone index. Unknown cells are -1.</summary>
    public static int IndexForGearCell(string cell)
    {
        if (string.IsNullOrEmpty(cell))
            return -1;
        switch (cell)
        {
            case "underwear":
                return Under;
            case "lear":
                return LeftEar;
            case "rear":
                return RightEar;
            case "neck":
                return Neck;
            case "lfinger":
                return LeftFinger;
            case "rfinger":
                return RightFinger;
            case "head":
                return Head;
            case "rhand":
                return RightHand;
            case "lhand":
                return LeftHand;
            case "gloves":
                return Gloves;
            case "chest":
            case "fullarmor":
            case "onepiece":
            case "alldress":
                // Mobius full-body robes occupy the chest paperdoll cell.
                return Chest;
            case "legs":
                return Legs;
            case "feet":
            case "boots":
                return Feet;
            case "hat1":
            case "hair":
                return Hair;
            case "hat2":
            case "hair2":
                return Hair2;
            default:
                return -1;
        }
    }

    /// <summary>
    /// Two-handed weapons occupy the right hand. Full armor and formal wear occupy the chest.
    /// A ghost icon on the other cell still clears that primary slot.
    /// </summary>
    public static int UnequipIndex(int cellIndex, string bodySlot)
    {
        if (bodySlot == "lrhand")
            return RightHand;
        if (bodySlot == "fullarmor" || bodySlot == "alldress")
            return Chest;
        if (cellIndex < 0 || cellIndex >= Count)
            return -1;
        return cellIndex;
    }
}

public struct NetPaperdollSlot
{
    public int ObjectId;
    public int ItemId;
    public int EnchantLevel;
}
