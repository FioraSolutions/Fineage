using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Fineage Net gear/inventory wire → Interlude-shaped item and player vitals DTOs.</summary>
public static class NetGearMap
{
    public static ItemListDto ToItemListDto(NetItemEntry[] rows)
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

    public static InventoryUpdateDto ToInventoryUpdateDto(NetInventoryChange[] rows)
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

    /// <summary>
    /// Copies UserInfo class, speeds, paperdoll, vitals, and template stats onto a spawn record.
    /// Position stays on the spawn packet.
    /// </summary>
    public static void OverlayUserInfo(PlayerInfoInterlude info, NetUserInfo user)
    {
        if (info.Identity == null || info.Appearance == null || info.Stats == null)
            return;

        info.Identity.PlayerClass = user.ClassId;
        info.Appearance.Race = (int)MapClassId.GetCharacterRace(user.ClassId);
        bool mage = CharacterClassParser.IsMage((CharacterClass)user.ClassId);
        info.Appearance.BaseClass = mage ? (int)BaseClass.MMagic : (int)BaseClass.Fighter;
        if (!string.IsNullOrEmpty(user.Name))
            info.Identity.Name = user.Name;
        if (user.RunSpeed > 0.001f)
        {
            int run = Mathf.RoundToInt(user.RunSpeed);
            info.Stats.BaseRunSpeed = run;
            info.Stats.RunRealSpeed = user.RunSpeed;
            info.Stats.Speed = run;
        }

        if (user.WalkSpeed > 0.001f)
        {
            info.Stats.BaseWalkingSpeed = Mathf.RoundToInt(user.WalkSpeed);
            info.Stats.WalkRealSpeed = user.WalkSpeed;
        }

        info.Appearance.Running = user.Running;
        info.Identity.IsRunning = user.Running;
        if (user.HasClan)
            info.Identity.ClanId = user.ClanId;
        if (user.HasPaperdoll)
            NetEntityMap.ApplyUserPaperdoll(info.Appearance, user.Paperdoll);

        if (user.HasVitals)
        {
            if (user.Level > 0)
                info.Stats.Level = user.Level;
            if (user.MaxHp > 0)
                info.Stats.MaxHp = user.MaxHp;
            if (user.MaxMp > 0)
                info.Stats.MaxMp = user.MaxMp;
            if (user.MaxCp > 0)
                info.Stats.MaxCp = user.MaxCp;
            info.Stats.Exp = user.Exp;
            info.Stats.Sp = user.Sp;
            if (info.Status != null)
            {
                if (user.MaxHp > 0 || user.CurHp > 0)
                    info.Status.SetHp(user.CurHp);
                if (user.MaxMp > 0 || user.CurMp > 0)
                    info.Status.SetMp(user.CurMp);
                info.Status.Cp = user.CurCp;
                PlayerResourceCaps.EnsureMaxAtLeastCurrent(info.Stats, info.Status);
            }
        }

        if (user.HasStats)
        {
            info.Stats.Str = user.Str;
            info.Stats.Dex = user.Dex;
            info.Stats.Con = user.Con;
            info.Stats.Int = user.Int;
            info.Stats.Wit = user.Wit;
            info.Stats.Men = user.Men;
            info.Stats.PAtk = user.PAtk;
            info.Stats.MAtk = user.MAtk;
            info.Stats.PDef = user.PDef;
            info.Stats.MDef = user.MDef;
            info.Stats.PAtkSpd = user.PAtkSpd;
            info.Stats.MAtkSpd = user.MAtkSpd;
            info.Stats.PAccuracy = user.Accuracy;
            info.Stats.PCritical = user.CritRate;
        }
    }

    /// <summary>
    /// <c>UserInfo</c> wire → <see cref="NetUserInfo"/>, the shape
    /// <see cref="NetGearSession.NoteUserInfo"/> and <see cref="OverlayUserInfo"/> consume.
    ///
    /// <para>
    /// Every flag is set from the message rather than guessed: the zone server always
    /// populates vitals, stats and clan (0 meaning "none"), and an empty paperdoll is a
    /// legitimate answer for an unequipped character, not a missing tail.
    /// </para>
    /// </summary>
    public static NetUserInfo ToUserInfo(UserInfoNetMsg msg)
    {
        var user = new NetUserInfo
        {
            ObjectId = (int)msg.EntityValue,
            Name = msg.Name,
            ClassId = msg.ClassId,
            X = msg.X,
            Y = msg.Y,
            Z = msg.Z,
            RunSpeed = msg.RunSpeed,
            WalkSpeed = msg.WalkSpeed,
            Running = msg.Running,
            HasClan = true,
            ClanId = msg.ClanId,
            HasVitals = true,
            Level = msg.Level,
            CurHp = msg.CurHp,
            MaxHp = msg.MaxHp,
            CurMp = msg.CurMp,
            MaxMp = msg.MaxMp,
            CurCp = msg.CurCp,
            MaxCp = msg.MaxCp,
            Exp = msg.Exp,
            Sp = msg.Sp,
            HasStats = true,
            Str = msg.Str,
            Dex = msg.Dex,
            Con = msg.Con,
            Int = msg.Int,
            Wit = msg.Wit,
            Men = msg.Men,
            PAtk = msg.PAtk,
            MAtk = msg.MAtk,
            PDef = msg.PDef,
            MDef = msg.MDef,
            PAtkSpd = msg.PAtkSpd,
            MAtkSpd = msg.MAtkSpd,
            Accuracy = msg.Accuracy,
            CritRate = msg.CritRate
        };

        PaperdollSlotNet[] slots = msg.Paperdoll;
        if (slots == null || slots.Length == 0)
        {
            user.HasPaperdoll = false;
            user.Paperdoll = System.Array.Empty<NetPaperdollSlot>();
            return user;
        }

        var paperdoll = new NetPaperdollSlot[slots.Length];
        for (int i = 0; i < slots.Length; i++)
        {
            PaperdollSlotNet src = slots[i];
            paperdoll[i] = new NetPaperdollSlot
            {
                ObjectId = src?.ObjectId ?? 0,
                ItemId = src?.ItemId ?? 0,
                EnchantLevel = src?.EnchantLevel ?? 0
            };
        }

        user.HasPaperdoll = true;
        user.Paperdoll = paperdoll;
        return user;
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
        // The Net ItemList row only carries the paperdoll index, not the body token or kind, so resolve
        // both from the local grp tables. Without this every item was ItemCategory.Item / ItemSlot.none,
        // and the equipped weapon never reached the paperdoll: the bag had items, the weapon did not.
        if (category == ItemCategory.Item && slot == ItemSlot.none)
            ResolveFromLocalTables(row.ItemId, ref category, ref slot);
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
        // PlayerInventory.InventoryChange stores those as 1, 2, and 3.
        return NetInventoryBook.ToClientChange(zoneChange);
    }

    /// <summary>
    /// Fills category/body slot from the client's own weapongrp / armorgrp tables when the wire row
    /// did not carry them. Net ItemList sends ObjectId, ItemId, Count, EnchantLevel and EquippedSlot only.
    /// </summary>
    static void ResolveFromLocalTables(int itemId, ref ItemCategory category, ref ItemSlot slot)
    {
        ItemTable table = ItemTable.Instance;
        if (table == null)
            return;

        Weapon weapon = table.GetWeapon(itemId);
        if (weapon != null && weapon.Weapongrp != null)
        {
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

    static bool IsJewelItemSlot(ItemSlot slot)
    {
        switch (slot)
        {
            case ItemSlot.neck:
            case ItemSlot.lear:
            case ItemSlot.rear:
            case ItemSlot.lfinger:
            case ItemSlot.rfinger:
                return true;
            default:
                return false;
        }
    }

    static ItemCategory CategoryFromZone(NetItemEntry row)
    {
        if (row.ItemId == 57)
            return ItemCategory.Adena;
        if (IsJewelBody(row.BodySlot))
            return ItemCategory.Jewel;
        string kind = row.Kind ?? "";
        if (kind == "weapon")
            return ItemCategory.Weapon;
        if (kind == "armor")
            return ItemCategory.ShieldArmor;
        return ItemCategory.Item;
    }

    static ItemSlot SlotFromBodyToken(string bodySlot)
    {
        if (string.IsNullOrEmpty(bodySlot) || bodySlot == "none")
            return ItemSlot.none;
        switch (bodySlot)
        {
            case "neck":
                return ItemSlot.neck;
            case "head":
                return ItemSlot.head;
            case "lhand":
                return ItemSlot.lhand;
            case "gloves":
                return ItemSlot.gloves;
            case "chest":
                return ItemSlot.chest;
            case "rhand":
                return ItemSlot.rhand;
            case "lear":
                return ItemSlot.lear;
            case "feet":
                return ItemSlot.feet;
            case "legs":
                return ItemSlot.legs;
            case "lrhand":
                return ItemSlot.lrhand;
            case "fullarmor":
                return ItemSlot.fullarmor;
            case "rear":
                return ItemSlot.rear;
            case "lfinger":
                return ItemSlot.lfinger;
            case "rfinger":
                return ItemSlot.rfinger;
            case "underwear":
                return ItemSlot.underwear;
            case "both_ears":
                return ItemSlot.lear;
            case "both_fingers":
                return ItemSlot.lfinger;
            case "hair":
            case "hairall":
                return ItemSlot.hat1;
            case "hair2":
                return ItemSlot.hat2;
            case "alldress":
                return ItemSlot.chest;
            default:
                return ItemSlot.none;
        }
    }

    static bool IsJewelBody(string bodySlot)
    {
        switch (bodySlot)
        {
            case "neck":
            case "lear":
            case "rear":
            case "both_ears":
            case "lfinger":
            case "rfinger":
            case "both_fingers":
                return true;
            default:
                return false;
        }
    }
}
