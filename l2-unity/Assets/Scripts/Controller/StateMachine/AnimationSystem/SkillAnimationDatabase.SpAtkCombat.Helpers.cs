using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    sealed class RaceClipProfile
    {
        public readonly string Token;
        public readonly string Folder;
        public readonly string MeshStem;
        public readonly string Race;

        public RaceClipProfile(string token, string folder, string meshStem, string race)
        {
            Token = token;
            Folder = folder;
            MeshStem = meshStem;
            Race = race;
        }
    }

    /// <summary>Races in lookup order: longer tokens before shorter (FDarkElf before FElf, …).</summary>
    static readonly RaceClipProfile[] SpAtkRaceTable =
    {
        new RaceClipProfile(FDarkElf, FOLDER_FDarkElf, "FDarkElf_m000_b", FDarkElf),
        new RaceClipProfile(MDarkElf, FOLDER_MDarkElf, "MDarkElf_m000_b", MDarkElf),
        new RaceClipProfile(FShaman, FOLDER_FShaman, "FShaman_m000_b", FShaman),
        new RaceClipProfile(MShaman, FOLDER_MShaman, "MShaman_m000_b", MShaman),
        new RaceClipProfile(FFighter, FOLDER_FFighter, "FFighter_m000_b", FFighter),
        new RaceClipProfile(MFighter, FOLDER_MFighter, "MFighter_m000_b", MFighter),
        new RaceClipProfile(FMagic, FOLDER_FMagic, "FMagic_m000_b", FMagic),
        new RaceClipProfile(MMagic, FOLDER_MMagic, "MMagic_M000_b", MMagic),
        new RaceClipProfile(FElf, FOLDER_FElf, "FElf_m000_b", FElf),
        new RaceClipProfile(MElf, FOLDER_MElf, "MElf_m000_b", MElf),
        new RaceClipProfile(FDwarf, FOLDER_FDwarf, "FDwarf_m001_b", FDwarf),
        new RaceClipProfile(MDwarf, FOLDER_MDwarf, "MDwarf_m000_b", MDwarf),
        new RaceClipProfile(FOrc, FOLDER_FOrc, "FOrc_m000_b", FOrc),
        new RaceClipProfile(MOrc, FOLDER_MOrc, "MOrc_m000_b", MOrc),
    };

    static readonly string[] SpAtkRaceTokens =
    {
        MFighter, FFighter, MMagic, FMagic, MElf, FElf, MDarkElf, FDarkElf,
        MDwarf, FDwarf, MOrc, FOrc, MShaman, FShaman,
    };

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

    public static IReadOnlyList<string> AllSpAtkRaceTokens() => SpAtkRaceTokens;

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
            if (!PlayerSpAtkAnim.TryParse(token + "_1HS", out _, out _, out _))
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

        RaceClipProfile profile = ResolveSpAtkProfile(raceName);
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

    static RaceClipProfile ResolveSpAtkProfile(string raceName)
    {
        if (string.IsNullOrEmpty(raceName))
            return null;

        for (int i = 0; i < SpAtkRaceTable.Length; i++)
        {
            RaceClipProfile row = SpAtkRaceTable[i];
            if (string.Equals(raceName, row.Race, StringComparison.OrdinalIgnoreCase))
                return row;
        }

        for (int i = 0; i < SpAtkRaceTable.Length; i++)
        {
            RaceClipProfile row = SpAtkRaceTable[i];
            if (ContainsRaceToken(raceName, row.Token))
                return row;
        }

        return null;
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

        RaceClipProfile profile = ResolveSpAtkProfile(raceName);
        if (profile == null)
        {
            if (SpAtkProfileMissLogged.Add(raceName ?? "<null>"))
            {
                Debug.LogError(
                    "[SkillAnimation] SpAtk profile missing raceName=" + raceName
                    + " state=" + normalizedState + " (no row in SpAtkRaceTable).");
            }

            return null;
        }

        string weaponStem = CanonicalClipWeaponStem(weaponToken);
        string clipStateStem = spatkStem + "_" + weaponStem;
        string cacheKey = clipStateStem + "_" + profile.Race;
        if (SpAtkClipCache.TryGetValue(cacheKey, out AnimationClip cached))
            return cached;

        string[] weaponVariants = { weaponStem, weaponToken };

        AnimationClip loaded = null;
        for (int i = 0; i < weaponVariants.Length && loaded == null; i++)
        {
            if (string.IsNullOrEmpty(weaponVariants[i]))
                continue;

            string stem = spatkStem + "_" + weaponVariants[i];
            string resourceName = profile.MeshStem + ".ao_" + stem + "_" + profile.Race;
            string path = profile.Folder + resourceName;
            loaded = Resources.Load<AnimationClip>(path);
        }

        if (loaded == null && SpAtkClipMissLogged.Add(cacheKey))
        {
            Debug.LogError(
                "[SkillAnimation] SpAtk clip missing race=" + profile.Race
                + " state=" + normalizedState
                + " spatk=" + spAtkNumber.ToString("00")
                + " weapon=" + weaponToken
                + " expectedStem=" + clipStateStem
                + " folder=" + profile.Folder
                + " meshStem=" + profile.MeshStem + ".");
        }

        SpAtkClipCache[cacheKey] = loaded;
        return loaded;
    }
}