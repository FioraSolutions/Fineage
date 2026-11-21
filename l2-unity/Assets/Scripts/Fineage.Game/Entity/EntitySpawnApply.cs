/// <summary>
/// Pure EntitySpawn → identity field mapping (#930). Keep ApplyNpcSpawn thin; EditMode tests this.
/// </summary>
public static class EntitySpawnApply
{
    /// <summary>Mobius live door entities: NpcKind, template/npc id 0, name from Doors.xml.</summary>
    public static bool IsLiveDoorSpawn(int templateId, string name) =>
        templateId == 0 && !string.IsNullOrEmpty(name);

    public static EntityType ResolveDoorEntityType(bool attackable) =>
        attackable ? EntityType.Monster : EntityType.NPC;

    public static EntityType ResolveEntityType(bool attackable) =>
        attackable ? EntityType.Monster : EntityType.NPC;

    public static float ResolveHeadingDegrees(int heading) =>
        VectorUtils.HeadingToUnityQuaternionForNpc(heading);
}
