using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController
{
    public void SetAnimationProperty(int animId, float value)
    {
        SetAnimationProperty(animId, value, false);
    }

    public void SetAnimationProperty(int animId, float value, bool forceReset)
    {
        if (!TryGetAnimator())
            return;
        //Debug.Log("animId " + animId + "/" + _animator.parameters.Length);
        if (animId >= 0 && animId < _animator.parameters.Length)
        {
            if (_resetStateOnReceive || forceReset)
            {
                ClearAnimParams();
            }

            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    DriveFloat(anim.name, value);
                    break;
                case AnimatorControllerParameterType.Int:
                    DriveInt(anim.name, (int)value);
                    break;
                case AnimatorControllerParameterType.Bool:
                    SetBool(anim.name, value == 1f);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    DriveTrigger(anim.name);
                    break;
            }
        }
    }

    public float GetAnimationProperty(int animId)
    {
        if (!TryGetAnimator())
            return 0f;
        if (animId >= 0 && animId < _animator.parameters.Length)
        {
            AnimatorControllerParameter anim = _animator.parameters[animId];

            switch (anim.type)
            {
                case AnimatorControllerParameterType.Float:
                    return _animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Int:
                    return (int)_animator.GetFloat(anim.name);
                case AnimatorControllerParameterType.Bool:
                    return _animator.GetBool(anim.name) == true ? 1f : 0;
            }
        }

        return 0f;
    }

    public void SetFloat(string name, float value)
    {
        if (!TryGetAnimator())
            return;
        DriveFloat(name, value);
    }

    public float GetFloat(string name)
    {
        if (!TryGetAnimator())
            return 0f;
        return  _animator.GetFloat(_animator.name);
    }

    public void SetInt(string name, int value)
    {
        if (!TryGetAnimator())
            return;
        DriveInt(name, value);
    }

    public int GetInt(string name)
    {
        if (!TryGetAnimator())
            return 0;
        return _animator.GetInteger(_animator.name);
    }
}
