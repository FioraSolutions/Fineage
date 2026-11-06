using UnityEngine;

/// <summary>
/// Playback-rate channels for character animators. Keep them orthogonal:
/// <list type="bullet">
/// <item><b>Locomotion</b> — Animator floats <c>run_speed</c> / <c>walk_speed</c>
/// (<see cref="CharTemplateRegistry"/>, <see cref="NumberUtils.NpcLocomotionAnimRate()"/>).</item>
/// <item><b>Combat</b> — Animator float <c>patkspd</c> and <see cref="CastClipPlayable"/> phase speeds.</item>
/// <item><b>Gesture</b> — fixed <see cref="GestureRate"/> (pickup, death, rebirth, lobby social).
/// Gesture AnimatorStates must not use locomotion/combat speed parameters.</item>
/// </list>
/// </summary>
public static class AnimPlayback
{
    public const float GestureRate = 1f;
    /// <summary>
    /// Interlude PicItem is a short authored gesture (~4 frames @ 24fps ≈ 0.17s for MMagic).
    /// Not tied to server HitTime / PAtk — play at clip length, do not stretch.
    /// </summary>
    public const float PickupFallbackSeconds = 0.2f;

    /// <summary>
    /// Clear root Animator.speed and attack multiplier so a gesture plays at authored timing.
    /// Does not touch run_speed/walk_speed — gait stays ready when locomotion resumes.
    /// </summary>
    public static void PrepareGesture(IAnimationController controller)
    {
        if (controller == null)
            return;
        controller.SetAnimatorSpeed(GestureRate);
        controller.SetPAtkSpeed(GestureRate);
    }

    /// <summary>Authored PicItem length (no time-stretch). Used for gait resume wait.</summary>
    public static float PickupPlaySeconds(float clipLength)
    {
        return clipLength > 0.01f ? clipLength : PickupFallbackSeconds;
    }
}
