using System.Collections.Generic;
using UnityEngine;

public static partial class AnimationExitCrossFade
{
    public static float Resolve(IAnimationController controller)
    {
        float fallback = LocomotionCrossFadeDefaults.FixedDuration;
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
