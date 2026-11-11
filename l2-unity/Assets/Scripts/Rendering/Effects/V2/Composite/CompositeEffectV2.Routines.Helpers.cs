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

        // One frame for CastClip / Animator to dispatch OnAnimationShoot on the same wall.
        yield return null;

        // Match legacy CompositePrefabEffect.ShootFallbackRoutine: if the anim event
        // never reached us (CastClip path / late GetAnimationEvents), still spawn+launch
        // the flying orb (el_*_fl). Also retry when a direct event observed but launch failed.
        if (!_v2DirectShootObserved ||
            HasUnlaunchedShotProjectile() ||
            _pendingAnimationShoot.Count > 0)
        {
            RefreshV2Context();
            // Late subscribe can still help hit/ta parts after launch.
            if (!_v2SubscribedShoot || _v2AnimationEvents == null)
                SubscribeV2ShootIfNeeded();
            ProcessV2Shoot(_v2DirectShootObserved ? "fallback-retry" : "fallback");
        }

        _v2ShootWatchdogRoutine = null;
    }

    bool HasUnlaunchedShotProjectile()
    {
        if (_v2Parts == null)
        {
            return false;
        }

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            ShotProjectilePart shot = _v2Parts[i] as ShotProjectilePart;
            if (shot == null || !shot.IsLaunchedProjectile)
            {
                continue;
            }

            if (!_launchedProjectiles.Contains(shot))
            {
                return true;
            }
        }

        return false;
    }
}
