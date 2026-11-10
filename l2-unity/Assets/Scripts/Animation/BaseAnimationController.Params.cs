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

    // The cast graph owns the mesh. These writes update that controller playable
    // in place; they do not create a graph.
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

    void DriveTrigger(string paramName)
    {
        // drop_* / race-specific gestures may be missing on the active controller.
        if (!AnimatorBoolSet.HasTrigger(_animator, paramName))
            return;
        _animator.SetTrigger(paramName);
        _castPlayable?.SetTrigger(paramName);
    }

    void DriveResetTrigger(string paramName)
    {
        if (!AnimatorBoolSet.HasTrigger(_animator, paramName))
            return;
        _animator.ResetTrigger(paramName);
        _castPlayable?.ResetTrigger(paramName);
    }

    void DriveCrossFade(string stateName, float duration, int layer)
    {
        _animator.CrossFade(stateName, duration, layer);
        _castPlayable?.CrossFade(Animator.StringToHash(stateName), duration, layer);
    }

    void DriveCrossFadeInFixedTime(string stateName, int stateHash, float fixedDuration, int layer, float fixedTimeOffset)
    {
        _animator.CrossFadeInFixedTime(stateName, fixedDuration, layer, fixedTimeOffset);
        _castPlayable?.CrossFadeInFixedTime(stateHash, fixedDuration, layer, fixedTimeOffset);
    }

    public void SetBool(string name, bool value , string entityName = "")
    {
        if (!TryGetAnimator())
            return;

        if (value)
        {
            _lastAnimationVariableName = name;
        }

        DriveBool(name, value);
    }

    public void SetBoolDisabledOther(string nameAnim  , bool value, string[] disableds)
    {
        if (!TryGetAnimator())
            return;
        DisabledOtherAnim(disableds);
        DriveBool(nameAnim, value);
    }

    private void DisabledOtherAnim(string[] disableds)
    {
        if (!TryGetAnimator())
            return;
        foreach (string name in disableds)
        {
            DriveBool(name, false);
        }
    }


    public bool GetBool(string name)
    {
        if (!TryGetAnimator())
            return false;
        // Races without that weapon set (MMagic has no wait_1HS) spammed "Parameter does not exist".
        if (!AnimatorBoolSet.HasBool(_animator, name))
            return false;
        return _animator.GetBool(name);
    }



    public bool IsFinishAnimation(string name)
    {
        if (!TryGetAnimator())
            return false;

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        bool isName = stateInfo.IsName(name);

        if (GetNormalizedTimeOffsetSpeed() >= 1 && stateInfo.IsName(name))
        {
            return true;

        }

        return false;
    }

  

    public AnimatorStateInfo GetStateInfo()
    {
        if (!TryGetAnimator())
            return default;
        return  _animator.GetCurrentAnimatorStateInfo(0);
    }


    // Update animator variable based on Animation Id
}
