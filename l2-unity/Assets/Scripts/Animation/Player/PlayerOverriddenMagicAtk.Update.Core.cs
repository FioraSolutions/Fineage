using UnityEngine;

public partial class PlayerOverriddenMagicAtk
{
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_castData == null)
        {
            // Without castData still allow phase finish / idle so runner is not stuck forever.
            if (!_phaseFinished)
            {
                TryFinishPhase(animator, stateInfo, globalElapsed: -1f);
            }
            return;
        }


        float localElapsed = Time.time - _stateEnterTime;
        float globalElapsed = Time.time - _castData.StartTime;

        if (!_forcedShotTriggered && stateIndex == 1 && !isFinalShotState && ShouldUseForceSync(_castData))
        {
            float rawShootAt = Mathf.Max(0.01f, _castData.serverTimeToShoot);
            float shotEventLeadTime = Mathf.Max(0f, _castData.shotEventTime);
            float compensatedShootAt = Mathf.Max(0.01f, rawShootAt - shotEventLeadTime - ShotBlendCompensationSeconds);
            // Debug.Log(
                // $"[MAGIC_CHECK] now={Time.time:F3} state={parameterName} idx={stateIndex} " +
                // $"global={globalElapsed:F3} rawShootAt={rawShootAt:F3} shotEventLead={shotEventLeadTime:F3} compShootAt={compensatedShootAt:F3} " +
                // $"ready={(globalElapsed >= compensatedShootAt)} forced={_forcedShotTriggered}");
            if (globalElapsed >= compensatedShootAt)
            {
                _forcedShotTriggered = true;
                // Global animator slowdown (CastEnd speed) also slows state transitions.
                // Restore normal speed right before forcing MagicShot to avoid delayed transition.
                float speedBeforeForce = animator.speed;
                animator.speed = 1.0f;
                // Debug.Log(
                    // $"{MagicSpeedTraceTag} FORCE_SYNC state={parameterName} global={globalElapsed:F3}s " +
                    // $"speedBeforeForce={speedBeforeForce:F3} speedAfterForce={animator.speed:F3} rawShootAt={rawShootAt:F3}s compShootAt={compensatedShootAt:F3}s");
                int objectId = animator.GetInteger(AnimatorUtils.OBJECT_ID);
                if (MagicShotCoordinator.TryStartShot(objectId, _castData, ShotSourceForceSync, out string coordinatorMessage))
                {
                    animator.ResetTrigger(TriggerCastMid);
                    animator.ResetTrigger(TriggerCastEnd);
                    animator.ResetTrigger(TriggerMagicShot);
                    animator.SetTrigger(TriggerMagicShot);
                    // Debug.Log(
                        // $"[MAGIC_TRIGGER] now={Time.time:F3} global={globalElapsed:F3} " +
                        // $"targetRaw={rawShootAt:F3} shotEventLead={shotEventLeadTime:F3} targetComp={compensatedShootAt:F3} " +
                        // $"deltaRaw={(globalElapsed - rawShootAt):F3} deltaComp={(globalElapsed - compensatedShootAt):F3} " +
                        // $"castDataHash={_castData.GetHashCode()} animId={animator.GetInstanceID()} objectId={objectId}");
                    // Debug.Log($"{coordinatorMessage} action=run");
                }
                else
                {
                    // Debug.Log($"{coordinatorMessage} action=skip");
                }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // Debug.Log(
                    // $"[ForceShotSync] Triggered MagicShot from CastEnd at global={globalElapsed:F3}s " +
                    // $"targetRaw={rawShootAt:F3}s shotEventLead={shotEventLeadTime:F3}s targetComp={compensatedShootAt:F3}s comp={ShotBlendCompensationSeconds:F3}s " +
                    // $"deltaRaw={globalElapsed - rawShootAt:F3}s deltaComp={globalElapsed - compensatedShootAt:F3}s " +
                    // $"animatorSpeedNow={animator.speed:F3} animatorInstanceId={animator.GetInstanceID()}.");
#endif
            }
        }

        // Проверяем, наступил ли момент выстрела (только для финального стейта)
        if (isFinalShotState && !_eventLogged)
        {
            // Вычисляем текущее время ВНУТРИ клипа с учетом скорости
            float localElapsed1 = Time.time - _stateEnterTime;
            float currentClipTime = localElapsed1 * animator.speed;

            // Если мы прошли точку ивента (например, 0.541с)
            if (currentClipTime >= _eventTimeInClip)
            {
                _eventLogged = true;
                // Debug.Log($"<color=cyan>[FIRE_SYNC]</color> ВЫСТРЕЛ! " +
                          // $"Global: {globalElapsed:F3}s (Цель: {_castData.HitTime - _castData.FlightTime:F3}s) | " +
                          // $"Разница: {globalElapsed - (_castData.HitTime - _castData.FlightTime):F4}s");
                // Debug.Log(
                    // $"{MagicSpeedTraceTag} FIRE_SYNC state={parameterName} local={localElapsed1:F3}s clipTime={currentClipTime:F3}s " +
                    // $"eventTime={_eventTimeInClip:F3}s animSpeed={animator.speed:F3}");
            }
        }

        if (!_phaseFinished)
        {
            TryFinishPhase(animator, stateInfo, globalElapsed);
        }
    }
}
