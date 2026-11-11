using System;
using UnityEngine;

public abstract partial class AnimationEventsBase : MonoBehaviour
{
    public void AttackShot(string animationName)
    {
        animationName = NormalizeAnimationEventName(animationName);
        OnAnimationAttackShot?.Invoke(animationName);
    }

    public void OnAnimationHit(string animationName)
    {
        animationName = NormalizeAnimationEventName(animationName);
        OnAnimationStartHit?.Invoke(animationName);
    }

    public void OnAnimationAttackHitEnd(string animationName)
    {
        OnAnimationFinishedHit?.Invoke(animationName);
    }

    public void OnAnimationLoadArrow(string animationName)
    {
        animationName = NormalizeAnimationEventName(animationName);
        OnAnimationStartLoadArrow?.Invoke(animationName);
    }
}
