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
            WalkRealSpeed = spawned.WalkSpeed > 0.001f ? spawned.WalkSpeed : 80f
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
}
