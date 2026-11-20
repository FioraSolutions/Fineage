/// <summary>
/// Per-equipped-weapon combat presentation: anim suffix, range, ranged cycle timing, hit fraction.
/// Resolved once at equip via <see cref="CombatStyleProfileResolver"/>; gameplay reads fields only.
/// </summary>
public sealed class CombatStyleProfile
{
    public const float InterludeBowHitFraction = 0.82f;
    public const float InterludeMeleeHitFraction = 0.5f;

    public static readonly CombatStyleProfile BareHand = new CombatStyleProfile(
        animSuffix: "hand",
        attackRangeUu: 40,
        isRanged: false,
        isDualAnimSuffix: false,
        hitFractionOverAttackCycle: InterludeMeleeHitFraction,
        isResolved: true);

    public static readonly CombatStyleProfile Unresolved = new CombatStyleProfile(
        animSuffix: string.Empty,
        attackRangeUu: 0,
        isRanged: false,
        isDualAnimSuffix: false,
        hitFractionOverAttackCycle: 0f,
        isResolved: false);

    public CombatStyleProfile(
        string animSuffix,
        int attackRangeUu,
        bool isRanged,
        float hitFractionOverAttackCycle,
        bool isDualAnimSuffix,
        bool isResolved)
    {
        AnimSuffix = animSuffix ?? string.Empty;
        AttackRangeUu = attackRangeUu;
        IsRanged = isRanged;
        IsDualAnimSuffix = isDualAnimSuffix;
        HitFractionOverAttackCycle = hitFractionOverAttackCycle;
        IsResolved = isResolved;
    }

    public string AnimSuffix { get; }

    public int AttackRangeUu { get; }

    /// <summary>Bow-style attacks: facing, ranged hit path, distance-based cycle time.</summary>
    public bool IsRanged { get; }

    /// <summary>Dual-sword jatk timing (suffix dual).</summary>
    public bool IsDualAnimSuffix { get; }

    public float HitFractionOverAttackCycle { get; }

    public bool IsResolved { get; }
}