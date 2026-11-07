using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Fineage Net presence/spawn wire → Interlude-shaped entity DTOs.</summary>
public static class NetEntityMap
{
    /// <summary>
    /// Legacy PlayerSpawned staging only — live peers/self use <c>EntitySpawnMsg</c> in
    /// <c>NetSession</c>. Vitals/stats beyond pose come from <see cref="NetGearMap.OverlayUserInfo"/>.
    /// </summary>
    public static PlayerInfoInterlude ToPlayerInfo(NetPlayerSpawned spawned)
    {
        EntityIdentity identity = new EntityIdentity
        {
            EntityType = EntityType.Player,
            Id = spawned.ObjectId,
            Name = string.IsNullOrEmpty(spawned.Name) ? "Unknown" : spawned.Name,
            PlayerClass = spawned.ClassId
        };
        identity.SetL2jPos(spawned.X, spawned.Y, spawned.Z);
        // EntityActionMove / UserSpawner read Identity.IsRunning (not Appearance.Running):
        // without this a remote player walks until a ChangeMoveType arrives.
        identity.IsRunning = spawned.Running;

        int maxHp = spawned.MaxHp > 0 ? spawned.MaxHp : 0;
        PlayerStats stats = new PlayerStats
        {
            BaseRunSpeed = Mathf.RoundToInt(spawned.RunSpeed > 0.001f ? spawned.RunSpeed : 120f),
            BaseWalkingSpeed = Mathf.RoundToInt(spawned.WalkSpeed > 0.001f ? spawned.WalkSpeed : 80f),
            RunRealSpeed = spawned.RunSpeed > 0.001f ? spawned.RunSpeed : 120f,
            WalkRealSpeed = spawned.WalkSpeed > 0.001f ? spawned.WalkSpeed : 80f,
            Level = spawned.Level > 0 ? spawned.Level : 0,
            MaxHp = maxHp
        };

        PlayerAppearance appearance = new PlayerAppearance
        {
            Running = spawned.Running,
            Race = spawned.Race,
            Sex = spawned.Sex,
            Face = 0,
            HairStyle = 0,
            HairColor = 0
        };

        PlayerStatus status = new PlayerStatus();
        if (maxHp > 0)
            status.SetHp(maxHp);
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

    /// <summary>Thin Fineage Net CharInfo (1005) → Interlude-shaped DTO for knownlist spawn.</summary>
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
}
