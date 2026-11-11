using UnityEngine;
using System.Collections.Generic;
using System;

public partial class NewIdleState : StateBase
{
    private bool TryGetActiveCombatSwing(out string swingState, out float swingNorm)
    {
        swingState = null;
        swingNorm = 0f;

        int objectId = _stateMachine.GetObjectId();
        string recent = ControllerPacketServices.Animations != null
            ? ControllerPacketServices.Animations.GetCurrentAnimationName(objectId)
            : null;
        if (!IsCombatSwingStateName(recent))
        {
            return false;
        }

        IAnimationController controller = null;
        if (ControllerPacketServices.Animations is AnimationManager concrete)
        {
            controller = concrete.GetPlayerController(objectId);
        }
        else
        {
            controller = AnimationPacketServices.PlayerAnimation;
        }

        Animator animator = controller != null ? controller.GetAnimator() : null;
        if (animator == null)
        {
            return false;
        }

        AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(0);
        if (current.IsName(recent) && current.normalizedTime < CombatSwingNormGate)
        {
            swingState = recent;
            swingNorm = current.normalizedTime;
            return true;
        }

        if (animator.IsInTransition(0))
        {
            AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(0);
            if (next.IsName(recent) && next.normalizedTime < CombatSwingNormGate)
            {
                swingState = recent;
                swingNorm = next.normalizedTime;
                return true;
            }
        }

        return false;
    }

    private static bool IsCombatSwingStateName(string stateName)
    {
        if (string.IsNullOrEmpty(stateName))
        {
            return false;
        }

        return stateName.StartsWith("jatk", System.StringComparison.OrdinalIgnoreCase)
            || stateName.StartsWith("SpAtk", System.StringComparison.OrdinalIgnoreCase);
    }

    private void PlayAnimation(Animation animation)
    {
        ControllerPacketServices.Animations.PlayAnimation(_stateMachine.GetObjectId() , animation.ToString(), true);
    }

    private void PlayAnimationDeferred(Animation animation, bool fromCombatSmb = false)
    {
        int objectId = _stateMachine.GetObjectId();
        string animName = animation.ToString();
        ControllerPacketServices.Queue(() =>
        {
            // A Move packet can arrive before this frame. Do not put atkwait back over walk/run.
            if (_stateMachine.ShouldKeepLocomotion() ||
                _stateMachine.State != PlayerState.IDLE ||
                _stateMachine.Intention != Intention.INTENTION_IDLE)
            {
                return;
            }

            // Re-check only for early external WAIT_RETURN; SMB wall-finish must CrossFade.
            if (!fromCombatSmb && TryGetActiveCombatSwing(out _, out _))
            {
                return;
            }

            // WaitReturnLog.Play(objectId, animName);
            ControllerPacketServices.Animations.PlayAnimation(objectId, animName, true);
        });
    }
}
