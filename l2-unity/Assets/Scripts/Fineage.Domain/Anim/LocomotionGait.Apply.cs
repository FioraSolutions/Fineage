using UnityEngine;

public static partial class LocomotionGait
{
    /// <summary>
    /// Push walk/run animator multipliers derived from the same unity m/s that movement uses.
    /// </summary>
    public static void ApplyPlaybackRates(
        IAnimationController animation,
        Animator animator,
        string runClipName,
        string walkClipName,
        float unityRunMetersPerSecond,
        float unityWalkMetersPerSecond)
    {
        if (animation == null)
            return;

        float runRate = Rate(animator, runClipName, unityRunMetersPerSecond, MinRunRate, MaxRate);
        float walkRate = Rate(animator, walkClipName, unityWalkMetersPerSecond, MinWalkRate, MaxRate);
        animation.SetRunSpeed(runRate);
        animation.SetWalkSpeed(walkRate);
    }

    public static float RunRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinRunRate, MaxRate);

    public static float WalkRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinWalkRate, MaxRate);
}