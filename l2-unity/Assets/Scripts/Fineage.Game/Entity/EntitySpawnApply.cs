/// <summary>
/// Pure EntitySpawn → identity field mapping (#930). Keep ApplyNpcSpawn thin; EditMode tests this.
/// </summary>
public static class EntitySpawnApply
{
    public static EntityType ResolveEntityType(bool attackable) =>
        attackable ? EntityType.Monster : EntityType.NPC;

    public static float ResolveHeadingDegrees(int heading) =>
        VectorUtils.HeadingToUnityQuaternionForNpc(heading);
}
