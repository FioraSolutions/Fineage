using UnityEngine;

/// <summary>
/// CrossFade policy for Monster_Basic states.
/// <list type="bullet">
/// <item>Locomotion (wait/walk/run/atkwait) — skip when already playing.</item>
/// <item>Attack one-shots (atk / spatk) — always restart from 0.</item>
/// <item>Death — terminal: CrossFade once, never rewind (predict + Die must not restart the corpse).</item>
/// </list>
/// </summary>
public static class MonsterCrossFade
{
    public static bool TryPlay(
        IAnimationController controller,
        string stateName,
        MonsterAnimState family,
        float? fixedDuration = null)
    {
        if (controller == null || string.IsNullOrEmpty(stateName))
            return false;

        bool alreadyPlaying = PlayerLocomotionCrossFade.IsAlreadyPlaying(controller, stateName);
        if (alreadyPlaying)
        {
            // Terminal death + looping locomotion: leave the current state alone.
            if (family == MonsterAnimState.Death || !MonsterAnim.IsOneShot(family))
                return false;
        }

        float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
        if (duration <= 0f)
        {
            Animator animator = controller.GetAnimator();
            if (animator == null)
                return false;
            animator.Play(stateName, 0, 0f);
            animator.Update(0f);
            return true;
        }

        if (MonsterAnim.IsOneShot(family))
        {
            controller.CrossFadeRestartInFixedTime(stateName, duration);
            return true;
        }

        controller.CrossFadeInFixedTime(stateName, duration);
        return true;
    }

    /// <summary>
    /// Unknown / legacy names (e.g. damageaction) — CrossFade by raw state name, no skip.
    /// </summary>
    public static bool TryPlayRaw(
        IAnimationController controller,
        string stateName,
        float? fixedDuration = null)
    {
        if (controller == null || string.IsNullOrEmpty(stateName))
            return false;

        float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
        controller.CrossFadeInFixedTime(stateName, duration);
        return true;
    }
}
