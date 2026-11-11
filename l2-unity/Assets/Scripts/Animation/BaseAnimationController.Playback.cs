using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BaseAnimationController : AnimationEventsBase, IAnimationController
{
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
