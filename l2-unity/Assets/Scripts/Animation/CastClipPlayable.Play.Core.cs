using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public sealed partial class CastClipPlayable
{
    public bool TryPlay(string overrideAnimName, string phaseTriggerName)
    {
        // Only the first cast builds the graph. Later casts reuse it.
        if (!IsGraphAlive)
        {
            Bind(_owner, _animator);
        }

        if (!IsGraphAlive || _animator == null || string.IsNullOrEmpty(overrideAnimName))
        {
            if (SkillAnimationRunner.Trace)
            {
                Debug.Log(
                    "[SUMMON_ANIM] TryPlay reject graph=" + IsGraphAlive +
                    " animator=" + (_animator != null) +
                    " override='" + overrideAnimName + "'" +
                    " phase=" + phaseTriggerName);
            }
            return false;
        }

        string raceName = _animator.runtimeAnimatorController != null
            ? _animator.runtimeAnimatorController.name
            : null;
        AnimationClip clip = SkillAnimationDatabase.GetOverrideClip(overrideAnimName, raceName);
        if (clip == null)
        {
            if (SkillAnimationRunner.Trace)
            {
                Debug.Log(
                    "[SUMMON_ANIM] TryPlay clip missing race=" + raceName +
                    " key=" + overrideAnimName + "_" + raceName +
                    " phase=" + phaseTriggerName);
            }
            return false;
        }

        _stateIndex = StateIndex(phaseTriggerName);
        _finalShot = _stateIndex == 2;
        _objectId = _animator.GetInteger(AnimatorUtils.OBJECT_ID);
        _castData = EntityActionSkill.ResolveCastData(_objectId);
        _speed = PhaseSpeed(_castData, _stateIndex);
        _clipLength = clip.length;
        _enterTime = Time.time;
        _lastSample = 0f;
        _finished = false;
        _finishName = SkillAnimationDatabase.ResolveOverrideCompletionEventName(phaseTriggerName);
        _events = clip.events;
        _eventFired = _events != null && _events.Length > 0 ? new bool[_events.Length] : null;

        if (_finalShot)
        {
            MagicShotCoordinator.TryStartShot(_objectId, _castData, "playable", out _);
        }

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
        float sinceCast = _castData != null ? Time.time - _castData.StartTime : -1f;
        float wallPlay = _speed > 0.0001f ? _clipLength / _speed : 0f;
        float barRem = CastBarOverlay.Remaining01;
        Debug.Log(
            "[CastSync] ANIM.phase t=" + sinceCast.ToString("F3") +
            " trigger=" + phaseTriggerName +
            " override=" + overrideAnimName +
            " clip=" + clip.name +
            " len=" + _clipLength.ToString("F3") +
            " speed=" + _speed.ToString("F3") +
            " wallPlay=" + wallPlay.ToString("F3") +
            " idx=" + _stateIndex +
            " animShotAt=" + (_castData != null ? _castData.AnimationShotAtSeconds.ToString("F3") : "-") +
            " sfxShoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("F3") : "-") +
            " barRemainPct=" + (barRem < 0f ? -1f : barRem * 100f).ToString("F0"));
        MagicGaitTrace.EventId(_objectId, "CastClip.TryPlay",
            "phase=" + phaseTriggerName +
            " override=" + overrideAnimName +
            " clip=" + clip.name +
            " len=" + _clipLength.ToString("F3") +
            " speed=" + _speed.ToString("F3") +
            " finalShot=" + _finalShot);
        return true;
    }

    public bool PlayOneShot(AnimationClip clip)
    {
        if (!IsGraphAlive)
            Bind(_owner, _animator);
        if (!IsGraphAlive || clip == null)
            return false;
        if (_clipPlayable.IsValid())
        {
            _mixer.DisconnectInput(1);
            _graph.DestroyPlayable(_clipPlayable);
        }
        _clipPlayable = AnimationClipPlayable.Create(_graph, clip);
        _clipPlayable.SetApplyFootIK(false);
        _clipPlayable.SetSpeed(1f);
        _graph.Connect(_clipPlayable, 0, _mixer, 1);
        _mixer.SetInputWeight(0, 0f);
        _mixer.SetInputWeight(1, 1f);
        _oneShot = true;
        _speed = 1f;
        _clipLength = clip.length;
        _enterTime = Time.time;
        _lastSample = 0f;
        _finalShot = false;
        _events = null;
        _eventFired = null;
        _finished = false;
        _active = true;
        return true;
    }
}
