using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One locomotion playback-rate pipeline for every player race/sex/weapon clip
/// and every NPC mesh: rate = unitySpeed * clipLength / stride.
/// Unity move speed is already <see cref="NumberUtils.ScaleToUnity"/> (0.0189).
/// Stride is measured from the clip (both feet's swing relative to the pelvis).
/// </summary>
public static partial class LocomotionGait
{
    public const float MinRunRate = 0.2f;
    public const float MinWalkRate = 0.12f;
    public const float MaxRate = 4f;
    public const float MinStrideMeters = 0.05f;
    /// <summary>Used only when a clip exists but no feet or root travel can be measured.</summary>
    public const float FallbackStrideMeters = 1.5f;

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
        float stride = strideMeters >= MinStrideMeters ? strideMeters : FallbackStrideMeters;
        return Mathf.Clamp(unityMetersPerSecond * clipLength / stride, minRate, maxRate);
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

    public static float RunRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinRunRate, MaxRate);

    public static float WalkRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinWalkRate, MaxRate);
}
