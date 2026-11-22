/// <summary>Engine-free Attack packet hit rows (first hit + ExtraHits tail).</summary>
public static class AttackHitWireRows
{
    public static Hit[] BuildHitList(
        int targetId,
        int firstDamage,
        int firstFlags,
        int[] extraDamage,
        int[] extraFlags)
    {
        int extraCount = extraDamage != null ? extraDamage.Length : 0;
        if (extraCount == 0)
            return new[] { new Hit(targetId, firstDamage, firstFlags) };

        var rows = new Hit[1 + extraCount];
        rows[0] = new Hit(targetId, firstDamage, firstFlags);
        for (int i = 0; i < extraCount; i++)
        {
            int damage = extraDamage[i];
            int flags = extraFlags != null && i < extraFlags.Length ? extraFlags[i] : 0;
            rows[i + 1] = new Hit(targetId, damage, flags);
        }

        return rows;
    }
}