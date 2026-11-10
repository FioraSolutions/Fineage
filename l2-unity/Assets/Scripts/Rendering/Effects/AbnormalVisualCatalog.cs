using System;
using System.Collections.Generic;

/// <summary>
/// Mobius CT0 <c>abnormalVisualEffect</c> → client pose/FX policy.
/// Sleep/Stun stay in idle; only <c>FLESH_STONE</c> (Anchor / Medusa / …) petrifies.
/// </summary>
public enum AbnormalPoseKind
{
    None = 0,
    /// <summary>Interlude wait / atkwait — Sleep, Stun.</summary>
    Idle = 1,
    /// <summary>Freeze current pose + stone look — Anchor (FLESH_STONE).</summary>
    Petrify = 2,
}

[Flags]
public enum AbnormalAveFlags
{
    None = 0,
    Sleep = 1 << 0,
    Stun = 1 << 1,
    Root = 1 << 2,
    FleshStone = 1 << 3,
    /// <summary>Mobius <c>abnormalVisualEffect=DOT_POISON</c> (Poison / Poisonous Cloud).</summary>
    DotPoison = 1 << 4,
}

/// <summary>Resolves PartySpelled / AbnormalStatusUpdate rows into Interlude AVE pose + FX flags.</summary>
public static class AbnormalVisualCatalog
{
    /// <summary>Mobius skills with <c>abnormalVisualEffect=FLESH_STONE</c> (Anchor, Medusa, petrify scrolls, …).</summary>
    static readonly HashSet<int> FleshStoneSkillIds = new()
    {
        367, 1170, 2239, 3089, 3102, 3117, 3193, 4111, 4383, 4515, 4578, 4616, 5172,
    };

    public static AbnormalAveFlags ResolveFlags(IList<EffectHolder> effects)
    {
        AbnormalAveFlags flags = AbnormalAveFlags.None;
        if (effects == null)
            return flags;
        for (int i = 0; i < effects.Count; i++)
        {
            EffectHolder e = effects[i];
            if (e == null)
                continue;
            flags |= FlagFor(e);
        }
        return flags;
    }

    public static AbnormalPoseKind ResolvePose(AbnormalAveFlags flags)
    {
        // Petrify wins over idle (Anchor while also stunned is still stone).
        if ((flags & AbnormalAveFlags.FleshStone) != 0)
            return AbnormalPoseKind.Petrify;
        if ((flags & (AbnormalAveFlags.Sleep | AbnormalAveFlags.Stun)) != 0)
            return AbnormalPoseKind.Idle;
        return AbnormalPoseKind.None;
    }

    static AbnormalAveFlags FlagFor(EffectHolder e)
    {
        if (FleshStoneSkillIds.Contains(e.Id))
            return AbnormalAveFlags.FleshStone;

        string slot = e.Slot;
        if (!string.IsNullOrEmpty(slot))
        {
            if (slot.Equals("SLEEP", StringComparison.OrdinalIgnoreCase))
                return AbnormalAveFlags.Sleep;
            if (slot.Equals("STUN", StringComparison.OrdinalIgnoreCase) ||
                slot.Equals("POSSESSION", StringComparison.OrdinalIgnoreCase))
                return AbnormalAveFlags.Stun;
            if (slot.StartsWith("ROOT", StringComparison.OrdinalIgnoreCase))
                return AbnormalAveFlags.Root;
            // PARALYZE without FLESH_STONE skill id uses PARALYZE AVE (not stone) — idle-like hold.
            if (slot.Equals("PARALYZE", StringComparison.OrdinalIgnoreCase) ||
                slot.Equals("ANESTHESIA", StringComparison.OrdinalIgnoreCase))
                return AbnormalAveFlags.Stun;
            if (slot.Equals("POISON", StringComparison.OrdinalIgnoreCase) ||
                slot.Equals("DOT_POISON", StringComparison.OrdinalIgnoreCase))
                return AbnormalAveFlags.DotPoison;
            return AbnormalAveFlags.None;
        }

        // Legacy snapshots without Slot — known control / DoT skills only.
        switch (e.Id)
        {
            case 1069:
            case 1072:
            case 4046:
            case 4185:
                return AbnormalAveFlags.Sleep;
            case 92:
            case 48:
                return AbnormalAveFlags.Stun;
            case 1201:
            case 402:
            case 5023:
                return AbnormalAveFlags.Root;
            case 129:
            case 1167:
            case 1168:
            case 4036:
            case 703:
                return AbnormalAveFlags.DotPoison;
            default:
                return AbnormalAveFlags.None;
        }
    }
}
