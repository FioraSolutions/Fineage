using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2
{
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

        Transform setupOwner = followTransform;
        if (setupOwner == null)
        {
            // Target-side attachments must not get the caster as OwnerTarget
            // (FitToBounds / OwnerTarget would scale/bind impact FX to the mage).
            if (IsTargetSideAttachment(part.spawnAttachmentPoint) &&
                _context != null &&
                _context.TargetTransform != null)
            {
                setupOwner = _context.TargetTransform;
            }
            else
            {
                setupOwner = _owner != null ? _owner : instance.transform;
            }
        }
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
        // Server-time backup / destroy-flush spawn without an AnimationShoot event.
        // Only launch once we are at/past shoot wall (or immediate-shot flag).
        if (ShouldLaunchShotOnSpawn(part))
        {
            TryLaunchOnShoot(part, instance);
        }
        RaiseStartSpawnAndMaybeSpawnLight();
        LogHomeSpawnPart(part, followTransform, worldPosition, spawnPosition, afterInstantiate, afterAttach, instance, setupOwner);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Spawned {part.Describe()} owner='{(setupOwner != null ? setupOwner.name : "null")}'.");
#endif
    }
}
