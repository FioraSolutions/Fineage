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
        _events = clip.events;
        _eventFired = _events != null && _events.Length > 0 ? new bool[_events.Length] : null;
        bool castTimed = TryPeekCastPhaseSpeed(out float castSpeed);
        _speed = castTimed ? castSpeed : MissingCastTimingSpeed(_objectId, phaseStateName);
        _clipLength = clip.length;
        _enterTime = Time.time;
        _lastSample = 0f;
        _finished = false;

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

        if (castTimed && _castData != null)
            _castData.CommitSpAtkPhase();

        MagicGaitTrace.EventId(_objectId, "CastClip.TryPlaySpAtkCombat",
            "phase=" + phaseStateName +
            " clip=" + clip.name +
            " len=" + _clipLength.ToString("F3") +
            " speed=" + _speed.ToString("F3"));
        return true;
    }

    bool TryPeekCastPhaseSpeed(out float speed)
    {
        speed = 0f;
        World world = WorldApplyPacketServices.GameWorld;
        if (world == null)
            world = ControllerPacketServices.GameWorld;
        Entity entity = world != null ? world.GetEntityNoLockSync(_objectId) : null;
        MagicCastData cast = entity != null ? entity.GetMagicCastData() : null;
        if (cast == null || cast.SkillId <= 0 || cast.HitTime <= 0.01f)
            return false;

        int phase = cast.PeekSpAtkPhase();
        _stateIndex = phase;
        _castData = cast;
        speed = cast.SpeedForSpAtkPhase(phase);
        return speed > 0.0001f;
    }

    float MissingCastTimingSpeed(int objectId, string phaseStateName)
    {
        Debug.LogError(
            "[CastSync] SpAtk playback missing cast timing objectId=" + objectId +
            " phase=" + phaseStateName +
            " skillId=0 — SetupDuration was not run for this cast");
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