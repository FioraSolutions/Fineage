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

    public static AttackHitCue[] PlanCues(AttackDto dto)
    {
        if (dto == null)
            return System.Array.Empty<AttackHitCue>();
        int count = CountHits(dto);
        var cues = new AttackHitCue[count];
        for (int i = 0; i < count; i++)
            cues[i] = AttackHitWireRows.CueFor(ResolveHit(dto, i), i, count);
        return cues;
    }

    public static Hit[] BuildHitList(
        int targetId,
        int firstDamage,
        int firstFlags,
        int[] extraDamage,
        int[] extraFlags) =>
        AttackHitWireRows.BuildHitList(targetId, firstDamage, firstFlags, extraDamage, extraFlags);
}