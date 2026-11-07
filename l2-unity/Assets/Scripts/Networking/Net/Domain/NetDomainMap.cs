using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>ZoneServer v2 wire → domain events.</summary>
public static class NetDomainMap
{
    public static PlayerInfoInterlude ToPlayerInfo(NetPlayerSpawned spawned)
    {
        EntityIdentity identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = spawned.ObjectId,
            Name = string.IsNullOrEmpty(spawned.Name) ? "Test" : spawned.Name,
            PlayerClass = 0
        };
        identity.SetL2jPos(spawned.X, spawned.Y, spawned.Z);
        // EntityActionMove / UserSpawner read Identity.IsRunning (not Appearance.Running):
        // without this a remote player walks until a ChangeMoveType arrives.
        identity.IsRunning = spawned.Running;

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

    public static DomainMoveTo Move(NetMoveTo move)
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
    public static CharInfoDto ToCharInfoDto(NetCharInfo info)
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
        dto.Identity.IsRunning = true;
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
    public static SpawnItemDto ToSpawnItemDto(NetSpawnItem item)
    {
        SpawnItemApplyFields fields = new SpawnItemApplyFields
        {
            ObjectId = item.ObjectId,
            ItemId = item.ItemId,
            X = item.X,
            Y = item.Y,
            Z = item.Z,
            Stackable = item.Stackable,
            Count = item.Count,
            Unknown = item.Unknown
        };
        return SpawnItemDto.FromNet(
            fields.ObjectId,
            fields.ItemId,
            fields.X,
            fields.Y,
            fields.Z,
            fields.Stackable,
            fields.Count,
            fields.Unknown);
    }

    public static DropItemDto ToDropItemDto(NetDropItem item)
    {
        return DropItemDto.FromNet(
            item.CharObjectId,
            item.ObjectId,
            item.ItemId,
            item.X,
            item.Y,
            item.Z,
            item.Stackable,
            item.Count,
            item.Unknown);
    }

    public static NpcInfoDto ToNpcInfoDto(NetNpcInfo info)
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
        // Zone V2 NpcInfo has no PAtkSpd key yet — Interlude default so MonsterStateAtk
        // does not freeze at patkspd=0 (CalculateTimeL2j(0) → Inf).
        const int defaultPAtkSpd = 253;
        dto.Stats.PAtkSpd = defaultPAtkSpd;
        dto.Stats.PAtkRealSpeed = defaultPAtkSpd;
        float run = info.RunSpeed > 0.001f ? info.RunSpeed : 120f;
        float walk = info.WalkSpeed > 0.001f ? info.WalkSpeed : 80f;
        dto.Stats.BaseRunSpeed = Mathf.RoundToInt(run);
        dto.Stats.BaseWalkingSpeed = Mathf.RoundToInt(walk);
        dto.Stats.WalkRealSpeed = walk;
        dto.Stats.RunRealSpeed = run;
        dto.Appearance.CollisionRadius = 10f;
        dto.Appearance.CollisionHeight = 22f;
        return dto;
    }

    public static AttackDto ToAttackDto(NetAttack attack)
    {
        var dto = new AttackDto();
        dto.InitFromNet(
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

    public static List<StatusUpdate.Attribute> ToStatusAttributes(NetStatusUpdate status)
    {
        var list = new List<StatusUpdate.Attribute>(status.AttributeIds.Length);
        for (int i = 0; i < status.AttributeIds.Length; i++)
            list.Add(new StatusUpdate.Attribute(status.AttributeIds[i], status.Values[i]));
        return list;
    }

    public static AskJoinPartyDto ToAskJoinPartyDto(NetPartyAskJoin ask) =>
        AskJoinPartyDto.FromNet(ask.RequestorObjectId, ask.RequestorName, ask.ItemDistribution);

    /// <summary>One-line party roster summary for chat feedback. Display only — no wire codec.</summary>
    public static string FormatPartyMemberList(in NetPartyMemberList list)
    {
        if (list.Members == null || list.Members.Length == 0)
            return "Party: (none)";
        var names = new string[list.Members.Length];
        for (int i = 0; i < list.Members.Length; i++)
            names[i] = list.Members[i].Name ?? "?";
        return "Party (" + list.Members.Length + "): " + string.Join(", ", names);
    }

    public static SendTradeRequestDto ToSendTradeRequestDto(NetTradeAsk ask) =>
        SendTradeRequestDto.FromNet(ask.RequestorObjectId, ask.RequestorName);

    public static TradeStartDto ToTradeStartDto(NetTradeStart start) =>
        TradeStartDto.FromNet(start.PartnerObjectId, start.PartnerName);

    public static ItemInstance ToTradeOfferItem(in NetTradeOfferUpdate update)
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

    public static AbnormalStatusUpdateDto ToAbnormalStatusUpdateDto(NetAbnormalStatusUpdate update) =>
        AbnormalStatusUpdateDto.FromZoneEffects(update.Effects);

    public static DieDto ToDieDto(NetDie die) => DieDto.FromZoneWire(die.ObjectId);

    public static ReviveDto ToReviveDto(NetRevive revive) => new ReviveDto { ObjectId = revive.ObjectId };

    public static TeleportToLocationDto ToTeleportToLocationDto(NetTeleportToLocation teleport) =>
        TeleportToLocationDto.FromZoneWire(teleport.ObjectId, teleport.X, teleport.Y, teleport.Z, teleport.Heading);

    public static SkillListDto ToSkillListDto(NetSkillEntry[] rows)
    {
        var dto = new SkillListDto();
        if (rows == null)
            return dto;
        for (int i = 0; i < rows.Length; i++)
        {
            NetSkillEntry row = rows[i];
            bool passive = IsPassiveSkill(row.SkillId, row.Level);
            bool disabled = row.ReuseDelayMs > 0;
            dto.Skills.Add(new SkillInstance(row.SkillId, row.Level, passive, disabled));
        }

        return dto;
    }

    public static MagicSkillUseDto ToMagicSkillUseDto(NetMagicSkillUse use)
    {
        var dto = new MagicSkillUseDto();
        dto.InitFromNet(
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
            ApplyUserPaperdoll(info.Appearance, user.Paperdoll);

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

    public static void ApplyUserPaperdoll(PlayerAppearance appearance, NetPaperdollSlot[] slots)
    {
        if (appearance == null || slots == null)
            return;
        ApplyObjectIds(appearance.PaperDoll, slots);
        ApplyItemIds(appearance, NetPaperdoll.ItemIds(slots));
    }

    public static void ApplyItemIds(PlayerAppearance appearance, int[] itemIds)
    {
        if (appearance == null || itemIds == null)
            return;

        PaperDollSelection doll = appearance.PaperDoll;
        doll.Item_Under = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Under);
        doll.Item_Lear = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftEar);
        doll.Item_Rear = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightEar);
        doll.Item_Neck = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Neck);
        doll.Item_LFinger = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftFinger);
        doll.Item_RFinger = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightFinger);
        doll.Item_Head = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Head);
        doll.Item_RHand = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.RightHand);
        doll.Item_LHand = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.LeftHand);
        doll.Item_Gloves = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Gloves);
        doll.Item_Chest = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Chest);
        doll.Item_Legs = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Legs);
        doll.Item_Feet = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Feet);
        doll.Item_Cloak = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Cloak);
        doll.Item_Face = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Face);
        doll.Item_Hair = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Hair);
        doll.Item_Hair2 = NetPaperdoll.ItemIdAt(itemIds, NetPaperdoll.Hair2);
        appearance.RHand = doll.Item_RHand;
        appearance.LHand = doll.Item_LHand;
        appearance.Gloves = doll.Item_Gloves;
        appearance.Chest = doll.Item_Chest;
        appearance.Legs = doll.Item_Legs;
        appearance.Feet = doll.Item_Feet;
        appearance.PaperdollReady = true;
    }

    static void ApplyObjectIds(PaperDollSelection doll, NetPaperdollSlot[] slots)
    {
        if (doll == null || slots == null)
            return;
        for (int i = 0; i < slots.Length; i++)
        {
            int objectId = slots[i].ObjectId;
            switch (i)
            {
                case NetPaperdoll.Under:
                    doll.Obj_Under = objectId;
                    break;
                case NetPaperdoll.LeftEar:
                    doll.Obj_Lear = objectId;
                    break;
                case NetPaperdoll.RightEar:
                    doll.Obj_Pear = objectId;
                    break;
                case NetPaperdoll.Neck:
                    doll.Obj_Neck = objectId;
                    break;
                case NetPaperdoll.LeftFinger:
                    doll.Obj_LFinger = objectId;
                    break;
                case NetPaperdoll.RightFinger:
                    doll.Obj_RFinger = objectId;
                    break;
                case NetPaperdoll.Head:
                    doll.Obj_Head = objectId;
                    break;
                case NetPaperdoll.RightHand:
                    doll.Obj_RHand = objectId;
                    break;
                case NetPaperdoll.LeftHand:
                    doll.Obj_LHand = objectId;
                    break;
                case NetPaperdoll.Gloves:
                    doll.Obj_Gloves = objectId;
                    break;
                case NetPaperdoll.Chest:
                    doll.Obj_Chest = objectId;
                    break;
                case NetPaperdoll.Legs:
                    doll.Obj_Legs = objectId;
                    break;
                case NetPaperdoll.Feet:
                    doll.Obj_Feet = objectId;
                    break;
                case NetPaperdoll.Cloak:
                    doll.Obj_Cloak = objectId;
                    break;
                case NetPaperdoll.Face:
                    doll.Obj_Face = objectId;
                    break;
                case NetPaperdoll.Hair:
                    doll.Obj_Hair = objectId;
                    break;
                case NetPaperdoll.Hair2:
                    doll.Obj_Hair2 = objectId;
                    break;
            }
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
}
