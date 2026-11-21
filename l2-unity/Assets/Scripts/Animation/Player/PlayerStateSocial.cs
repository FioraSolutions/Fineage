using System;
using UnityEngine;

/// <summary>
/// Player_Basic <c>social01</c> has no graph exit: when the gesture clip ends, CrossFade back to wait
/// (or atkwait for local hero in auto-attack), same hand-off as <see cref="PlayerStatePickup"/>.
/// </summary>
public class PlayerStateSocial : StateMachineBehaviour
{
    const float FinishNorm = 0.95f;
    bool _switched;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _switched = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_switched || stateInfo.normalizedTime < FinishNorm || animator.IsInTransition(layerIndex))
            return;
        SwitchToIdle(animator);
    }

    void SwitchToIdle(Animator animator)
    {
        _switched = true;
        int objectId = AnimatorUtils.GetObjectId(animator);
        IAnimationManager animations = AnimationPacketServices.Animations;
        if (animations == null || objectId == 0)
            return;

        if (EntityActionVisual.IsSitPoseHeld(objectId))
            return;

        bool local = AnimatorUtils.IsLocalPlayerAnimator(animator);
        bool autoAttack = local
            && DomainPacketServices.LocalPlayer != null
            && DomainPacketServices.LocalPlayer.isAutoAttack;

        if (local && autoAttack && PlayerStatePacketServices.PlayerState != null)
        {
            PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN);
            return;
        }

        AnimationPacketServices.QueueOnMain(() =>
        {
            animations.PlayAnimation(objectId, AnimationNames.WAIT.ToString(), true);
        });
    }
}