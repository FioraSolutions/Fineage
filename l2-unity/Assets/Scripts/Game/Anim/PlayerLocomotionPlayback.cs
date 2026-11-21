using UnityEngine;

/// <summary>
/// Shared player/NPC locomotion rate path: catalog strides for players, measured stride for creatures.
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
        if (entity != null && IsPlayerBodyEntity(entity) && TryPlayerProfile(entity, out CharacterRaceAnimation profile))
        {
            ResolvePlayerRates(
                profile,
                animator,
                runClipName,
                walkClipName,
                unityRunMetersPerSecond,
                unityWalkMetersPerSecond,
                out runRate,
                out walkRate);
            return;
        }

        LocomotionGait.ResolvePlaybackRates(
            animator,
            runClipName,
            walkClipName,
            unityRunMetersPerSecond,
            unityWalkMetersPerSecond,
            out runRate,
            out walkRate);
    }

    static bool IsPlayerBodyEntity(Entity entity) =>
        entity is PlayerEntity || entity is UserEntity;

    static bool TryPlayerProfile(Entity entity, out CharacterRaceAnimation profile)
    {
        profile = entity.RaceId;
        return System.Enum.IsDefined(typeof(CharacterRaceAnimation), profile);
    }

    static void ResolvePlayerRates(
        CharacterRaceAnimation profile,
        Animator animator,
        string runClipName,
        string walkClipName,
        float unityRunMetersPerSecond,
        float unityWalkMetersPerSecond,
        out float runRate,
        out float walkRate)
    {
        AnimationClip runClip = LocomotionGait.FindClip(animator, runClipName)
            ?? PlayerLocomotionBodyCatalog.LoadHandClip(profile, walk: false);
        AnimationClip walkClip = LocomotionGait.FindClip(animator, walkClipName)
            ?? PlayerLocomotionBodyCatalog.LoadHandClip(profile, walk: true);

        if (runClip == null)
            LocomotionGait.LogMissingLocomotionClip(animator, runClipName, profile);
        if (walkClip == null)
            LocomotionGait.LogMissingLocomotionClip(animator, walkClipName, profile);

        Transform skeleton = animator != null ? animator.transform : null;
        float runStride = PlayerLocomotionBodyCatalog.ResolveRunStride(profile, runClip, skeleton);
        float walkStride = PlayerLocomotionBodyCatalog.ResolveWalkStride(profile, walkClip, skeleton);

        if (runStride < LocomotionGait.MinStrideMeters)
        {
            PlayerLocomotionBodyCatalog.LogStrideFallback(profile, walk: false, runStride);
            runStride = LocomotionGait.FallbackStrideMeters;
        }
        if (walkStride < LocomotionGait.MinStrideMeters)
        {
            PlayerLocomotionBodyCatalog.LogStrideFallback(profile, walk: true, walkStride);
            walkStride = LocomotionGait.FallbackStrideMeters;
        }

        float runLen = runClip != null && runClip.length > 0.01f ? runClip.length : 0.67f;
        float walkLen = walkClip != null && walkClip.length > 0.01f ? walkClip.length : 0.8f;

        runRate = LocomotionGait.PlaybackRate(
            unityRunMetersPerSecond,
            runLen,
            runStride,
            LocomotionGait.MinRunRate,
            LocomotionGait.MaxRate);
        walkRate = LocomotionGait.PlaybackRate(
            unityWalkMetersPerSecond,
            walkLen,
            walkStride,
            LocomotionGait.MinWalkRate,
            LocomotionGait.MaxRate);
    }
}