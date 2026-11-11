using UnityEngine;

public partial class PlayerStateJatkBow : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Animations != null)
            Animations.SetPAtkSpeed(AnimatorUtils.GetObjectId(animator), 1f);
    }

    private void SwitchToIdle()
    {
        if (_isSwitchIdle)
            return;

        _isSwitchIdle = true;
        // ReplayAttackTrace.Write(
            // "BOW_SWITCH_IDLE",
            // DomainPacketServices.LocalPlayer,
            // DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.GetTargetEntity() : null,
            // "elapsed=" + (Time.time - _startTime).ToString("F3") +
            // " endAt=" + _endTime.ToString("F3") +
            // " releaseAt=" + _releaseAt.ToString("F3"));
        if (DomainPacketServices.LocalPlayer != null)
            DomainPacketServices.LocalPlayer.IsAttack = false;
        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
            return;
        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }

    private void StopAnimationTrigger(Animator animator, string animParameterName)
    {
        // CrossFade combat path: jatk*_bow is a state name, not a Bool param.
        if (!AnimatorBoolSet.HasBool(animator, animParameterName))
            return;
        if (animator.GetBool(animParameterName))
        {
            Animations.StopCurrentAnimation(
                AnimatorUtils.GetObjectId(animator), animParameterName, "player");
        }
    }
}
