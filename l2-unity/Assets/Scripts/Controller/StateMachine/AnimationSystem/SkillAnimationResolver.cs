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

    /// <summary>Skill currently resolving a clip. Callers set this before GetOverrideClip.</summary>
    public static int ActiveSkillId { get; private set; }

    public static void SetActiveSkill(int skillId)
    {
        ActiveSkillId = skillId < 0 ? 0 : skillId;
    }

    /// <summary>Body token (MFighter, FDarkElf, …) → Interlude race and sex.</summary>
    public static void DescribeBody(string bodyName, out string race, out string sex)
    {
        race = bodyName ?? "";
        sex = "?";
        if (string.IsNullOrEmpty(bodyName))
            return;

        if (HasBodyToken(bodyName, "FDarkElf")) { race = "DarkElf"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MDarkElf")) { race = "DarkElf"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FShaman")) { race = "Orc"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MShaman")) { race = "Orc"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FFighter")) { race = "Human"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MFighter")) { race = "Human"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FMagic")) { race = "Human"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MMagic")) { race = "Human"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FElf")) { race = "Elf"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MElf")) { race = "Elf"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FDwarf")) { race = "Dwarf"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MDwarf")) { race = "Dwarf"; sex = "M"; return; }
        if (HasBodyToken(bodyName, "FOrc")) { race = "Orc"; sex = "F"; return; }
        if (HasBodyToken(bodyName, "MOrc")) { race = "Orc"; sex = "M"; return; }
    }

    static bool HasBodyToken(string bodyName, string token)
    {
        return bodyName.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0;
    }

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

    /// <summary>
    /// Body motion that must play from its own clip. A miss must not fall through
    /// to the animator trigger (Player_Basic SpAtk01).
    /// </summary>
    public static bool IsCombatBodyState(string triggerName)
    {
        return PlayerSpAtkAnim.TryResolve(triggerName, out _)
            || PlayerBasicAttackAnim.TryResolve(triggerName, out _);
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
            SkillAnimationDatabase.GetSpAtkCombatClip(AnimationManager.SpAtk06HandState, raceName) != null)
        {
            LogSpAtk06Hand(skillId, raceName, stateName, AnimationManager.SpAtk06HandState);
            return AnimationManager.SpAtk06HandState;
        }

        int spatk = SpAtkIndex(stateName);
        if (spatk > 0)
        {
            if (SkillAnimationDatabase.GetSpAtkCombatClip(stateName, raceName) != null)
                return stateName;

            LogMissingCombatClip(skillId, raceName, stateName);
            return stateName;
        }

        string oneHs = stem + "1HS";
        if (!string.Equals(oneHs, stateName, StringComparison.Ordinal) && hasState(oneHs))
        {
            LogWeaponStateFallback(skillId, raceName, stateName, oneHs);
            return oneHs;
        }

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

        string path = "(unresolved)";
        if (SkillAnimationDatabase.TryGetSpAtkCombatResourcePath(stateName, raceName, out string resourcesPath)
            && !string.IsNullOrEmpty(resourcesPath))
        {
            path = "Resources/" + resourcesPath;
        }

        DescribeBody(raceName, out string race, out string sex);
        Debug.LogError(
            "[SkillAnimation] clip missing skillId=" + skillId +
            " race=" + race +
            " sex=" + sex +
            " body=" + raceName +
            " weapon=" + weapon +
            " expectedClip=" + clipStem +
            " path=" + path +
            " animatorState=" + stateName +
            " (InterludeAnimResourceTake.tsv → Resources; no substitute swing).");
    }

    static void LogWeaponStateFallback(int skillId, string raceName, string requested, string used)
    {
        string key = skillId + "|" + (raceName ?? "") + "|" + requested + ">" + used;
        if (!MixMissLogged.Add(key))
            return;

        DescribeBody(raceName, out string race, out string sex);
        Debug.LogError(
            "[SkillAnimation] weapon-state fallback skillId=" + skillId +
            " race=" + race +
            " sex=" + sex +
            " body=" + raceName +
            " expected=" + requested +
            " used=" + used +
            " (animator has no requested state).");
    }

    static readonly HashSet<string> SpAtk06HandLogged = new HashSet<string>(StringComparer.Ordinal);

    static void LogSpAtk06Hand(int skillId, string raceName, string from, string to)
    {
        string key = skillId + "|" + raceName + "|" + from;
        if (!SpAtk06HandLogged.Add(key))
            return;

        Debug.Log(
            "[SkillAnimation] SpAtk06 maps to the shared hand gesture skillId=" + skillId +
            " race=" + raceName +
            " from=" + from +
            " to=" + to);
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
        int skillId = ActiveSkillId;
        string key = skillId + "|" + (slotName ?? "") + "|" + (raceName ?? "");
        if (!OverrideMissLogged.Add(key))
            return;

        DescribeBody(raceName, out string race, out string sex);
        string expected = ExpectedOverridePath(slotName, raceName);
        Debug.LogError(
            "[SkillAnimation] override unresolved skillId=" + skillId +
            " race=" + race +
            " sex=" + sex +
            " body=" + raceName +
            " slot=" + slotName +
            " expected=" + expected +
            " (SkillAnimationDatabase.GetOverrideClip; import clip or add trigger).");
    }

    public static void LogOverrideCompletionFallback(
        string requestedSlot,
        string fallbackSlot,
        string raceName,
        string loadedClipName)
    {
        int skillId = ActiveSkillId;
        string key = skillId + "|" + requestedSlot + ">" + fallbackSlot + "|" + raceName;
        if (!OverrideFallbackLogged.Add(key))
            return;

        DescribeBody(raceName, out string race, out string sex);
        string expected = ExpectedOverridePath(requestedSlot, raceName);
        Debug.LogError(
            "[SkillAnimation] override completion fallback skillId=" + skillId +
            " race=" + race +
            " sex=" + sex +
            " body=" + raceName +
            " expected=" + expected +
            " requested=" + requestedSlot +
            " used=" + fallbackSlot +
            " clip=" + loadedClipName +
            " (import the dedicated take; shared mid/shot is not the original).");
    }

    static string ExpectedOverridePath(string slotName, string raceName)
    {
        string path;
        if (SkillAnimationDatabase.TryGetOverrideResourcePath(slotName, raceName, out path)
            && !string.IsNullOrEmpty(path))
            return "Resources/" + path;
        return (slotName ?? "") + "_" + (raceName ?? "");
    }
}