using UnityEngine;
using System.Collections.Generic;

public partial class L2Particle
{
    void TryDestroyWhenAuthoredStreamsComplete()
    {
        if (!IsLifetimeOwnedByAuthoredStreams || _playStartedAt < 0f)
        {
            return;
        }

        if (!ParticleEmitterV2.TryAllComplete(this, out bool anyEnabled) || !anyEnabled)
        {
            return;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float lived = Time.time - _playStartedAt;
        if (lived < 0.25f)
        {
            // V2TaSpawnLog.Warn(
                // "AUTH STREAMS COMPLETE TOO FAST effect='" + name +
                // "' lived=" + lived.ToString("0.###") +
                // "s — destroying GO before burst is visible");
        }
#endif
        Destroy(gameObject);
    }

    public override void Setup(EffectSettings settings, MagicCastData castData, Transform owner)
    {

        base.Setup(settings, castData, owner);

        _settings = settings;
        _castData = castData;


        if (castData != null)
        {
            //_settings.defaultLifeTime = castData.HitTime;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (ShouldTraceLifetime())
        {
            // Debug.Log(
                // $"[TA_LIFETIME_SETUP] effect='{name}' now={Time.time:F3}s " +
                // $"settingsLife={(_settings != null ? _settings.defaultLifeTime : -1f):F3}s " +
                // $"hideTime={(_settings != null ? _settings.hideTime : -1f):F3}s " +
                // $"castStart={(_castData != null ? _castData.StartTime : -1f):F3}s " +
                // $"castHit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
                // $"castFlight={(_castData != null ? _castData.FlightTime : -1f):F3}s " +
                // $"owner='{(_owner != null ? _owner.name : "null")}'.");
        }
#endif

    }

    public override void SetProgress(float normalizedTime)
    {
        throw new System.NotImplementedException();
    }
}
