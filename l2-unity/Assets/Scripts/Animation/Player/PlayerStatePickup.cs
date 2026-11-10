using System;
using UnityEngine;

/// <summary>
/// Pickup has no animator graph exit. When the clip finishes, CrossFade stand pose:
/// atkwait if AutoAttackStart is still on, otherwise wait. Do not fire WAIT_RETURN
/// unless the player is already in combat stance — that event is for combat SMBs.
/// </summary>
public class PlayerStatePickup : StateMachineBehaviour
{
    const float FinishNorm = 0.95f;
    bool _switched;

    static IAnimationManager Animations => AnimationPacketServices.Animations;

    static void QueueOnMain(Action action) => AnimationPacketServices.QueueOnMain(action);

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _switched = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_switched || stateInfo.normalizedTime < FinishNorm)
            return;
        SwitchToStand(animator);
    }

    void SwitchToStand(Animator animator)
    {
        _switched = true;
        int objectId = AnimatorUtils.GetObjectId(animator);
        IAnimationManager animations = Animations;
        if (animations == null || objectId == 0)
            return;

        bool local = AnimatorUtils.IsLocalPlayerAnimator(animator);
        bool autoAttack = local
            && DomainPacketServices.LocalPlayer != null
            && DomainPacketServices.LocalPlayer.isAutoAttack;

        if (local && PlayerStatePacketServices.PlayerState != null)
        {
            PlayerState st = PlayerStatePacketServices.PlayerState.State;
            if (st == PlayerState.MAGIC_SKILLS || st == PlayerState.PHYSICAL_SKILLS)
                PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);

            if (autoAttack)
            {
                PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN);
                return;
            }
        }

        QueueOnMain(() =>
        {
            animations.PlayAnimation(
                objectId, AnimationNames.WAIT.ToString(), true);
        });
    }
}
