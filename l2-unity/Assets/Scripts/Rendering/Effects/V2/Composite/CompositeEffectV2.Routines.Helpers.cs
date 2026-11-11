using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    IEnumerator SpawnPendingV2Routine()
    {
        while (_pendingDelayed.Count > 0)
        {
            float now = Time.time;
            for (int i = _pendingDelayed.Count - 1; i >= 0; i--)
            {
                PendingV2Part pending = _pendingDelayed[i];
                if (pending == null || pending.Part == null)
                {
                    _pendingDelayed.RemoveAt(i);
                    continue;
                }

                if (now >= pending.SpawnAtTime)
                {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    // if (V2TaSpawnLog.Matches(pending.Part))
                    {
                        // V2TaSpawnLog.Info(
                            // "DELAY DUE part='" + pending.Part.name +
                            // "' spawnAt=" + pending.SpawnAtTime.ToString("F3") +
                            // " now=" + now.ToString("F3") +
                            // " late=" + (now - pending.SpawnAtTime).ToString("0.###") +
                            // "s frame=" + Time.frameCount);
                    }
#endif
                    SpawnV2Part(pending.Part);
                    _pendingDelayed.RemoveAt(i);
                }
            }

            yield return null;
        }

        _v2PendingRoutine = null;
    }

    void StartV2ShootWatchdogIfNeeded()
    {
        if (_castData == null || !RequiresV2AnimationShoot() || _castData.serverTimeToShoot <= 0f)
        {
            return;
        }

        if (_v2ShootWatchdogRoutine != null)
        {
            StopCoroutine(_v2ShootWatchdogRoutine);
        }

        _v2ShootWatchdogRoutine = StartCoroutine(V2ShootWatchdogRoutine());
    }

    float FallbackShootDelay()
    {
        if (_castData != null && _castData.AnimationShotAtSeconds > 0.01f)
        {
            return _castData.AnimationShotAtSeconds;
        }

        return _castData != null ? _castData.serverTimeToShoot : 0f;
    }

    IEnumerator V2ShootWatchdogRoutine()
    {
        float shootAt = _castData.StartTime + FallbackShootDelay();
        float remaining = shootAt - Time.time;
        if (remaining > 0f)
        {
            yield return new WaitForSeconds(remaining);
        }

        // Let the Animator dispatch an event that crossed the same wall during
        // this frame. This routine is diagnostic only and never launches.
        yield return null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float fromCast = Time.time - _castData.StartTime;
        string message =
            $"{DebugPrefix} [MAGIC_SHOOT_WATCHDOG] directObserved={_v2DirectShootObserved} " +
            $"fromCast={fromCast:F3}s expected={FallbackShootDelay():F3}s " +
            $"pending={_pendingAnimationShoot.Count} launched={_launchedProjectiles.Count}.";
        if (_v2DirectShootObserved)
        {
            // Debug.Log(message);
        }
        else
        {
            // Debug.LogWarning(message + " AnimationEvent was not received; projectile was NOT launched.");
        }
#endif
        _v2ShootWatchdogRoutine = null;
    }
}
