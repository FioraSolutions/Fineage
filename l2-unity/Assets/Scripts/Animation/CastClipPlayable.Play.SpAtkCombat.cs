using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
    private const float SpAtkSwitchToIdleNorm = 0.92f;

    /// <summary>
    /// Plays a SpAtk swing clip on the cast graph when the animator template has no state.
    /// Dispatches clip events (AttackShot, …) and completes the skill await with
    /// <paramref name="phaseStateName"/>.
    /// </summary>
    public bool TryPlaySpAtkCombat(AnimationClip clip, string phaseStateName)
    {
        if (!IsGraphAlive)
            Bind(_owner, _animator);

        if (!IsGraphAlive || _animator == null || clip == null || string.IsNullOrEmpty(phaseStateName))
            return false;

        _objectId = _animator.GetInteger(AnimatorUtils.OBJECT_ID);
        _finishName = phaseStateName;
        _finalShot = false;
        _oneShot = false;
        _spAtkCombat = true;
        _stateIndex = 1;
        _castData = null;
        _speed = ResolveSpAtkPlaybackSpeed(_objectId, clip, phaseStateName);
        _clipLength = clip.length;
        _enterTime = Time.time;
        _lastSample = 0f;
        _finished = false;
        _events = clip.events;
        _eventFired = _events != null && _events.Length > 0 ? new bool[_events.Length] : null;

        if (_clipPlayable.IsValid())
        {
            _mixer.DisconnectInput(1);
            _graph.DestroyPlayable(_clipPlayable);
        }

        _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
        _clipPlayable.SetApplyFootIK(false);
        _clipPlayable.SetSpeed(_speed);
        _graph.Connect(_clipPlayable, 0, _mixer, 1);
        _mixer.SetInputWeight(0, 0f);
        _mixer.SetInputWeight(1, 1f);
        _active = true;

        if (Animations != null)
            Animations.SetPAtkSpeed(_objectId, _speed);

        MagicGaitTrace.EventId(_objectId, "CastClip.TryPlaySpAtkCombat",
            "phase=" + phaseStateName +
            " clip=" + clip.name +
            " len=" + _clipLength.ToString("F3") +
            " speed=" + _speed.ToString("F3"));
        return true;
    }

    float ResolveSpAtkPlaybackSpeed(int objectId, AnimationClip clip, string phaseStateName)
    {
        float hitCenter = 0.35f;
        if (_events != null)
        {
            float startHit = 0f;
            float endHit = 0f;
            for (int i = 0; i < _events.Length; i++)
            {
                if (_events[i].functionName == "OnAnimationHit")
                    startHit = _events[i].time;
                else if (_events[i].functionName == "OnAnimationAttackHitEnd")
                    endHit = _events[i].time;
            }

            if (startHit > 0.01f || endHit > 0.01f)
                hitCenter = (startHit + endHit) * 0.5f;
        }

        int serverTimeMs = _animator != null ? _animator.GetInteger("sptimeatk") : 0;
        if (serverTimeMs <= 0)
        {
            World world = WorldApplyPacketServices.GameWorld;
            Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
            MagicCastData cast = entity != null ? entity.GetMagicCastData() : null;
            if (cast != null && cast.HitTime > 0f)
                serverTimeMs = Mathf.RoundToInt(cast.HitTime * 1000f);
        }

        if (PlayerStateSpAtk.IsSpAtk06Gesture(phaseStateName) && serverTimeMs > 0)
        {
            float gestureSeconds = (serverTimeMs / 1000f) - PlayerStateSpAtk.HitTimeCompensationSeconds;
            if (gestureSeconds >= PlayerStateSpAtk.MinServerHitSeconds)
            {
                float targetLength = clip.length * PlayerStateSpAtk.SwitchToIdleNormalizedTime;
                return targetLength / gestureSeconds;
            }
        }
        else if (serverTimeMs > 0 && hitCenter > 0.01f)
        {
            float serverHit = (serverTimeMs / 1000f) - PlayerStateSpAtk.HitTimeCompensationSeconds;
            if (serverHit < PlayerStateSpAtk.MinServerHitSeconds)
                serverHit = PlayerStateSpAtk.MinServerHitSeconds;
            return hitCenter / serverHit;
        }

        World world = WorldApplyPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(objectId) : null;
        if (Animations != null && entity != null)
            return Animations.ApplyLinearMeleePAtkSpeed(objectId, phaseStateName, clip.length);

        return AnimPlayback.AuthoredRate;
    }

    bool SpAtkCombatPhaseDone(float clipTime)
    {
        if (_clipLength <= 0.0001f)
            return true;

        float norm = clipTime / _clipLength;
        float localElapsed = Time.time - _enterTime;
        float wallNeed = _clipLength / Mathf.Max(0.0001f, _speed);
        return norm >= SpAtkSwitchToIdleNorm || localElapsed >= wallNeed;
    }
}