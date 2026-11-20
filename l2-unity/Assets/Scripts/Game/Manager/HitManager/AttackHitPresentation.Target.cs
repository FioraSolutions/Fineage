/// <summary>
/// Entity lookup for attack hits — lives outside Fineage.Game (Entity is not in that asmdef).
/// </summary>
public static class AttackHitPresentationTarget
{
    public static Entity ResolveHitTarget(Entity fallback, Hit hit, System.Func<int, Entity> lookup)
    {
        if (hit == null)
            return fallback;
        if (lookup != null)
        {
            Entity resolved = lookup(hit.TargetId);
            if (resolved != null)
                return resolved;
        }
        if (fallback != null && fallback.Identity != null && fallback.Identity.Id == hit.TargetId)
            return fallback;
        return fallback;
    }
}