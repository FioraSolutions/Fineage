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
        PlayerResourceCaps.EnsureMaxAtLeastCurrent(stats, status);

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
        if (info.HasClan)
            dto.Identity.ClanId = info.ClanId;
        dto.Appearance.BaseClass = info.ClassId;
        dto.Appearance.Running = true;
        dto.Stats.BaseRunSpeed = 120;
        dto.Stats.BaseWalkingSpeed = 80;
        dto.Stats.RunRealSpeed = 120f;
        dto.Stats.WalkRealSpeed = 80f;
        dto.Status.SetHp(100);
        dto.Status.SetMp(100);
        dto.Stats.MaxHp = PlayerResourceCaps.EffectiveMax(0, dto.Status.GetHp());
        dto.Stats.MaxMp = PlayerResourceCaps.EffectiveMax(0, dto.Status.GetMp());
        PlayerResourceCaps.EnsureMaxAtLeastCurrent(dto.Stats, dto.Status);
        if (info.HasPaperdoll)
            ApplyItemIds(dto.Appearance, info.PaperdollItemIds);
        return dto;
    }

    /// <summary>Thin Zone v2 NpcInfo (1006) → Interlude-shaped DTO (Gremlins, guards, …).</summary>
    public static SpawnItemDto ToSpawnItemDto(ZoneV2SpawnItem item)
    {
        SpawnItemApplyFields fields = ZoneV2Messages.ToSpawnItemApplyFields(item);
        return SpawnItemDto.FromZoneV2(
            fields.ObjectId,
            fields.ItemId,
            fields.X,
            fields.Y,
            fields.Z,
            fields.Stackable,
            fields.Count,
            fields.Unknown);
    }

    public static NpcInfoDto ToNpcInfoDto(ZoneV2NpcInfo info)
    {
        NpcInfoDto dto = new NpcInfoDto();
        dto.Identity.Id = info.ObjectId;
        dto.Identity.NpcId = info.NpcTemplateId;
        if (!string.IsNullOrEmpty(info.Name))
            dto.Identity.Name = info.Name;
        if (!string.IsNullOrEmpty(info.Title))
            dto.Identity.Title = info.Title;
        if (!string.IsNullOrEmpty(info.NpcType))
            dto.Identity.NpcClass = info.NpcType;
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

    public static AskJoinPartyDto ToAskJoinPartyDto(ZoneV2PartyAskJoin ask) =>
        AskJoinPartyDto.FromZoneV2(ask.RequestorObjectId, ask.RequestorName, ask.ItemDistribution);

    public static SendTradeRequestDto ToSendTradeRequestDto(ZoneV2TradeAsk ask) =>
        SendTradeRequestDto.FromZoneV2(ask.RequestorObjectId, ask.RequestorName);

    public static TradeStartDto ToTradeStartDto(ZoneV2TradeStart start) =>
        TradeStartDto.FromZoneV2(start.PartnerObjectId, start.PartnerName);

    public static ItemInstance ToTradeOfferItem(in ZoneV2TradeOfferUpdate update)
    {
        return new ItemInstance(
            update.InventoryObjectId,
            update.ItemId,
            ItemLocation.Trade,
            0,
            update.Count < 1 ? 1 : update.Count,
            ItemCategory.Item,
            false,
            ItemSlot.none,
            update.EnchantLevel,
            9999);
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
                    ArmorType = row.ArmorType,
                    EnchantLevel = row.EnchantLevel
                };
                AddZoneRow(entry, row.Change, bag, equip, ref indexBag, ref indexEquip);
            }
        }

        return InventoryUpdateDto.FromZoneChanges(bag, equip);
    }

    public static AbnormalStatusUpdateDto ToAbnormalStatusUpdateDto(ZoneV2AbnormalStatusUpdate update) =>
        AbnormalStatusUpdateDto.FromZoneEffects(update.Effects);

    public static DieDto ToDieDto(ZoneV2Die die) => DieDto.FromZoneWire(die.ObjectId);

    public static ReviveDto ToReviveDto(ZoneV2Revive revive) => new ReviveDto { ObjectId = revive.ObjectId };

    public static TeleportToLocationDto ToTeleportToLocationDto(ZoneV2TeleportToLocation teleport) =>
        TeleportToLocationDto.FromZoneWire(teleport.ObjectId, teleport.X, teleport.Y, teleport.Z, teleport.Heading);

    public static SkillListDto ToSkillListDto(ZoneV2SkillEntry[] rows)
    {
        var dto = new SkillListDto();
        if (rows == null)
            return dto;
        for (int i = 0; i < rows.Length; i++)
        {
            ZoneV2SkillEntry row = rows[i];
            bool passive = IsPassiveSkill(row.SkillId, row.Level);
            bool disabled = row.ReuseDelayMs > 0;
            dto.Skills.Add(new SkillInstance(row.SkillId, row.Level, passive, disabled));
        }

        return dto;
    }

    public static MagicSkillUseDto ToMagicSkillUseDto(ZoneV2MagicSkillUse use)
    {
        var dto = new MagicSkillUseDto();
        dto.InitFromZoneV2(
            use.CasterObjectId,
            use.TargetObjectId,
            use.SkillId,
            use.SkillLevel,
            use.HitTimeMs,
            use.ReuseDelayMs);
        return dto;
    }

    static bool IsPassiveSkill(int skillId, int level)
    {
        Skillgrp skill = SkillgrpTable.Instance != null ? SkillgrpTable.Instance.GetSkill(skillId, level) : null;
        return skill != null && skill.OperateType == 2;
    }

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
        return ZoneV2InventoryBook.ToClientChange(zoneChange);
    }

    static ItemCategory CategoryFromZone(ZoneV2ItemEntry row)
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

    /// <summary>
    /// Copies UserInfo class, speeds, and paperdoll onto a spawn record.
    /// Position stays on the spawn packet.
    /// </summary>
    public static void OverlayUserInfo(PlayerInfoInterlude info, ZoneV2UserInfo user)
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
        if (user.HasClan)
            info.Identity.ClanId = user.ClanId;
        if (user.HasPaperdoll)
            ApplyUserPaperdoll(info.Appearance, user.Paperdoll);
    }

    public static void ApplyUserPaperdoll(PlayerAppearance appearance, ZoneV2PaperdollSlot[] slots)
    {
        if (appearance == null || slots == null)
            return;
        ApplyObjectIds(appearance.PaperDoll, slots);
        ApplyItemIds(appearance, ZoneV2Paperdoll.ItemIds(slots));
    }

    public static void ApplyItemIds(PlayerAppearance appearance, int[] itemIds)
    {
        if (appearance == null || itemIds == null)
            return;

        PaperDollSelection doll = appearance.PaperDoll;
        doll.Item_Under = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Under);
        doll.Item_Lear = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.LeftEar);
        doll.Item_Rear = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.RightEar);
        doll.Item_Neck = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Neck);
        doll.Item_LFinger = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.LeftFinger);
        doll.Item_RFinger = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.RightFinger);
        doll.Item_Head = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Head);
        doll.Item_RHand = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.RightHand);
        doll.Item_LHand = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.LeftHand);
        doll.Item_Gloves = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Gloves);
        doll.Item_Chest = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Chest);
        doll.Item_Legs = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Legs);
        doll.Item_Feet = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Feet);
        doll.Item_Cloak = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Cloak);
        doll.Item_Face = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Face);
        doll.Item_Hair = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Hair);
        doll.Item_Hair2 = ZoneV2Paperdoll.ItemIdAt(itemIds, ZoneV2Paperdoll.Hair2);
        appearance.RHand = doll.Item_RHand;
        appearance.LHand = doll.Item_LHand;
        appearance.Gloves = doll.Item_Gloves;
        appearance.Chest = doll.Item_Chest;
        appearance.Legs = doll.Item_Legs;
        appearance.Feet = doll.Item_Feet;
        appearance.PaperdollReady = true;
    }

    static void ApplyObjectIds(PaperDollSelection doll, ZoneV2PaperdollSlot[] slots)
    {
        if (doll == null || slots == null)
            return;
        for (int i = 0; i < slots.Length; i++)
        {
            int objectId = slots[i].ObjectId;
            switch (i)
            {
                case ZoneV2Paperdoll.Under:
                    doll.Obj_Under = objectId;
                    break;
                case ZoneV2Paperdoll.LeftEar:
                    doll.Obj_Lear = objectId;
                    break;
                case ZoneV2Paperdoll.RightEar:
                    doll.Obj_Pear = objectId;
                    break;
                case ZoneV2Paperdoll.Neck:
                    doll.Obj_Neck = objectId;
                    break;
                case ZoneV2Paperdoll.LeftFinger:
                    doll.Obj_LFinger = objectId;
                    break;
                case ZoneV2Paperdoll.RightFinger:
                    doll.Obj_RFinger = objectId;
                    break;
                case ZoneV2Paperdoll.Head:
                    doll.Obj_Head = objectId;
                    break;
                case ZoneV2Paperdoll.RightHand:
                    doll.Obj_RHand = objectId;
                    break;
                case ZoneV2Paperdoll.LeftHand:
                    doll.Obj_LHand = objectId;
                    break;
                case ZoneV2Paperdoll.Gloves:
                    doll.Obj_Gloves = objectId;
                    break;
                case ZoneV2Paperdoll.Chest:
                    doll.Obj_Chest = objectId;
                    break;
                case ZoneV2Paperdoll.Legs:
                    doll.Obj_Legs = objectId;
                    break;
                case ZoneV2Paperdoll.Feet:
                    doll.Obj_Feet = objectId;
                    break;
                case ZoneV2Paperdoll.Cloak:
                    doll.Obj_Cloak = objectId;
                    break;
                case ZoneV2Paperdoll.Face:
                    doll.Obj_Face = objectId;
                    break;
                case ZoneV2Paperdoll.Hair:
                    doll.Obj_Hair = objectId;
                    break;
                case ZoneV2Paperdoll.Hair2:
                    doll.Obj_Hair2 = objectId;
                    break;
            }
        }
    }
}
