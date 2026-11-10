using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void SubscribeShootEventIfNeeded()
    {
        CompositePlaybackSubscriptions.SubscribeShootIfNeeded(
            _parts,
            _context,
            _shootEventSources,
            HandleAnimationShoot,
            Animations,
            out _animationEvents,
            ref _isSubscribedToAnyShoot);
    }

    void SubscribeProjectileHitEventIfNeeded()
    {
        CompositePlaybackSubscriptions.SubscribeHitIfNeeded(
            _parts,
            HandleProjectileEffectHit,
            ref _isSubscribedToProjectileEffectHit);
    }

    void HandleAnimationShoot(string _)
    {
        ProcessShootEvent("direct");
    }

    void ProcessShootEvent(string channel)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (_lastShootAt > 0f && _pendingAnimationShootParts.Count > 0)
            // EffectDoublePlayLog.Repeat(
                // $"Composite.ShootEvent first={_lastShootChannel} this={channel}",
                // this,
                // _lastShootAt,
                // _lastShootStack);
        _lastShootChannel = channel;
        _lastShootAt = Time.time;
        // _lastShootStack = EffectDoublePlayLog.CaptureStack();
#endif
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
        else
        {
            // Debug.Log($"[MAGIC_PROJECTILE_SYNC] ShootEvent channel={channel} castData=null sincePlayStarted={Time.time - _playStartedAt:F3}s");
        }
#endif
        CompositeProjectileLaunchHelper.RevealOnShootParts(_parts, _spawnedPartInstances);
        CompositeProjectileLaunchHelper.ProcessShootLaunches(
            _parts,
            _spawnedPartInstances,
            _launchedProjectileParts,
            _context.TargetTransform,
            _playStartedAt,
            DebugPrefix);

        IHomeProjectileService homeProjectiles = HomeProjectiles;
        if (homeProjectiles != null)
        {
            homeProjectiles.ProcessShootLaunches(
                _parts,
                _spawnedPartInstances,
                _launchedHomeProjectileParts,
                _context);
        }
    }

    void SpawnPendingAnimationShootParts()
    {
        if (_pendingAnimationShootParts.Count == 0)
        {
            return;
        }

        for (int i = 0; i < _pendingAnimationShootParts.Count; i++)
        {
            CompositePrefabPart part = _pendingAnimationShootParts[i];
            if (part == null || _spawnedPartInstances.ContainsKey(part))
            {
                continue;
            }

            float extraDelay = Mathf.Max(0f, part.spawnDelaySeconds);
            if (extraDelay > 0f)
            {
                _pendingParts.Add(new PendingCompositePart
                {
                    Part = part,
                    SpawnAtTime = Time.time + extraDelay
                });
                continue;
            }

            SpawnPart(part);
        }

        _pendingAnimationShootParts.Clear();
        StartPendingPartsRoutineIfNeeded();
    }

    void HandleProjectileEffectHit(
        GameObject projectilePrefab,
        Transform target,
        Vector3 hitPoint,
        Vector3 hitDirection,
        int attackerEntityId)
    {
        var hits = HitPacketServices.Hits;
        if (_pendingHitColliderParts.Count == 0 || projectilePrefab == null || hits == null)
        {
            return;
        }

        if (!hits.TryPrepareProjectileEffectHit(
                projectilePrefab,
                hitPoint,
                hitDirection,
                attackerEntityId,
                attacker => CompositePlaybackSubscriptions.IsFromLaunchedCompositeProjectile(
                    attacker, _spawnedPartInstances),
                out Vector3 resolvedHitPoint,
                out Vector3 resolvedHitDirection))
        {
            return;
        }

        EffectResolveContext previous = _context;
        bool hadHitPoint = previous != null && previous.HasHitPoint;
        Vector3 previousHitPoint = hadHitPoint ? previous.HitPoint : Vector3.zero;
        bool hadHitDirection = previous != null && previous.HasHitDirection;
        Vector3 previousHitDirection = hadHitDirection ? previous.HitDirection : Vector3.forward;

        if (_context != null)
        {
            _context.HasHitPoint = true;
            _context.HitPoint = resolvedHitPoint;
            _context.HasHitDirection = true;
            _context.HitDirection = resolvedHitDirection;
        }

        for (int i = 0; i < _pendingHitColliderParts.Count; i++)
        {
            SpawnPart(_pendingHitColliderParts[i]);
        }

        if (_context != null)
        {
            _context.HasHitPoint = hadHitPoint;
            _context.HitPoint = previousHitPoint;
            _context.HasHitDirection = hadHitDirection;
            _context.HitDirection = previousHitDirection;
        }

        _pendingHitColliderParts.Clear();
    }
}
