using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
    public void Tick()
    {
        if (!_active || _finished || !_clipPlayable.IsValid())
        {
            return;
        }

        if (_oneShot)
        {
            if (Time.time - _enterTime >= _clipLength)
                Stop();
            return;
        }

        float clipTime = (float)_clipPlayable.GetTime();
        if (clipTime + 0.0001f < _lastSample)
        {
            clipTime = _clipLength;
        }

        _lastSample = clipTime;
        DispatchEvents(clipTime);

        float localElapsed = Time.time - _enterTime;
        float speed = Mathf.Max(0.0001f, _speed);
        float wallNeed = _clipLength / speed;
        bool wallDone = localElapsed >= wallNeed;
        float norm = _clipLength > 0.0001f ? clipTime / _clipLength : 1f;
        bool normDone = norm >= PhaseDoneNormalizedTime;
        if (!wallDone && !normDone)
        {
            return;
        }

        bool finishedShot = _finalShot;
        _finished = true;
        if (_clipPlayable.IsValid())
        {
            _clipPlayable.SetSpeed(0f);
        }

        MagicGaitTrace.EventId(_objectId, "CastClip.phaseDone",
            "finishName=" + _finishName +
            " finalShot=" + finishedShot +
            " norm=" + norm.ToString("F3") +
            " wallDone=" + wallDone);

        if (Animations != null)
        {
            Animations.NotifyMagicPhaseFinished(_objectId, _finishName);
        }

        if (finishedShot)
        {
            SwitchToIdle();
            Stop();
        }
    }

    private void DispatchEvents(float clipTime)
    {
        if (_events == null || _eventFired == null || _owner == null)
        {
            return;
        }

        for (int i = 0; i < _events.Length; i++)
        {
            if (_eventFired[i] || clipTime < _events[i].time)
            {
                continue;
            }

            _eventFired[i] = true;
            string payload = _events[i].stringParameter;
            switch (_events[i].functionName)
            {
                case "OnAnimationShoot":
                    _owner.OnAnimationShoot(payload);
                    break;
                case "OnAnimationComplete":
                    _owner.OnAnimationComplete(payload);
                    break;
                case "AttackShot":
                    _owner.AttackShot(payload);
                    break;
                case "OnAnimationHit":
                    _owner.OnAnimationHit(payload);
                    break;
                case "OnAnimationAttackHitEnd":
                    _owner.OnAnimationAttackHitEnd(payload);
                    break;
                case "OnAnimationLoadArrow":
                    _owner.OnAnimationLoadArrow(payload);
                    break;
            }
        }
    }

    private void SwitchToIdle()
    {
        MagicGaitTrace.EventId(_objectId, "CastClip.SwitchToIdle",
            "local=" + EntityActionSkill.IsLocalPlayer(_objectId));
        if (!EntityActionSkill.IsLocalPlayer(_objectId))
        {
            EntityActionSkill.FinishRemoteCast(_objectId);
            return;
        }

        if (DomainPacketServices.LocalPlayer != null)
        {
            DomainPacketServices.LocalPlayer.IsAttack = false;
        }

        if (PlayerStatePacketServices.PlayerState != null && PlayerStatePacketServices.PlayerState.ShouldKeepLocomotion())
        {
            return;
        }

        if (PlayerStatePacketServices.PlayerState == null)
        {
            return;
        }

        PlayerStatePacketServices.PlayerState.ChangeIntention(Intention.INTENTION_IDLE);
        PlayerStatePacketServices.PlayerState.NotifyEvent(Event.WAIT_RETURN, NewIdleState.WaitReturnFromCombatSmb);
    }

    private static int StateIndex(string triggerName)
    {
        if (string.IsNullOrEmpty(triggerName))
        {
            return 1;
        }

        if (triggerName.StartsWith("CastMid", System.StringComparison.Ordinal))
        {
            return 0;
        }

        if (triggerName.StartsWith("MagicShot", System.StringComparison.Ordinal))
        {
            return 2;
        }

        return 1;
    }

    private static float PhaseSpeed(MagicCastData castData, int stateIndex)
    {
        if (castData == null)
        {
            return 1f;
        }

        if (stateIndex == 0)
        {
            return castData.SpeedMid;
        }

        if (stateIndex == 1)
        {
            return castData.SpeedEnd;
        }

        return castData.SpeedShot;
    }
}
