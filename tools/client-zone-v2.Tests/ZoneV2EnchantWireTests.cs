using MessagePack;
using Xunit;

namespace Fineage.Client.ZoneV2.Tests;

/// <summary>
/// Parsing and enchant-window state. These run without Play Mode, the same way the other zone wire tests do.
/// </summary>
public class ZoneV2EnchantWireTests
{
    [Fact]
    public void Opcodes_match_the_backend_enchant_contract()
    {
        Assert.Equal(39, (ushort)ZoneV2MsgType.EnchantRequest);
        Assert.Equal(40, (ushort)ZoneV2MsgType.EnchantConfirm);
        Assert.Equal(41, (ushort)ZoneV2MsgType.EnchantCancel);
        Assert.Equal(1060, (ushort)ZoneV2MsgType.EnchantResult);
    }

    [Fact]
    public void Request_confirm_and_cancel_match_the_backend_keys()
    {
        byte[] request = ZoneV2Messages.EncodeEnchantRequest(100_040);
        var requestMsg = MessagePackSerializer.Deserialize<EnchantObjectTestMsg>(request);
        Assert.Equal(100_040, requestMsg.ObjectId);

        byte[] confirm = ZoneV2Messages.EncodeEnchantConfirm(100_010);
        var confirmMsg = MessagePackSerializer.Deserialize<EnchantObjectTestMsg>(confirm);
        Assert.Equal(100_010, confirmMsg.ObjectId);

        byte[] cancel = ZoneV2Messages.EncodeEnchantCancel();
        var cancelMsg = MessagePackSerializer.Deserialize<EnchantCancelTestMsg>(cancel);
        Assert.NotNull(cancelMsg);
        Assert.Equal(0x90, cancel[0]);
    }

    [Fact]
    public void Enchant_result_reads_success_keep_reset_and_crystallize()
    {
        AssertResult(Pack(Result(ZoneV2Enchant.ResultSuccess, 4, 100_010, 0, 0)), ZoneV2Enchant.ResultSuccess, 4, 100_010, 0, 0);
        AssertResult(Pack(Result(ZoneV2Enchant.ResultKeep, 3, 100_010, 0, 0)), ZoneV2Enchant.ResultKeep, 3, 100_010, 0, 0);
        AssertResult(Pack(Result(ZoneV2Enchant.ResultReset, 0, 100_010, 0, 0)), ZoneV2Enchant.ResultReset, 0, 100_010, 0, 0);
        AssertResult(Pack(Result(ZoneV2Enchant.ResultCrystallize, 0, 100_010, 1458, 12)), ZoneV2Enchant.ResultCrystallize, 0, 100_010, 1458, 12);
        AssertResult(Pack(Result(ZoneV2Enchant.ResultCancel, 0, 0, 0, 0)), ZoneV2Enchant.ResultCancel, 0, 0, 0, 0);
    }

    [Fact]
    public void Enchant_result_ignores_a_trailing_key()
    {
        var msg = new EnchantResultExtraMsg
        {
            Result = ZoneV2Enchant.ResultSuccess,
            EnchantLevel = 2,
            ObjectId = 9,
            CrystalItemId = 0,
            CrystalCount = 0,
            Future = "later"
        };

        Assert.True(ZoneV2Messages.TryDecodeEnchantResult(Pack(msg), out ZoneV2EnchantResult result));
        Assert.Equal(2, result.EnchantLevel);
        Assert.Equal(9, result.ObjectId);
    }

    [Fact]
    public void Item_list_and_inventory_update_read_enchant_level()
    {
        var list = new EnchantItemListMsg
        {
            Items =
            [
                new EnchantItemMsg
                {
                    ObjectId = 100_010,
                    ItemId = 2369,
                    Count = 1,
                    EquippedSlot = 7,
                    BodySlot = "rhand",
                    Kind = "weapon",
                    Icon = "icon.weapon_squires_sword_i00",
                    WeaponType = "sword",
                    ArmorType = "",
                    EnchantLevel = 3
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeItemList(Pack(list), out ZoneV2ItemEntry[] items));
        Assert.Equal(3, items[0].EnchantLevel);
        Assert.Equal(2369, items[0].ItemId);

        var update = new EnchantInventoryUpdateMsg
        {
            Changes =
            [
                new EnchantInventoryChangeMsg
                {
                    Change = 1,
                    ObjectId = 100_010,
                    ItemId = 2369,
                    Count = 1,
                    EquippedSlot = -1,
                    BodySlot = "rhand",
                    Kind = "weapon",
                    Icon = "icon.weapon_squires_sword_i00",
                    WeaponType = "sword",
                    ArmorType = "",
                    EnchantLevel = 4
                }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeInventoryUpdate(Pack(update), out ZoneV2InventoryChange[] changes));
        Assert.Equal(4, changes[0].EnchantLevel);
        Assert.Equal(1, changes[0].Change);
    }

    [Fact]
    public void A_string_in_the_enchant_slot_does_not_break_an_older_row()
    {
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

        Assert.True(ZoneV2Messages.TryDecodeItemList(Pack(extra), out ZoneV2ItemEntry[] items));
        Assert.Equal(0, items[0].EnchantLevel);
        Assert.Equal("sword", items[0].WeaponType);
    }

    [Fact]
    public void User_info_and_char_info_read_paperdoll_enchant()
    {
        var user = new EnchantUserInfoMsg
        {
            ObjectId = 1,
            Name = "Fighter",
            ClassId = 0,
            X = 1,
            Y = 2,
            Z = 3,
            RunSpeed = 120f,
            WalkSpeed = 80f,
            Running = true,
            Paperdoll =
            [
                new EnchantPaperdollSlotMsg { ObjectId = 100_010, ItemId = 2369, EnchantLevel = 5 }
            ]
        };

        Assert.True(ZoneV2Messages.TryDecodeUserInfo(Pack(user), out ZoneV2UserInfo info));
        Assert.True(info.HasPaperdoll);
        Assert.Equal(5, info.Paperdoll[0].EnchantLevel);
        Assert.Equal(2369, info.Paperdoll[0].ItemId);

        int[] ids = new int[17];
        int[] levels = new int[17];
        ids[7] = 2369;
        levels[7] = 5;
        var other = new EnchantCharInfoMsg
        {
            ObjectId = 2,
            Name = "Other",
            X = 4,
            Y = 5,
            Z = 6,
            ClassId = 0,
            Paperdoll = ids,
            ClanId = 12,
            ClanCrestId = 0,
            Enchant = levels
        };

        Assert.True(ZoneV2Messages.TryDecodeCharInfo(Pack(other), out ZoneV2CharInfo seen));
        Assert.Equal(12, seen.ClanId);
        Assert.Equal(5, seen.PaperdollEnchant[7]);
        Assert.Equal(2369, ZoneV2Paperdoll.ItemIdAt(seen.PaperdollItemIds, 7));
    }

    [Fact]
    public void Window_selects_a_target_and_refuses_the_scroll()
    {
        ZoneV2Enchant.Session session = ZoneV2Enchant.OpenFor(40);
        Assert.True(session.Open);
        Assert.False(ZoneV2Enchant.CanConfirm(session));

        session = ZoneV2Enchant.Select(session, 40);
        Assert.Equal(0, session.TargetObjectId);
        Assert.False(ZoneV2Enchant.CanConfirm(session));

        session = ZoneV2Enchant.Select(session, 10);
        Assert.Equal(10, session.TargetObjectId);
        Assert.True(ZoneV2Enchant.CanConfirm(session));

        ZoneV2Enchant.Session closed = default;
        Assert.False(ZoneV2Enchant.CanConfirm(ZoneV2Enchant.Select(closed, 10)));
    }

    [Fact]
    public void Success_writes_the_level_and_crystallize_removes_the_item()
    {
        var items = new Dictionary<int, ZoneV2ItemEntry>
        {
            [10] = new ZoneV2ItemEntry { ObjectId = 10, ItemId = 2369, Count = 1, EnchantLevel = 3, BodySlot = "rhand", Kind = "weapon" },
            [40] = new ZoneV2ItemEntry { ObjectId = 40, ItemId = 729, Count = 2, Kind = "etc" }
        };

        ZoneV2Enchant.Apply(items, new ZoneV2EnchantResult
        {
            Result = ZoneV2Enchant.ResultSuccess,
            EnchantLevel = 4,
            ObjectId = 10
        });
        Assert.Equal(4, items[10].EnchantLevel);
        Assert.Equal("+4 Squire's Sword", ZoneV2Enchant.WithLevel("Squire's Sword", items[10].EnchantLevel));

        ZoneV2Enchant.View kept = ZoneV2Enchant.ViewOf(new ZoneV2EnchantResult
        {
            Result = ZoneV2Enchant.ResultKeep,
            EnchantLevel = 4,
            ObjectId = 10
        });
        Assert.True(kept.CloseWindow);
        Assert.True(kept.ShowEnchant);
        Assert.False(kept.RemoveItem);

        ZoneV2Enchant.Apply(items, new ZoneV2EnchantResult
        {
            Result = ZoneV2Enchant.ResultReset,
            EnchantLevel = 0,
            ObjectId = 10
        });
        Assert.Equal(0, items[10].EnchantLevel);
        Assert.Equal("Squire's Sword", ZoneV2Enchant.WithLevel("Squire's Sword", 0));

        ZoneV2Enchant.View broken = ZoneV2Enchant.ViewOf(new ZoneV2EnchantResult
        {
            Result = ZoneV2Enchant.ResultCrystallize,
            EnchantLevel = 0,
            ObjectId = 10,
            CrystalItemId = 1458,
            CrystalCount = 12
        });
        Assert.True(broken.RemoveItem);
        Assert.Equal(1458, broken.CrystalItemId);
        Assert.Equal(12, broken.CrystalCount);
        ZoneV2Enchant.Apply(items, new ZoneV2EnchantResult
        {
            Result = ZoneV2Enchant.ResultCrystallize,
            ObjectId = 10,
            CrystalItemId = 1458,
            CrystalCount = 12
        });
        Assert.False(items.ContainsKey(10));
        Assert.True(items.ContainsKey(40));
    }

    [Fact]
    public void Cancel_closes_the_window_and_leaves_the_bag()
    {
        var items = new Dictionary<int, ZoneV2ItemEntry>
        {
            [10] = new ZoneV2ItemEntry { ObjectId = 10, ItemId = 2369, EnchantLevel = 1 }
        };
        ZoneV2Enchant.View view = ZoneV2Enchant.ViewOf(new ZoneV2EnchantResult { Result = ZoneV2Enchant.ResultCancel });
        Assert.True(view.CloseWindow);
        Assert.False(view.RemoveItem);
        Assert.False(view.ShowEnchant);
        ZoneV2Enchant.Apply(items, new ZoneV2EnchantResult { Result = ZoneV2Enchant.ResultCancel });
        Assert.Equal(1, items[10].EnchantLevel);
    }

    [Fact]
    public void Scroll_types_and_error_lines()
    {
        Assert.True(ZoneV2Enchant.IsScrollType("scrl_enchant_wp"));
        Assert.True(ZoneV2Enchant.IsScrollType("scrl_enchant_am"));
        Assert.True(ZoneV2Enchant.IsScrollType("bless_scrl_enchant_wp"));
        Assert.True(ZoneV2Enchant.IsScrollType("ancient_crystal_enchant_am"));
        Assert.False(ZoneV2Enchant.IsScrollType("scrl_inc_enchant_prop_am"));
        Assert.False(ZoneV2Enchant.IsScrollType("recipe"));
        Assert.False(ZoneV2Enchant.IsScrollType(null));

        Assert.Equal("The scroll grade does not match the item.", ZoneV2Enchant.Notice("grade_mismatch", ""));
        Assert.Equal("That scroll cannot enchant this item.", ZoneV2Enchant.Notice("slot_mismatch", null));
        Assert.Equal("That item is already at the maximum enchant.", ZoneV2Enchant.Notice("max_enchant", "  "));
        Assert.Equal("Custom server sentence.", ZoneV2Enchant.Notice("not_enchantable", " Custom server sentence. "));
        Assert.True(ZoneV2Enchant.ClearsWindow("not_scroll"));
        Assert.True(ZoneV2Enchant.ClearsWindow("dead"));
        Assert.False(ZoneV2Enchant.ClearsWindow("bad_request"));
        Assert.False(ZoneV2Enchant.ClearsWindow("party_full"));
    }

    [Fact]
    public void Inventory_book_keeps_the_enchant_on_a_modify()
    {
        var items = new Dictionary<int, ZoneV2ItemEntry>();
        ZoneV2InventoryBook.Apply(items, new ZoneV2InventoryChange
        {
            Change = ZoneV2InventoryBook.Modify,
            ObjectId = 10,
            ItemId = 2369,
            Count = 1,
            EquippedSlot = -1,
            BodySlot = "rhand",
            Kind = "weapon",
            EnchantLevel = 6
        });
        Assert.Equal(6, items[10].EnchantLevel);

        ZoneV2InventoryBook.Apply(items, new ZoneV2InventoryChange
        {
            Change = ZoneV2InventoryBook.Remove,
            ObjectId = 10,
            ItemId = 2369,
            Count = 0,
            EnchantLevel = 6
        });
        Assert.False(items.ContainsKey(10));
    }

    static void AssertResult(byte[] body, int code, int level, int objectId, int crystalId, int crystalCount)
    {
        Assert.True(ZoneV2Messages.TryDecodeEnchantResult(body, out ZoneV2EnchantResult result));
        Assert.Equal(code, result.Result);
        Assert.Equal(level, result.EnchantLevel);
        Assert.Equal(objectId, result.ObjectId);
        Assert.Equal(crystalId, result.CrystalItemId);
        Assert.Equal(crystalCount, result.CrystalCount);
    }

    static EnchantResultTestMsg Result(int code, int level, int objectId, int crystalId, int crystalCount) =>
        new EnchantResultTestMsg
        {
            Result = code,
            EnchantLevel = level,
            ObjectId = objectId,
            CrystalItemId = crystalId,
            CrystalCount = crystalCount
        };

    static byte[] Pack<T>(T message) => MessagePackSerializer.Serialize(message);
}

[MessagePackObject]
public sealed class EnchantObjectTestMsg
{
    [Key(0)] public int ObjectId { get; set; }
}

[MessagePackObject]
public sealed class EnchantCancelTestMsg
{
}

[MessagePackObject]
public sealed class EnchantResultTestMsg
{
    [Key(0)] public int Result { get; set; }
    [Key(1)] public int EnchantLevel { get; set; }
    [Key(2)] public int ObjectId { get; set; }
    [Key(3)] public int CrystalItemId { get; set; }
    [Key(4)] public int CrystalCount { get; set; }
}

[MessagePackObject]
public sealed class EnchantResultExtraMsg
{
    [Key(0)] public int Result { get; set; }
    [Key(1)] public int EnchantLevel { get; set; }
    [Key(2)] public int ObjectId { get; set; }
    [Key(3)] public int CrystalItemId { get; set; }
    [Key(4)] public int CrystalCount { get; set; }
    [Key(5)] public string Future { get; set; } = "";
}

[MessagePackObject]
public sealed class EnchantItemMsg
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
    [Key(9)] public int EnchantLevel { get; set; }
}

[MessagePackObject]
public sealed class EnchantItemListMsg
{
    [Key(0)] public EnchantItemMsg[] Items { get; set; } = [];
}

[MessagePackObject]
public sealed class EnchantInventoryChangeMsg
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
    [Key(10)] public int EnchantLevel { get; set; }
}

[MessagePackObject]
public sealed class EnchantInventoryUpdateMsg
{
    [Key(0)] public EnchantInventoryChangeMsg[] Changes { get; set; } = [];
}

[MessagePackObject]
public sealed class EnchantPaperdollSlotMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public int ItemId { get; set; }
    [Key(2)] public int EnchantLevel { get; set; }
}

[MessagePackObject]
public sealed class EnchantUserInfoMsg
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
    [Key(9)] public EnchantPaperdollSlotMsg[] Paperdoll { get; set; } = [];
}

[MessagePackObject]
public sealed class EnchantCharInfoMsg
{
    [Key(0)] public int ObjectId { get; set; }
    [Key(1)] public string Name { get; set; } = "";
    [Key(2)] public int X { get; set; }
    [Key(3)] public int Y { get; set; }
    [Key(4)] public int Z { get; set; }
    [Key(5)] public int ClassId { get; set; }
    [Key(6)] public int[] Paperdoll { get; set; } = [];
    [Key(7)] public int ClanId { get; set; }
    [Key(8)] public int ClanCrestId { get; set; }
    [Key(9)] public int[] Enchant { get; set; } = [];
}
