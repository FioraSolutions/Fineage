using UnityEngine;

public partial class PlayerStateSpAtkBow : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Animations.SetPAtkSpeed(AnimatorUtils.GetObjectId(animator), 1f);
    }

    private void SwitchToIdle(Animator animator)
    {
        if (_isSwitchIdle)
            return;

        _isSwitchIdle = true;
        if (DomainPacketServices.LocalPlayer != null)
            DomainPacketServices.LocalPlayer.IsAttack = false;

        int objectId = AnimatorUtils.GetObjectId(animator);
        string phaseName = Animations != null
            ? Animations.GetCurrentAnimationName(objectId)
            : null;
        if (string.IsNullOrEmpty(phaseName))
            phaseName = !string.IsNullOrEmpty(parameterName) ? parameterName : motionName;

        Animations?.NotifyMagicPhaseFinished(objectId, phaseName);

        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
            return;
        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }
}
