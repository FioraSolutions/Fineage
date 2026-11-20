using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    sealed class RaceClipProfile
    {
        public readonly string Folder;
        public readonly string MeshStem;
        public readonly string Race;

        public RaceClipProfile(string folder, string meshStem, string race)
        {
            Folder = folder;
            MeshStem = meshStem;
            Race = race;
        }
    }

    static readonly Dictionary<string, RaceClipProfile> SpAtkRaceProfiles =
        new Dictionary<string, RaceClipProfile>(StringComparer.OrdinalIgnoreCase);

    static readonly Dictionary<string, AnimationClip> SpAtkClipCache =
        new Dictionary<string, AnimationClip>(StringComparer.Ordinal);

    static readonly HashSet<string> SpAtkProfileMissLogged = new HashSet<string>(StringComparer.Ordinal);
    static readonly HashSet<string> SpAtkClipMissLogged = new HashSet<string>(StringComparer.Ordinal);

    static void EnsureSpAtkProfiles()
    {
        if (SpAtkRaceProfiles.Count > 0)
            return;

        SpAtkRaceProfiles[MFighter] = new RaceClipProfile(FOLDER_MFighter, "MFighter_m000_b", MFighter);
        SpAtkRaceProfiles[FFighter] = new RaceClipProfile(FOLDER_FFighter, "FFighter_m000_b", FFighter);
        SpAtkRaceProfiles[MMagic] = new RaceClipProfile(FOLDER_MMagic, "MMagic_M000_b", MMagic);
        SpAtkRaceProfiles[FMagic] = new RaceClipProfile(FOLDER_FMagic, "FMagic_m000_b", FMagic);
        SpAtkRaceProfiles[MElf] = new RaceClipProfile(FOLDER_MElf, "MElf_m000_b", MElf);
        SpAtkRaceProfiles[FElf] = new RaceClipProfile(FOLDER_FElf, "FElf_m000_b", FElf);
        SpAtkRaceProfiles[MDarkElf] = new RaceClipProfile(FOLDER_MDarkElf, "MDarkElf_m000_b", MDarkElf);
        SpAtkRaceProfiles[FDarkElf] = new RaceClipProfile(FOLDER_FDarkElf, "FDarkElf_m000_b", FDarkElf);
        SpAtkRaceProfiles[MDwarf] = new RaceClipProfile(FOLDER_MDwarf, "MDwarf_m000_b", MDwarf);
        SpAtkRaceProfiles[FDwarf] = new RaceClipProfile(FOLDER_FDwarf, "FDwarf_m001_b", FDwarf);
        SpAtkRaceProfiles[MOrc] = new RaceClipProfile(FOLDER_MOrc, "MOrc_m000_b", MOrc);
        SpAtkRaceProfiles[FOrc] = new RaceClipProfile(FOLDER_FOrc, "FOrc_m000_b", FOrc);
        SpAtkRaceProfiles[MShaman] = new RaceClipProfile(FOLDER_MShaman, "MShaman_m000_b", MShaman);
        SpAtkRaceProfiles[FShaman] = new RaceClipProfile(FOLDER_FShaman, "FShaman_m000_b", FShaman);
    }

    static RaceClipProfile ResolveSpAtkProfile(string raceName)
    {
        EnsureSpAtkProfiles();
        if (string.IsNullOrEmpty(raceName))
            return null;

        if (SpAtkRaceProfiles.TryGetValue(raceName, out RaceClipProfile exact))
            return exact;

        foreach (KeyValuePair<string, RaceClipProfile> pair in SpAtkRaceProfiles)
        {
            if (ContainsRaceToken(raceName, pair.Key))
                return pair.Value;
        }

        return null;
    }

    /// <summary>
    /// Animator state suffix is lowercase (dual, hand); imported clips use Dual, Hand, 1HS, …
    /// </summary>
    static string CanonicalClipWeaponStem(string weaponSuffix)
    {
        if (string.IsNullOrEmpty(weaponSuffix))
            return weaponSuffix;

        switch (weaponSuffix.ToLowerInvariant())
        {
            case "dual":
                return "Dual";
            case "hand":
                return "Hand";
            case "pole":
                return "Pole";
            case "bow":
                return "Bow";
            default:
                return weaponSuffix;
        }
    }

    static bool TrySplitSpAtkState(string stateName, out string spatkStem, out string weaponSuffix)
    {
        spatkStem = null;
        weaponSuffix = null;
        if (string.IsNullOrEmpty(stateName) ||
            stateName.IndexOf("SpAtk", StringComparison.OrdinalIgnoreCase) != 0)
            return false;

        int split = stateName.LastIndexOf('_');
        if (split <= 0)
            return false;

        spatkStem = stateName.Substring(0, split);
        weaponSuffix = stateName.Substring(split + 1);
        return true;
    }

    /// <summary>
    /// Loads a race-specific SpAtk swing when Player_Basic has no matching state
    /// (SpAtk03..SpAtk28). Resources path:
    /// Data/Animations/.../{mesh}.ao_SpAtk18_Dual_MFighter
    /// </summary>
    public static AnimationClip GetSpAtkCombatClip(string stateName, string raceName)
    {
        if (!TrySplitSpAtkState(stateName, out string spatkStem, out string weaponSuffix))
            return null;

        RaceClipProfile profile = ResolveSpAtkProfile(raceName);
        if (profile == null)
        {
            if (SpAtkProfileMissLogged.Add(raceName ?? "<null>"))
            {
                Debug.LogError(
                    "[SkillAnimation] SpAtk profile missing raceName=" + raceName
                    + " state=" + stateName + " (no race clip folder in SpAtkRaceProfiles).");
            }

            return null;
        }

        string weaponStem = CanonicalClipWeaponStem(weaponSuffix);
        string clipStateStem = spatkStem + "_" + weaponStem;
        string cacheKey = clipStateStem + "_" + profile.Race;
        if (SpAtkClipCache.TryGetValue(cacheKey, out AnimationClip cached))
            return cached;

        string[] weaponVariants =
        {
            weaponStem,
            weaponSuffix,
            CanonicalClipWeaponStem(weaponSuffix)
        };

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
                + " state=" + stateName + " folder=" + profile.Folder
                + " meshStem=" + profile.MeshStem + ".");
        }

        SpAtkClipCache[cacheKey] = loaded;
        return loaded;
    }
}