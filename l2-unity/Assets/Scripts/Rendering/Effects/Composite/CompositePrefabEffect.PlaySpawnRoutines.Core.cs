using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void StartShootFallbackRoutineIfNeeded()
    {
        if (_castData == null || !CompositeProjectileLaunchHelper.RequiresAnimationShootEvent(_parts))
        {
            return;
        }

        if (_castData.serverTimeToShoot <= 0f)
        {
            return;
        }

        if (_fallbackShootRoutine != null)
        {
            StopCoroutine(_fallbackShootRoutine);
        }

        _fallbackShootRoutine = StartCoroutine(ShootFallbackRoutine());
    }

    float FallbackShootDelay()
    {
        if (_castData != null && _castData.AnimationShotAtSeconds > 0.01f)
        {
            return _castData.AnimationShotAtSeconds;
        }

        return _castData != null ? _castData.serverTimeToShoot : 0f;
    }

    IEnumerator ShootFallbackRoutine()
    {
        float shootAt = _castData.StartTime + FallbackShootDelay();
        float remaining = shootAt - Time.time;
        if (remaining > 0f)
        {
            yield return new WaitForSeconds(remaining);
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            // Debug.Log($"{DebugPrefix} Shoot fallback catch-up: event time already passed by {-remaining:F3}s.");
        }
#endif

        ProcessShootEvent("fallback");
        _fallbackShootRoutine = null;
    }
}
