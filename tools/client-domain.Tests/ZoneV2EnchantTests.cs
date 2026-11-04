using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Enchant window state and inventory book. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class ZoneV2EnchantTests
{
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
}
