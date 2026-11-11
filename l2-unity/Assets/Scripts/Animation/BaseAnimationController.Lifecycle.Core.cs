using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController
{
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

        // Local player: defer CastClip graph until first cast (CPU), but never CullCompletely —
        // wall-clock CastSync can report phaseDone while bones stay frozen when bounds cull.
        _castIdleSkip = GetComponent<UserEntity>() != null;
        if (_castIdleSkip)
        {
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
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
}
