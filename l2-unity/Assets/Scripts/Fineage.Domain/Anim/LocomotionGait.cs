using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One locomotion playback-rate pipeline for every player race/sex/weapon clip
/// and every NPC mesh.
/// Original client (UE2 <c>APawn::PlayRunning</c>): PlayAnim rate = Velocity / AnimSpeed,
/// where AnimSpeed is the pawn's authored GroundSpeed (walk multiplies WalkingPct).
/// L2 pawn classes store that authored speed (FFighter GroundSpeed=120, GroundMinSpeed=80)
/// and Engine.u also exports RunAnimRate / WalkAnimRate. The clips themselves are in-place,
/// so the matching rate is currentSpeed / authoredSpeed.
/// Authored speed here is the baked cycle distance divided by the clip length
/// (stride per cycle / seconds), one formula for every race and NPC.
/// A missing stride returns 0 — callers log race/sex/gait and do not substitute a default.
/// </summary>
public static partial class LocomotionGait
{
    public const float MinRunRate = 0.2f;
    public const float MinWalkRate = 0.12f;
    public const float MaxRate = 4f;
    public const float MinStrideMeters = 0.05f;

    static readonly Dictionary<long, float> StrideCache = new Dictionary<long, float>();

    public static float PlaybackRate(
        float unityMetersPerSecond,
        float clipLength,
        float strideMeters,
        float minRate,
        float maxRate)
    {
        if (unityMetersPerSecond <= 0.001f || clipLength < 0.01f)
            return Mathf.Clamp(1f, minRate, maxRate);
        if (strideMeters < MinStrideMeters)
            return 0f;
        // rate = currentSpeed / authoredSpeed, authoredSpeed = strideMeters / clipLength.
        return Mathf.Clamp(unityMetersPerSecond * clipLength / strideMeters, minRate, maxRate);
    }

    /// <summary>
    /// Resolve the locomotion clip on <paramref name="animator"/> and return the
    /// run_speed / walk_speed multiplier that keeps the feet under the body.
    /// </summary>
    public static float Rate(
        Animator animator,
        string clipName,
        float unityMetersPerSecond,
        float minRate,
        float maxRate)
    {
        if (animator == null)
            return Mathf.Clamp(1f, minRate, maxRate);

        AnimationClip clip = FindClip(animator, clipName);
        if (clip == null && !string.IsNullOrEmpty(clipName))
            LogMissingLocomotionClip(animator, clipName);
        return RateForClip(clip, animator.transform, unityMetersPerSecond, minRate, maxRate);
    }

    public static float RateForClip(
        AnimationClip clip,
        Transform skeleton,
        float unityMetersPerSecond,
        float minRate,
        float maxRate)
    {
        if (clip == null || skeleton == null)
            return Mathf.Clamp(1f, minRate, maxRate);

        float stride = MeasureStride(clip, skeleton);
        return PlaybackRate(unityMetersPerSecond, clip.length, stride, minRate, maxRate);
    }

    public static void ClearStrideCache() => StrideCache.Clear();
}
