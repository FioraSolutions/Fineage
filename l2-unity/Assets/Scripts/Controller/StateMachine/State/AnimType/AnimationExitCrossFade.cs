using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resolves CrossFade duration when leaving a registered Animator state.
/// Override per clip via <see cref="Animation.ExitCrossFadeDuration"/>; else default.
/// </summary>
public static class AnimationExitCrossFade
{
    static Animation[] _exitOverrides;

    static Animation[] ExitOverrides
    {
        get
        {
            if (_exitOverrides != null)
            {
                return _exitOverrides;
            }

            var list = new List<Animation>(8);
            CollectExitOverrides(SpecialAnimationNames.arrayAtkSpecials, list);
            CollectExitOverrides(SpecialAnimationNames.arrayPhisicalAtkSpecials, list);
            CollectExitOverrides(SpecialAnimationNames.arrayMagicAtkSpecials, list);
            _exitOverrides = list.ToArray();
            return _exitOverrides;
        }
    }

    static void CollectExitOverrides(Animation[] source, List<Animation> dest)
    {
        if (source == null)
        {
            return;
        }

        for (int i = 0; i < source.Length; i++)
        {
            Animation anim = source[i];
            if (anim != null && anim.ExitCrossFadeDuration.HasValue)
            {
                dest.Add(anim);
            }
        }
    }

    public static float Resolve(IAnimationController controller)
    {
        float fallback = LocomotionCrossFadeSettings.FixedDuration;
        if (controller == null)
        {
            return fallback;
        }

        Animator animator = controller.GetAnimator();
        if (animator == null)
        {
            return fallback;
        }

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);
        Animation[] overrides = ExitOverrides;
        for (int i = 0; i < overrides.Length; i++)
        {
            Animation anim = overrides[i];
            if (current.IsName(anim.Value))
            {
                return anim.ExitCrossFadeDuration.Value;
            }
        }

        return fallback;
    }
}
