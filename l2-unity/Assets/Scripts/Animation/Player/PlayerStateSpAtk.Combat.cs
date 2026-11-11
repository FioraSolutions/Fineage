using UnityEngine;

public partial class PlayerStateSpAtk : StateMachineBehaviour
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        int objectId = AnimatorUtils.GetObjectId(animator);
        Animations.SetPAtkSpeed(objectId, _linearPatkSpd);

        float elapsed = Time.time - _startTime;
        if (elapsed >= _serverHitTime && !_hitExecuted)
            _hitExecuted = true;

        if (stateInfo.normalizedTime >= SwitchToIdleNormalizedTime)
            SwitchToIdle(animator);
    }

    private void SwitchToIdle(Animator animator)
    {
        if (_isSwitchIdle)
            return;

        _isSwitchIdle = true;
        int objectId = AnimatorUtils.GetObjectId(animator);
        Animations.SetPAtkSpeed(objectId, 1f);
        animator.speed = 1f;

        string phaseName = Animations != null
            ? Animations.GetCurrentAnimationName(objectId)
            : null;
        if (string.IsNullOrEmpty(phaseName))
            phaseName = motionName;

        Animations?.NotifyMagicPhaseFinished(objectId, phaseName);

        if (!AnimatorUtils.IsLocalPlayerAnimator(animator))
            return;

        if (DomainPacketServices.LocalPlayer != null)
            DomainPacketServices.LocalPlayer.IsAttack = false;
        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
            return;
        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Animations.SetPAtkSpeed(AnimatorUtils.GetObjectId(animator), 1f);
        animator.speed = 1f;
    }
}
