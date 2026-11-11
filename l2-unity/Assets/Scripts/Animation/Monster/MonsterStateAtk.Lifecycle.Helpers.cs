public partial class MonsterStateAtk : MonsterStateBase
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

    }

    bool TryFinishSkill(AnimatorStateInfo stateInfo)
    {
        if (_entity == null || _entity.ActionSlot.Action != EntityActionKind.Skill)
            return false;

        if (!_loggedSkillHold && stateInfo.normalizedTime >= 1f)
        {
            _loggedSkillHold = true;
            // MobSkillLog.ClipEnd(_entity);
        }

        if (_isSwitchIdle || stateInfo.normalizedTime < 1f || _entity.IsDead())
        {
            if (!_loggedSkillStuck &&
                _isSwitchIdle &&
                stateInfo.IsName("atk01") &&
                stateInfo.normalizedTime >= 1.05f)
            {
                _loggedSkillStuck = true;
                // MobSkillLog.Stuck(_entity);
            }
            return true;
        }
        if (EntityActionPacketServices.Actions == null)
            return true;

        string restore = (_entity.Identity != null && AnimationPacketServices.Moves != null && AnimationPacketServices.Moves.IsMoving(_entity.Identity.Id)) ? (_entity.Running ? "run" : "walk") : "wait";
        _isSwitchIdle = true;
        EntityActionPacketServices.Actions.Set(_entity, EntityActionKind.Idle, restore);
        // MobSkillLog.Done(_entity, restore);
        return true;
    }

    private void SwitchToIdle(AnimatorStateInfo stateInfo)
    {
        float currentNormalizedTime = stateInfo.normalizedTime;

        if (_isSwitchIdle || currentNormalizedTime <= 0.9f)
            return;
        if (_entity == null || _entity.IsDead() || EntityActionPacketServices.Actions == null)
            return;
        if (_entity.ActionSlot.Action != EntityActionKind.Attack)
            return;
        if (IsStaleSwing())
            return;

        _isSwitchIdle = true;
        EntityActionPacketServices.Actions.Set(_entity, EntityActionKind.Idle, null);
    }
}
