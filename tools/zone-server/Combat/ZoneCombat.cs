namespace Fineage.ZoneServer.Combat;

/// <summary>Minimal melee constants — not full Interlude formulas.</summary>
public static class ZoneCombat
{
    /// <summary>L2 world units; generous for thin slice near TI spawn mobs.</summary>
    public const int MeleeRange = 900;

    public const int SampleMeleeDamage = 32;

    public static long DistanceSq2D(int ax, int ay, int bx, int by)
    {
        long dx = ax - bx;
        long dy = ay - by;
        return dx * dx + dy * dy;
    }

    public static bool InMeleeRange(int ax, int ay, int bx, int by) =>
        DistanceSq2D(ax, ay, bx, by) <= (long)MeleeRange * MeleeRange;
}
