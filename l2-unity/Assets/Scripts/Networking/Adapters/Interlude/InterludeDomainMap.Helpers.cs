public static partial class InterludeDomainMap
{
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
