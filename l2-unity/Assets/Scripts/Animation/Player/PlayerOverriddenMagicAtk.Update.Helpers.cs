using UnityEngine;

public partial class PlayerOverriddenMagicAtk
{
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
