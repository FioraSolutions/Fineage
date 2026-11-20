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

    /// <summary>
    /// Skillgrp combos that play a cast/throw cycle (Cast*, MagicThrow, MagicShot, MagicNoTarget)
    /// must use the override/cast runner even when is_magic is 0 (e.g. Sand Bomb animation L).
    /// Mix / SpAtk physical combos stay on the physical path.
    /// </summary>
    public static bool IsCastStyleCombo(MagicSkillUseDto useSkill)
    {
        if (!TryGetPlayableCombo(useSkill, out AnimationCombo combo) || combo == null)
            return false;

        string[] cycle = combo.GetAnimCycle();
        if (cycle == null)
            return false;

        for (int i = 0; i < cycle.Length; i++)
        {
            string name = cycle[i];
            if (string.IsNullOrWhiteSpace(name))
                continue;
            string trimmed = name.Trim();
            if (string.Equals(trimmed, "none", System.StringComparison.OrdinalIgnoreCase))
                continue;
            if (trimmed.StartsWith("Cast", System.StringComparison.Ordinal) ||
                trimmed.StartsWith("MagicThrow", System.StringComparison.Ordinal) ||
                trimmed.StartsWith("MagicShot", System.StringComparison.Ordinal) ||
                trimmed.StartsWith("MagicNoTarget", System.StringComparison.Ordinal))
                return true;
        }

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
