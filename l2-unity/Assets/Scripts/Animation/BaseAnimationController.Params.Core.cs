using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController
{
    public void ToggleAnimationTrigger(string name)
    {
        if (!TryGetAnimator())
            return;
        DriveTrigger(name);
    }

    public void ResetAnimationTrigger(string name)
    {
        if (!TryGetAnimator())
            return;
        DriveResetTrigger(name);
    }

    public void ToggleAnimationCrossFade(string name , float duration)
    {
        if (!TryGetAnimator())
            return;
        DriveCrossFade(name, duration, 0);
    }

    public void CrossFadeInFixedTime(string stateName, float fixedDuration, int layer = 0)
    {
        if (!TryGetAnimator() || string.IsNullOrEmpty(stateName))
        {
            return;
        }

        int hash = Animator.StringToHash(stateName);
        DriveCrossFadeInFixedTime(stateName, hash, fixedDuration, layer, 0f);
        _lastAnimationVariableName = stateName;
        // Debug.Log(
            // $"[ANIM_CROSSFADE] id={objectId} go={name} state={stateName} hasState={hasState} fixedDuration={fixedDuration:F3}s layer={layer}");
    }

    public void CrossFadeRestartInFixedTime(string stateName, float fixedDuration, int layer = 0)
    {
        if (!TryGetAnimator() || string.IsNullOrEmpty(stateName))
            return;

        // fixedTimeOffset is seconds from the start of the destination clip, not normalized time.
        DriveCrossFadeInFixedTime(stateName, Animator.StringToHash(stateName), fixedDuration, layer, 0f);
        _lastAnimationVariableName = stateName;
        // Debug.Log(
            // $"[ANIM_RESTART] id={objectId} go={name} state={stateName} fixedDuration={fixedDuration:F3}s " +
            // $"normBefore={normBefore:F3} inTransition={inTransition} layer={layer}");
    }

    public Animator GetAnimator() => _animator;

    void DriveFloat(string paramName, float value)
    {
        // Standalone: SetFloat on a missing param logs every frame (alloc + GC pressure).
        if (!AnimatorBoolSet.HasFloat(_animator, paramName))
            return;
        _animator.SetFloat(paramName, value);
        _castPlayable?.SetFloat(paramName, value);
    }

    void DriveBool(string paramName, bool value)
    {
        if (!AnimatorBoolSet.HasBool(_animator, paramName))
            return;
        _animator.SetBool(paramName, value);
        _castPlayable?.SetBool(paramName, value);
    }

    void DriveInt(string paramName, int value)
    {
        _animator.SetInteger(paramName, value);
        _castPlayable?.SetInteger(paramName, value);
    }
}
