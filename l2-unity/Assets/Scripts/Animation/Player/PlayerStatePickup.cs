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

    static IAnimationManager Animations
    {
        get
        {
            if (App.HasContainer)
            {
                try
                {
                    return App.Resolve<IAnimationManager>();
                }
                catch
                {
                }
            }

            if (App.GameContainer != null)
            {
                try
                {
                    return (IAnimationManager)App.GameContainer.Resolve(typeof(IAnimationManager));
                }
                catch
                {
                }
            }

            return IncomingPacketActions.Animations;
        }
    }

    static void QueueOnMain(Action action)
    {
        if (App.HasContainer)
        {
            try
            {
                App.Resolve<PacketApplyQueue>().Queue(action);
                return;
            }
            catch
            {
            }
        }

        if (App.GameContainer != null)
        {
            try
            {
                ((PacketApplyQueue)App.GameContainer.Resolve(typeof(PacketApplyQueue))).Queue(action);
                return;
            }
            catch
            {
            }
        }

        IncomingPacketActions.Queue(action);
    }

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
            && PlayerEntity.Instance != null
            && PlayerEntity.Instance.isAutoAttack;

        if (local && PlayerStateMachine.Instance != null)
        {
            PlayerState st = PlayerStateMachine.Instance.State;
            if (st == PlayerState.MAGIC_SKILLS || st == PlayerState.PHYSICAL_SKILLS)
                PlayerStateMachine.Instance.ChangeIntention(Intention.INTENTION_IDLE);

            if (autoAttack)
            {
                PlayerStateMachine.Instance.NotifyEvent(Event.WAIT_RETURN);
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
