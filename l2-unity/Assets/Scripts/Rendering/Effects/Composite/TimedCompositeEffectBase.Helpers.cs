using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class TimedCompositeEffectBase : BaseEffect
{
    protected void CleanupRuntimeSettings()
    {
        for (int i = 0; i < _runtimeSettings.Count; i++)
        {
            if (_runtimeSettings[i] != null)
            {
                Destroy(_runtimeSettings[i]);
            }
        }

        _runtimeSettings.Clear();
        _rootRuntimeSettings = null;
    }

    protected void StopAndClearCoroutine(ref Coroutine routine)
    {
        if (routine == null)
        {
            return;
        }

        StopCoroutine(routine);
        routine = null;
    }

    protected void UnsubscribeShootEventSources(
        List<AnimationEventsBase> shootEventSources,
        Action<string> onAnimationShootHandler,
        ref AnimationEventsBase animationEvents,
        ref bool isSubscribedToAnyShoot)
    {
        CompositePlaybackSubscriptions.UnsubscribeShoot(
            shootEventSources,
            onAnimationShootHandler,
            ref isSubscribedToAnyShoot);
        animationEvents = null;
    }

    protected virtual void OnTimedCompositeDestroy()
    {
    }

    protected override void OnDestroy()
    {
        OnTimedCompositeDestroy();
        CleanupRuntimeSettings();
        base.OnDestroy();
    }
}
