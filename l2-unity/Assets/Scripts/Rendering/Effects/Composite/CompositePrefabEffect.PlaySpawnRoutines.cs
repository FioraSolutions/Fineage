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

    void StartPendingPartsRoutineIfNeeded()
    {
        if (_pendingParts.Count > 0 && _pendingSpawnRoutine == null)
        {
            _pendingSpawnRoutine = StartCoroutine(SpawnPendingPartsRoutine());
        }
    }

    IEnumerator SpawnPendingPartsRoutine()
    {
        while (_pendingParts.Count > 0)
        {
            float now = Time.time;
            for (int i = _pendingParts.Count - 1; i >= 0; i--)
            {
                PendingCompositePart pending = _pendingParts[i];
                if (pending == null || pending.Part == null)
                {
                    _pendingParts.RemoveAt(i);
                    continue;
                }

                if (now >= pending.SpawnAtTime)
                {
                    SpawnPart(pending.Part);
                    _pendingParts.RemoveAt(i);
                }
            }

            yield return null;
        }

        _pendingSpawnRoutine = null;
    }

    void DestroyCompositeByLifetime()
    {
        EffectSettings lifeTimeSettings = SelectLifetimeSettings();
        if (lifeTimeSettings != null)
        {
            DestoryEffect(lifeTimeSettings, _castData);
        }
    }

}
