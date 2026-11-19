using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SetupDurationHelper
{
    public static int CastStyleOf(MagicSkillUseDto useSkill)
    {
        if (useSkill == null || useSkill.SkillGrp == null)
        {
            return 1;
        }

        return useSkill.SkillGrp.CastStyle;
    }

    public static bool MixComboOf(MagicSkillUseDto useSkill)
    {
        if (useSkill == null || useSkill.SkillGrp == null || string.IsNullOrEmpty(useSkill.SkillGrp.Animation))
        {
            return false;
        }

        return useSkill.SkillGrp.Animation.IndexOf("Mix", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static bool IsUsePotion(MagicSkillUseDto useSkill)
    {
        return useSkill != null && PotionSkillIds.Contains(useSkill.SkillId);
    }

    /// <summary>
    /// The Interlude client picks the cast graph from the skillgrp <c>animation</c> tag, not from
    /// <c>is_magic</c>. Trick (11) and Switch (12) are <c>is_magic=0</c> utilities that animate
    /// <c>{[C]}</c> = CastShort → CastEnd → MagicThrow, and Spoil (254) / Unlock (27) sit on the same
    /// cast tags. CastShort / CastLong only exist as patches over the cast graph (CastMid) and
    /// MagicThrow over MagicShot, so those skills must run through the magic runner — cast bar,
    /// stationary cast, shoot notify, and the art split between caster and victim. The physical
    /// SpAtk path has no such states and played nothing.
    /// </summary>
    public static bool UsesCastAnimation(MagicSkillUseDto useSkill)
    {
        if (useSkill == null || TablePacketServices.SkillGrps == null)
            return false;

        return UsesCastAnimation(
            TablePacketServices.SkillGrps.GetAnimComboBySkillId(useSkill.SkillId, useSkill.SkillLvl));
    }

    /// <summary>True when the combo's first playable clip is a Cast* tag (CastShort/CastMid/CastLong/CastEnd).</summary>
    public static bool UsesCastAnimation(AnimationCombo combo)
    {
        string[] cycle = combo != null ? combo.GetAnimCycle() : null;
        if (cycle == null)
            return false;

        for (int i = 0; i < cycle.Length; i++)
        {
            string clip = cycle[i];
            if (string.IsNullOrWhiteSpace(clip))
                continue;

            clip = clip.Trim();
            if (string.Equals(clip, "none", StringComparison.OrdinalIgnoreCase))
                continue;

            return clip.StartsWith("Cast", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public static bool TryGetPlayableCombo(MagicSkillUseDto useSkill, out AnimationCombo combo)
    {
        combo = null;
        if (useSkill == null || TablePacketServices.SkillGrps == null)
            return false;

        combo = TablePacketServices.SkillGrps.GetAnimComboBySkillId(useSkill.SkillId, useSkill.SkillLvl);
        if (combo == null)
            return false;

        string[] cycle = combo.GetAnimCycle();
        if (cycle == null)
            return false;

        for (int i = 0; i < cycle.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(cycle[i]))
                continue;
            if (string.Equals(cycle[i].Trim(), "none", StringComparison.OrdinalIgnoreCase))
                continue;
            return true;
        }

        combo = null;
        return false;
    }

    public static bool IsPickupAnimationPlaying(int objectId)
    {
        if (ControllerPacketServices.Animations == null)
            return false;

        string current = ControllerPacketServices.Animations.GetCurrentAnimationName(objectId);
        return !string.IsNullOrEmpty(current)
            && current.IndexOf("pickup", StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
