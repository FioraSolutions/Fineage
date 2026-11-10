using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    void PrepareV2Playback(BaseEffect instance)
    {
        if (HasParticleGroupV2(instance) && !HasLegacyParticleAuthoring(instance))
        {
            return;
        }

        ParticleStreamHijack.Convert(instance);
    }

    void QueueV2Parts()
    {
        _pendingDelayed.Clear();
        _pendingHitCollider.Clear();
        _pendingAnimationShoot.Clear();

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part == null || !part.IsSpawnable)
            {
                continue;
            }

            if (part.spawnTiming == CompositePartSpawnTiming.OnHitCollider)
            {
                _pendingHitCollider.Add(part);
                continue;
            }

            if (part.spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
            {
                _pendingAnimationShoot.Add(part);
                if (TraceCrasherWind())
                    Debug.Log("[WINDSTRIKE] Composite queue shoot part=" + part.name);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // if (V2TaSpawnLog.Matches(part))
                {
                    // V2TaSpawnLog.Info(
                        // "QUEUE shoot part='" + part.name +
                        // "' hit=" + (_castData != null ? _castData.HitTime.ToString("0.###") : "-") +
                        // "s shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
                        // "s wantsShoot=" + part.WantsAnimationShoot);
                }
#endif
                continue;
            }

            float delay = CompositeEffectUtilities.ResolveSpawnDelay(part.spawnTiming, _castData);
            delay += Mathf.Max(0f, part.spawnDelaySeconds);
            if (delay <= 0f)
            {
                if (TraceCrasherWind())
                    Debug.Log("[WINDSTRIKE] Composite spawn now part=" + part.name);
                SpawnV2Part(part);
                continue;
            }

            float spawnAt = Time.time + delay;
            _pendingDelayed.Add(new PendingV2Part
            {
                Part = part,
                SpawnAtTime = spawnAt
            });
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (V2TaSpawnLog.Matches(part))
            {
                float compositeLife = _rootRuntimeSettings != null
                    ? _rootRuntimeSettings.defaultLifeTime
                    : (_settings != null ? _settings.defaultLifeTime : -1f);
                float destroyAt = Time.time + Mathf.Max(0f, compositeLife);
                // V2TaSpawnLog.Warn(
                    // "QUEUE delayed part='" + part.name + "' type=" + part.GetType().Name +
                    // " timing=" + part.spawnTiming +
                    // " delay=" + delay.ToString("0.###") +
                    // "s spawnAt=" + spawnAt.ToString("F3") +
                    // " compositeLife=" + compositeLife.ToString("0.###") +
                    // "s destroyAt=" + destroyAt.ToString("F3") +
                    // " hit=" + (_castData != null ? _castData.HitTime.ToString("0.###") : "-") +
                    // "s shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
                    // "s outlives=" + part.OutlivesComposite +
                    // (destroyAt <= spawnAt + 0.02f
                        // ? " RACE: composite dies at/before delayed spawn"
                        // : string.Empty));
            }
#endif
        }
    }

    void SpawnV2Part(CompositePart part)
    {
        if (part == null || !part.IsSpawnable)
        {
            return;
        }

        if (_spawned.ContainsKey(part))
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (V2TaSpawnLog.Matches(part))
            {
                // V2TaSpawnLog.Warn(
                    // "SPAWN SKIP already spawned part='" + part.name +
                    // "' frame=" + Time.frameCount + " t=" + Time.time.ToString("F3"));
            }
#endif
            return;
        }

        RefreshV2Context();
        EnsureHitPointFromTarget();
        if (!part.TryResolveSpawn(_v2Resolver, _context, out Transform followTransform, out Vector3 worldPosition))
        {
            if (TraceCrasherWind())
            {
                Debug.Log(
                    "[WINDSTRIKE] Composite resolve FAIL part=" + part.name +
                    " attach=" + part.spawnAttachmentPoint +
                    " target=" + (_context != null && _context.TargetTransform != null
                        ? _context.TargetTransform.name
                        : "null"));
            }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (V2TaSpawnLog.Matches(part))
            {
                // V2TaSpawnLog.Warn(
                    // "RESOLVE FAIL part='" + part.name +
                    // "' attach=" + part.spawnAttachmentPoint +
                    // " target=" + (_context != null && _context.TargetTransform != null
                        // ? _context.TargetTransform.name
                        // : "null"));
            }
#endif
            // Debug.LogWarning(
                // $"[HOME_SPAWN] resolve FAILED part='{part.name}' type={part.GetType().Name} " +
                // $"placement={(part.placement != null ? part.placement.GetType().Name : "null")}");
            // Debug.LogWarning($"{DebugPrefix} could not resolve placement for part {part.name}.");
            return;
        }

        Vector3 spawnPosition = CompositeEffectUtilities.ResolveSpawnPosition(
            followTransform,
            worldPosition,
            Vector3.zero);
        Quaternion rotation = ResolveV2SpawnRotation(part, followTransform);
        BaseEffect instance = Instantiate(part.prefab, spawnPosition, rotation);
        instance.gameObject.SetActive(true);
        if (TraceCrasherWind())
            Debug.Log("[WINDSTRIKE] Composite spawned part=" + part.name + " pos=" + instance.transform.position);
        Vector3 afterInstantiate = instance.transform.position;
        CompositePartSpawnHelper.AttachIfFollow(part.follow, followTransform, instance.transform, worldPosition);
        Vector3 afterAttach = instance.transform.position;
        CompositePartSpawnHelper.ApplyScale(part.scale, instance.transform);

        Transform setupOwner = followTransform != null
            ? followTransform
            : (_owner != null ? _owner : instance.transform);
        EffectSettings partSettings = CreateRuntimeSettings(
            _settings,
            applyTimedLifetime: part.UsesCastHitLifetime);
        if (partSettings == null)
        {
            // Debug.LogWarning($"{DebugPrefix} settings are null for part {part.name}.");
            Destroy(instance.gameObject);
            return;
        }

        part.ConfigurePlayback(instance, partSettings, _castData, _context);
        instance.Setup(partSettings, _castData, setupOwner);
        PrepareV2Playback(instance);
        part.OnAfterSpawn(instance, _context);
        HomeFlightPart spawnedHome = part as HomeFlightPart;
        if (spawnedHome != null)
        {
            HomeProjectiles?.EnsureDualFlightRoots(instance, spawnedHome.mirrorDualFlight);
        }
        instance.Play();
        _spawned[part] = instance;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (V2TaSpawnLog.Matches(part))
        {
            // V2TaSpawnLog.Info(
                // "SPAWNED part='" + part.name + "' type=" + part.GetType().Name +
                // " timing=" + part.spawnTiming +
                // " attach=" + part.spawnAttachmentPoint +
                // " follow=" + part.follow +
                // " parent='" + (instance.transform.parent != null ? instance.transform.parent.name : "null") +
                // "' goActive=" + instance.gameObject.activeInHierarchy +
                // " authoredLife=" + instance.IsLifetimeOwnedByAuthoredStreams +
                // " sincePlay=" + (Time.time - _playStartedAt).ToString("0.###") +
                // "s frame=" + Time.frameCount);
        }
#endif
        TryLaunchImmediate(part, instance);
        RaiseStartSpawnAndMaybeSpawnLight();
        LogHomeSpawnPart(part, followTransform, worldPosition, spawnPosition, afterInstantiate, afterAttach, instance, setupOwner);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Spawned {part.Describe()} owner='{(setupOwner != null ? setupOwner.name : "null")}'.");
#endif
    }

    Quaternion ResolveV2SpawnRotation(CompositePart part, Transform resolvedTransform)
    {
        if (part != null && part.UsesWeaponBoneRotation && resolvedTransform != null)
        {
            return resolvedTransform.rotation;
        }

        // L2 actor +X is flight/cast; Unity LookRotation is Z-forward. Same -90
        // yaw as V1 CompositePartSpawnHelper — required for MeshRing/cs burst too,
        // not only ShotProjectilePart (otherwise the halo faces into/side of caster).
        if (_context != null &&
            _context.HasHitDirection &&
            _context.HitDirection.sqrMagnitude > 0.0001f)
        {
            return Quaternion.LookRotation(_context.HitDirection.normalized, Vector3.up) *
                   Quaternion.Euler(0f, HitDirectionYawOffsetDegrees, 0f);
        }

        if (resolvedTransform != null && (part == null || part.follow || part.inheritRotation))
        {
            Vector3 forward = resolvedTransform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.0001f)
            {
                return Quaternion.LookRotation(forward.normalized, Vector3.up) *
                       Quaternion.Euler(0f, HitDirectionYawOffsetDegrees, 0f);
            }
        }

        return CompositeEffectUtilities.ResolveSpawnRotation(part != null && part.inheritRotation, resolvedTransform);
    }

    void RefreshV2Context()
    {
        _context = CompositeEffectUtilities.RebuildPreservingHit(_context, _owner, _castData);
    }

    void EnsureHitPointFromTarget()
    {
        if (_context == null || _context.HasHitPoint)
        {
            return;
        }

        Transform target = _context.TargetTransform != null
            ? _context.TargetTransform
            : _context.CasterTransform;
        Entity targetEntity = _context.TargetEntity != null
            ? _context.TargetEntity
            : _context.CasterEntity;
        if (targetEntity == null && target != null)
        {
            targetEntity = target.GetComponentInParent<Entity>();
        }

        Transform anchor = HitAnchorResolver.ResolveHitAnchor(targetEntity, target);
        if (anchor == null)
        {
            return;
        }

        _context.HasHitPoint = true;
        _context.HitPoint = anchor.position;
        if (!_context.HasHitDirection)
        {
            Vector3 fromCaster = _owner != null
                ? (anchor.position - _owner.position)
                : Vector3.zero;
            fromCaster.y = 0f;
            if (fromCaster.sqrMagnitude > 0.0001f)
            {
                _context.HasHitDirection = true;
                _context.HitDirection = fromCaster.normalized;
            }
        }
    }
}
