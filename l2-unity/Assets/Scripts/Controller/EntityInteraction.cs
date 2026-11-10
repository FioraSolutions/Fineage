/// <summary>
/// Interaction SoT (#930): wire <c>Attackable</c> (via <see cref="NetNpcSession"/>) drives
/// cursor and click intent — not <c>is MonsterEntity</c> alone (Folk can reuse monster meshes).
/// </summary>
public static class EntityInteraction
{
    public enum CursorKind
    {
        Default,
        Attack,
        Talk
    }

    /// <summary>
    /// Hover / targeted cursor for a world entity.
    /// </summary>
    public static CursorKind ResolveCursor(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return CursorKind.Default;

        if (PetVisuals.IsPet(entity.Identity.Owned, entity.Identity.NpcId))
            return CursorKind.Talk;

        if (IsAttackable(entity))
            return CursorKind.Attack;

        if (entity is NpcEntity || entity is MonsterEntity || entity is NetworkEntity)
        {
            if (entity is PlayerEntity || entity is UserEntity)
                return CursorKind.Default;
            return CursorKind.Talk;
        }

        return CursorKind.Default;
    }

    /// <summary>
    /// True when a non-force click should swing instead of talk.
    /// </summary>
    public static bool IsAttackable(Entity entity)
    {
        if (entity == null || entity.Identity == null)
            return false;

        return NetNpcSession.IsAttackable(
            entity.Identity.Id,
            entity is MonsterEntity);
    }

    /// <summary>
    /// True when a peaceful network NPC should open talk (not attack) on click.
    /// </summary>
    public static bool ShouldTalkOnClick(Entity entity, bool forceAttack)
    {
        if (forceAttack || entity == null)
            return false;
        if (entity is PlayerEntity || entity is UserEntity || entity is ItemEntity)
            return false;
        if (!(entity is NetworkEntity))
            return false;
        return !IsAttackable(entity);
    }
}
