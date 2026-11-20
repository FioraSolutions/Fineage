using UnityEngine;

public static partial class LocomotionGait
{
    /// <summary>
    /// Run/walk animator multipliers derived from the same unity m/s that movement uses.
    /// </summary>
    public static (float runRate, float walkRate) ComputePlaybackRates(
        Animator animator,
        string runClipName,
        string walkClipName,
        float unityRunMetersPerSecond,
        float unityWalkMetersPerSecond)
    {
        float runRate = Rate(animator, runClipName, unityRunMetersPerSecond, MinRunRate, MaxRate);
        float walkRate = Rate(animator, walkClipName, unityWalkMetersPerSecond, MinWalkRate, MaxRate);
        return (runRate, walkRate);
    }

    public static float RunRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinRunRate, MaxRate);

    public static float WalkRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinWalkRate, MaxRate);
}