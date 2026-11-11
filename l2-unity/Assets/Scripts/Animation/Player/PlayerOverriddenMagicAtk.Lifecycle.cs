using UnityEngine;

public partial class PlayerOverriddenMagicAtk : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _isSwitchIdle = false;
        _phaseFinished = false;
        _forcedShotTriggered = false;
        _stateEnterTime = Time.time;
        _lastExitDiagLogTime = -1f;

        _objectId = animator.GetInteger(AnimatorUtils.OBJECT_ID);
        _castData = EntityActionSkill.ResolveCastData(_objectId);
        float targetSpeed = 1.0f;
        if (_castData != null)
        {
            if (stateIndex == 0) targetSpeed = _castData.SpeedMid;
            else if (stateIndex == 1) targetSpeed = _castData.SpeedEnd;
            else if (stateIndex == 2) targetSpeed = _castData.SpeedShot;
        }

        animator.speed = targetSpeed;

        // ПОЛУЧАЕМ ВРЕМЯ ИВЕНТА
        AnimationClip clip = AnimationDataCache.GetActiveClip(animator, layerIndex);
        _clipLengthAtEnter = clip != null ? clip.length : stateInfo.length;
        if (clip != null)
        {
            _eventTimeInClip = AnimationDataCache.GetEventTimeByName(animator, clip, "OnAnimationShoot");
        }

        StopAnimationTrigger(animator, parameterName);
        _eventLogged = false; // Сбрасываем флаг при входе в стейт

        // Debug.Log(
            // $"[MAGIC_ENTER] state={parameterName} idx={stateIndex} final={isFinalShotState} " +
            // $"now={Time.time:F3} castDataNull={(_castData == null)} " +
            // $"castDataHash={(_castData != null ? _castData.GetHashCode().ToString() : "null")} " +
            // $"start={(_castData != null ? _castData.StartTime.ToString("F3") : "null")} " +
            // $"hit={(_castData != null ? _castData.HitTime.ToString("F3") : "null")} " +
            // $"flight={(_castData != null ? _castData.FlightTime.ToString("F3") : "null")} " +
            // $"shootAt={(_castData != null ? _castData.serverTimeToShoot.ToString("F3") : "null")} " +
            // $"globalAtEnter={(_castData != null ? (Time.time - _castData.StartTime).ToString("F3") : "null")} " +
            // $"animSpeed={animator.speed:F3} clipLen={_clipLengthAtEnter:F3} " +
            // $"exitGate=wall|norm>={PhaseDoneNormalizedTime:F2}→NotifyPhase{(isFinalShotState ? "+SwitchToIdle" : "")} " +
            // $"animId={animator.GetInstanceID()}");

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Only reset animator.speed when leaving MagicShot (stateIndex 2). Intermediate exits
        // (CastMid / CastEnd) must not set speed=1: Unity can call the next state's OnStateEnter
        // before the previous state's OnStateExit, so Exit would overwrite SpeedShot and
        // OnAnimationShoot runs at animSpeed=1 (FIRE_SYNC vs CastTimingSetup mismatch).
        if (stateIndex == 2)
        {
            animator.speed = 1.0f;
        }

        float finalLocalTime = Time.time - _stateEnterTime;
        float globalSinceCastStart = (_castData != null) ? (Time.time - _castData.StartTime) : -1f;
        // Debug.Log(
            // $"[MAGIC_EXIT] state={parameterName} idx={stateIndex} now={Time.time:F3} " +
            // $"globalAtExit={(_castData != null ? (Time.time - _castData.StartTime).ToString("F3") : "null")} " +
            // $"normalized={stateInfo.normalizedTime:F3} animSpeed={animator.speed:F3} animId={animator.GetInstanceID()}");
        // Debug.Log(
            // $"[AnimLog] EXIT {parameterName} | Total Local: {finalLocalTime:F3}s | " +
            // $"Global: {globalSinceCastStart:F3}s | normalized: {stateInfo.normalizedTime:F3} | " +
            // $"Layer: {layerIndex} | Animator: {animator.GetInstanceID()}");

    }

    private void SwitchToIdle()
    {
        if (_isSwitchIdle)
        {
            return;
        }

        _isSwitchIdle = true;
        float localElapsed = Time.time - _stateEnterTime;
        MagicGaitTrace.EventId(_objectId, "SMB.MagicShot.SwitchToIdle",
            "state=" + parameterName + " local=" + localElapsed.ToString("F3") +
            " (animator path, not CastClip)");

        if (!EntityActionSkill.IsLocalPlayer(_objectId))
        {
            EntityActionSkill.FinishRemoteCast(_objectId);
            return;
        }

        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.IsAttack = false;
        }

        // MOVE_TO after Ice Bolt already started walk/run. Do not pull that back into atkwait.
        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
        {
            // Debug.Log(
                // $"{MagicExitLogTag} SwitchToIdle SKIP_LOCOMOTION state={parameterName} " +
                // $"sm={PlayerStatePacketServices.PlayerState.State} intention={PlayerStatePacketServices.PlayerState.Intention}");
            return;
        }

        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }
}
