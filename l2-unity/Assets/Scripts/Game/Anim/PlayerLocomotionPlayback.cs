using System;
using UnityEngine;

/// <summary>
/// One locomotion playback path for every player race/sex/weapon clip and every NPC.
/// Rate = (current speed / pawn GroundSpeed) * (Unity clip length / ukx cycle length).
/// </summary>
public static class PlayerLocomotionPlayback
{
    public static void ResolvePlaybackRates(
        Entity entity,
        Animator animator,
        string runClipName,
        string walkClipName,
        float unityRunMetersPerSecond,
        float unityWalkMetersPerSecond,
        float authoredRunMetersPerSecond,
        float authoredWalkMetersPerSecond,
        out float runRate,
        out float walkRate)
    {
        string subject = Describe(entity);
        runRate = ResolveOne(
            entity,
            animator,
            runClipName,
            walk: false,
            unityRunMetersPerSecond,
            authoredRunMetersPerSecond,
            subject,
            LocomotionGait.MinRunRate);
        walkRate = ResolveOne(
            entity,
            animator,
            walkClipName,
            walk: true,
            unityWalkMetersPerSecond,
            authoredWalkMetersPerSecond,
            subject,
            LocomotionGait.MinWalkRate);
    }

    public static string Describe(Entity entity)
    {
        if (entity is PlayerEntity || entity is UserEntity)
        {
            CharacterRaceAnimation profile = entity.RaceId;
            string name = profile.ToString();
            string sex = name.Length > 0 && name[0] == 'F' ? "F" : "M";
            string race = name.Length > 1 ? name.Substring(1) : name;
            return "race=" + race + " sex=" + sex + " profile=" + name;
        }

        string npc = "npc";
        int id = 0;
        if (entity != null && entity.Identity != null)
        {
            if (!string.IsNullOrEmpty(entity.Identity.Name))
                npc = entity.Identity.Name;
            id = entity.Identity.Id;
        }

        return "npc=\"" + npc + "\" id=" + id;
    }

    static float ResolveOne(
        Entity entity,
        Animator animator,
        string clipName,
        bool walk,
        float unityMetersPerSecond,
        float authoredMetersPerSecond,
        string subject,
        float minRate)
    {
        string gait = walk ? "walk" : "run";
        AnimationClip clip = LocomotionGait.FindClip(animator, clipName);
        if (clip == null && IsPlayerBody(entity))
            clip = ResolvePlayerClip(entity.RaceId, clipName, walk);

        if (clip == null)
        {
            LocomotionGait.LogMissingLocomotionClip(animator, clipName, subject);
            LogClock(subject, gait, clipName, "missing clip");
            return 0f;
        }

        if (!TryOfficialCycle(clip.name, out float officialSeconds, out string clockError))
        {
            LogClock(subject, gait, clip.name, clockError);
            return 0f;
        }

        if (authoredMetersPerSecond <= 0.001f)
        {
            LogClock(subject, gait, clip.name, "missing GroundSpeed (BaseRun/BaseWalk is 0)");
            return 0f;
        }

        return LocomotionGait.PlaybackRateForSequence(
            unityMetersPerSecond,
            authoredMetersPerSecond,
            clip.length,
            officialSeconds,
            minRate,
            LocomotionGait.MaxRate);
    }

    static bool TryOfficialCycle(string clipName, out float officialSeconds, out string error)
    {
        officialSeconds = 0f;
        error = null;
        if (!AnimNotifySequenceResolver.TryResolve(clipName, null, out string sequence, out error))
            return false;
        if (!AnimNotifyTable.TryGetBySequence(sequence, out var bindings) || bindings.Count == 0)
        {
            error = "no notify row for sequence=\"" + sequence + "\"";
            return false;
        }

        float rate = bindings[0].Rate;
        int frames = bindings[0].FrameCount;
        if (rate <= 0f || frames <= 0)
        {
            error = "sequence=\"" + sequence + "\" missing rate=" + rate.ToString("G9") + " frames=" + frames;
            return false;
        }

        officialSeconds = frames / rate;
        return true;
    }

    static void LogClock(string subject, string gait, string clipName, string detail)
    {
        string key = (subject ?? "") + "|" + (gait ?? "") + "|" + (clipName ?? "") + "|" + (detail ?? "");
        if (!_loggedClock.Add(key))
            return;
        Debug.LogError(
            "[LocomotionClock] " + (string.IsNullOrEmpty(subject) ? "subject=unknown" : subject)
            + " gait=" + gait
            + " clip=\"" + (clipName ?? "") + "\" "
            + (detail ?? "missing sequence clock"));
    }

    static readonly System.Collections.Generic.HashSet<string> _loggedClock =
        new System.Collections.Generic.HashSet<string>();

    static bool IsPlayerBody(Entity entity) =>
        entity is PlayerEntity || entity is UserEntity;

    /// <summary>
    /// Animator overrides name the state (run_1HS). When the animator is not
    /// bound yet, load that weapon clip from the body catalog path.
    /// </summary>
    static AnimationClip ResolvePlayerClip(CharacterRaceAnimation profile, string clipName, bool walk)
    {
        if (!PlayerLocomotionBodyCatalog.TryGet(profile, out PlayerLocomotionBodyCatalog.Row row))
        {
            LocomotionStrideCatalog.LogMissing(DescribeProfile(profile), walk ? "walk" : "run", clipName);
            return null;
        }

        string hand = walk ? row.WalkHandClipResources : row.RunHandClipResources;
        if (!LocomotionStrideCatalog.TryResolveResourcesPath(hand, WeaponToken(clipName), out string path))
            return null;
        return Resources.Load<AnimationClip>(path);
    }

    public static string DescribeProfile(CharacterRaceAnimation profile)
    {
        string name = profile.ToString();
        string sex = name.Length > 0 && name[0] == 'F' ? "F" : "M";
        string race = name.Length > 1 ? name.Substring(1) : name;
        return "race=" + race + " sex=" + sex + " profile=" + name;
    }

    static string WeaponToken(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return "hand";
        int split = clipName.IndexOf('_');
        if (split < 0 || split >= clipName.Length - 1)
            return "hand";
        return clipName.Substring(split + 1);
    }

}
