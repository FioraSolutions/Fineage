public partial class MonsterStateAtk : MonsterStateBase
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        _isSwitchIdle = false;
        _loggedSkillHold = false;
        _loggedSkillStuck = false;
        _startTime = Time.time;

        LoadComponents(animator);
        _swingId = _entity != null ? _entity.ActionSlot.AttackSwingId : 0;

        // EnterEventgrp plays atk01 when a summon appears (strider 12527 and the same state).
        // That is not a combat swing. PAtk speed is still 0, so the attack curve writes
        // patkspd=0 and the body freezes on the first frame. Keep patkspd at 1 here.
        // ReturnAppearToWait sends the one-shot back to wait; Attack/Skill keep the curve.
        if (!IsCombatSwing())
        {
            if (_networkAnimationController != null)
                _networkAnimationController.SetPAtkSpeed(1f);
            StopAnimationTrigger(animator, parameterName);
            return;
        }

        AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(0);

        if (clipInfos == null || clipInfos.Length == 0)
        {
            clipInfos = animator.GetCurrentAnimatorClipInfo(0);
        }

        _animationCurve = new AnimationCurve();
        //float test = CalcBaseParam.CalculateTimeL2j(_entity.Stats.PAtkRealSpeed);
        float timeAtk = CalcBaseParam.CalculateTimeL2j(_entity.Stats.PAtkRealSpeed);
        _endTime = TimeUtils.ConvertMsToSec(timeAtk);
        float timeAnimation = clipInfos[0].clip.length;
        RecreateAnimationCurve(_animationCurve, _endTime, timeAnimation);

        // Debug.Log("Calc Time Monster Atk>>> All Time Animation " + timeAnimation + " timeAtk " + timeAtk);
        _networkAnimationController.UpdateAnimatorAtkSpeedL2j(timeAtk, timeAnimation);
        if (_entity != null && _entity.ActionSlot.Action == EntityActionKind.Skill)
        {
            // MobSkillLog.Enter(_entity, timeAnimation, timeAtk, animator.GetFloat("patkspd"));
        }
        _lastNormalizedTime = 0f;
        if (audioHandler != null)
        {
            audioHandler.PlaySoundAtRatio(MonsterSoundEvent.Atk, audioHandler.AtkRatio);
            audioHandler.PlaySoundAtRatio(MonsterSoundEvent.Swish, audioHandler.SwishRatio);
        }
        StopAnimationTrigger(animator, parameterName);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (TryFinishSkill(stateInfo))
            return;
        if (!IsCombatSwing())
        {
            ReturnAppearToWait(animator, stateInfo, layerIndex);
            return;
        }
        if (IsStaleSwing())
            return;

        if (audioHandler != null && (stateInfo.normalizedTime - _lastNormalizedTime) >= 1f)
        {
            _lastNormalizedTime = stateInfo.normalizedTime;
            audioHandler.PlaySoundAtRatio(MonsterSoundEvent.Atk, audioHandler.AtkRatio);
            audioHandler.PlaySoundAtRatio(MonsterSoundEvent.Swish, audioHandler.SwishRatio);
        }

        float currentTime = Time.time;
        float timeOut = currentTime - _startTime;

        if (timeOut >= _endTime) SwitchToIdle(stateInfo);

        float normalizedTime = timeOut / _endTime;
        float speed = _animationCurve.Evaluate(normalizedTime);
        _networkAnimationController.SetPAtkSpd(speed);
    }

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
