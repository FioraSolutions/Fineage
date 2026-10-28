using System.Collections.Generic;
using UnityEngine;

/// <summary>ZoneServer v2 wire → domain events.</summary>
public static class ZoneV2DomainMap
{
    public static PlayerInfoInterlude ToPlayerInfo(ZoneV2PlayerSpawned spawned)
    {
        EntityIdentity identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = spawned.ObjectId,
            Name = string.IsNullOrEmpty(spawned.Name) ? "Test" : spawned.Name,
            PlayerClass = 0
        };
        identity.SetL2jPos(spawned.X, spawned.Y, spawned.Z);

        PlayerStats stats = new PlayerStats
        {
            BaseRunSpeed = Mathf.RoundToInt(spawned.RunSpeed > 0.001f ? spawned.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawned.WalkSpeed > 0.001f ? spawned.WalkSpeed : 80f),
            RunRealSpeed = spawned.RunSpeed > 0.001f ? spawned.RunSpeed : 120f,
            WalkRealSpeed = spawned.WalkSpeed > 0.001f ? spawned.WalkSpeed : 80f,
            MaxHp = 200,
            MaxMp = 50,
            MaxCp = 100,
            Level = 1
        };

        PlayerAppearance appearance = new PlayerAppearance
        {
            Running = spawned.Running,
            Race = 0,
            Sex = 0,
            Face = 0,
            HairStyle = 0,
            HairColor = 0
        };

        PlayerStatus status = new PlayerStatus();
        status.SetHp(100);
        status.SetMp(100);
        status.Cp = 100;

        return new PlayerInfoInterlude
        {
            Identity = identity,
            Status = status,
            Stats = stats,
            Appearance = appearance,
            Skills = new CharacterSkills()
        };
    }

    public static DomainMoveTo Move(ZoneV2MoveTo move)
    {
        CharMoveToLocationDto dto = CharMoveToLocationDto.FromL2(
            move.ObjectId,
            move.DestX, move.DestY, move.DestZ,
            move.OriginX, move.OriginY, move.OriginZ);
        return new DomainMoveTo
        {
            ObjectId = move.ObjectId,
            Destination = dto.NewPosition,
            Origin = dto.OldPosition,
            InterludeDto = dto
        };
    }

    public static DomainPlayerSpawned PlayerSpawned(PlayerInfoInterlude info)
    {
        return InterludeDomainMap.PlayerSpawned(info);
    }

    /// <summary>Thin Zone v2 CharInfo (1005) → Interlude-shaped DTO for knownlist spawn.</summary>
    public static CharInfoDto ToCharInfoDto(ZoneV2CharInfo info)
    {
        CharInfoDto dto = new CharInfoDto();
        dto.Identity.Id = info.ObjectId;
        dto.Identity.Name = string.IsNullOrEmpty(info.Name) ? "Unknown" : info.Name;
        dto.Identity.EntityType = EntityType.User;
        dto.Identity.SetL2jPos(info.X, info.Y, info.Z);
        dto.Identity.PlayerClass = info.ClassId;
        dto.Appearance.BaseClass = info.ClassId;
        dto.Appearance.Running = true;
        dto.Stats.BaseRunSpeed = 120;
        dto.Stats.BaseWalkingSpeed = 80;
        dto.Stats.RunRealSpeed = 120f;
        dto.Stats.WalkRealSpeed = 80f;
        dto.Status.SetHp(100);
        dto.Status.SetMp(100);
        return dto;
    }

    /// <summary>Thin Zone v2 NpcInfo (1006) → Interlude-shaped DTO (Gremlins, guards, …).</summary>
    public static NpcInfoDto ToNpcInfoDto(ZoneV2NpcInfo info)
    {
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = info.ObjectId;
        dto.Identity.NpcId = info.NpcTemplateId;
        dto.Identity.SetHideHp(info.NpcTemplateId);
        dto.Identity.EntityType = EntityType.NPC;
        dto.Identity.SetL2jPos(info.X, info.Y, info.Z);
        dto.Stats.Level = 1;
        dto.Status.SetHp(100);
        dto.Stats.MaxHp = 100;
        dto.Stats.BaseRunSpeed = 120;
        dto.Stats.BaseWalkingSpeed = 80;
        dto.Stats.WalkRealSpeed = 80f;
        dto.Stats.RunRealSpeed = 120f;
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        return dto;
    }

    public static AttackDto ToAttackDto(ZoneV2Attack attack)
    {
        var dto = new AttackDto();
        dto.InitFromZoneV2(
            attack.AttackerObjectId,
            attack.TargetObjectId,
            attack.Damage,
            attack.Flags,
            attack.AttackerX,
            attack.AttackerY,
            attack.AttackerZ,
            attack.TargetX,
            attack.TargetY,
            attack.TargetZ);
        return dto;
    }

    public static List<StatusUpdate.Attribute> ToStatusAttributes(ZoneV2StatusUpdate status)
    {
        var list = new List<StatusUpdate.Attribute>(status.AttributeIds.Length);
        for (int i = 0; i < status.AttributeIds.Length; i++)
            list.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        return list;
    }

    public static CreatureMessage ToCreatureMessage(ZoneV2CreatureSay say)
    {
        ChatTypeData data = ChatTypes.GetById(say.ChatType);
        if (data == null)
            data = ChatTypes.GetById(0);
        string name = string.IsNullOrEmpty(say.Name) ? "Unknown" : say.Name;
        string text = say.Text ?? "";
        if (data != null && (data.Type == 10 || data.Type == 18))
            return new CreatureMessage("Announcements", text, data);
        return new CreatureMessage(name, text, data);
    }

    public static ItemListDto ToItemListDto(ZoneV2ItemEntry[] rows)
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

    public static InventoryUpdateDto ToInventoryUpdateDto(ZoneV2InventoryChange[] rows)
    {
        var bag = new Dictionary<int, ItemInstance>();
        var equip = new Dictionary<int, ItemInstance>();
        int indexBag = 0;
        int indexEquip = 0;
        if (rows != null)
        {
            for (int i = 0; i < rows.Length; i++)
            {
                ZoneV2InventoryChange row = rows[i];
                var entry = new ZoneV2ItemEntry
                {
                    ObjectId = row.ObjectId,
                    ItemId = row.ItemId,
                    Count = row.Count,
                    EquippedSlot = row.EquippedSlot,
                    BodySlot = row.BodySlot,
                    Kind = row.Kind,
                    Icon = row.Icon,
                    WeaponType = row.WeaponType,
                    ArmorType = row.ArmorType
                };
                AddZoneRow(entry, row.Change, bag, equip, ref indexBag, ref indexEquip);
            }
        }

        return InventoryUpdateDto.FromZoneChanges(bag, equip);
    }

    public static AbnormalStatusUpdateDto ToAbnormalStatusUpdateDto(ZoneV2AbnormalStatusUpdate update) =>
        AbnormalStatusUpdateDto.FromZoneEffects(update.Effects);

    static void AddZoneRow(
        ZoneV2ItemEntry row,
        int zoneChange,
        Dictionary<int, ItemInstance> bag,
        Dictionary<int, ItemInstance> equip,
        ref int indexBag,
        ref int indexEquip)
    {
        bool worn = row.EquippedSlot >= 0;
        ItemCategory category = CategoryFromZone(row);
        ItemSlot slot = SlotFromBodyToken(row.BodySlot);
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
            0,
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
        switch (zoneChange)
        {
            case 0:
                return (int)PlayerInventory.InventoryChange.ADDED;
            case 1:
                return (int)PlayerInventory.InventoryChange.MODIFIED;
            case 2:
                return (int)PlayerInventory.InventoryChange.REMOVED;
            default:
                return (int)PlayerInventory.InventoryChange.MODIFIED;
        }
    }

    static ItemCategory CategoryFromZone(ZoneV2ItemEntry row)
    {
        if (row.ItemId == 57)
            return ItemCategory.Adena;
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
            default:
                return ItemSlot.none;
        }
    }
}
