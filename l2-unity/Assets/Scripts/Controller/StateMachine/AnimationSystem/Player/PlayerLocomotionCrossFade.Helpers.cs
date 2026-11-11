using System;
using UnityEngine;

public static partial class PlayerLocomotionCrossFade
{
    public static bool IsAlreadyPlaying(IAnimationController controller, string stateName, int layer = 0)
    {
        return controller != null && IsAlreadyPlaying(controller.GetAnimator(), stateName, layer);
    }

    public static bool ShouldSkip(
        IAnimationController controller,
        string stateName,
        string recentAnimationName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return true;
        }

        // Trust Animator only. Recent-name short-circuit was wrong for MagicShot/Cast:
        // PlayerAnimationTrigger never SetRecentName, so recent stays wait_* from before cast.
        // WAIT_RETURN then skipped CrossFade → character stuck in MagicShot forever.
        bool animPlaying = IsAlreadyPlaying(controller, stateName);
        if (!animPlaying &&
            !string.IsNullOrEmpty(recentAnimationName) &&
            string.Equals(recentAnimationName, stateName, StringComparison.Ordinal))
        {
            // Debug.Log(
                // $"[ANIM_CROSSFADE] stale_recent_ignored recent={recentAnimationName} " +
                // $"want={stateName} (animator not in that state — will CrossFade)");
        }

        return animPlaying;
    }

    public static bool TryPlay(
        IAnimationController controller,
        string stateName,
        float? fixedDuration = null)
    {
        if (controller == null || string.IsNullOrEmpty(stateName))
        {
            return false;
        }

        if (IsAlreadyPlaying(controller, stateName))
        {
            // Debug.Log($"[ANIM_CROSSFADE] SKIP already playing state={stateName}");
            return false;
        }

        float duration = fixedDuration ?? LocomotionCrossFadeSettings.ResolveDuration(stateName, controller);
        controller.CrossFadeInFixedTime(stateName, duration);
        return true;
    }
}
