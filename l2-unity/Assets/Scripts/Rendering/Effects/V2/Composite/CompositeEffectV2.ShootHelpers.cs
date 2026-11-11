using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2
{
    void UnsubscribeV2Hit()
    {
        if (ProjectilePacketServices.Projectiles != null && _v2SubscribedHit)
        {
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile -= HandleV2ProjectileHit;
            _v2SubscribedHit = false;
        }
    }

    void HandleV2AnimationShoot(string _)
    {
        _v2DirectShootObserved = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float fromCast = _castData != null ? Time.time - _castData.StartTime : -1f;
        float expected = _castData != null ? _castData.AnimationShotAtSeconds : -1f;
        // Debug.Log(
            // $"{DebugPrefix} [MAGIC_SHOOT_SOURCE] source=AnimationEvent " +
            // $"fromCast={fromCast:F3}s expected={expected:F3}s delta={fromCast - expected:F3}s.");
#endif
        ProcessV2Shoot("direct");
    }

    void ProcessV2Shoot(string channel)
    {
        SpawnPendingAnimationShootParts();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_castData != null)
        {
            float globalSinceCast = Time.time - _castData.StartTime;
            // Debug.Log(
                // "[MAGIC_PROJECTILE_SYNC] ShootEvent " +
                // $"channel={channel} globalSinceCast={globalSinceCast:F3}s hit={_castData.HitTime:F3}s " +
                // $"flight={_castData.FlightTime:F3}s serverShoot={_castData.serverTimeToShoot:F3}s " +
                // $"deltaGlobalToShoot={globalSinceCast - _castData.serverTimeToShoot:F3}s " +
                // $"deltaGlobalToHit={globalSinceCast - _castData.HitTime:F3}s sincePlayStarted={Time.time - _playStartedAt:F3}s");
        }
#endif
        foreach (KeyValuePair<CompositePart, BaseEffect> pair in _spawned)
        {
            CompositePart part = pair.Key;
            BaseEffect instance = pair.Value;
            if (part == null || instance == null)
            {
                continue;
            }

            part.OnAnimationShoot(instance, _context);
            TryLaunchOnShoot(part, instance);
        }
    }

    void SpawnPendingAnimationShootParts()
    {
        if (_pendingAnimationShoot.Count == 0)
        {
            return;
        }

        // Debug.Log($"[HOME_SPAWN] AnimationShoot spawning {_pendingAnimationShoot.Count} part(s)");
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        for (int logI = 0; logI < _pendingAnimationShoot.Count; logI++)
        {
            CompositePart shootPart = _pendingAnimationShoot[logI];
            // if (V2TaSpawnLog.Matches(shootPart))
            {
                // V2TaSpawnLog.Info(
                    // "SHOOT QUEUE part='" + shootPart.name +
                    // "' sincePlay=" + (Time.time - _playStartedAt).ToString("0.###") +
                    // "s channel pending count=" + _pendingAnimationShoot.Count);
            }
        }
#endif

        for (int i = 0; i < _pendingAnimationShoot.Count; i++)
        {
            CompositePart part = _pendingAnimationShoot[i];
            if (part == null || _spawned.ContainsKey(part))
            {
                continue;
            }

            float extraDelay = Mathf.Max(0f, part.spawnDelaySeconds);
            if (extraDelay > 0f)
            {
                _pendingDelayed.Add(new PendingV2Part
                {
                    Part = part,
                    SpawnAtTime = Time.time + extraDelay
                });
                continue;
            }

            SpawnV2Part(part);
        }

        _pendingAnimationShoot.Clear();
        StartV2PendingRoutineIfNeeded();
    }

    void HandleV2ProjectileHit(
        GameObject projectilePrefab,
        Transform target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        int attackerEntityId)
    {
        if (_pendingHitCollider.Count == 0 || projectilePrefab == null)
        {
            return;
        }

        Vector3 resolvedHitPoint = hitPoint;
        Vector3 resolvedHitDirection = hitDirection;
        var hits = HitPacketServices.Hits;
        if (hits != null &&
            !hits.TryPrepareProjectileEffectHit(
                projectilePrefab,
                hitPoint,
                hitDirection,
                attackerEntityId,
                IsFromLaunchedV2Projectile,
                out resolvedHitPoint,
                out resolvedHitDirection))
        {
            if (projectilePrefab.transform == null ||
                !IsFromLaunchedV2Projectile(projectilePrefab.transform))
            {
                return;
            }

            resolvedHitPoint = hitPoint;
            resolvedHitDirection = hitDirection.sqrMagnitude > 0.0001f ? hitDirection : Vector3.forward;
        }

        bool hadHitPoint = _context != null && _context.HasHitPoint;
        Vector3 previousHitPoint = hadHitPoint ? _context.HitPoint : Vector3.zero;
        bool hadHitDirection = _context != null && _context.HasHitDirection;
        Vector3 previousHitDirection = hadHitDirection ? _context.HitDirection : Vector3.forward;

        if (_context != null)
        {
            _context.HasHitPoint = true;
            _context.HitPoint = resolvedHitPoint;
            _context.HasHitDirection = true;
            _context.HitDirection = resolvedHitDirection;
        }

        for (int i = 0; i < _pendingHitCollider.Count; i++)
        {
            SpawnV2Part(_pendingHitCollider[i]);
        }

        if (_context != null)
        {
            _context.HasHitPoint = hadHitPoint;
            _context.HitPoint = previousHitPoint;
            _context.HasHitDirection = hadHitDirection;
            _context.HitDirection = previousHitDirection;
        }

        _pendingHitCollider.Clear();
    }
}
