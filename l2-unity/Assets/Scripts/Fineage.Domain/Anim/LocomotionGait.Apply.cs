using UnityEngine;

public static partial class LocomotionGait
{
    public static float RunRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinRunRate, MaxRate);

    public static float WalkRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinWalkRate, MaxRate);

    public static void ResolvePlaybackRates(
        Animator animator,
        string runClipName,
        string walkClipName,
        float unityRunMetersPerSecond,
        float unityWalkMetersPerSecond,
        out float runRate,
        out float walkRate)
    {
        runRate = Rate(animator, runClipName, unityRunMetersPerSecond, MinRunRate, MaxRate);
        walkRate = Rate(animator, walkClipName, unityWalkMetersPerSecond, MinWalkRate, MaxRate);
    }
}