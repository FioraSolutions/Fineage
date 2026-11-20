using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController, ILocomotionPlaybackRates
{
    public bool TryPlayCastPhase(string overrideAnimName, string phaseTriggerName)
    {
        EnsureCastPlayable();
        if (_castPlayable == null)
        {
            return false;
        }

        return _castPlayable.TryPlay(overrideAnimName, phaseTriggerName);
    }

    public bool TryPlayOneShot(AnimationClip clip)
    {
        EnsureCastPlayable();
        return _castPlayable != null && _castPlayable.PlayOneShot(clip);
    }

    public bool TryPlaySpAtkCombat(AnimationClip clip, string phaseStateName)
    {
        EnsureCastPlayable();
        return _castPlayable != null && _castPlayable.TryPlaySpAtkCombat(clip, phaseStateName);
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
}
