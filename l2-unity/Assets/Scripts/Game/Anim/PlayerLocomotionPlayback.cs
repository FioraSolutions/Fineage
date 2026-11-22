using System;
using UnityEngine;

/// <summary>
/// One locomotion playback path for every player race/sex/weapon clip and every NPC.
/// Rate = current planar speed / authored speed, where authored speed is the baked
/// cycle distance divided by the clip length.
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
            subject,
            LocomotionGait.MinRunRate);
        walkRate = ResolveOne(
            entity,
            animator,
            walkClipName,
            walk: true,
            unityWalkMetersPerSecond,
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
            LocomotionStrideCatalog.LogMissing(subject, gait, clipName);
            return 0f;
        }

        if (clip.length < 0.01f || !LocomotionStrideCatalog.TryGet(clip.name, out float stride))
        {
            LocomotionStrideCatalog.LogMissing(subject, gait, clip.name);
            return 0f;
        }

        return LocomotionGait.PlaybackRate(
            unityMetersPerSecond,
            clip.length,
            stride,
            minRate,
            LocomotionGait.MaxRate);
    }

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
