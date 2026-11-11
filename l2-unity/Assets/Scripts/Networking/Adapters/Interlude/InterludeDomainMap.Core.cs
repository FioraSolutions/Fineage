public static partial class InterludeDomainMap
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
}
