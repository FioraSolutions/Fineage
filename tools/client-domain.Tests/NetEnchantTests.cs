using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Enchant window state and inventory book. Salvaged from the retired <c>client-zone-v2.Tests</c>
/// MessagePack codec host; only the assertions that touch surviving domain code remain.
/// </summary>
public class NetEnchantTests
{
    [Fact]
    public void Window_selects_a_target_and_refuses_the_scroll()
    {
        NetEnchant.Session session = NetEnchant.OpenFor(40);
        Assert.True(session.Open);
        Assert.False(NetEnchant.CanConfirm(session));

        session = NetEnchant.Select(session, 40);
        Assert.Equal(0, session.TargetObjectId);
        Assert.False(NetEnchant.CanConfirm(session));

        session = NetEnchant.Select(session, 10);
        Assert.Equal(10, session.TargetObjectId);
        Assert.True(NetEnchant.CanConfirm(session));

        NetEnchant.Session closed = default;
        Assert.False(NetEnchant.CanConfirm(NetEnchant.Select(closed, 10)));
    }

    [Fact]
    public void Success_writes_the_level_and_crystallize_removes_the_item()
    {
        var items = new Dictionary<int, NetItemEntry>
        {
            [10] = new NetItemEntry { ObjectId = 10, ItemId = 2369, Count = 1, EnchantLevel = 3, BodySlot = "rhand", Kind = "weapon" },
            [40] = new NetItemEntry { ObjectId = 40, ItemId = 729, Count = 2, Kind = "etc" }
        };

        NetEnchant.Apply(items, new NetEnchantResult
        {
            Result = NetEnchant.ResultSuccess,
            EnchantLevel = 4,
            ObjectId = 10
        });
        Assert.Equal(4, items[10].EnchantLevel);
        Assert.Equal("+4 Squire's Sword", NetEnchant.WithLevel("Squire's Sword", items[10].EnchantLevel));

        NetEnchant.View kept = NetEnchant.ViewOf(new NetEnchantResult
        {
            Result = NetEnchant.ResultKeep,
            EnchantLevel = 4,
            ObjectId = 10
        });
        Assert.True(kept.CloseWindow);
        Assert.True(kept.ShowEnchant);
        Assert.False(kept.RemoveItem);

        NetEnchant.Apply(items, new NetEnchantResult
        {
            Result = NetEnchant.ResultReset,
            EnchantLevel = 0,
            ObjectId = 10
        });
        Assert.Equal(0, items[10].EnchantLevel);
        Assert.Equal("Squire's Sword", NetEnchant.WithLevel("Squire's Sword", 0));

        NetEnchant.View broken = NetEnchant.ViewOf(new NetEnchantResult
        {
            Result = NetEnchant.ResultCrystallize,
            EnchantLevel = 0,
            ObjectId = 10,
            CrystalItemId = 1458,
            CrystalCount = 12
        });
        Assert.True(broken.RemoveItem);
        Assert.Equal(1458, broken.CrystalItemId);
        Assert.Equal(12, broken.CrystalCount);
        NetEnchant.Apply(items, new NetEnchantResult
        {
            Result = NetEnchant.ResultCrystallize,
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
        var items = new Dictionary<int, NetItemEntry>
        {
            [10] = new NetItemEntry { ObjectId = 10, ItemId = 2369, EnchantLevel = 1 }
        };
        NetEnchant.View view = NetEnchant.ViewOf(new NetEnchantResult { Result = NetEnchant.ResultCancel });
        Assert.True(view.CloseWindow);
        Assert.False(view.RemoveItem);
        Assert.False(view.ShowEnchant);
        NetEnchant.Apply(items, new NetEnchantResult { Result = NetEnchant.ResultCancel });
        Assert.Equal(1, items[10].EnchantLevel);
    }

    [Fact]
    public void Scroll_types_and_error_lines()
    {
        Assert.True(NetEnchant.IsScrollType("scrl_enchant_wp"));
        Assert.True(NetEnchant.IsScrollType("scrl_enchant_am"));
        Assert.True(NetEnchant.IsScrollType("bless_scrl_enchant_wp"));
        Assert.True(NetEnchant.IsScrollType("ancient_crystal_enchant_am"));
        Assert.False(NetEnchant.IsScrollType("scrl_inc_enchant_prop_am"));
        Assert.False(NetEnchant.IsScrollType("recipe"));
        Assert.False(NetEnchant.IsScrollType(null));

        Assert.Equal("The scroll grade does not match the item.", NetEnchant.Notice("grade_mismatch", ""));
        Assert.Equal("That scroll cannot enchant this item.", NetEnchant.Notice("slot_mismatch", null));
        Assert.Equal("That item is already at the maximum enchant.", NetEnchant.Notice("max_enchant", "  "));
        Assert.Equal("Custom server sentence.", NetEnchant.Notice("not_enchantable", " Custom server sentence. "));
        Assert.True(NetEnchant.ClearsWindow("not_scroll"));
        Assert.True(NetEnchant.ClearsWindow("dead"));
        Assert.False(NetEnchant.ClearsWindow("bad_request"));
        Assert.False(NetEnchant.ClearsWindow("party_full"));
    }

    [Fact]
    public void Inventory_book_keeps_the_enchant_on_a_modify()
    {
        var items = new Dictionary<int, NetItemEntry>();
        NetInventoryBook.Apply(items, new NetInventoryChange
        {
            Change = NetInventoryBook.Modify,
            ObjectId = 10,
            ItemId = 2369,
            Count = 1,
            EquippedSlot = -1,
            BodySlot = "rhand",
            Kind = "weapon",
            EnchantLevel = 6
        });
        Assert.Equal(6, items[10].EnchantLevel);

        NetInventoryBook.Apply(items, new NetInventoryChange
        {
            Change = NetInventoryBook.Remove,
            ObjectId = 10,
            ItemId = 2369,
            Count = 0,
            EnchantLevel = 6
        });
        Assert.False(items.ContainsKey(10));
    }
}
