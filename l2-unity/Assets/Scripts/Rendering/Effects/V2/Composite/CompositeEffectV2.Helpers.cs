using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        base.Setup(settings, castData, owner);
        ClearV2Runtime();
        if (!ShouldUseLegacyPartPipeline)
        {
            SubscribeV2ShootIfNeeded();
            SubscribeV2HitIfNeeded();
        }
    }

    protected override void PlayV2()
    {
        if (!HasSpawnableV2Parts)
        {
            if (TraceCrasherWind())
                Debug.Log("[WINDSTRIKE] Composite no V2 parts");
            return;
        }

        _playStartedAt = Time.time;
        RefreshV2Context();
        LogHomeSpawnPlay();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Play started at={_playStartedAt:F3}s hit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"flight={(_castData != null ? _castData.FlightTime : -1f):F3}s serverShoot={(_castData != null ? _castData.serverTimeToShoot : -1f):F3}s.");
#endif

        QueueV2Parts();
        if (TraceCrasherWind())
        {
            Debug.Log(
                "[WINDSTRIKE] Composite queued shoot=" + _pendingAnimationShoot.Count +
                " delayed=" + _pendingDelayed.Count +
                " spawned=" + _spawned.Count +
                " immediate=" + (_castData != null && _castData.SpawnShotVisualsImmediately));
        }
        if (_castData != null && _castData.SpawnShotVisualsImmediately)
            SpawnPendingAnimationShootParts();
        if (TraceCrasherWind())
            Debug.Log("[WINDSTRIKE] Composite after shoot spawned=" + _spawned.Count);
        StartV2PendingRoutineIfNeeded();
        StartV2ShootWatchdogIfNeeded();
        if (!SkipDestroyCompositeByLifetime)
        {
            EffectSettings lifeTimeSettings = SelectLifetimeSettings();
            if (lifeTimeSettings != null)
            {
                ExtendLifetimeThroughProjectileImpact(lifeTimeSettings);
                DestoryEffect(lifeTimeSettings, _castData);
            }
        }
    }

    protected override void PreparePartPlayback(CompositePrefabPart part, BaseEffect instance)
    {
        PrepareV2Playback(instance);
    }
}
