/// <summary>How one Attack-packet hit is drawn (number, crit, miss, shield).</summary>
public enum AttackHitLabelKind
{
    Damage = 0,
    Critical = 1,
    Miss = 2,
    Shield = 3
}

/// <summary>
/// One presentation of an Attack hit list row. <see cref="CycleFraction"/> is Mobius
/// <c>calculateTimeToHit</c> (non-bow = timeAtk/2) then hit <c>i</c> of <c>N</c> at
/// <c>sAtk * (i+1) / N</c> — dual/fist land at timeAtk/4 and timeAtk/2.
/// </summary>
public readonly struct AttackHitCue
{
    public AttackHitCue(int index, float cycleFraction, Hit hit, string label, AttackHitLabelKind kind)
    {
        Index = index;
        CycleFraction = cycleFraction;
        Hit = hit;
        Label = label ?? "";
        Kind = kind;
    }

    public int Index { get; }
    public float CycleFraction { get; }
    public Hit Hit { get; }
    public string Label { get; }
    public AttackHitLabelKind Kind { get; }
    public bool Soulshot => Hit != null && Hit.hasSoulshot();
}

/// <summary>Engine-free Attack packet hit rows (first hit + ExtraHits tail).</summary>
public static class AttackHitWireRows
{
    public static float HitCycleFraction(int hitIndex, int hitCount)
    {
        if (hitCount <= 0 || hitIndex < 0 || hitIndex >= hitCount)
            return 0f;
        return 0.5f * (hitIndex + 1) / hitCount;
    }

    public static AttackHitCue CueFor(Hit hit, int index, int hitCount)
    {
        AttackHitLabelKind kind;
        string label;
        if (hit == null)
        {
            kind = AttackHitLabelKind.Damage;
            label = "";
        }
        else if (hit.isMiss())
        {
            kind = AttackHitLabelKind.Miss;
            label = "Miss";
        }
        else if (hit.isCrit())
        {
            kind = AttackHitLabelKind.Critical;
            label = hit.Damage.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (hit.isShielded())
        {
            kind = AttackHitLabelKind.Shield;
            label = hit.Damage.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            kind = AttackHitLabelKind.Damage;
            label = hit.Damage.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        return new AttackHitCue(index, HitCycleFraction(index, hitCount), hit, label, kind);
    }

    public static AttackHitCue[] PlanCues(
        int targetId,
        int firstDamage,
        int firstFlags,
        int[] extraDamage,
        int[] extraFlags)
    {
        Hit[] rows = BuildHitList(targetId, firstDamage, firstFlags, extraDamage, extraFlags);
        var cues = new AttackHitCue[rows.Length];
        for (int i = 0; i < rows.Length; i++)
            cues[i] = CueFor(rows[i], i, rows.Length);
        return cues;
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