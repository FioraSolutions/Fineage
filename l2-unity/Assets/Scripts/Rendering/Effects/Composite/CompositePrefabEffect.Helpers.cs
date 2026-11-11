using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect : TimedCompositeEffectBase
{
    protected override float ResolveCastTimedLifetimeSeconds()
    {
        if (_matchLifetimeToSkillAnimation &&
            _castData != null &&
            _castData.SkillAnimationDuration > 0f)
        {
            return _castData.SkillAnimationDuration;
        }

        return base.ResolveCastTimedLifetimeSeconds();
    }

    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        WireRenderingPacketServices();
        base.Setup(settings, castData, owner);
        InitializeTimedComposite(settings, castData);
        _context = CompositeEffectUtilities.BuildContext(owner, castData);
        _spawnedPartInstances.Clear();
        _launchedProjectileParts.Clear();
        _launchedHomeProjectileParts.Clear();
        _pendingHitColliderParts.Clear();
        _pendingAnimationShootParts.Clear();
        _lightSpawned = false;
        if (ShouldUseLegacyPartPipeline)
        {
            SubscribeShootEventIfNeeded();
            SubscribeProjectileHitEventIfNeeded();
        }
    }

    public override void Play()
    {
        if (!ShouldUseLegacyPartPipeline)
        {
            PlayV2();
            return;
        }

        if (_parts == null || _parts.Length == 0)
        {
            // Debug.LogWarning("CompositePrefabEffect: no parts configured.");
            return;
        }

        _lightSpawned = false;
        _lastShootChannel = null;
        _lastShootAt = -1f;
        _lastShootStack = null;
        _playStartedAt = Time.time;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Play started at={_playStartedAt:F3}s hit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"flight={(_castData != null ? _castData.FlightTime : -1f):F3}s serverShoot={(_castData != null ? _castData.serverTimeToShoot : -1f):F3}s.");
#endif

        CompositePartScheduler.Queue(
            _parts,
            _castData,
            _pendingParts,
            _pendingHitColliderParts,
            _pendingAnimationShootParts,
            SpawnPart);
        if (_castData != null && _castData.SpawnShotVisualsImmediately)
            ProcessShootEvent("none-anim");
        StartPendingPartsRoutineIfNeeded();
        StartShootFallbackRoutineIfNeeded();
        if (!_skipDestroyCompositeByLifetime)
        {
            DestroyCompositeByLifetime();
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        else
        {
            // Debug.Log($"{DebugPrefix} SkipDestroyCompositeByLifetime=true — корень композита не будет уничтожен по lifetime (дочерние объекты остаются в сцене).");
        }
#endif
    }
}
