using System;

/// <summary>
/// Fineage Net payload rows shared by the Net domain mapping.
/// Engine-free: no UnityEngine references.
/// </summary>

public struct NetIdCount
{
    public int Id;
    public int Count;
}

public struct NetNpcHtml
{
    public int NpcObjectId;
    public int NpcId;
    public string Html;
    public int ItemId;
}

public struct NetQuestJournal
{
    public int QuestId;
    public string Name;
    public int State;
    public int Cond;
    public int ProgressFlags;
    public int JournalWord;
    public bool Repeatable;
}

public struct NetQuestList
{
    public NetQuestJournal[] Active;
    public byte[] CompletedMask;
    public int[] CompletedQuestIds;
    public int MaxActive;
}

public struct NetQuestDialog
{
    public int NpcObjectId;
    public int NpcId;
    public int QuestId;
    public string Hook;
    public string Event;
    public string Html;
}

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

public struct NetUserInfo
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
    public bool HasPaperdoll;
    public NetPaperdollSlot[] Paperdoll;
    public bool HasClan;
    public int ClanId;
    public int ClanCrestId;
    public bool HasVitals;
    public int Level;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp;
    public bool HasStats;
    public int Str, Dex, Con, Int, Wit, Men;
    public int PAtk, MAtk, PDef, MDef, PAtkSpd, MAtkSpd, Accuracy, CritRate;
}

public struct NetMyTargetSelected
{
    public int ObjectId;
    public int ColorHint;
}

public struct NetAttack
{
    public int AttackerObjectId;
    public int TargetObjectId;
    public int Damage;
    public byte Flags;
    public int AttackerX;
    public int AttackerY;
    public int AttackerZ;
    public int TargetX;
    public int TargetY;
    public int TargetZ;
}

public struct NetStatusUpdate
{
    public int ObjectId;
    public int[] AttributeIds;
    public int[] Values;
}

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

public struct NetSkillEntry
{
    public int SkillId;
    public int Level;
    public int ReuseDelayMs;
}

public struct NetMagicSkillUse
{
    public int CasterObjectId;
    public int TargetObjectId;
    public int SkillId;
    public int SkillLevel;
    public int HitTimeMs;
    public int ReuseDelayMs;
}

public struct NetDie
{
    public int ObjectId;
    public int KillerObjectId;
}

public struct NetRevive
{
    public int ObjectId;
}

public struct NetTeleportToLocation
{
    public int ObjectId;
    public int X;
    public int Y;
    public int Z;
    public int Heading;
}

public struct NetAbnormalStatusUpdate
{
    public int ObjectId;
    public NetAbnormalEffect[] Effects;
}

public struct NetAbnormalEffect
{
    public int Id;
    public int Value;
    public int DurationSeconds;
}

public struct NetPartyAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ItemDistribution;
}

public struct NetPartyMemberEntry
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public int CurHp;
    public int MaxHp;
    public int CurMp;
    public int MaxMp;
}

public struct NetPartyMemberList
{
    public int LeaderObjectId;
    public int PartyId;
    public NetPartyMemberEntry[] Members;
}

public struct NetTradeAsk
{
    public int RequestorObjectId;
    public string RequestorName;
}

public struct NetTradeStart
{
    public int PartnerObjectId;
    public string PartnerName;
}

public struct NetTradeOfferUpdate
{
    public int IsOwn;
    public int InventoryObjectId;
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct NetTradeConfirmState
{
    public int SelfConfirmed;
    public int PartnerConfirmed;
}

public struct NetClanAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ClanId;
    public string ClanName;
    public int PledgeType;
}

public struct NetPledgeStatus
{
    public int ClanId;
    public string Name;
    public int Level;
    public int LeaderObjectId;
    public int CrestId;
    public int MemberCount;
    public long DissolvingExpiryUnixMs;
    public long CharPenaltyExpiryUnixMs;
}

public struct NetPledgeMember
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public bool Online;
    public int PledgeType;
}

public struct NetPledgeMemberList
{
    public int ClanId;
    public int LeaderObjectId;
    public NetPledgeMember[] Members;
}

/// <summary>
/// Login roster row (<c>CharacterSummaryMsg</c>). Keys 0–14 identity/appearance/position;
/// keys 15–24 CharSelect vitals (cur/max HP MP CP, exp, sp, karma, weaponItemId);
/// key 25 paperdoll int[17]. Array length 26.
/// </summary>
public struct NetCharacterSummary
{
    public int CharacterId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int Level;
    public int Slot;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
    public int DeleteSecondsLeft;
    public bool IsLastPlayed;
    public int CurHp, MaxHp, CurMp, MaxMp, CurCp, MaxCp;
    public long Exp;
    public int Sp, Karma, WeaponItemId;
    /// <summary>Equipped item ids by paperdoll slot (length 17). Null when the login did not send key 25.</summary>
    public int[] Paperdoll;
}

/// <summary>Character select ticket. <see cref="Token"/> is the zone Auth password.</summary>
public struct NetHandoff
{
    public string Account;
    public string CharacterName;
    public int CharacterId;
    public int ClassId;
    public string Token;
    public string ZoneHost;
    public int ZonePort;
    public long ExpiresUnixSeconds;
}

public struct NetCharacterCreated
{
    public int CharacterId;
    public string Name;
    public int ClassId;
    public int Race;
    public int Sex;
    public int HairStyle;
    public int HairColor;
    public int Face;
    public int X;
    public int Y;
    public int Z;
}

public struct NetCharacterDeleted
{
    public int Slot;
    public int CharacterId;
    public bool HardDeleted;
}

public struct NetCharacterDeleteFailed
{
    public int Slot;
    public int CharacterId;
    public string Code;
    public string Message;
}

/// <summary>Opcode 1012. <see cref="ChatType"/> is the type the speaker sent (0 all, 1 shout).</summary>
public struct NetCreatureSay
{
    public int ObjectId;
    public string Name;
    public string Text;
    public int ChatType;
}

/// <summary>Opcode 1040. <see cref="Text"/> is the line to show. Id 0 means show it as written.</summary>
public struct NetSystemMessage
{
    public int Id;
    public string[] Params;
    public string Text;
}
