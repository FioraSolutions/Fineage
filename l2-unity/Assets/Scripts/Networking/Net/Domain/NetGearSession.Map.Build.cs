using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    public static ItemInstance ToItemInstance(CharacterInventory.Row row, int bagIndex)
    {
        var entry = new NetItemEntry
        {
            ObjectId = row.ObjectId,
            ItemId = row.ItemId,
            Count = row.Count,
            EquippedSlot = row.EquippedSlot,
            BodySlot = row.BodySlot ?? "",
            Kind = row.Kind ?? "",
            Icon = row.Icon ?? "",
            WeaponType = row.WeaponType ?? "",
            ArmorType = row.ArmorType ?? "",
            EnchantLevel = row.EnchantLevel
        };
        bool worn = entry.EquippedSlot >= 0;
        ItemCategory category = CategoryFromZone(entry);
        ItemSlot slot = SlotFromBodyToken(entry.BodySlot);
        if (worn && slot == ItemSlot.none)
        {
            Debug.LogError("[Inventory] objectId " + entry.ObjectId + " itemId " + entry.ItemId
                + " is equipped at slot " + entry.EquippedSlot + " with body '" + entry.BodySlot
                + "'; the paperdoll cell is unknown");
        }
        else if (!string.IsNullOrEmpty(entry.BodySlot) && entry.BodySlot != "none" && slot == ItemSlot.none)
        {
            Debug.LogError("[Inventory] objectId " + entry.ObjectId + " body '" + entry.BodySlot + "' is not an Interlude slot token");
        }
        NormalizeEquippedIdentity(ref category, ref slot);
        ItemLocation location = worn ? ItemLocation.Equipped : ItemLocation.Inventory;
        return new ItemInstance(
            entry.ObjectId,
            entry.ItemId,
            location,
            bagIndex,
            entry.Count,
            category,
            worn,
            slot,
            entry.EnchantLevel,
            9999);
    }

    public static CharacterInventory.Row ToRow(NetItemEntry entry)
    {
        return new CharacterInventory.Row
        {
            ObjectId = entry.ObjectId,
            ItemId = entry.ItemId,
            Count = entry.Count,
            EnchantLevel = entry.EnchantLevel,
            EquippedSlot = entry.EquippedSlot,
            BodySlot = entry.BodySlot ?? "",
            Kind = entry.Kind ?? "",
            Icon = entry.Icon ?? "",
            WeaponType = entry.WeaponType ?? "",
            ArmorType = entry.ArmorType ?? ""
        };
    }
}
