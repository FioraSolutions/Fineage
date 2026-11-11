using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    static ItemListDto ToItemListDto(NetItemEntry[] rows)
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        int indexBag = 0;
        int indexEquip = 0;
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
                AddZoneRow(rows[i], -1, bag, equip, ref indexBag, ref indexEquip);
        }

        return ItemListDto.FromZoneInventory(bag, equip);
    }

    static InventoryUpdateDto ToInventoryUpdateDto(NetInventoryChange[] rows)
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        int indexBag = 0;
        int indexEquip = 0;
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                NetInventoryChange row = rows[i];
                var entry = new NetItemEntry
                {
                    ObjectId = row.ObjectId,
                    ItemId = row.ItemId,
                    Count = row.Count,
                    EquippedSlot = row.EquippedSlot,
                    BodySlot = row.BodySlot,
                    Kind = row.Kind,
                    Icon = row.Icon,
                    WeaponType = row.WeaponType,
                    ArmorType = row.ArmorType,
                    EnchantLevel = row.EnchantLevel
                };
                AddZoneRow(entry, row.Change, bag, equip, ref indexBag, ref indexEquip);
            }
        }

        return InventoryUpdateDto.FromZoneChanges(bag, equip);
    }

    static void AddZoneRow(
        NetItemEntry row,
        int zoneChange,
        Dictionary<int, ItemInstance> bag,
        Dictionary<int, ItemInstance> equip,
        ref int indexBag,
        ref int indexEquip)
    {
        bool worn = row.EquippedSlot >= 0;
        ItemCategory category = CategoryFromZone(row);
        ItemSlot slot = SlotFromBodyToken(row.BodySlot);
        // Wire rows normally carry Kind + BodySlot. Fall back to local grp tables only when absent
        // (legacy payloads / incomplete rows), then normalize paper-doll identity once.
        if (category == ItemCategory.Item && slot == ItemSlot.none)
            ResolveFromLocalTables(row.ItemId, ref category, ref slot);
        NormalizeEquippedIdentity(ref category, ref slot);
        ItemLocation location = worn ? ItemLocation.Equipped : ItemLocation.Inventory;
        int index = worn ? indexEquip++ : indexBag++;
        var item = new ItemInstance(
            row.ObjectId,
            row.ItemId,
            location,
            index,
            row.Count,
            category,
            worn,
            slot,
            row.EnchantLevel,
            9999);
        if (zoneChange >= 0)
            item.LastChange = MapZoneInventoryChange(zoneChange);

        if (worn)
            equip[row.ObjectId] = item;
        else
            bag[row.ObjectId] = item;
    }

    static int MapZoneInventoryChange(int zoneChange)
    {
        // Fineage.Protocol.InventoryChange: 0 add, 1 modify, 2 remove.
        // Client InventoryChange stores those as 1, 2, and 3.
        return NetInventoryBook.ToClientChange(zoneChange);
    }

    static void ResolveFromLocalTables(int itemId, ref ItemCategory category, ref ItemSlot slot)
    {
        ItemTable table = TablePacketServices.Items;
        if (table == null)
            return;

        Weapon weapon = table.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
        {
            // Raw weapongrp identity (shields are handness 0 → ItemSlot.lhand_shield).
            // NormalizeEquippedIdentity remaps that to ShieldArmor + lhand.
            category = ItemCategory.Weapon;
            slot = weapon.Weapongrp.BodyPart;
            return;
        }

        Armor armor = table.GetArmor(itemId);
        if (armor != null && armor.Armorgrp != null)
        {
            slot = armor.Armorgrp.BodyPart;
            // Rings/earrings/necklaces live in armorgrp but tooltips use Jewel/ACCESSORY, not armor.
            category = IsJewelItemSlot(slot) ? ItemCategory.Jewel : ItemCategory.ShieldArmor;
            return;
        }

        if (table.GetEtcItem(itemId) != null)
            category = itemId == 57 ? ItemCategory.Adena : ItemCategory.Item;
    }
}
