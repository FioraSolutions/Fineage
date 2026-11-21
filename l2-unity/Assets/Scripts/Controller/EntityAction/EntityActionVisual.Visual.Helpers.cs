
using UnityEngine;

public static partial class EntityActionVisual
{
    public static void PlayCombatWait(Entity entity, float? fadeDuration = null)
    {
        UserEntity userWait = entity as UserEntity;
        if (userWait != null && userWait.IsAttackVisualPlaying())
            return;

        if (entity is UserEntity user)
        {
            MagicGaitTrace.Event(entity, "PlayCombatWait", "→atkwait");
            PlayUserLocomotion(user, "atkwait", fadeDuration);
            return;
        }

        if (!CanPlayAnim(entity))
            return;

        TryPlayCreatureAnim(entity, MonsterAnim.AtkWait);
    }

    public static void PlaySocial(Entity entity, int actionId)
    {
        if (entity == null)
            return;

        if (InterludePacketPresentationCatalog.TryResolveSocialAction(
                actionId, out InterludePresentationKind socialKind))
        {
            LevelUpCelebrationPresentation.TryPlay(
                entity, socialKind, "SocialAction actionId=" + actionId);
        }

        // Cast owns the Animator while MagicSkillPresentation holds AnimChannel.Cast.
        if (!AnimChannel.CanPlay(AnimChannel.Kind.Social))
        {
            Debug.LogError(
                "[SocialAnim] action id=" + actionId + " blocked: AnimChannel.Cast is active on entity=" +
                entity.name);
            return;
        }

        string socialState = InterludeSocialAnimationCatalog.SocialAnimatorStateName;
        if (entity is PlayerEntity || entity is UserEntity)
        {
            if (!PlayerSocialAnimApply.TryPrepareSocialClip(entity, actionId, out socialState))
                return;
        }

        NetworkAnimationController nac = entity.GetAnimatorController();
        if (nac != null)
        {
            Animator animator = nac.GetAnimator();
            if (animator == null)
            {
                Debug.LogError(
                    "[SocialAnim] action id=" + actionId + " entity=" + entity.name +
                    " missing Animator on NetworkAnimationController");
                return;
            }

            int stateHash = Animator.StringToHash(socialState);
            if (!animator.HasState(0, stateHash))
            {
                Debug.LogError(
                    "[SocialAnim] action id=" + actionId + " entity=" + entity.name +
                    " animator state missing name=" + socialState +
                    " controller=" + (animator.runtimeAnimatorController != null
                        ? animator.runtimeAnimatorController.name
                        : "(null)"));
                return;
            }

            AnimPlayback.PrepareGesture(nac);
            nac.CrossFadeInFixedTime(socialState, LocomotionCrossFadeSettings.FixedDuration);
            return;
        }

        if (entity.Identity != null && ControllerPacketServices.Animations != null)
        {
            if (!ControllerPacketServices.Animations.TryPlayExactAnimatorState(
                    entity.Identity.Id, socialState, out _))
                return;
            return;
        }

        TryPlayCreatureAnim(entity, socialState);
    }

    public static void PlayWaitType(Entity entity, WaitType waitType)
    {
        if (entity == null)
            return;

        // The local hero is a PlayerEntity, not a UserEntity: it drives its own Player_Basic controller,
        // and it also receives these packets (the zone includes the actor in every wait-type fan-out).
        if (waitType == WaitType.WT_SITTING || waitType == WaitType.WT_STANDING)
        {
            PlaySitStand(entity, waitType);
            return;
        }

        if (waitType == WaitType.WT_START_FAKEDEATH)
        {
            // WaitType 2 is not bDead: the pawn plays the death clip but keeps HP/MP (Mobius _isFakeDeath).
            PlayFakeDeathPose(entity);
            return;
        }

        if (waitType == WaitType.WT_STOP_FAKEDEATH)
        {
            // Mobius stopFakeDeath broadcasts the wait type and then Revive. The rebirth clip for the local hero
            // comes from the Revive packet (state machine → REBIRTH → "rebirth"), so here only the latch is released.
            if (entity is PlayerEntity)
            {
                ClearFakeDeathPose(entity);
                return;
            }
            PlayRevive(entity);
            return;
        }

        if (entity is UserEntity userStop)
            PlayUserLocomotion(userStop, "wait");
    }

    /// <summary>
    /// <c>ChangeWaitType</c> sitting/standing. Player_Basic carries <c>sit</c> (Sit clip, then the looping
    /// <c>sit_wait</c>) and <c>stand</c>; every race override controller swaps in its own clips.
    /// </summary>
    static void PlaySitStand(Entity entity, WaitType waitType)
    {
        if (waitType == WaitType.WT_SITTING)
        {
            AnimChannel.Clear();
            HoldSitPose(entity);
            PlaySitClip(entity);
            return;
        }

        ClearSitPose(entity);

        // Standing back up: the hero plays the race stand clip; PlayerStateStand hands it to wait at the end.
        if (entity is PlayerEntity && entity.Identity != null && ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayExactAnimatorState(entity.Identity.Id, "stand");
            return;
        }

        if (entity is UserEntity user)
        {
            PlayUserLocomotion(user, "wait");
            return;
        }

        if (entity.Identity != null && ControllerPacketServices.Animations != null)
            ControllerPacketServices.Animations.PlayAnimation(
                entity.Identity.Id, AnimationNames.WAIT.ToString(), true);
    }

    /// <summary>
    /// The hero CrossFades the Player_Basic <c>sit</c> state through AnimationManager (no bool graph drives it);
    /// everyone else CrossFades the same state on their override controller.
    /// </summary>
    static void PlaySitClip(Entity entity)
    {
        if (entity is PlayerEntity && entity.Identity != null && ControllerPacketServices.Animations != null)
        {
            ControllerPacketServices.Animations.PlayExactAnimatorState(entity.Identity.Id, "sit");
            return;
        }

        NetworkAnimationController nac = entity.GetAnimatorController();
        if (nac == null)
            return;
        nac.SetAnimatorSpeed(1f);
        nac.CrossFadeInFixedTime("sit", LocomotionCrossFadeSettings.FixedDuration);
    }
}
