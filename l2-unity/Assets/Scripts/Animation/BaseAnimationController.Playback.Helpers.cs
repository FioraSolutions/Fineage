using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController
{
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
