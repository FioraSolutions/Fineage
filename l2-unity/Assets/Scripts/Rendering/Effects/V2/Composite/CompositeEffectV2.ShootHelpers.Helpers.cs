using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
    void ProcessV2Shoot(string channel)
    {
        SpawnPendingAnimationShootParts(shotsOnly: false);
        NotifySpawnedPartsAnimationShoot(channel);
    }

    /// <summary>
    /// Destroy-time / late flush: launch flying orbs only. Spawning IndependentEffectPart
    /// (cs) at HitTime looks like the impact flash on the caster.
    /// </summary>
    void ProcessV2ShootShotsOnly(string channel)
    {
        SpawnPendingAnimationShootParts(shotsOnly: true);
        NotifySpawnedPartsAnimationShoot(channel);
    }

    void NotifySpawnedPartsAnimationShoot(string channel)
    {
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

    void SpawnPendingAnimationShootParts(bool shotsOnly)
    {
        if (_pendingAnimationShoot.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _pendingAnimationShoot.Count; i++)
        {
            CompositePart part = _pendingAnimationShoot[i];
            if (part == null || _spawned.ContainsKey(part))
            {
                continue;
            }

            if (shotsOnly &&
                !(part is ShotProjectilePart) &&
                !(part is HomeFlightPart))
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

        // Drop remaining pending. For shotsOnly that means late cs is discarded on purpose
        // (would otherwise flash on the caster at HitTime).
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
