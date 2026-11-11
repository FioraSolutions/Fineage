using System.Collections.Generic;
using UnityEngine;

public partial class L2Particle
{
    public override void Play()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (_playStartedAt > 0f && Time.time - _playStartedAt < 8f)
            // EffectDoublePlayLog.Repeat("L2Particle.Play", this, _playStartedAt, _firstPlayStack);
        // _firstPlayStack = EffectDoublePlayLog.CaptureStack();
#endif
        _playStartedAt = Time.time;
        ResetTimer();
        if (!_skipScheduledDestroyForDebug &&
            !IsDestroyDeferredUntilHomeArrival &&
            !IsLifetimeOwnedByProjectile &&
            !IsLifetimeOwnedByAuthoredStreams &&
            !IsLifetimeOwnedByHost)
        {
            DestoryEffect(_settings, _castData);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (_skipScheduledDestroyForDebug)
        {
            // Debug.Log($"[L2Particle] SkipScheduledDestroyForDebug: '{name}' — DestoryEffect не вызывается.");
        }

        if (ShouldTraceLifetime())
        {
            // Debug.Log(
                // $"[L2Particle] FX_PLAY effect='{name}' playAt={_playStartedAt:F3}s " +
                // $"scheduledLife={(_settings != null ? _settings.defaultLifeTime : -1f):F3}s " +
                // $"scheduledHide={(_settings != null ? _settings.hideTime : -1f):F3}s " +
                // $"settingsAsset='{(_settings != null ? _settings.name : "null")}' " +
                // $"shaderLife={BuildShaderLifetimeRangesSnapshot()} " +
                // $"skipDestroy={_skipScheduledDestroyForDebug}.");
        }
#endif
    }

    public void ResetTimer()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // if (_playStartedAt > 0f && Time.time - _playStartedAt > 0.02f && Time.time - _playStartedAt < 8f)
            // EffectDoublePlayLog.Repeat("L2Particle.ResetTimer", this, _playStartedAt, _firstPlayStack);
#endif
        RefreshParticleGroups();
        L2FxAlphaBlendDrawOrder.Apply(this);
        ConfigurePrePlayModules();

        for (int i = 0; i < _particleGroups.Length; i++)
        {
            EffectPart part = _particleGroups[i];
            if (part == null)
                continue;

            part.Setup(_settings, _castData);

            if (_owner != null)
                part.OwnerTarget = _owner;

            // Move-click mark (and other host-played FX) call Play() without Setup(),
            // so _settings stays null. Still emit parts — old path used ResetTimer(1f, null).
            if (_settings != null)
            {
                part.FollowTarget = _settings.isFollowCaster ? _owner : null;
                if (!_settings.isFollowCaster)
                    part.SurfaceNormal = _surfaceNormal;
            }
            else
            {
                part.FollowTarget = null;
                part.SurfaceNormal = _surfaceNormal;
            }

            part.PlayPart();
        }
    }

    void ConfigurePrePlayModules()
    {
        MonoBehaviour[] behaviours = GetComponentsInChildren<MonoBehaviour>(true);
        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IEffectPrePlayConfigurator configurator)
            {
                configurator.ConfigureBeforePlay(_owner);
            }
        }
    }

    private void RefreshParticleGroups()
    {
        if (_particleGroups == null || _particleGroups.Length == 0)
        {
            _particleGroups = GetComponentsInChildren<EffectPart>();
            return;
        }

        List<EffectPart> refreshed = new List<EffectPart>(_particleGroups.Length + 1);
        for (int i = 0; i < _particleGroups.Length; i++)
        {
            EffectPart part = _particleGroups[i];
            if (part != null && part.isActiveAndEnabled && !refreshed.Contains(part))
            {
                refreshed.Add(part);
            }
        }

        EffectPart[] children = GetComponentsInChildren<EffectPart>();
        for (int i = 0; i < children.Length; i++)
        {
            EffectPart part = children[i];
            if (part == null || !part.isActiveAndEnabled || refreshed.Contains(part))
            {
                continue;
            }

            if (part is IParticleEmitterV2 ||
                (part is ParticleGroup group && group.IsHomeFlightAnchor))
            {
                refreshed.Add(part);
            }
        }

        _particleGroups = refreshed.ToArray();
    }
}
