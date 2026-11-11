using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController
{
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
}
