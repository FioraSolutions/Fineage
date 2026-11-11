using UnityEngine;

public partial class MonsterStateDeath : MonsterStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LoadComponents(animator);
        StopAnimationTrigger(animator, parameterName);
        // Unstick after a frozen atk (patkspd=0 from missing Zone PAtk).
        if (_networkAnimationController != null)
            _networkAnimationController.SetPAtkSpeed(1f);
        SetSpeed(animator, 1f);
        // Death SFX is owned by EntityActionVisual.PlayDeath → MonsterCombatSound
        // (CrossFade path; SMB alone was easy to miss / double).

        if (IsPoseHeld())
            SetSpeed(animator, 0f);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator == null || animator.IsInTransition(layerIndex))
            return;

        if (IsPoseHeld())
        {
            SetSpeed(animator, 0f);
            return;
        }

        // Clip Loop Time is off. This only catches a clip that still wraps past the end.
        if (stateInfo.normalizedTime < 1f)
            return;

        if (_entity != null && _entity.ActionSlot != null)
            _entity.ActionSlot.DeathPoseHeld = true;
        animator.Play(stateInfo.fullPathHash, layerIndex, HoldNorm);
        SetSpeed(animator, 0f);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool backToDeath = animator != null
            && animator.IsInTransition(layerIndex)
            && animator.GetNextAnimatorStateInfo(layerIndex).IsName(MonsterAnim.Death);
        if (backToDeath)
            return;

        SetSpeed(animator, 1f);
        if (_entity != null && _entity.ActionSlot != null)
            _entity.ActionSlot.DeathPoseHeld = false;
    }
}
