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

    /// <summary>
    /// Npc_Basic / Monster_Basic bare-state pawns (not User weapon-suffix path).
    /// Town combatants are <c>type: Guard</c> (TI Guard + elven Sentinel); they share this
    /// SMB with Folks for locomotion — Attack/Die packets only arrive for Guards/Monsters.
    /// </summary>
    public static bool UsesCreatureAnim(Entity entity)
    {
        return entity != null && entity is not UserEntity && entity.HasAnimator();
    }

    /// <summary>
    /// Play a shared creature SMB state (<c>wait</c>/<c>walk</c>/<c>run</c>/<c>atk01</c>/<c>death</c>/…).
    /// </summary>
    public static bool TryPlayCreatureAnim(Entity entity, string state, float? fixedDuration = null)
    {
        if (!UsesCreatureAnim(entity) || entity.Identity == null || string.IsNullOrEmpty(state))
            return false;
        if (ControllerPacketServices.Animations == null)
            return false;

        ControllerPacketServices.Animations.PlayCreatureAnimation(
            entity.Identity.Id, state, fixedDuration);
        return true;
    }

    static bool IsPlayingDeath(Entity entity)
    {
        if (entity == null)
            return false;

        NetworkAnimationController nac = entity.GetAnimatorController();
        Animator animator = nac != null ? nac.GetAnimator() : null;
        if (animator == null)
            return false;

        // MonsterAnim = shared Npc_Basic / Monster_Basic state catalog.
        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(animator, MonsterAnim.Death))
            return true;
        if (PlayerLocomotionCrossFade.IsAlreadyPlaying(animator, PlayerDeathAnim.Death))
            return true;

        AnimatorClipInfo[] clips = animator.GetCurrentAnimatorClipInfo(0);
        if (clips == null || clips.Length == 0 || clips[0].clip == null)
            return false;
        return clips[0].clip.name.IndexOf("death", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static bool IsCreatureDeathVisualActive(Entity entity)
    {
        if (entity == null)
            return false;
        if (entity.ActionSlot != null && entity.ActionSlot.DeathPoseHeld)
            return true;
        return IsPlayingDeath(entity);
    }
}
