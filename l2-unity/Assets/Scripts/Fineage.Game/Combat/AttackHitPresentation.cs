/// <summary>
/// Shared Attack packet hit resolution for combat presentation (per-hit flags, no weapon special-cases).
/// </summary>
public static class AttackHitPresentation
{
    public const int HitFlagUsess = 0x10;

    public static int CountHits(AttackDto dto)
    {
        if (dto == null)
            return 0;
        if (dto.ArrHit != null && dto.ArrHit.Length > 0)
            return dto.ArrHit.Length;
        return dto.FirstHit != null ? 1 : 0;
    }

    public static Hit ResolveHit(AttackDto dto, int hitIndex)
    {
        if (dto == null || hitIndex < 0)
            return null;
        if (dto.ArrHit != null && hitIndex < dto.ArrHit.Length)
            return dto.ArrHit[hitIndex];
        if (hitIndex == 0)
            return dto.FirstHit;
        return null;
    }

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