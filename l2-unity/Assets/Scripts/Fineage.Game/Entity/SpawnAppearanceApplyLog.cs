/// <summary>Logs when EntitySpawn appearance/stats use packet fallbacks (audit batch 5).</summary>
public static class SpawnAppearanceApplyLog
{
    public static void LogDefaultMaxHp(int entityId, int packetMaxHp)
    {
        FineageLog.Error(
            FineageLogCategory.Net,
            "EntitySpawn: missing MaxHp for objectId=" + entityId +
            " (packet=" + packetMaxHp + "); using client default 100");
    }

    public static void LogDefaultMoveSpeed(int entityId, float runSpeed, float walkSpeed)
    {
        FineageLog.Error(
            FineageLogCategory.Net,
            "EntitySpawn: missing run/walk speed for objectId=" + entityId +
            " (run=" + runSpeed + " walk=" + walkSpeed + "); using 120/80 defaults");
    }

    public static void LogRaceDerivedFromClass(int entityId, int spawnRace, int classId)
    {
        FineageLog.Error(
            FineageLogCategory.Net,
            "EntitySpawn: invalid Race byte=" + spawnRace + " for objectId=" + entityId +
            "; deriving race from classId=" + classId);
    }
}