using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Interlude player body mesh (race×sex×fighter/mystic) → hand locomotion clips and stride data.
/// Clips are the .ukx-derived Resources assets (no per-race if-chains outside this table).
/// </summary>
public static class PlayerLocomotionBodyCatalog
{
    public const string LogTag = "[PlayerLocomotionBody]";

    /// <summary>Default Interlude humanoid run/walk from packet baselines (GenderModel-style).</summary>
    public const float DefaultRunSpeedL2j = 120f;
    public const float DefaultWalkSpeedL2j = 80f;

    public readonly struct Row
    {
        public Row(
            CharacterRaceAnimation profile,
            string runHandClipResources,
            string walkHandClipResources,
            string waitHandClipResources,
            string playerPrefabResources)
        {
            Profile = profile;
            RunHandClipResources = runHandClipResources;
            WalkHandClipResources = walkHandClipResources;
            WaitHandClipResources = waitHandClipResources;
            PlayerPrefabResources = playerPrefabResources;
        }

        public CharacterRaceAnimation Profile { get; }
        /// <summary>Path under Resources/ without extension.</summary>
        public string RunHandClipResources { get; }
        public string WalkHandClipResources { get; }
        public string WaitHandClipResources { get; }
        public string PlayerPrefabResources { get; }
    }

    const string AuthoritativeStridesRelativePath = "Data/Meta/player_locomotion_strides.json";

    static readonly Dictionary<CharacterRaceAnimation, Row> ByProfile = Build();
    static readonly Dictionary<CharacterRaceAnimation, float> RunStrideCache = new();
    static readonly Dictionary<CharacterRaceAnimation, float> WalkStrideCache = new();
    static readonly HashSet<string> Logged = new();
    static bool _authoritativeStridesLoaded;
    static readonly Dictionary<CharacterRaceAnimation, float> AuthoritativeRunStride = new();
    static readonly Dictionary<CharacterRaceAnimation, float> AuthoritativeWalkStride = new();

    static Dictionary<CharacterRaceAnimation, Row> Build()
    {
        var d = new Dictionary<CharacterRaceAnimation, Row>();
        Register(d, CharacterRaceAnimation.MElf,
            "Data/Animations/Elf/MElf/Clips/MElf_m000_b.ao_Run_Hand_MElf",
            "Data/Animations/Elf/MElf/Clips/MElf_m000_b.ao_Walk_Hand_MElf",
            "Data/Animations/Elf/MElf/Clips/MElf_m000_b.ao_wait_Hand_MElf",
            "Data/Animations/Elf/MElf/Player_MElf");
        Register(d, CharacterRaceAnimation.FElf,
            "Data/Animations/Elf/FElf/Clips/FElf_m000_b.ao_Run_Hand_FElf",
            "Data/Animations/Elf/FElf/Clips/FElf_m000_b.ao_Walk_Hand_FElf",
            "Data/Animations/Elf/FElf/Clips/FElf_m000_b.ao_wait_Hand_FElf",
            "Data/Animations/Elf/FElf/Player_FElf");
        Register(d, CharacterRaceAnimation.MDarkElf,
            "Data/Animations/DarkElf/MDarkElf/Clips/MDarkElf_m000_b.ao_Run_Hand_MDarkElf",
            "Data/Animations/DarkElf/MDarkElf/Clips/MDarkElf_m000_b.ao_Walk_Hand_MDarkElf",
            "Data/Animations/DarkElf/MDarkElf/Clips/MDarkElf_m000_b.ao_wait_Hand_MDarkElf",
            "Data/Animations/DarkElf/MDarkElf/Player_MDarkElf");
        Register(d, CharacterRaceAnimation.FDarkElf,
            "Data/Animations/DarkElf/FDarkElf/Clips/FDarkElf_m000_b.ao_run_Hand_FDarkElf",
            "Data/Animations/DarkElf/FDarkElf/Clips/FDarkElf_m000_b.ao_walk_Hand_FDarkElf",
            "Data/Animations/DarkElf/FDarkElf/Clips/FDarkElf_m000_b.ao_wait_Hand_FDarkElf",
            "Data/Animations/DarkElf/FDarkElf/Player_FDarkElf");
        Register(d, CharacterRaceAnimation.MDwarf,
            "Data/Animations/Dwarf/MDwarf/Clips/MDwarf_m000_b.ao_Run_Hand_MDwarf",
            "Data/Animations/Dwarf/MDwarf/Clips/MDwarf_m000_b.ao_Walk_Hand_MDwarf",
            "Data/Animations/Dwarf/MDwarf/Clips/MDwarf_m000_b.ao_wait_Hand_MDwarf",
            "Data/Animations/Dwarf/MDwarf/Player_MDwarf");
        Register(d, CharacterRaceAnimation.FDwarf,
            "Data/Animations/Dwarf/FDwarf/Clips/FDwarf_m001_b.ao_run_Hand_FDwarf",
            "Data/Animations/Dwarf/FDwarf/Clips/FDwarf_m001_b.ao_walk_Hand_FDwarf",
            "Data/Animations/Dwarf/FDwarf/Clips/FDwarf_m001_b.ao_wait_Hand_FDwarf",
            "Data/Animations/Dwarf/FDwarf/Player_FDwarf");
        Register(d, CharacterRaceAnimation.MOrc,
            "Data/Animations/Orc/MOrc/Clips/MOrc_m000_b.ao_Run_Hand_MOrc",
            "Data/Animations/Orc/MOrc/Clips/MOrc_m000_b.ao_Walk_Hand_MOrc",
            "Data/Animations/Orc/MOrc/Clips/MOrc_m000_b.ao_Wait_Hand_MOrc",
            "Data/Animations/Orc/MOrc/Player_MOrc");
        Register(d, CharacterRaceAnimation.FOrc,
            "Data/Animations/Orc/FOrc/Clips/FOrc_m000_b.ao_Run_Hand_FOrc",
            "Data/Animations/Orc/FOrc/Clips/FOrc_m000_b.ao_Walk_Hand_FOrc",
            "Data/Animations/Orc/FOrc/Clips/FOrc_m000_b.ao_Wait_Hand_FOrc",
            "Data/Animations/Orc/FOrc/Player_FOrc");
        Register(d, CharacterRaceAnimation.MShaman,
            "Data/Animations/Orc/MShaman/Clips/MShaman_m000_b.ao_Run_Hand_MShaman",
            "Data/Animations/Orc/MShaman/Clips/MShaman_m000_b.ao_Walk_Hand_MShaman",
            "Data/Animations/Orc/MShaman/Clips/MShaman_m000_b.ao_Wait_Hand_MShaman",
            "Data/Animations/Orc/MShaman/Player_MShaman");
        Register(d, CharacterRaceAnimation.FShaman,
            "Data/Animations/Orc/FShaman/Clips/FShaman_m000_b.ao_Run_Hand_FShaman",
            "Data/Animations/Orc/FShaman/Clips/FShaman_m000_b.ao_Walk_Hand_FShaman",
            "Data/Animations/Orc/FShaman/Clips/FShaman_m000_b.ao_Wait_Hand_FShaman",
            "Data/Animations/Orc/FShaman/Player_FShaman");
        Register(d, CharacterRaceAnimation.MMagic,
            "Data/Animations/Magic/MMagic/Clips/MMagic_M000_b.ao_run_Hand_MMagic",
            "Data/Animations/Magic/MMagic/Clips/MMagic_M000_b.ao_walk_Hand_MMagic",
            "Data/Animations/Magic/MMagic/Clips/MMagic_M000_b.ao_wait_Hand_MMagic",
            "Data/Animations/Human/MMagic/Player_MMagic");
        Register(d, CharacterRaceAnimation.FMagic,
            "Data/Animations/Magic/FMagic/Clips/FMagic_m000_b.ao_Run_Hand_FMagic",
            "Data/Animations/Magic/FMagic/Clips/FMagic_m000_b.ao_Walk_Hand_FMagic",
            "Data/Animations/Magic/FMagic/Clips/FMagic_m000_b.ao_wait_Hand_FMagic",
            "Data/Animations/Human/FMagic/Player_FMagic");
        Register(d, CharacterRaceAnimation.MFighter,
            "Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_run_Hand_MFighter",
            "Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_walk_Hand_MFighter",
            "Data/Animations/Fighter/MFighter/Clips/MFighter_m000_b.ao_wait_Hand_MFighter",
            "Data/Animations/Human/MFighter/Player_MFighter");
        Register(d, CharacterRaceAnimation.FFighter,
            "Data/Animations/Fighter/FFighter/Clips/FFighter_m000_b.ao_run_Hand_FFighter",
            "Data/Animations/Fighter/FFighter/Clips/FFighter_m000_b.ao_Walk_Hand_FFighter",
            "Data/Animations/Fighter/FFighter/Clips/FFighter_m000_b.ao_wait_Hand_FFighter",
            "Data/Animations/Human/FFighter/Player_FFighter");
        return d;
    }

    static void Register(
        Dictionary<CharacterRaceAnimation, Row> d,
        CharacterRaceAnimation profile,
        string run,
        string walk,
        string wait,
        string prefab)
    {
        d[profile] = new Row(profile, run, walk, wait, prefab);
    }

    public static IEnumerable<Row> AllRows()
    {
        foreach (Row row in ByProfile.Values)
            yield return row;
    }

    public static bool TryGet(CharacterRaceAnimation profile, out Row row) =>
        ByProfile.TryGetValue(profile, out row);

    public static AnimationClip LoadHandClip(CharacterRaceAnimation profile, bool walk)
    {
        if (!TryGet(profile, out Row row))
        {
            LogMissingProfile(profile, "clip");
            return null;
        }

        string path = walk ? row.WalkHandClipResources : row.RunHandClipResources;
        return LoadClip(path, profile);
    }

    public static AnimationClip LoadWaitClip(CharacterRaceAnimation profile) =>
        TryGet(profile, out Row row) ? LoadClip(row.WaitHandClipResources, profile) : null;

    static AnimationClip LoadClip(string resourcesPath, CharacterRaceAnimation profile)
    {
        if (string.IsNullOrEmpty(resourcesPath))
        {
            LogMissing(profile, "empty Resources path");
            return null;
        }

        var clip = Resources.Load<AnimationClip>(resourcesPath);
        if (clip == null)
            LogMissing(profile, "missing Resources clip \"" + resourcesPath + "\"");

        return clip;
    }

    public static Transform LoadMeasureSkeleton(CharacterRaceAnimation profile)
    {
        if (!TryGet(profile, out Row row))
            return null;

        var prefab = Resources.Load<GameObject>(row.PlayerPrefabResources);
        if (prefab == null)
        {
            LogMissing(profile, "missing player prefab \"" + row.PlayerPrefabResources + "\" for stride measure");
            return null;
        }

        GameObject instance = UnityEngine.Object.Instantiate(prefab);
        instance.hideFlags = HideFlags.HideAndDontSave;
        return instance.transform;
    }

    public static void DestroyMeasureSkeleton(Transform skeleton)
    {
        if (skeleton != null)
            UnityEngine.Object.DestroyImmediate(skeleton.gameObject);
    }

    public static float ResolveRunStride(CharacterRaceAnimation profile, AnimationClip clip, Transform skeleton)
    {
        if (RunStrideCache.TryGetValue(profile, out float cached))
            return cached;

        float stride = Measure(profile, clip, skeleton, walk: false);
        RunStrideCache[profile] = stride;
        return stride;
    }

    public static float ResolveWalkStride(CharacterRaceAnimation profile, AnimationClip clip, Transform skeleton)
    {
        if (WalkStrideCache.TryGetValue(profile, out float cached))
            return cached;

        float stride = Measure(profile, clip, skeleton, walk: true);
        WalkStrideCache[profile] = stride;
        return stride;
    }

    static float Measure(CharacterRaceAnimation profile, AnimationClip clip, Transform skeleton, bool walk)
    {
        if (clip == null)
        {
            if (!TryGet(profile, out Row row))
            {
                LogMissingProfile(profile, "stride");
                return 0f;
            }

            clip = LoadClip(walk ? row.WalkHandClipResources : row.RunHandClipResources, profile);
        }

        if (clip == null || clip.length < 0.01f)
            return 0f;

        bool ownedSkeleton = skeleton == null;
        if (ownedSkeleton)
            skeleton = LoadMeasureSkeleton(profile);

        if (skeleton == null)
            return 0f;

        try
        {
            float stride = LocomotionGait.MeasureStride(clip, skeleton);
            if (stride < LocomotionGait.MinStrideMeters
                && TryGetAuthoritativeStride(profile, walk, out float authoritative))
            {
                return authoritative;
            }

            if (stride < LocomotionGait.MinStrideMeters)
            {
                LogMissing(
                    profile,
                    "unmeasurable " + (walk ? "walk" : "run") + " stride race=" + profile + " clip=\""
                    + clip.name + "\" (feet/root travel below " + LocomotionGait.MinStrideMeters + "m)");
                return 0f;
            }

            return stride;
        }
        finally
        {
            if (ownedSkeleton)
                DestroyMeasureSkeleton(skeleton);
        }
    }

    /// <summary>
    /// Confirm idle state uses the catalog wait_Hand clip (not Stand / empty slot).
    /// </summary>
    public static void ValidateWaitHandOverride(CharacterRaceAnimation profile, Animator animator, int entityId)
    {
        if (animator == null || !TryGet(profile, out Row row))
            return;

        AnimationClip expected = LoadWaitClip(profile);
        if (expected == null)
            return;

        AnimationClip bound = LocomotionGait.FindClip(animator, "wait_hand");
        if (bound == null)
        {
            LogIdleOnce(profile, entityId, "animator has no wait_hand state clip");
            return;
        }

        if (!ClipMatchesIdle(expected, bound))
        {
            LogIdleOnce(
                profile,
                entityId,
                "wait_hand override=\"" + bound.name + "\" expected catalog=\"" + expected.name
                + "\" Resources=\"" + row.WaitHandClipResources + "\"");
        }
    }

    static bool ClipMatchesIdle(AnimationClip expected, AnimationClip bound)
    {
        if (bound == null || expected == null)
            return false;
        if (string.Equals(bound.name, expected.name, StringComparison.OrdinalIgnoreCase))
            return true;

        string e = expected.name.ToLowerInvariant();
        string b = bound.name.ToLowerInvariant();
        return b.Contains("wait_hand") && !b.Contains("stand") && e.Contains("wait") && !e.Contains("stand");
    }

    public static void ClearStrideCacheForTests()
    {
        RunStrideCache.Clear();
        WalkStrideCache.Clear();
        Logged.Clear();
    }

    static bool TryGetAuthoritativeStride(CharacterRaceAnimation profile, bool walk, out float meters)
    {
        EnsureAuthoritativeStridesLoaded();
        if (walk)
            return AuthoritativeWalkStride.TryGetValue(profile, out meters);
        return AuthoritativeRunStride.TryGetValue(profile, out meters);
    }

    static void EnsureAuthoritativeStridesLoaded()
    {
        if (_authoritativeStridesLoaded)
            return;
        _authoritativeStridesLoaded = true;
        AuthoritativeRunStride.Clear();
        AuthoritativeWalkStride.Clear();

        string path = Path.Combine(Application.streamingAssetsPath, AuthoritativeStridesRelativePath);
        if (!File.Exists(path))
            return;

        try
        {
            string json = File.ReadAllText(path);
            var root = JsonUtility.FromJson<AuthoritativeStridesRootDto>(json);
            if (root?.profiles == null)
                return;

            foreach (AuthoritativeStrideRowDto row in root.profiles)
            {
                if (row == null || string.IsNullOrEmpty(row.profile))
                    continue;
                if (!Enum.TryParse(row.profile, true, out CharacterRaceAnimation profile))
                    continue;
                if (row.runMeters >= LocomotionGait.MinStrideMeters)
                    AuthoritativeRunStride[profile] = row.runMeters;
                if (row.walkMeters >= LocomotionGait.MinStrideMeters)
                    AuthoritativeWalkStride[profile] = row.walkMeters;
            }
        }
        catch (Exception ex)
        {
            FineageLog.Error(
                FineageLogCategory.World,
                LogTag + " failed to load authoritative strides: " + ex.Message);
        }
    }

    [Serializable]
    sealed class AuthoritativeStridesRootDto
    {
        public AuthoritativeStrideRowDto[] profiles;
    }

    [Serializable]
    sealed class AuthoritativeStrideRowDto
    {
        public string profile;
        public float runMeters;
        public float walkMeters;
    }

    public static void LogStrideFallback(CharacterRaceAnimation profile, bool walk, float measured)
    {
        string key = profile + "@fallback@" + (walk ? "walk" : "run") + "@" + measured.ToString("F3");
        if (!Logged.Add(key))
            return;

        FineageLog.Error(
            FineageLogCategory.World,
            LogTag + " profile=" + profile + " using FallbackStrideMeters="
            + LocomotionGait.FallbackStrideMeters + " for " + (walk ? "walk" : "run")
            + " (measured=" + measured.ToString("F3") + "m)");
    }

    static void LogMissingProfile(CharacterRaceAnimation profile, string field)
    {
        string key = profile + "@" + field;
        if (!Logged.Add(key))
            return;

        FineageLog.Error(
            FineageLogCategory.World,
            LogTag + " missing body row profile=" + profile + " field=" + field);
    }

    static void LogMissing(CharacterRaceAnimation profile, string detail)
    {
        string key = profile + "#" + detail;
        if (!Logged.Add(key))
            return;

        FineageLog.Error(FineageLogCategory.World, LogTag + " profile=" + profile + " " + detail);
    }

    static void LogIdleOnce(CharacterRaceAnimation profile, int entityId, string detail)
    {
        string key = profile + "@idle@" + entityId + "@" + detail;
        if (!Logged.Add(key))
            return;

        FineageLog.Error(
            FineageLogCategory.World,
            LogTag + " idle wait_hand entityId=" + entityId + " profile=" + profile + " " + detail);
    }
}