/// <summary>Interlude DTO → domain event mapping (wire adapter v1).</summary>
public static class InterludeDomainMap
{
    public static DomainMoveTo Move(CharMoveToLocationDto dto)
    {
        if (dto == null)
            return null;
        return new DomainMoveTo
        {
            ObjectId = dto.ObjId,
            Destination = dto.NewPosition,
            Origin = dto.OldPosition,
            InterludeDto = dto
        };
    }

    public static DomainStopMove Stop(StopMoveDto dto)
    {
        if (dto == null)
            return null;
        return new DomainStopMove
        {
            ObjectId = dto.ObjId,
            StopPos = dto.StopPos,
            Heading = dto.Heading,
            InterludeDto = dto
        };
    }

    public static DomainKnownlistAdd Char(CharInfoDto dto)
    {
        if (dto == null || dto.Identity == null)
            return null;
        return new DomainKnownlistAdd
        {
            Kind = KnownlistKind.User,
            ObjectId = dto.Identity.Id,
            InterludeChar = dto
        };
    }

    public static DomainKnownlistAdd Npc(NpcInfoDto dto)
    {
        if (dto == null || dto.Identity == null)
            return null;
        return new DomainKnownlistAdd
        {
            Kind = KnownlistKind.Npc,
            ObjectId = dto.Identity.Id,
            InterludeNpc = dto
        };
    }

    public static DomainPlayerSpawned PlayerSpawned(PlayerInfoInterlude info)
    {
        if (info.Identity == null)
            return null;
        return new DomainPlayerSpawned
        {
            ObjectId = info.Identity.Id,
            Identity = info.Identity,
            Status = info.Status,
            Stats = info.Stats,
            Appearance = info.Appearance
        };
    }

    public static DomainStatsChanged Stats(UserInfoDto dto)
    {
        if (dto == null || dto.PlayerInfoInterlude.Identity == null)
            return null;
        return new DomainStatsChanged
        {
            ObjectId = dto.PlayerInfoInterlude.Identity.Id,
            InterludeUserInfo = dto
        };
    }
}
