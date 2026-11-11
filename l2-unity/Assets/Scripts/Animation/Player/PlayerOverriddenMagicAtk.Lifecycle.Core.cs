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
}
