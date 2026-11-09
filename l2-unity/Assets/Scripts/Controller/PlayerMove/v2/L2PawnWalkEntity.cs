/// <summary>
/// Entity-typed helpers for <see cref="L2PawnWalk"/>.
/// </summary>
public static class L2PawnWalkEntity
{
    public static bool IsCharInfoPawn(Entity entity)
    {
        return entity is UserEntity;
    }
}
