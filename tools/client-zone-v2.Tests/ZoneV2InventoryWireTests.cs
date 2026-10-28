using Fineage.ZoneServer.Wire;
using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

public class ZoneV2InventoryWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_contract()
    {
        Assert.Equal(1014, (ushort)ZoneV2MsgType.ItemList);
        Assert.Equal(1015, (ushort)ZoneV2MsgType.InventoryUpdate);
        Assert.Equal(1016, (ushort)ZoneV2MsgType.UserInfo);
        Assert.Equal(1005, (ushort)ZoneV2MsgType.CharInfo);
        Assert.Equal(1018, (ushort)ZoneV2MsgType.SpawnItem);
        Assert.Equal(14, (ushort)ZoneV2MsgType.PickupRequest);
        Assert.Equal(13, (ushort)ZoneV2MsgType.GrantItemRequest);
        Assert.Equal(15, (ushort)ZoneV2MsgType.UseItem);
        Assert.Equal(16, (ushort)ZoneV2MsgType.UnequipItem);
        Assert.Equal(17, (ushort)ZoneV2MsgType.GiveItem);
        Assert.Equal(7, ZoneV2Paperdoll.RightHand);
        Assert.Equal(17, ZoneV2Paperdoll.Count);
    }

    [Fact]
    public void Spawn_item_decodes_gremlin_drop_shape()
    {
        var msg = new BackendSpawnItemMsg
        {
            ObjectId = 100_001,
            ItemId = 57,
            X = -84318,
            Y = 244579,
            Z = -3730,
            Stackable = true,
            Count = 25
        };

        Assert.True(ZoneV2Messages.TryDecodeSpawnItem(Pack(msg), out ZoneV2SpawnItem item));
        SpawnItemApplyFields fields = ZoneV2Messages.ToSpawnItemApplyFields(item);
        Assert.Equal(100_001, fields.ObjectId);
        Assert.Equal(57, fields.ItemId);
        Assert.Equal(-84318, fields.X);
        Assert.True(fields.Stackable);
        Assert.Equal(25, fields.Count);
        Assert.Equal(244579, fields.Y);
        Assert.Equal(-3730, fields.Z);
    }

    [Fact]
    public void Pickup_request_body_is_messagepack_object_id()
    {
        byte[] body = ZoneV2Messages.EncodePickupRequest(100_001);
        var msg = MessagePackSerializer.Deserialize<PickupRequestTestMsg>(body);
        Assert.Equal(100_001, msg.ObjectId);
    }

    [Fact]
    public void Item_list_marks_the_squire_sword_equipped_in_the_right_hand()
    {
        var list = new BackendItemListMsg
        {
            Items =
            [
                Sword(100000),
                new BackendItemMsg
                {
                    ObjectId = 100001,
                    ItemId = 5588,
                    Count = 1,
                    EquippedSlot = -1,
                    BodySlot = "none",
                    Kind = "etc",
                    Icon = "icon.etc_spellbook_blue_i00",
                    WeaponType = "",
                    ArmorType = ""
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeItemList(Pack(list), out ZoneV2ItemEntry[] items));
        ZoneV2ItemEntry sword = Assert.Single(items, row => row.ItemId == 2369);
        Assert.Equal(100000, sword.ObjectId);
        Assert.Equal(1, sword.Count);
        Assert.Equal(7, sword.EquippedSlot);
        Assert.Equal("rhand", sword.BodySlot);
        Assert.Equal("weapon", sword.Kind);
        Assert.Equal("icon.weapon_squires_sword_i00", sword.Icon);
        Assert.Equal("sword", sword.WeaponType);
        Assert.Equal("", sword.ArmorType);

        ZoneV2ItemEntry guide = Assert.Single(items, row => row.ItemId == 5588);
        Assert.Equal(-1, guide.EquippedSlot);
        Assert.Equal("etc", guide.Kind);
        Assert.Equal("none", guide.BodySlot);
    }

    [Fact]
    public void Item_row_with_a_missing_or_extra_key_still_decodes()
    {
        var shortRow = new ShortItemListMsg
        {
            Items = [new ShortItemMsg { ObjectId = 5, ItemId = 2369, Count = 1, EquippedSlot = 7 }]
        };
        Assert.True(ZoneV2Messages.TryDecodeItemList(Pack(shortRow), out ZoneV2ItemEntry[] shortItems));
        Assert.Equal(2369, shortItems[0].ItemId);
        Assert.Equal(7, shortItems[0].EquippedSlot);
        Assert.Equal("none", shortItems[0].BodySlot);
        Assert.Equal("etc", shortItems[0].Kind);

        var extra = new ExtraItemListMsg
        {
            Items =
            [
                new ExtraItemMsg
                {
                    ObjectId = 5,
                    ItemId = 2369,
                    Count = 1,
                    EquippedSlot = 7,
                    BodySlot = "rhand",
                    Kind = "weapon",
                    Icon = "icon.weapon_squires_sword_i00",
                    WeaponType = "sword",
                    ArmorType = "",
                    Future = "ignore-me"
                }
            ]
        };
        Assert.True(ZoneV2Messages.TryDecodeItemList(Pack(extra), out ZoneV2ItemEntry[] extraItems));
        Assert.Equal("sword", extraItems[0].WeaponType);
        Assert.Equal(7, extraItems[0].EquippedSlot);
    }

    [Fact]
    public void Inventory_update_reads_an_equip_change()
    {
        var update = new BackendInventoryUpdateMsg
        {
            Changes =
            [
                new BackendInventoryChangeMsg
                {
                    Change = 1,
                    ObjectId = 100002,
                    ItemId = 10,
                    Count = 1,
                    EquippedSlot = 7,
                    BodySlot = "rhand",
                    Kind = "weapon",
                    Icon = "icon.weapon_dagger_i00",
                    WeaponType = "dagger",
                    ArmorType = ""
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeInventoryUpdate(Pack(update), out ZoneV2InventoryChange[] changes));
        ZoneV2InventoryChange change = Assert.Single(changes);
        Assert.Equal(1, change.Change);
        Assert.Equal(10, change.ItemId);
        Assert.Equal(7, change.EquippedSlot);
        Assert.Equal("dagger", change.WeaponType);
        Assert.Equal("", change.ArmorType);
    }

    [Fact]
    public void Use_unequip_and_give_requests_match_the_backend_keys()
    {
        byte[] use = ZoneV2Messages.EncodeUseItem(100_002);
        var useMsg = MessagePackSerializer.Deserialize<UseItemTestMsg>(use);
        Assert.Equal(100_002, useMsg.ObjectId);

        byte[] unequip = ZoneV2Messages.EncodeUnequipItem(ZoneV2Paperdoll.RightHand);
        var unequipMsg = MessagePackSerializer.Deserialize<UnequipItemTestMsg>(unequip);
        Assert.Equal(7, unequipMsg.PaperdollSlot);

        byte[] give = ZoneV2Messages.EncodeGiveItem(2369, 1);
        var giveMsg = MessagePackSerializer.Deserialize<GiveItemTestMsg>(give);
        Assert.Equal(2369, giveMsg.ItemId);
        Assert.Equal(1, giveMsg.Count);

        byte[] defaultCount = ZoneV2Messages.EncodeGiveItem(57, 0);
        var adena = MessagePackSerializer.Deserialize<GiveItemTestMsg>(defaultCount);
        Assert.Equal(57, adena.ItemId);
        Assert.Equal(1, adena.Count);
    }

    [Fact]
    public void Inventory_update_add_unequip_and_remove_keep_their_change_codes()
    {
        var update = new BackendInventoryUpdateMsg
        {
            Changes =
            [
                new BackendInventoryChangeMsg
                {
                    Change = 0,
                    ObjectId = 100010,
                    ItemId = 2369,
                    Count = 1,
                    EquippedSlot = -1,
                    BodySlot = "rhand",
                    Kind = "weapon",
                    Icon = "icon.weapon_squires_sword_i00",
                    WeaponType = "sword",
                    ArmorType = ""
                },
                new BackendInventoryChangeMsg
                {
                    Change = 1,
                    ObjectId = 100011,
                    ItemId = 23,
                    Count = 1,
                    EquippedSlot = -1,
                    BodySlot = "chest",
                    Kind = "armor",
                    Icon = "icon.armor_t03_u_i00",
                    WeaponType = "",
                    ArmorType = "light"
                },
                new BackendInventoryChangeMsg
                {
                    Change = 2,
                    ObjectId = 100012,
                    ItemId = 57,
                    Count = 0,
                    EquippedSlot = -1,
                    BodySlot = "none",
                    Kind = "etc"
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeInventoryUpdate(Pack(update), out ZoneV2InventoryChange[] changes));
        Assert.Equal(3, changes.Length);
        Assert.Equal(0, changes[0].Change);
        Assert.Equal(-1, changes[0].EquippedSlot);
        Assert.Equal(1, changes[1].Change);
        Assert.Equal(-1, changes[1].EquippedSlot);
        Assert.Equal("light", changes[1].ArmorType);
        Assert.Equal(2, changes[2].Change);
        Assert.Equal(57, changes[2].ItemId);
    }

    [Fact]
    public void Inventory_book_equips_then_unequips_then_removes()
    {
        var items = new Dictionary<int, ZoneV2ItemEntry>();
        ZoneV2InventoryBook.Apply(items, Row(0, 100010, 2369, -1));
        Assert.Equal(-1, items[100010].EquippedSlot);
        Assert.Equal(ZoneV2InventoryBook.ClientAdded, ZoneV2InventoryBook.ToClientChange(0));

        ZoneV2InventoryBook.Apply(items, Row(1, 100010, 2369, ZoneV2Paperdoll.RightHand));
        Assert.Equal(7, items[100010].EquippedSlot);
        Assert.Equal("rhand", items[100010].BodySlot);
        Assert.Equal(ZoneV2InventoryBook.ClientModified, ZoneV2InventoryBook.ToClientChange(1));

        ZoneV2InventoryBook.Apply(items, Row(1, 100010, 2369, -1));
        Assert.Equal(-1, items[100010].EquippedSlot);

        ZoneV2InventoryBook.Apply(items, Row(2, 100010, 2369, -1));
        Assert.False(items.ContainsKey(100010));
        Assert.Equal(ZoneV2InventoryBook.ClientRemoved, ZoneV2InventoryBook.ToClientChange(2));
    }

    [Fact]
    public void Paperdoll_click_uses_the_slot_that_holds_the_item()
    {
        Assert.Equal(0, ZoneV2Paperdoll.IndexForGearCell("underwear"));
        Assert.Equal(3, ZoneV2Paperdoll.IndexForGearCell("neck"));
        Assert.Equal(6, ZoneV2Paperdoll.IndexForGearCell("head"));
        Assert.Equal(7, ZoneV2Paperdoll.IndexForGearCell("rhand"));
        Assert.Equal(8, ZoneV2Paperdoll.IndexForGearCell("lhand"));
        Assert.Equal(9, ZoneV2Paperdoll.IndexForGearCell("gloves"));
        Assert.Equal(10, ZoneV2Paperdoll.IndexForGearCell("chest"));
        Assert.Equal(11, ZoneV2Paperdoll.IndexForGearCell("legs"));
        Assert.Equal(12, ZoneV2Paperdoll.IndexForGearCell("feet"));
        Assert.Equal(16, ZoneV2Paperdoll.IndexForGearCell("hat2"));
        Assert.Equal(-1, ZoneV2Paperdoll.IndexForGearCell("belt"));

        Assert.Equal(7, ZoneV2Paperdoll.UnequipIndex(8, "lrhand"));
        Assert.Equal(10, ZoneV2Paperdoll.UnequipIndex(11, "fullarmor"));
        Assert.Equal(10, ZoneV2Paperdoll.UnequipIndex(10, "alldress"));
        Assert.Equal(8, ZoneV2Paperdoll.UnequipIndex(8, "lhand"));
        Assert.Equal(-1, ZoneV2Paperdoll.UnequipIndex(-1, "rhand"));
    }

    [Fact]
    public void Equip_errors_become_a_short_line_and_give_parses_the_admin_bypass()
    {
        Assert.Equal("You cannot do that while dead.", ZoneV2EquipNotices.Line("dead", ""));
        Assert.Equal("Nothing is equipped there.", ZoneV2EquipNotices.Line("empty", "  "));
        Assert.Equal("Legs are blocked by full armor.", ZoneV2EquipNotices.Line("blocked", "Legs are blocked by full armor."));
        Assert.Equal("The server refused that.", ZoneV2EquipNotices.Line("nope", null));

        Assert.True(ZoneV2GiveCommand.TryParse("admin_give 2369 2", out int itemId, out int count));
        Assert.Equal(2369, itemId);
        Assert.Equal(2, count);
        Assert.True(ZoneV2GiveCommand.TryParse("admin_give 57", out itemId, out count));
        Assert.Equal(57, itemId);
        Assert.Equal(1, count);
        Assert.False(ZoneV2GiveCommand.TryParse("admin_teleport 1", out _, out _));
    }

    static ZoneV2InventoryChange Row(int change, int objectId, int itemId, int equippedSlot)
    {
        return new ZoneV2InventoryChange
        {
            Change = change,
            ObjectId = objectId,
            ItemId = itemId,
            Count = 1,
            EquippedSlot = equippedSlot,
            BodySlot = equippedSlot == 7 ? "rhand" : "none",
            Kind = "weapon",
            Icon = "icon.weapon_squires_sword_i00",
            WeaponType = "sword",
            ArmorType = ""
        };
    }

    [Fact]
    public void User_info_paperdoll_right_hand_is_the_squire_sword()
    {
        var slots = new BackendPaperdollSlotMsg[ZoneV2Paperdoll.Count];
        for (int i = 0; i < slots.Length; i++)
            slots[i] = new BackendPaperdollSlotMsg();
        slots[ZoneV2Paperdoll.RightHand] = new BackendPaperdollSlotMsg { ObjectId = 100000, ItemId = 2369 };
        slots[ZoneV2Paperdoll.Chest] = new BackendPaperdollSlotMsg { ObjectId = 100010, ItemId = 23 };

        var user = new BackendUserInfoMsg
        {
            ObjectId = 42,
            Name = "Fighter",
            ClassId = 0,
            X = -71338,
            Y = 258271,
            Z = -3104,
            RunSpeed = 120f,
            WalkSpeed = 80f,
            Running = true,
            Paperdoll = slots
        };

        Assert.True(ZoneV2Messages.TryDecodeUserInfo(Pack(user), out ZoneV2UserInfo info));
        Assert.Equal(42, info.ObjectId);
        Assert.Equal("Fighter", info.Name);
        Assert.Equal(0, info.ClassId);
        Assert.Equal(-71338, info.X);
        Assert.Equal(120f, info.RunSpeed);
        Assert.Equal(80f, info.WalkSpeed);
        Assert.True(info.Running);
        Assert.True(info.HasPaperdoll);
        Assert.Equal(17, info.Paperdoll.Length);
        Assert.Equal(2369, ZoneV2Paperdoll.RightHandItemId(info.Paperdoll));
        Assert.Equal(100000, info.Paperdoll[ZoneV2Paperdoll.RightHand].ObjectId);
        Assert.Equal(23, info.Paperdoll[ZoneV2Paperdoll.Chest].ItemId);
        Assert.Equal(0, info.Paperdoll[ZoneV2Paperdoll.LeftHand].ItemId);
    }

    [Fact]
    public void User_info_without_paperdoll_still_decodes()
    {
        var user = new UserInfoWithoutPaperdollMsg
        {
            ObjectId = 7,
            Name = "Legacy",
            ClassId = 10,
            X = 1,
            Y = 2,
            Z = 3,
            RunSpeed = 120f,
            WalkSpeed = 80f,
            Running = true
        };

        Assert.True(ZoneV2Messages.TryDecodeUserInfo(Pack(user), out ZoneV2UserInfo info));
        Assert.Equal(10, info.ClassId);
        Assert.False(info.HasPaperdoll);
        Assert.Null(info.Paperdoll);
        Assert.Equal(0, ZoneV2Paperdoll.RightHandItemId(info.Paperdoll));
    }

    [Fact]
    public void Char_info_key_6_is_the_weapon_for_other_players()
    {
        int[] fighter = new int[ZoneV2Paperdoll.Count];
        fighter[ZoneV2Paperdoll.RightHand] = 2369;
        var other = new BackendCharInfoMsg
        {
            ObjectId = 99,
            Name = "Other",
            X = -71417,
            Y = 258270,
            Z = -3104,
            ClassId = 0,
            Paperdoll = fighter
        };

        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(other), out ZoneV2CharInfo info));
        Assert.True(info.HasPaperdoll);
        Assert.Equal(2369, ZoneV2Paperdoll.ItemIdAt(info.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
        Assert.Equal("Other", info.Name);

        int[] mystic = new int[ZoneV2Paperdoll.Count];
        mystic[ZoneV2Paperdoll.RightHand] = 6;
        var wand = new BackendCharInfoMsg
        {
            ObjectId = 100,
            Name = "Mystic",
            X = -90875,
            Y = 248162,
            Z = -3570,
            ClassId = 10,
            Paperdoll = mystic
        };
        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(wand), out ZoneV2CharInfo mysticInfo));
        Assert.Equal(6, ZoneV2Paperdoll.ItemIdAt(mysticInfo.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
        Assert.Equal(10, mysticInfo.ClassId);
    }

    [Fact]
    public void Legacy_char_info_without_key_6_still_decodes()
    {
        var legacy = new CharInfoMsg
        {
            ObjectId = 3,
            Name = "Old",
            X = 10,
            Y = 20,
            Z = 30,
            ClassId = 0
        };

        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(legacy), out ZoneV2CharInfo info));
        Assert.False(info.HasPaperdoll);
        Assert.Null(info.PaperdollItemIds);
        Assert.Equal(0, ZoneV2Paperdoll.ItemIdAt(info.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
        Assert.Equal("Old", info.Name);
    }

    [Fact]
    public void Char_info_with_a_trailing_key_keeps_the_weapon()
    {
        int[] paperdoll = new int[ZoneV2Paperdoll.Count];
        paperdoll[ZoneV2Paperdoll.RightHand] = 2369;
        var msg = new CharInfoWithExtraMsg
        {
            ObjectId = 4,
            Name = "Future",
            X = 1,
            Y = 2,
            Z = 3,
            ClassId = 0,
            Paperdoll = paperdoll,
            Future = "later"
        };

        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(msg), out ZoneV2CharInfo info));
        Assert.Equal(2369, ZoneV2Paperdoll.ItemIdAt(info.PaperdollItemIds, ZoneV2Paperdoll.RightHand));
    }

    [Fact]
    public void Npc_info_accepts_legacy_ints_and_backend_gremlin_name()
    {
        var legacy = new NpcInfoMsg
        {
            ObjectId = 8,
            NpcTemplateId = 20001,
            X = -90000,
            Y = 250000,
            Z = -3500
        };
        Assert.True(ZoneV2Messages.TryDecodeNpcInfo(Pack(legacy), out ZoneV2NpcInfo oldNpc));
        Assert.Equal(20001, oldNpc.NpcTemplateId);
        Assert.Equal(-90000, oldNpc.X);
        Assert.Equal(250000, oldNpc.Y);
        Assert.Equal(-3500, oldNpc.Z);

        var gremlin = new BackendNpcInfoMsg
        {
            ObjectId = 9,
            NpcId = 20001,
            Name = "Gremlin",
            X = -84318,
            Y = 244579,
            Z = -3730,
            Heading = 0,
            Attackable = true,
            RunSpeed = 80f,
            WalkSpeed = 40f
        };
        Assert.True(ZoneV2Messages.TryDecodeNpcInfo(Pack(gremlin), out ZoneV2NpcInfo npc));
        Assert.Equal(20001, npc.NpcTemplateId);
        Assert.Equal(-84318, npc.X);
        Assert.Equal(244579, npc.Y);
        Assert.Equal(-3730, npc.Z);
    }

    static BackendItemMsg Sword(int objectId)
    {
        return new BackendItemMsg
        {
            ObjectId = objectId,
            ItemId = 2369,
            Count = 1,
            EquippedSlot = 7,
            BodySlot = "rhand",
            Kind = "weapon",
            Icon = "icon.weapon_squires_sword_i00",
            WeaponType = "sword",
            ArmorType = ""
        };
    }

    static byte[] Pack<T>(T message)
    {
        return MessagePackSerializer.Serialize(message);
    }
}

[MessagePackObject]
public sealed partial class BackendSpawnItemMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public bool Stackable { get; set; }
    [Key(6)] public int Count { get; set; } = 1;
    [Key(7)] public int Unknown { get; set; }
}

[MessagePackObject]
public sealed partial class PickupRequestTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class UseItemTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

[MessagePackObject]
public sealed partial class UnequipItemTestMsg
{
    [Key(0)] public int PaperdollSlot { get; set; }
}

[MessagePackObject]
public sealed partial class GiveItemTestMsg
{
    [Key(0)] public int ItemId { get; set; }
    [Key(1)] public int Count { get; set; }
}

[MessagePackObject]
public sealed partial class BackendItemMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int Count { get; set; }
    [Key(3)] public int EquippedSlot { get; set; } = -1;
    [Key(4)] public string BodySlot { get; set; } = "none";
    [Key(5)] public string Kind { get; set; } = "etc";
    [Key(6)] public string Icon { get; set; } = "";
    [Key(7)] public string WeaponType { get; set; } = "";
    [Key(8)] public string ArmorType { get; set; } = "";
}

[MessagePackObject]
public sealed partial class BackendItemListMsg
{
    [Key(0)] public BackendItemMsg[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed partial class ShortItemMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int Count { get; set; }
    [Key(3)] public int EquippedSlot { get; set; } = -1;
}

[MessagePackObject]
public sealed partial class ShortItemListMsg
{
    [Key(0)] public ShortItemMsg[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed partial class ExtraItemMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int Count { get; set; }
    [Key(3)] public int EquippedSlot { get; set; } = -1;
    [Key(4)] public string BodySlot { get; set; } = "none";
    [Key(5)] public string Kind { get; set; } = "etc";
    [Key(6)] public string Icon { get; set; } = "";
    [Key(7)] public string WeaponType { get; set; } = "";
    [Key(8)] public string ArmorType { get; set; } = "";
    [Key(9)] public string Future { get; set; } = "";
}

[MessagePackObject]
public sealed partial class ExtraItemListMsg
{
    [Key(0)] public ExtraItemMsg[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed partial class BackendInventoryChangeMsg
{
    [Key(0)] public int Change { get; set; }
    [Key(1)] public int ObjectId { get; set; }
    [Key(2)] public int ItemId { get; set; }
    [Key(3)] public int Count { get; set; }
    [Key(4)] public int EquippedSlot { get; set; } = -1;
    [Key(5)] public string BodySlot { get; set; } = "none";
    [Key(6)] public string Kind { get; set; } = "etc";
    [Key(7)] public string Icon { get; set; } = "";
    [Key(8)] public string WeaponType { get; set; } = "";
    [Key(9)] public string ArmorType { get; set; } = "";
}

[MessagePackObject]
public sealed partial class BackendInventoryUpdateMsg
{
    [Key(0)] public BackendInventoryChangeMsg[] Changes { get; set; } = [];
}

[MessagePackObject]
public sealed partial class BackendPaperdollSlotMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
}

[MessagePackObject]
public sealed partial class BackendUserInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int X { get; set; }
    [Key(4)] public int Y { get; set; }
    [Key(5)] public int Z { get; set; }
    [Key(6)] public float RunSpeed { get; set; }
    [Key(7)] public float WalkSpeed { get; set; }
    [Key(8)] public bool Running { get; set; }
    [Key(9)] public BackendPaperdollSlotMsg[] Paperdoll { get; set; } = [];
}

[MessagePackObject]
public sealed partial class UserInfoWithoutPaperdollMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int ClassId { get; set; }
    [Key(3)] public int X { get; set; }
    [Key(4)] public int Y { get; set; }
    [Key(5)] public int Z { get; set; }
    [Key(6)] public float RunSpeed { get; set; }
    [Key(7)] public float WalkSpeed { get; set; }
    [Key(8)] public bool Running { get; set; }
}

[MessagePackObject]
public sealed partial class BackendCharInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
    [Key(6)] public int[] Paperdoll { get; set; } = [];
}

[MessagePackObject]
public sealed partial class CharInfoWithExtraMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
    [Key(6)] public int[] Paperdoll { get; set; } = [];
    [Key(7)] public string Future { get; set; } = "";
}

[MessagePackObject]
public sealed partial class BackendNpcInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int NpcId { get; set; }
    [Key(2)] public string Name { get; set; } = "";
    [Key(3)] public int X { get; set; }
    [Key(4)] public int Y { get; set; }
    [Key(5)] public int Z { get; set; }
    [Key(6)] public int Heading { get; set; }
    [Key(7)] public bool Attackable { get; set; }
    [Key(8)] public float RunSpeed { get; set; }
    [Key(9)] public float WalkSpeed { get; set; }
}
