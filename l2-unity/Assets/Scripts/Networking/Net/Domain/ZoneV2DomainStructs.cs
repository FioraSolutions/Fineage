using System;

/// <summary>
/// Zone V2 payload rows shared by the Net domain mapping after the MessagePack codec was retired.
/// Engine-free: no UnityEngine references.
/// </summary>

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

public struct ZoneV2PlayerSpawned
{
    public int ObjectId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
}

public struct ZoneV2MoveTo
{
    public int ObjectId;
    public int DestX;
    public int DestY;
    public int DestZ;
    public int OriginX;
    public int OriginY;
    public int OriginZ;
}

public struct ZoneV2CharInfo
{
    public int ObjectId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    public int ClassId;
    public bool HasPaperdoll;
    public int[] PaperdollItemIds;
    public bool HasClan;
    public int ClanId;
    public int ClanCrestId;
    /// <summary>Enchant of each paperdoll slot, parallel to <see cref="PaperdollItemIds"/>. Key 9.</summary>
    public int[] PaperdollEnchant;
}

/// <summary>
/// Paperdoll indexes from Fineage-Backend. Right hand is 7 (Squire's Sword 2369, Apprentice's Wand 6).
/// Missing indexes read as empty.
/// </summary>
public static class ZoneV2Paperdoll
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

    public static int RightHandItemId(ZoneV2PaperdollSlot[] slots)
    {
        if (slots == null || slots.Length <= RightHand)
            return 0;
        return slots[RightHand].ItemId;
    }

    public static int[] ItemIds(ZoneV2PaperdollSlot[] slots)
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

public struct ZoneV2PaperdollSlot
{
    public int ObjectId;
    public int ItemId;
    public int EnchantLevel;
}

public struct ZoneV2UserInfo
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int X;
    public int Y;
    public int Z;
    public float RunSpeed;
    public float WalkSpeed;
    public bool Running;
    public bool HasPaperdoll;
    public ZoneV2PaperdollSlot[] Paperdoll;
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

public struct ZoneV2NpcInfo
{
    public int ObjectId;
    public int NpcTemplateId;
    public string Name;
    public int X;
    public int Y;
    public int Z;
    /// <summary>Legacy five-int rows stay attackable. Backend key 7 is the real flag.</summary>
    public bool Attackable;
    /// <summary>Backend key 8; 0 when the wire omitted speeds.</summary>
    public float RunSpeed;
    /// <summary>Backend key 9; 0 when the wire omitted speeds.</summary>
    public float WalkSpeed;
    public string Title;
    public string NpcType;
}

public struct ZoneV2MyTargetSelected
{
    public int ObjectId;
    public int ColorHint;
}

public struct ZoneV2Attack
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

public struct ZoneV2StatusUpdate
{
    public int ObjectId;
    public int[] AttributeIds;
    public int[] Values;
}

public struct ZoneV2ItemEntry
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

public struct ZoneV2SpawnItem
{
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

public struct ZoneV2DropItem
{
    public int CharObjectId;
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

/// <summary>Wire spawn row after decode — same field set as Interlude <c>SpawnItemDto</c> apply.</summary>
public struct SpawnItemApplyFields
{
    public int ObjectId;
    public int ItemId;
    public int X;
    public int Y;
    public int Z;
    public bool Stackable;
    public int Count;
    public int Unknown;
}

public struct ZoneV2InventoryChange
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

public struct ZoneV2EnchantResult
{
    public int Result;
    public int EnchantLevel;
    public int ObjectId;
    public int CrystalItemId;
    public int CrystalCount;
}

public struct ZoneV2SkillEntry
{
    public int SkillId;
    public int Level;
    public int ReuseDelayMs;
}

public struct ZoneV2MagicSkillUse
{
    public int CasterObjectId;
    public int TargetObjectId;
    public int SkillId;
    public int SkillLevel;
    public int HitTimeMs;
    public int ReuseDelayMs;
}

public struct ZoneV2Die
{
    public int ObjectId;
    public int KillerObjectId;
}

public struct ZoneV2Revive
{
    public int ObjectId;
}

public struct ZoneV2TeleportToLocation
{
    public int ObjectId;
    public int X;
    public int Y;
    public int Z;
    public int Heading;
}

public struct ZoneV2AbnormalStatusUpdate
{
    public int ObjectId;
    public ZoneV2AbnormalEffect[] Effects;
}

public struct ZoneV2AbnormalEffect
{
    public int Id;
    public int Value;
    public int DurationSeconds;
}

public struct ZoneV2PartyAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ItemDistribution;
}

public struct ZoneV2PartyMemberEntry
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

public struct ZoneV2PartyMemberList
{
    public int LeaderObjectId;
    public int PartyId;
    public ZoneV2PartyMemberEntry[] Members;
}

public struct ZoneV2TradeAsk
{
    public int RequestorObjectId;
    public string RequestorName;
}

public struct ZoneV2TradeStart
{
    public int PartnerObjectId;
    public string PartnerName;
}

public struct ZoneV2TradeOfferUpdate
{
    public int IsOwn;
    public int InventoryObjectId;
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct ZoneV2TradeConfirmState
{
    public int SelfConfirmed;
    public int PartnerConfirmed;
}

public struct ZoneV2ClanAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ClanId;
    public string ClanName;
    public int PledgeType;
}

public struct ZoneV2PledgeStatus
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

public struct ZoneV2PledgeMember
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public bool Online;
    public int PledgeType;
}

public struct ZoneV2PledgeMemberList
{
    public int ClanId;
    public int LeaderObjectId;
    public ZoneV2PledgeMember[] Members;
}

/// <summary>
/// Login roster row (<c>CharacterSummaryMsg</c>). Keys 0–14 identity/appearance/position;
/// keys 15–24 CharSelect vitals (cur/max HP MP CP, exp, sp, karma, weaponItemId);
/// key 25 paperdoll int[17]. Array length 26.
/// </summary>
public struct ZoneV2CharacterSummary
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
public struct ZoneV2Handoff
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

public struct ZoneV2CharacterCreated
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

public struct ZoneV2CharacterDeleted
{
    public int Slot;
    public int CharacterId;
    public bool HardDeleted;
}

public struct ZoneV2CharacterDeleteFailed
{
    public int Slot;
    public int CharacterId;
    public string Code;
    public string Message;
}

/// <summary>Opcode 1012. <see cref="ChatType"/> is the type the speaker sent (0 all, 1 shout).</summary>
public struct ZoneV2CreatureSay
{
    public int ObjectId;
    public string Name;
    public string Text;
    public int ChatType;
}

/// <summary>Opcode 1040. <see cref="Text"/> is the line to show. Id 0 means show it as written.</summary>
public struct ZoneV2SystemMessage
{
    public int Id;
    public string[] Params;
    public string Text;
}
