using UnityEngine;

/// <summary>
/// Pushes computed locomotion rates into <see cref="IAnimationController"/>.
/// Lives in Controller because Domain must not reference this interface.
/// </summary>
public static class LocomotionGaitPlayback
{
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

        float runRate = LocomotionGait.Rate(animator, runClipName, unityRunMetersPerSecond, LocomotionGait.MinRunRate, LocomotionGait.MaxRate);
        float walkRate = LocomotionGait.Rate(animator, walkClipName, unityWalkMetersPerSecond, LocomotionGait.MinWalkRate, LocomotionGait.MaxRate);
        animation.SetRunSpeed(runRate);
        animation.SetWalkSpeed(walkRate);
    }
}