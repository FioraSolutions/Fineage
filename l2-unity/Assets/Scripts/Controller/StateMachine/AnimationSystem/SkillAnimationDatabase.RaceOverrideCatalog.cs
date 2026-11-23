using System;
using System.Collections.Generic;
using UnityEngine;

public partial class SkillAnimationDatabase
{
    static readonly Dictionary<string, AnimationClip> RaceOverrideClipCache =
        new Dictionary<string, AnimationClip>(StringComparer.Ordinal);

    static readonly HashSet<string> RaceOverrideMissLogged = new HashSet<string>(StringComparer.Ordinal);

    /// <summary>Retail take stems resolvable as Data/Animations/.../{mesh}.ao_{Slot}_{Race}.</summary>
    public static readonly string[] CatalogOverrideSlots =
    {
        "CastLong",
        "CastShort",
        "CastMid",
        "CastEnd",
        "MagicShot",
        "MagicThrow",
        "MagicNoTarget",
        "MagicNotarget",
        "Swim",
        "SwimWait",
        "SwimDeath",
        "SwimDeathWait",
        "Fishing_end",
        "Fishing_fly",
        "Fishing_pull",
        "Fishing_wait",
        "Fishing_control",
        "ChairSit",
        "ChairWait",
        "ChairStand",
        "Wyvern",
        "Falling",
        "Damagefly",
    };

    public static IReadOnlyList<string> AllRaceTokensForCatalog() => AllRaceTokens;

    public static bool TryGetOverrideResourcePath(string slotName, string raceName, out string resourcesPath)
    {
        resourcesPath = null;
        if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(raceName))
            return false;

        RaceClipProfile profile = ResolveRaceProfile(raceName);
        if (profile == null)
            return false;

        string normalized = NormalizeOverrideSlotName(slotName);
        resourcesPath = profile.Folder + profile.MeshStem + ".ao_" + normalized + "_" + profile.Race;
        return true;
    }

    public static AnimationClip TryLoadRaceOverrideClip(string slotName, string raceName)
    {
        if (string.IsNullOrEmpty(slotName) || string.IsNullOrEmpty(raceName))
            return null;

        string normalizedSlot = NormalizeOverrideSlotName(slotName);
        string cacheKey = normalizedSlot + "|" + raceName;
        if (RaceOverrideClipCache.TryGetValue(cacheKey, out AnimationClip cached))
            return cached;

        RaceClipProfile profile = ResolveRaceProfile(raceName);
        if (profile == null)
        {
            LogOverrideClipMiss(slotName, raceName, normalizedSlot, null, "no race row in RaceClipTable");
            RaceOverrideClipCache[cacheKey] = null;
            return null;
        }

        AnimationClip loaded = LoadOverrideFromProfile(profile, normalizedSlot);
        if (loaded == null && !string.Equals(normalizedSlot, slotName, StringComparison.Ordinal))
            loaded = LoadOverrideFromProfile(profile, slotName);

        if (loaded == null)
        {
            string pathHint = profile.Folder + profile.MeshStem + ".ao_" + normalizedSlot + "_" + profile.Race;
            LogOverrideClipMiss(slotName, raceName, normalizedSlot, pathHint, "clip not in Resources");
        }

        RaceOverrideClipCache[cacheKey] = loaded;
        return loaded;
    }

    static AnimationClip LoadOverrideFromProfile(RaceClipProfile profile, string slotStem)
    {
        if (profile == null || string.IsNullOrEmpty(slotStem))
            return null;

        string[] stems = MeshStemsForProfile(profile);
        string[] slotForms = SlotStemVariants(slotStem);
        for (int s = 0; s < stems.Length; s++)
        {
            for (int f = 0; f < slotForms.Length; f++)
            {
                string resourceName = stems[s] + ".ao_" + slotForms[f] + "_" + profile.Race;
                string path = profile.Folder + resourceName;
                AnimationClip clip = Resources.Load<AnimationClip>(path);
                if (clip != null)
                    return clip;
            }
        }

        return null;
    }

    static string[] SlotStemVariants(string slotStem)
    {
        if (string.Equals(slotStem, "MagicNoTarget", StringComparison.Ordinal))
            return new[] { "MagicNoTarget", "MagicNotarget" };
        if (string.Equals(slotStem, "CastLong", StringComparison.Ordinal))
            return new[] { "CastLong", "castLong" };
        if (string.Equals(slotStem, "CastShort", StringComparison.Ordinal))
            return new[] { "CastShort", "castShort" };
        if (string.Equals(slotStem, "CastMid", StringComparison.Ordinal))
            return new[] { "CastMid", "castMid" };
        if (string.Equals(slotStem, "CastEnd", StringComparison.Ordinal))
            return new[] { "CastEnd", "castEnd" };
        return new[] { slotStem };
    }

    static string NormalizeOverrideSlotName(string slotName)
    {
        if (string.IsNullOrEmpty(slotName))
            return slotName;
        string trimmed = slotName.Trim();
        if (string.Equals(trimmed, "MagicNotarget", StringComparison.OrdinalIgnoreCase))
            return "MagicNoTarget";
        if (string.Equals(trimmed, "MagicNoTarget", StringComparison.OrdinalIgnoreCase))
            return "MagicNoTarget";
        if (string.Equals(trimmed, "castLong", StringComparison.OrdinalIgnoreCase))
            return "CastLong";
        if (string.Equals(trimmed, "castShort", StringComparison.OrdinalIgnoreCase))
            return "CastShort";
        if (string.Equals(trimmed, "castMid", StringComparison.OrdinalIgnoreCase))
            return "CastMid";
        if (string.Equals(trimmed, "castEnd", StringComparison.OrdinalIgnoreCase))
            return "CastEnd";
        return trimmed;
    }

    static void LogOverrideClipMiss(
        string requestedSlot,
        string raceName,
        string normalizedSlot,
        string expectedPath,
        string reason)
    {
        int skillId = SkillAnimationResolver.ActiveSkillId;
        string key = skillId + "|" + requestedSlot + "|" + raceName + "|" + normalizedSlot;
        if (!RaceOverrideMissLogged.Add(key))
            return;

        SkillAnimationResolver.DescribeBody(raceName, out string race, out string sex);
        Debug.LogError(
            "[SkillAnimation] override clip missing skillId=" + skillId
            + " race=" + race
            + " sex=" + sex
            + " body=" + raceName
            + " slot=" + requestedSlot
            + " normalized=" + normalizedSlot
            + (string.IsNullOrEmpty(expectedPath) ? "" : " expected=" + expectedPath)
            + " reason=" + reason + ".");
    }

    /// <summary>Clear catalog cache for EditMode.</summary>
    public static void ClearRaceOverrideClipCacheForTests()
    {
        RaceOverrideClipCache.Clear();
        RaceOverrideMissLogged.Clear();
    }
}