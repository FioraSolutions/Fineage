using System;
using UnityEngine;

public static partial class PlayerLocomotionCrossFade
{
    public static bool IsLocomotionStateName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return false;
        }

        return name.StartsWith("walk", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("run", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("wait", StringComparison.OrdinalIgnoreCase)
            || name.StartsWith("atkwait", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAlreadyPlaying(Animator animator, string stateName, int layer = 0)
    {
        if (animator == null || string.IsNullOrEmpty(stateName))
        {
            return false;
        }

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(layer);
        if (current.IsName(stateName))
        {
            if (animator.IsInTransition(layer))
            {
                AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(layer);
                if (!next.IsName(stateName))
                {
                    return false;
                }
            }

            return true;
        }

        if (animator.IsInTransition(layer))
        {
            AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(layer);
            if (next.IsName(stateName))
            {
                return true;
            }
        }

        return false;
    }
}
