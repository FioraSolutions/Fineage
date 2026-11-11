using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {
        base.Setup(settings, castData, owner);
        RepairEmptyV2PartsIfNeeded();
        ClearV2Runtime();
        if (!ShouldUseLegacyPartPipeline)
        {
            SubscribeV2ShootIfNeeded();
            SubscribeV2HitIfNeeded();
        }
    }

    protected override void PlayV2()
    {
        RepairEmptyV2PartsIfNeeded();
        int partCount = _v2Parts != null ? _v2Parts.Length : 0;
        int spawnable = 0;
        if (_v2Parts != null)
        {
            for (int i = 0; i < _v2Parts.Length; i++)
            {
                if (_v2Parts[i] != null && _v2Parts[i].IsSpawnable)
                    spawnable++;
            }
        }

        Debug.Log(
            "[ShotOrb] PlayV2 go=" + name +
            " parts=" + partCount +
            " spawnable=" + spawnable +
            " cast=" + (_castData != null) +
            " shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
            " hit=" + (_castData != null ? _castData.HitTime.ToString("0.###") : "-") +
            " targetId=" + (_castData != null ? _castData.TargetObjectId.ToString() : "-"));

        if (!HasSpawnableV2Parts)
        {
            Debug.LogWarning("[ShotOrb] PlayV2 abort — no spawnable V2 parts on " + name);
            return;
        }

        _playStartedAt = Time.time;
        RefreshV2Context();
        LogHomeSpawnPlay();

        QueueV2Parts();
        Debug.Log(
            "[ShotOrb] PlayV2 queued shoot=" + _pendingAnimationShoot.Count +
            " delayed=" + _pendingDelayed.Count +
            " spawned=" + _spawned.Count +
            " target=" + (_context != null && _context.TargetTransform != null
                ? _context.TargetTransform.name
                : "null"));
        if (_castData != null && _castData.SpawnShotVisualsImmediately)
            SpawnPendingAnimationShootParts(shotsOnly: false);
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
