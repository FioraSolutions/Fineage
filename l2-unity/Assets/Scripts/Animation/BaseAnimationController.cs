using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController
{
    [SerializeField] protected Animator _animator;
    [SerializeField] protected bool _resetStateOnReceive = false;
    [SerializeField] protected float _spAtk01ClipLength = 1000;
    [SerializeField] protected Dictionary<string, float> _atkClipLengths;
    private string _lastAnimationVariableName;
    private float _lastAtkClipLength;
    private float _pAtkSpd;

    private readonly Dictionary<string, string> _base_motion = new Dictionary<string, string>(3);

    private const string BASE_MOTION_CAST_MID = "FDarkElf_m001_b.ao_CastMid_FDarkElf";
    private const string BASE_MOTION_END = "FDarkElf_m001_b.ao_CastEnd_FDarkElf";
    private const string BASE_MOTION_MAGIC_SHOOT = "FDarkElf_m001_b.ao_MagicShot_A_FDarkElf";

    private AnimatorOverrideController _overrideController;
    private CastClipPlayable _castPlayable;
    private bool _castIdleSkip;

    // Дубли OnAnimationShoot на одном кадре (два ивента на клипе / два слоя) — в подписчики уходит один раз.
    private int _lastDuplicateShootFrame = int.MinValue;
    private string _lastDuplicateShootAnim;

    protected bool TryGetAnimator()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>(true);
        return _animator != null;
    }

    public virtual void Initialize()
    {
        _lastAnimationVariableName = "wait_hand";
        if (!TryGetAnimator() || _animator.runtimeAnimatorController == null)
            return;
        SkillAnimationDatabase.LoadRaceAnimations(_animator.runtimeAnimatorController.name);

        // Re-entrant: pooled NPC/monster call Initialize again on each spawn.
        _base_motion.Clear();
        _base_motion["CastMid"] = BASE_MOTION_CAST_MID;
        _base_motion["CastEnd"] = BASE_MOTION_END;
        _base_motion["MagicShoot"] = BASE_MOTION_MAGIC_SHOOT;

        // Создаем экземпляр оверрайда на основе текущего контроллера
        if (_animator.runtimeAnimatorController is not AnimatorOverrideController)
        {
            _overrideController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
            _animator.runtimeAnimatorController = _overrideController;
        }
        else
        {
            _overrideController = (AnimatorOverrideController)_animator.runtimeAnimatorController;
        }

        // Leave death/corpse pose behind when reusing from ObjectPool.
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Update(0f);
        }

        _castIdleSkip = GetComponent<UserEntity>() != null;
        if (_castIdleSkip)
        {
            _animator.cullingMode = AnimatorCullingMode.CullCompletely;
            _castPlayable?.Dispose();
            _castPlayable = null;
            return;
        }

        _castPlayable ??= new CastClipPlayable();
        _castPlayable.Bind(this, _animator);
    }

    void EnsureCastPlayable()
    {
        if (!_castIdleSkip || _castPlayable != null)
            return;
        if (!TryGetAnimator())
            return;
        _castPlayable = new CastClipPlayable();
        _castPlayable.Bind(this, _animator);
    }

    void Update()
    {
        if (_castPlayable == null || !_castPlayable.IsActive)
            return;
        _castPlayable.Tick();
    }

    protected void OnDestroy()
    {
        _castPlayable?.Dispose();
        _castPlayable = null;
    }

    /// <summary>
    /// Play this race's cast clip without writing the override slot.
    /// Returns false when the race clip is missing, so the caller keeps the animator state.
    /// </summary>
    public bool TryPlayCastPhase(string overrideAnimName, string phaseTriggerName)
    {
        EnsureCastPlayable();
        if (_castPlayable == null)
        {
            return false;
        }

        return _castPlayable.TryPlay(overrideAnimName, phaseTriggerName);
    }

    /// <summary>Lobby: play a social clip once over the controller, then hand back to it.</summary>
    public bool TryPlayOneShot(AnimationClip clip)
    {
        EnsureCastPlayable();
        return _castPlayable != null && _castPlayable.PlayOneShot(clip);
    }

    public void StopCastClip()
    {
        if (_castPlayable != null && _castPlayable.IsActive)
        {
            Entity e = EntityActionSkill.ResolveEntity(
                _animator != null ? _animator.GetInteger(AnimatorUtils.OBJECT_ID) : 0);
            MagicGaitTrace.Event(e, "StopCastClip", "wasActive=true");
        }
        _castPlayable?.Stop();
    }

    public bool IsCastClipActive => _castPlayable != null && _castPlayable.IsActive;


    public void SetRunSpeed(float value)
    {
        if (!TryGetAnimator())
            return;
        // Debug.Log("SetRun speed test " + value);
        DriveFloat("run_speed", value);
    }

    public void SetWalkSpeed(float value)
    {
        if (!TryGetAnimator())
            return;
        // Debug.Log("SetWalk speed test " + value);
        DriveFloat("walk_speed", value);
    }

    public void SetWalkSpeedLobby(float value)
    {
        if (!TryGetAnimator())
            return;
        DriveFloat("walk_speed", value);
    }


    public void SetPAtkSpd(float value)
    {
        //Debug.Log("Update Patak speed set " + value);
        _pAtkSpd = value;
        if (_lastAtkClipLength != 0)
        {
            UpdateAnimatorAtkSpdMultiplier(_lastAtkClipLength);
        }
    }

    public void UpdateAnimatorAtkSpdMultiplier(float clipLength)
    {
        if (!TryGetAnimator())
            return;
        float newAtkSpd = clipLength * 1000f / _pAtkSpd;
        //Debug.Log("PATACK speed set " + newAtkSpd);
        DriveFloat("patkspd", newAtkSpd);
    }

    public void UpdateAnimatorAtkSpeedL2j(float timeAtk , float clipLength)
    {
        if (!TryGetAnimator())
            return;
        float newAtkSpd = clipLength * 1000f / timeAtk;
        DriveFloat("patkspd", newAtkSpd);
    }

    public void SetPAtkSpeed(float newAtkSpd)
    {
        if (!TryGetAnimator())
            return;
        //Debug.Log("PATACK speed set 2 " + newAtkSpd);
        DriveFloat("patkspd", newAtkSpd);
    }


    public void SetMAtkSpd(float value)
    {
        if (!TryGetAnimator())
            return;
        //TODO: update for cast animation
        float newMAtkSpd = _spAtk01ClipLength / value;
        DriveFloat("matkspd", newMAtkSpd);
    }

    public void SetCastSpeed(float value)
    {
        if (!TryGetAnimator())
            return;
        DriveFloat("cast_speed", value);
    }
    public float GetNormalizedTimeOffsetSpeed()
    {
        if (!TryGetAnimator())
            return 0f;
        float normalized =  GetStateInfo().normalizedTime;
        float speed = _animator.speed;
        return  normalized / speed;
    }
    public void SetShotSpeed(float value)
    {
        if (!TryGetAnimator())
            return;
        DriveFloat("shot_speed", value);
    }

    // Set all animation variables to false
    public void ClearAnimParams()
    {
        if (!TryGetAnimator())
            return;
        for (int i = 0; i < _animator.parameters.Length; i++)
        {
            AnimatorControllerParameter anim = _animator.parameters[i];
            if (anim.type == AnimatorControllerParameterType.Bool)
            {
                DriveBool(anim.name, false);
            }
        }
    }


}
