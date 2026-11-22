using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    static readonly Dictionary<string, string> SpAtkWeaponClipStem =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "hand", "Hand" },
            { "1HS", "1HS" },
            { "2HS", "2HS" },
            { "pole", "Pole" },
            { "dual", "Dual" },
            { "bow", "Bow" },
        };

    static readonly Dictionary<string, AnimationClip> SpAtkClipCache =
        new Dictionary<string, AnimationClip>(StringComparer.Ordinal);

    static readonly HashSet<string> SpAtkProfileMissLogged = new HashSet<string>(StringComparer.Ordinal);
    static readonly HashSet<string> SpAtkClipMissLogged = new HashSet<string>(StringComparer.Ordinal);

    public static IReadOnlyList<string> AllSpAtkRaceTokens() => AllRaceTokens;

    public static IReadOnlyDictionary<string, string> SpAtkWeaponSuffixMap() => SpAtkWeaponClipStem;

    public static bool IsMixComboTag(string tag) =>
        !string.IsNullOrEmpty(tag) &&
        tag.IndexOf("Mix", StringComparison.OrdinalIgnoreCase) >= 0;

    /// <summary>
    /// Non-<c>none</c> SpAtk phase tokens from an AnimationCombo row, in column order (anim_name → anim_name2 → anime_name3).
    /// Mix tags therefore yield up to three swings in retail playback order.
    /// </summary>
    public static string[] GetOrderedSpAtkPhases(AnimationCombo combo)
    {
        if (combo == null)
            return Array.Empty<string>();

        string[] cycle = combo.GetAnimCycle();
        if (cycle == null || cycle.Length == 0)
            return Array.Empty<string>();

        var phases = new List<string>();
        for (int i = 0; i < cycle.Length; i++)
        {
            string token = cycle[i];
            if (string.IsNullOrWhiteSpace(token))
                continue;
            token = token.Trim();
            if (string.Equals(token, "none", StringComparison.OrdinalIgnoreCase))
                continue;
            if (token.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) < 0)
                continue;
            // Ready/loop/shot charge combos (SpAtk_ready01…) are not swing indices.
            int spAtkAt = token.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase);
            int digitAt = spAtkAt + 5;
            if (digitAt >= token.Length || !char.IsDigit(token[digitAt]))
                continue;
            phases.Add(token);
        }

        return phases.ToArray();
    }

    public static bool TryBuildSpAtkStateName(string comboPhaseToken, PlayerAnimWeapon weapon, out string stateName)
    {
        string trigger = comboPhaseToken + ToWeaponTriggerSuffix(weapon);
        return PlayerSpAtkAnim.TryResolve(trigger, out stateName);
    }

    /// <summary>Resources path without extension, for EditMode coverage checks (#1620).</summary>
    public static bool TryGetSpAtkCombatResourcePath(string stateName, string raceName, out string resourcesPath)
    {
        resourcesPath = null;
        if (!PlayerSpAtkAnim.TryResolve(stateName, out string normalizedState))
            return false;
        if (!TrySplitSpAtkState(normalizedState, out string spatkStem, out string weaponToken))
            return false;

        RaceClipProfile profile = ResolveRaceProfile(raceName);
        if (profile == null)
            return false;

        string weaponStem = CanonicalClipWeaponStem(weaponToken);
        string stem = spatkStem + "_" + weaponStem;
        resourcesPath = profile.Folder + profile.MeshStem + ".ao_" + stem + "_" + profile.Race;
        return true;
    }

    static string ToWeaponTriggerSuffix(PlayerAnimWeapon weapon)
    {
        switch (weapon)
        {
            case PlayerAnimWeapon.Hand: return "_hand";
            case PlayerAnimWeapon.OneHS: return "_1HS";
            case PlayerAnimWeapon.TwoHS: return "_2HS";
            case PlayerAnimWeapon.Pole: return "_pole";
            case PlayerAnimWeapon.Dual: return "_dual";
            case PlayerAnimWeapon.Bow: return "_bow";
            default: return "_hand";
        }
    }

    static string CanonicalClipWeaponStem(string weaponSuffix)
    {
        if (string.IsNullOrEmpty(weaponSuffix))
            return weaponSuffix;

        if (SpAtkWeaponClipStem.TryGetValue(weaponSuffix, out string mapped))
            return mapped;

        return weaponSuffix;
    }

    static bool TrySplitSpAtkState(string stateName, out string spatkStem, out string weaponSuffix)
    {
        spatkStem = null;
        weaponSuffix = null;
        if (!PlayerSpAtkAnim.TryParse(stateName, out int number, out PlayerAnimWeapon weapon, out _))
            return false;

        spatkStem = "SpAtk" + number.ToString("00");
        weaponSuffix = PlayerAnimStateMapBase.ToWeaponSuffix(weapon);
        return true;
    }

    /// <summary>
    /// Loads a race-specific SpAtk swing when Player_Basic has no matching state
    /// (SpAtk03..SpAtk28). Resources path:
    /// Data/Animations/.../{mesh}.ao_SpAtk18_Dual_MFighter
    /// </summary>
    public static AnimationClip GetSpAtkCombatClip(string stateName, string raceName)
    {
        if (!PlayerSpAtkAnim.TryResolve(stateName, out string normalizedState) &&
            !PlayerSpAtkAnim.TryParse(stateName, out _, out _, out normalizedState))
            return null;

        if (!TrySplitSpAtkState(normalizedState, out string spatkStem, out string weaponToken))
            return null;

        PlayerSpAtkAnim.TryParse(normalizedState, out int spAtkNumber, out _, out _);

        RaceClipProfile profile = ResolveRaceProfile(raceName);
        if (profile == null)
        {
            if (SpAtkProfileMissLogged.Add(raceName ?? "<null>"))
            {
                Debug.LogError(
                    "[SkillAnimation] SpAtk profile missing raceName=" + raceName
                    + " state=" + normalizedState + " (no row in RaceClipTable).");
            }

            return null;
        }

        string weaponStem = CanonicalClipWeaponStem(weaponToken);
        string clipStateStem = spatkStem + "_" + weaponStem;
        string cacheKey = clipStateStem + "_" + profile.Race;
        if (SpAtkClipCache.TryGetValue(cacheKey, out AnimationClip cached))
            return cached;

        AnimationClip loaded = InterludeAnimationClipResolver.TryLoadPlayerSpAtkCombat(
            raceName,
            spAtkNumber,
            weaponToken,
            normalizedState);

        if (loaded == null)
        {
            string[] meshStems = MeshStemsForRaceProfile(profile);
            for (int i = 0; i < meshStems.Length; i++)
            {
                string legacyPath = profile.Folder + meshStems[i] + ".ao_" + clipStateStem + "_" + profile.Race;
                loaded = InterludeAnimationClipResolver.LoadExact(legacyPath);
                if (loaded != null)
                    break;
            }
        }

        if (loaded == null && SpAtkClipMissLogged.Add(cacheKey))
        {
            string query = InterludeAnimationClipResolver.BuildPlayerSpAtkQuery(
                spAtkNumber,
                weaponToken,
                profile.Race);
            Debug.LogError(
                "[SkillAnimation] SpAtk clip missing race=" + profile.Race
                + " state=" + normalizedState
                + " animTweenQuery=" + query
                + " folder=" + profile.Folder
                + " meshStem=" + profile.MeshStem
                + " (re-bake missing takes on Mac; MFighter dual list in Docs/MISSING_MFighter_DUAL_SPATK.md).");
        }

        SpAtkClipCache[cacheKey] = loaded;
        return loaded;
    }
}