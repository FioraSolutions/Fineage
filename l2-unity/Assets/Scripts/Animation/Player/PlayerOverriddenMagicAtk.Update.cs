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

    private void TryFinishPhase(Animator animator, AnimatorStateInfo stateInfo, float globalElapsed)
    {
        if (_phaseFinished)
        {
            return;
        }

        float localElapsed = Time.time - _stateEnterTime;
        float speed = Mathf.Max(0.0001f, animator.speed);
        float wallNeed = _clipLengthAtEnter / speed;
        bool wallDone = localElapsed >= wallNeed;
        float globalWall = _castData != null
            ? _castData.GetPhaseEndWallSeconds(stateIndex)
            : 0f;
        // Unity applies the transition after this update. Request the final
        // shot state one blend lead early so its AttackShot notify lands on
        // the client's absolute f514 wall instead of one transition late.
        if (stateIndex == 1 && globalWall > ShotBlendCompensationSeconds)
        {
            globalWall -= ShotBlendCompensationSeconds;
        }
        bool globalWallDone =
            globalElapsed >= 0f &&
            globalWall > 0f &&
            globalElapsed >= globalWall;
        bool normDone = stateInfo.normalizedTime >= PhaseDoneNormalizedTime;
        bool phaseDone = wallDone || normDone || globalWallDone;

        bool shouldDiag =
            phaseDone ||
            _lastExitDiagLogTime < 0f ||
            (localElapsed - _lastExitDiagLogTime) >= 0.5f;
        if (shouldDiag)
        {
            _lastExitDiagLogTime = localElapsed;
            // Debug.Log(
                // $"{MagicExitLogTag} CHECK state={parameterName} idx={stateIndex} final={isFinalShotState} " +
                // $"local={localElapsed:F3}s global={globalElapsed:F3}s " +
                // $"norm={stateInfo.normalizedTime:F3} (need>={PhaseDoneNormalizedTime:F2}) normDone={normDone} " +
                // $"speed={animator.speed:F3} clipLen={_clipLengthAtEnter:F3} wallNeed={wallNeed:F3}s wallDone={wallDone} " +
                // $"globalWall={globalWall:F3}s globalWallDone={globalWallDone} " +
                // $"alreadyFinished={_phaseFinished} gateWillFire={phaseDone}");
        }

        if (!phaseDone)
        {
            return;
        }

        _phaseFinished = true;
        int objectId = animator.GetInteger(AnimatorUtils.OBJECT_ID);
        // Debug.Log(
            // $"{MagicExitLogTag} PHASE_DONE state={parameterName} idx={stateIndex} final={isFinalShotState} " +
            // $"local={localElapsed:F3}s global={globalElapsed:F3}s norm={stateInfo.normalizedTime:F3} " +
            // $"wallDone={wallDone} globalWall={globalWall:F3}s globalWallDone={globalWallDone} " +
            // $"→ NotifyMagicPhaseFinished");

        if (Animations != null)
        {
            Animations.NotifyMagicPhaseFinished(objectId, parameterName);
        }

        if (isFinalShotState)
        {
            SwitchToIdle();
        }
    }

}
