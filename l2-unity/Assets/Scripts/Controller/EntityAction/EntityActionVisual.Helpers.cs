using UnityEngine;
using System;

public static partial class EntityActionVisual
{
    static bool CanPlayAnim(Entity entity)
    {
        if (entity != null && entity.HasAnimator())
            return true;

        return false;
    }

    static bool IsPlayingDeath(Entity entity)
    {
        if (entity == null)
            return false;

        NetworkAnimationController nac = entity.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : null;
        if (animator == null)
            return false;

        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(animator, MonsterAnim.Death))
            return true;
        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(animator, PlayerDeathAnim.Death))
            return true;

        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips == null || clips.Length == 0 || clips[0].clip == null)
            return false;
        return clips[0].clip.name.IndexOf("death", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
