public partial class MonsterStateAtk : MonsterStateBase
{
    bool IsCombatSwing()
    {
        return _entity != null &&
            (_entity.ActionSlot.Action == EntityActionKind.Attack ||
             _entity.ActionSlot.Action == EntityActionKind.Skill);
    }

    void ReturnAppearToWait(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_isSwitchIdle || stateInfo.normalizedTime < 1f)
            return;
        // Die → PlayDeath then Action=Stop leaves us mid-atk01; never CrossFade wait over death.
        if (_entity != null && _entity.IsDead())
        {
            _isSwitchIdle = true;
            return;
        }
        _isSwitchIdle = true;
        if (_entity != null && EntityActionPacketServices.Actions != null)
        {
            EntityActionPacketServices.Actions.Set(_entity, EntityActionKind.Idle, "wait");
            return;
        }
        animator.CrossFade("wait", 0.12f, layerIndex);
    }

    bool IsStaleSwing()
    {
        return _entity != null &&
            _swingId != 0 &&
            _entity.ActionSlot.AttackSwingId != _swingId;
    }

    private void StopAnimationTrigger(Animator animator, string parameterName)
    {
        if (string.IsNullOrEmpty(parameterName) || !AnimatorBoolSet.HasBool(animator, parameterName))
            return;
        if (!animator.GetBool(parameterName))
            return;
        if (Animations != null)
            Animations.StopMonsterCurrentAnimation(
                animator.GetInteger(AnimatorUtils.OBJECT_ID), parameterName);
    }

    private void RecreateAnimationCurve(AnimationCurve animationCurve, float timeAtk, float timeAnimation)
    {
        Keyframe startKey = new Keyframe(0f, 0f);
        Keyframe endKey = new Keyframe(timeAtk, timeAnimation);
        //default to sword slow down
        Keyframe slowDownAttackKey = new Keyframe(0.07511136f, 0.4373413f);

        animationCurve.AddKey(startKey);
        animationCurve.AddKey(slowDownAttackKey);
        animationCurve.AddKey(endKey);
    }
}
