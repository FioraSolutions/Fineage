using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// One locomotion playback-rate pipeline for every player race/sex/weapon clip
/// and every NPC mesh.
/// Original client (UE2 <c>APawn::PlayRunning</c>): PlayAnim rate = Velocity / GroundSpeed.
/// The sequence then advances at its ukx Rate (frames per second) over frameCount.
/// Unity clips are stored at the importer sample rate, which is not that Rate
/// (MFighter run is 13 frames at 15 fps and imported at 24). PlaybackRateForSequence
/// is the shared rate. A missing GroundSpeed or sequence clock returns 0.
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
    /// UE2 PlayRunning rate is Velocity / GroundSpeed, then the sequence plays at its
    /// own Rate (frames / second). Unity clips are timed at the importer sample rate,
    /// so the animator speed parameter is
    /// (current / authored) * (clipLength / (frameCount / sequenceRate)).
    /// A missing authored speed or sequence clock returns 0.
    /// </summary>
    public static float PlaybackRateForSequence(
        float currentMetersPerSecond,
        float authoredMetersPerSecond,
        float unityClipLength,
        float officialCycleSeconds,
        float minRate,
        float maxRate)
    {
        if (unityClipLength < 0.01f || officialCycleSeconds <= 0.001f || authoredMetersPerSecond <= 0.001f)
            return 0f;
        float speedRatio = currentMetersPerSecond <= 0.001f
            ? 1f
            : currentMetersPerSecond / authoredMetersPerSecond;
        float importScale = unityClipLength / officialCycleSeconds;
        return Mathf.Clamp(speedRatio * importScale, minRate, maxRate);
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
