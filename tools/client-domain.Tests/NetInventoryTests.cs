using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Inventory book, paperdoll slot mapping, and equip notices. Salvaged from the retired
/// <c>client-zone-v2.Tests</c> MessagePack codec host; only the assertions that touch
/// surviving domain code remain.
/// </summary>
public class NetInventoryTests
{
    [Fact]
    public void Inventory_book_equips_then_unequips_then_removes()
    {
        var items = new Dictionary<int, NetItemEntry>();
        NetInventoryBook.Apply(items, Row(0, 100010, 2369, -1));
        Assert.Equal(-1, items[100010].EquippedSlot);
        Assert.Equal(NetInventoryBook.ClientAdded, NetInventoryBook.ToClientChange(0));

        NetInventoryBook.Apply(items, Row(1, 100010, 2369, NetPaperdoll.RightHand));
        Assert.Equal(7, items[100010].EquippedSlot);
        Assert.Equal("rhand", items[100010].BodySlot);
        Assert.Equal(NetInventoryBook.ClientModified, NetInventoryBook.ToClientChange(1));

        NetInventoryBook.Apply(items, Row(1, 100010, 2369, -1));
        Assert.Equal(-1, items[100010].EquippedSlot);

        NetInventoryBook.Apply(items, Row(2, 100010, 2369, -1));
        Assert.False(items.ContainsKey(100010));
        Assert.Equal(NetInventoryBook.ClientRemoved, NetInventoryBook.ToClientChange(2));
    }

    [Fact]
    public void Paperdoll_click_uses_the_slot_that_holds_the_item()
    {
        Assert.Equal(0, NetPaperdoll.IndexForGearCell("underwear"));
        Assert.Equal(3, NetPaperdoll.IndexForGearCell("neck"));
        Assert.Equal(6, NetPaperdoll.IndexForGearCell("head"));
        Assert.Equal(7, NetPaperdoll.IndexForGearCell("rhand"));
        Assert.Equal(8, NetPaperdoll.IndexForGearCell("lhand"));
        Assert.Equal(9, NetPaperdoll.IndexForGearCell("gloves"));
        Assert.Equal(10, NetPaperdoll.IndexForGearCell("chest"));
        Assert.Equal(11, NetPaperdoll.IndexForGearCell("legs"));
        Assert.Equal(12, NetPaperdoll.IndexForGearCell("feet"));
        Assert.Equal(16, NetPaperdoll.IndexForGearCell("hat2"));
        Assert.Equal(-1, NetPaperdoll.IndexForGearCell("belt"));

        Assert.Equal(7, NetPaperdoll.UnequipIndex(8, "lrhand"));
        Assert.Equal(10, NetPaperdoll.UnequipIndex(11, "fullarmor"));
        Assert.Equal(10, NetPaperdoll.UnequipIndex(10, "alldress"));
        Assert.Equal(8, NetPaperdoll.UnequipIndex(8, "lhand"));
        Assert.Equal(-1, NetPaperdoll.UnequipIndex(-1, "rhand"));
    }

    [Fact]
    public void Equip_errors_become_a_short_line_and_give_parses_the_admin_bypass()
    {
        Assert.Equal("You cannot do that while dead.", NetEquipNotices.Line("dead", ""));
        Assert.Equal("Nothing is equipped there.", NetEquipNotices.Line("empty", "  "));
        Assert.Equal("Legs are blocked by full armor.", NetEquipNotices.Line("blocked", "Legs are blocked by full armor."));
        Assert.Equal("The server refused that.", NetEquipNotices.Line("nope", null));

        Assert.True(NetGiveCommand.TryParse("admin_give 2369 2", out int itemId, out int count));
        Assert.Equal(2369, itemId);
        Assert.Equal(2, count);
        Assert.True(NetGiveCommand.TryParse("admin_give 57", out itemId, out count));
        Assert.Equal(57, itemId);
        Assert.Equal(1, count);
        Assert.False(NetGiveCommand.TryParse("admin_teleport 1", out _, out _));
    }

    static NetInventoryChange Row(int change, int objectId, int itemId, int equippedSlot)
    {
        return new NetInventoryChange
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
}
