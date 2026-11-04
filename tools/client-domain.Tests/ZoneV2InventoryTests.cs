using System.Collections.Generic;
using Xunit;

namespace Fineage.Client.Domain.Tests;

/// <summary>
/// Inventory book, paperdoll slot mapping, and equip notices. Salvaged from the retired
/// <c>client-zone-v2.Tests</c> MessagePack codec host; only the assertions that touch
/// surviving domain code remain.
/// </summary>
public class ZoneV2InventoryTests
{
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
}
