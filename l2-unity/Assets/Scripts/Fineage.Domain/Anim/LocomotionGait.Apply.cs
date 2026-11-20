using UnityEngine;

public static partial class LocomotionGait
{
    public static float RunRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinRunRate, MaxRate);

    public static float WalkRate(Animator animator, string clipName, float unityMetersPerSecond) =>
        Rate(animator, clipName, unityMetersPerSecond, MinWalkRate, MaxRate);
}