using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ItemDropPresentationService
{
    void PlayActorAnimation(int objectId, string trigger)
    {
        IAnimationManager animations = Animations;
        if (objectId == 0 || animations == null)
            return;
        animations.PlayAnimationTrigger(objectId, trigger);
    }

    void PlayActorCrossFade(int objectId, string stateName)
    {
        IAnimationManager animations = Animations;
        if (objectId == 0 || animations == null)
            return;
        // Pickup is a gesture — must not inherit run_speed / patkspd (see AnimPlayback).
        animations.PlayGesture(objectId, stateName);
    }
}
