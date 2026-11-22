using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

/// <summary>
/// Data-driven physical skill animation resolution from skillgrp + AnimationCombo_Interlude.
/// Centralizes SpAtk phase naming and refuses silent SpAtk01/1HS substitution for SpAtk03+.
/// </summary>
public static class SkillAnimationResolver
{
    /// <summary>Mobius/Interlude tray icon for EtcStatusUpdate charge count (force / sonic momentum).</summary>
    public const int ForceChargeTraySkillId = 4271;

    static readonly HashSet<string> MixMissLogged = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>
    /// Returns cleaned SpAtk stem names for each combo phase (e.g. SpAtk08, SpAtk13, SpAtk22), or null.
    /// </summary>
    public static string[] GetSpAtkPhaseStems(int skillId, int skillLevel)
    {
        if (TablePacketServices.SkillGrps == null)
            return null;

        AnimationCombo combo = TablePacketServices.SkillGrps.GetAnimComboBySkillId(skillId, skillLevel);
        if (combo == null)
            return null;

        string[] cycle = combo.GetAnimCycle();
        if (cycle == null || cycle.Length == 0)
            return null;

        var stems = new List<string>(cycle.Length);
        for (int i = 0; i < cycle.Length; i++)
        {
            string name = cycle[i];
            if (string.IsNullOrWhiteSpace(name))
                continue;
            string trimmed = name.Trim();
            if (string.Equals(trimmed, "none", StringComparison.OrdinalIgnoreCase))
                continue;
            if (!trimmed.StartsWith("SpAtk", StringComparison.OrdinalIgnoreCase))
                continue;
            stems.Add(trimmed);
        }

        return stems.Count == 0 ? null : stems.ToArray();
    }

    /// <summary>Trigger prefix for <see cref="AnimationManager.PlayAnimationTrigger"/> (SpAtk08_ + weapon suffix).</summary>
    public static string ToTriggerPrefix(string spatkStem)
    {
        if (string.IsNullOrEmpty(spatkStem))
            return spatkStem;
        return spatkStem.EndsWith("_", StringComparison.Ordinal) ? spatkStem : spatkStem + "_";
    }

    /// <summary>
    /// First playable SpAtk trigger for legacy single-phase paths, or null if the skill has no SpAtk combo.
    /// </summary>
    public static bool TryGetPrimarySpAtkTrigger(int skillId, int skillLevel, out string triggerPrefix)
    {
        triggerPrefix = null;
        string[] stems = GetSpAtkPhaseStems(skillId, skillLevel);
        if (stems == null || stems.Length == 0)
            return false;
        triggerPrefix = ToTriggerPrefix(stems[0]);
        return true;
    }

    static int SpAtkIndex(string stateName)
    {
        if (string.IsNullOrEmpty(stateName) ||
            stateName.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) != 0)
            return -1;

        int i = 5;
        while (i < stateName.Length && char.IsDigit(stateName[i]))
            i++;
        if (i <= 5)
            return -1;

        return int.TryParse(stateName.Substring(5, i - 5), NumberStyles.None, CultureInfo.InvariantCulture, out int n)
            ? n
            : -1;
    }

    /// <summary>
    /// Resolves animator state for combat cross-fade. SpAtk03+ never borrow SpAtk01_1HS; missing clips are logged.
    /// </summary>
    public static string ResolvePlayableCombatState(
        IAnimationController controller,
        string stateName,
        string raceName,
        int skillId,
        Func<string, bool> hasState)
    {
        if (string.IsNullOrEmpty(stateName) || hasState(stateName))
            return stateName;

        int split = stateName.LastIndexOf('_');
        if (split <= 0)
            return stateName;

        string stem = stateName.Substring(0, split + 1);
        if (stateName.EndsWith("_hand", StringComparison.Ordinal))
            return stateName;

        if (stateName.IndexOf("SpAtk06_", StringComparison.Ordinal) == 0 &&
            hasState(AnimationManager.SpAtk06HandState))
            return AnimationManager.SpAtk06HandState;

        int spatk = SpAtkIndex(stateName);
        if (spatk >= 3)
        {
            if (SkillAnimationDatabase.GetSpAtkCombatClip(stateName, raceName) != null)
                return stateName;

            LogMissingCombatClip(skillId, raceName, stateName);
            return stateName;
        }

        string oneHs = stem + "1HS";
        if (!string.Equals(oneHs, stateName, StringComparison.Ordinal) && hasState(oneHs))
            return oneHs;

        if (stem.StartsWith("SpAtk", StringComparison.Ordinal))
            LogMissingCombatClip(skillId, raceName, stateName);

        return stateName;
    }

    public static void LogMissingCombatClip(int skillId, string raceName, string stateName)
    {
        string weapon = stateName.Contains("_")
            ? stateName.Substring(stateName.LastIndexOf('_') + 1)
            : "?";
        string stem = stateName.Contains("_")
            ? stateName.Substring(0, stateName.LastIndexOf('_'))
            : stateName;
        string clipStem = stem + "_" + CanonicalWeaponForClip(weapon);
        string key = skillId + "|" + raceName + "|" + clipStem;
        if (!MixMissLogged.Add(key))
            return;

        Debug.LogError(
            "[SkillAnimation] clip missing skillId=" + skillId +
            " race=" + raceName +
            " weapon=" + weapon +
            " expectedClipStem=" + clipStem +
            " animatorState=" + stateName +
            " (run L2MFighterDuelistSpAtkImporter after staging MFighter_anim.fbx).");
    }

    public static void LogMissingMixCombo(int skillId, int skillLevel, string animTag)
    {
        string key = skillId + "@" + skillLevel;
        if (!MixMissLogged.Add(key))
            return;

        Debug.LogError(
            "[SkillAnimation] AnimationCombo missing skillId=" + skillId +
            " level=" + skillLevel +
            " animTag=" + (animTag ?? "<null>") +
            " (check Skillgrp + AnimationCombo_Interlude).");
    }

    static string CanonicalWeaponForClip(string weaponSuffix)
    {
        if (string.IsNullOrEmpty(weaponSuffix))
            return weaponSuffix;
        switch (weaponSuffix.ToLowerInvariant())
        {
            case "dual": return "Dual";
            case "hand": return "Hand";
            case "pole": return "pole";
            case "bow": return "Bow";
            default: return weaponSuffix;
        }
    }

    static readonly HashSet<string> OverrideMissLogged = new HashSet<string>(StringComparer.Ordinal);
    static readonly HashSet<string> OverrideFallbackLogged = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>Override slots already reached through <c>GetOverrideClip</c> / cast playable.</summary>
    public static readonly string[] GameplayHookedOverrideSlots =
    {
        "CastLong", "CastShort", "CastMid", "CastEnd",
        "MagicShot", "MagicThrow", "MagicNoTarget", "MagicNotarget",
    };

    /// <summary>Clips in catalog but no gameplay trigger yet (#1620 follow-up).</summary>
    public static readonly string[] UnhookedOverrideSlots =
    {
        "Swim", "SwimWait", "SwimDeath", "SwimDeathWait",
        "Fishing_end", "Fishing_fly", "Fishing_pull", "Fishing_wait", "Fishing_control",
        "ChairSit", "ChairWait", "ChairStand",
        "Wyvern", "Falling", "Damagefly",
    };

    public static void LogMissingOverrideClip(string slotName, string raceName)
    {
        string key = (slotName ?? "") + "|" + (raceName ?? "");
        if (!OverrideMissLogged.Add(key))
            return;

        Debug.LogError(
            "[SkillAnimation] override unresolved slot=" + slotName +
            " race=" + raceName +
            " (SkillAnimationDatabase.GetOverrideClip; import clip or add trigger).");
    }

    public static void LogOverrideCompletionFallback(
        string requestedSlot,
        string fallbackSlot,
        string raceName,
        string loadedClipName)
    {
        string key = requestedSlot + ">" + fallbackSlot + "|" + raceName;
        if (!OverrideFallbackLogged.Add(key))
            return;

        Debug.LogError(
            "[SkillAnimation] override completion fallback requested=" + requestedSlot +
            " used=" + fallbackSlot +
            " race=" + raceName +
            " clip=" + loadedClipName +
            " (import dedicated take or accept shared mid/shot clip).");
    }
}