using UnityEngine;

public partial class MonsterStateDeath : MonsterStateBase
{
    bool IsPoseHeld()
    {
        return _entity != null && _entity.ActionSlot != null && _entity.ActionSlot.DeathPoseHeld;
    }

    void SetSpeed(Animator animator, float speed)
    {
        if (_networkAnimationController != null)
            _networkAnimationController.SetAnimatorSpeed(speed);
        else if (animator != null)
            animator.speed = speed;
    }

    private void StopAnimationTrigger(Animator animator, string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName))
            return;
        // Monster controllers often drive death via CrossFade only — no "death" bool.
        if (!AnimatorBoolSet.HasBool(animator, parameterName))
            return;
        if (!animator.GetBool(parameterName))
            return;
        if (Animations != null)
            Animations.StopMonsterCurrentAnimation(
                animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName);
    }
}
