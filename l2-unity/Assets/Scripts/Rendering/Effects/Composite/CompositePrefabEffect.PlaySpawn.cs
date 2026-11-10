using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void SpawnPart(CompositePrefabPart part)
    {
        if (part == null || part.prefab == null)
        {
            return;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (_spawnedPartInstances.ContainsKey(part))
            // EffectDoublePlayLog.Note(
                // "Composite.SpawnPart",
                // this,
                // $"part='{part.name}' already spawned instance='{_spawnedPartInstances[part]}'");
#endif

        _context = CompositeEffectUtilities.RebuildPreservingHit(_context, _owner, _castData);
        if (!CompositePartSpawnHelper.TryResolveAttachment(
                part, _context, _resolver, out Transform resolvedTransform, out Vector3 worldPosition))
        {
            return;
        }

        BaseEffect instance = CompositePartSpawnHelper.SpawnInstance(
            part, resolvedTransform, worldPosition, _owner, _context, UseLegacyLifetimeHacks);
        if (instance == null)
        {
            return;
        }

        if (!TryResolvePartSettings(part, out EffectSettings partSettings))
        {
            return;
        }

        if (UseLegacyLifetimeHacks)
        {
            CompositePartSpawnHelper.ApplyHideTimeOverride(part, partSettings);
        }

        Transform setupOwner = CompositePartSpawnHelper.ResolveSetupOwner(
            resolvedTransform, _owner, instance.transform);
        MagicCastData partCastData = part.passCastDataToPart ? _castData : null;
        instance.Setup(partSettings, partCastData, setupOwner);
        if (UseLegacyLifetimeHacks)
        {
            CompositePartSpawnHelper.ApplyLoopOverrides(part, instance.transform);
        }

        CompositePartSpawnHelper.ApplyHomeFlightOverrides(part, instance.transform);
        if (part.homeProjectile != null && part.homeProjectile.IsEnabled)
        {
            instance.PrepareDestroyOnHomeArrival();
        }

        PreparePartPlayback(part, instance);
        CompositePartSpawnHelper.ApplyShaderTargetPosition(part, instance.transform, _context, _resolver);
        instance.Play();
        CompositeProjectileLaunchHelper.ApplyPreShootVisibility(part, instance, DebugPrefix);
        _spawnedPartInstances[part] = instance;
        if (UseLegacyLifetimeHacks)
        {
            StartFinalShaderLifetimeRoutineIfNeeded(part, instance.transform, partSettings);
        }

        TryLaunchPartAsProjectileImmediately(part, instance);
        TryLaunchPartAsHomeProjectileImmediately(part, instance);
        LogSpawnedPart(part, partSettings, setupOwner);
        RaiseStartSpawnAndMaybeSpawnLight();
    }

    bool TryResolvePartSettings(CompositePrefabPart part, out EffectSettings partSettings)
    {
        EffectSettings sourceSettings = part.settingsOverride != null ? part.settingsOverride : _settings;
        partSettings = CreateRuntimeSettings(sourceSettings, part.useCastTimedLifetime);
        if (partSettings != null)
        {
            return true;
        }

        // Debug.LogWarning($"CompositePrefabEffect: settings are null for part {part.name}.");
        return false;
    }

    void StartFinalShaderLifetimeRoutineIfNeeded(
        CompositePrefabPart part,
        Transform instanceTransform,
        EffectSettings partSettings)
    {
        if (part == null || instanceTransform == null || !part.disableShaderLifetime ||
            !part.enableFinalShaderLifetimeOnFade || partSettings == null)
        {
            return;
        }

        float finalWindow = Mathf.Max(part.finalShaderLifetimeMax, part.finalShaderLifetimeMin, 0f);
        float delay = Mathf.Max(0f, partSettings.defaultLifeTime - partSettings.hideTime - finalWindow);
        StartCoroutine(EnableFinalShaderLifetimeAfterDelay(
            instanceTransform, delay, part.finalShaderLifetimeMin, part.finalShaderLifetimeMax));
    }

    IEnumerator EnableFinalShaderLifetimeAfterDelay(Transform instanceTransform, float delay, float rangeMin, float rangeMax)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (instanceTransform == null)
        {
            yield break;
        }

        EffectShaderLifetimeHelper.Apply(instanceTransform, true, rangeMin, rangeMax);
    }

    void LogSpawnedPart(CompositePrefabPart part, EffectSettings partSettings, Transform setupOwner)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Spawned part='{part.name}' point={part.attachmentPoint} " +
            // $"follow={part.followResolvedTransform} hitTime={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"lifeTime={partSettings.defaultLifeTime:F3}s scale={part.scale:F2} offset={part.positionOffset} owner='{(setupOwner != null ? setupOwner.name : "null")}'.");
#endif
    }
}
