using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// Plays a race cast clip on top of the existing animator controller.
/// Input 1 is the current CastMid / CastEnd / MagicShot clip. Swapping that
/// input does not write AnimatorOverrideController, so the controller is not rebuilt.
/// The graph is created once per body and kept. Rebuilding it per cast stalls
/// a crowd of CharInfo. While it is bound, the mesh is the mixer: input 0 is
/// this controller playable, input 1 is the cast clip. Locomotion must be
/// written here as well as on the Animator — the Animator crossfade does not
/// move this playable, so the body stays on the pose from Bind.
/// </summary>
public sealed class CastClipPlayable
{
    static IAnimationManager Animations => AnimationPacketServices.Animations;

    private const float PhaseDoneNormalizedTime = 1f;

    private BaseAnimationController _owner;
    private Animator _animator;
    private PlayableGraph _graph;
    private AnimationMixerPlayable _mixer;
    private AnimatorControllerPlayable _controllerPlayable;
    private AnimationClipPlayable _clipPlayable;
    private bool _ready;

    private bool _active;
    private bool _finished;
    private bool _finalShot;
    private int _stateIndex;
    private int _objectId;
    private float _speed = 1f;
    private float _clipLength;
    private float _enterTime;
    private float _lastSample;
    private string _finishName;
    private MagicCastData _castData;
    private AnimationEvent[] _events;
    private bool[] _eventFired;

    public void Bind(BaseAnimationController owner, Animator animator)
    {
        Dispose();
        _owner = owner;
        _animator = animator;
        if (owner == null || animator == null || animator.runtimeAnimatorController == null)
        {
            return;
        }

        _graph = PlayableGraph.Create(animator.name + ".CastClip");
        _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        _mixer = AnimationMixerPlayable.Create(_graph, 2);
        _controllerPlayable = AnimatorControllerPlayable.Create(_graph, animator.runtimeAnimatorController);
        _graph.Connect(_controllerPlayable, 0, _mixer, 0);
        _mixer.SetInputWeight(0, 1f);
        _mixer.SetInputWeight(1, 0f);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(_graph, "CastClip", animator);
        output.SetSourcePlayable(_mixer);
        _graph.Play();
        _ready = true;
        MatchAnimatorState();
    }

    /// <summary>Graph already exists. TryPlay must not Bind again on later casts.</summary>
    public bool IsGraphAlive => _ready && _graph.IsValid() && _controllerPlayable.IsValid();

    public void CrossFadeInFixedTime(int stateHash, float fixedDuration, int layer, float fixedTimeOffset)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.CrossFadeInFixedTime(stateHash, fixedDuration, layer, fixedTimeOffset);
    }

    public void CrossFade(int stateHash, float duration, int layer)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.CrossFade(stateHash, duration, layer, 0f);
    }

    public void SetFloat(string name, float value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetFloat(name, value);
    }

    public void SetBool(string name, bool value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetBool(name, value);
    }

    public void SetInteger(string name, int value)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetInteger(name, value);
    }

    public void SetTrigger(string name)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.SetTrigger(name);
    }

    public void ResetTrigger(string name)
    {
        if (!IsGraphAlive || string.IsNullOrEmpty(name))
            return;
        _controllerPlayable.ResetTrigger(name);
    }

    public void SetControllerSpeed(float value)
    {
        if (!IsGraphAlive)
            return;
        _controllerPlayable.SetSpeed(value);
    }

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

    public bool IsActive => _active;

    bool _oneShot;

    /// <summary>Plain one-shot (lobby social victory). No cast events / magic shot bookkeeping.</summary>
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

    public void Stop()
    {
        _oneShot = false;
        MagicGaitTrace.EventId(_objectId, "CastClip.Stop",
            "finishName=" + _finishName + " finalShot=" + _finalShot + " keepGraph=True");
        _active = false;
        _finished = true;
        if (!_ready)
        {
            return;
        }

        _mixer.SetInputWeight(0, 1f);
        _mixer.SetInputWeight(1, 0f);
        if (_clipPlayable.IsValid())
        {
            _clipPlayable.SetSpeed(0f);
        }

        if (_animator != null)
        {
            _animator.speed = 1f;
        }

        if (_controllerPlayable.IsValid())
        {
            _controllerPlayable.SetSpeed(1f);
        }
    }

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

    public void Dispose()
    {
        _active = false;
        _finished = true;
        _ready = false;
        if (_graph.IsValid())
        {
            _graph.Destroy();
        }

        _clipPlayable = default;
        _controllerPlayable = default;
        _mixer = default;
    }

    void MatchAnimatorState()
    {
        if (!_controllerPlayable.IsValid() || _animator == null)
        {
            return;
        }

        AnimatorControllerParameter[] parameters = _animator.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    _controllerPlayable.SetFloat(parameter.nameHash, _animator.GetFloat(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Int:
                    _controllerPlayable.SetInteger(parameter.nameHash, _animator.GetInteger(parameter.nameHash));
                    break;
                case AnimatorControllerParameterType.Bool:
                    _controllerPlayable.SetBool(parameter.nameHash, _animator.GetBool(parameter.nameHash));
                    break;
            }
        }

        _controllerPlayable.SetSpeed(1f);
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        _controllerPlayable.Play(state.fullPathHash, 0, state.normalizedTime);
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
