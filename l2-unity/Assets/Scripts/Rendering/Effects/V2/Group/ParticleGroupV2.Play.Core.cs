using UnityEngine;
using System.Collections.Generic;

public partial class ParticleGroupV2
{
    public override void PlayPart()
    {
        if (!isActiveAndEnabled)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // Debug.LogWarning(
                // "[ParticleGroupV2] Play SKIP inactive '" + name +
                // "' path=" + GetHierarchyPath() +
                // " goActive=" + gameObject.activeInHierarchy +
                // " enabled=" + enabled);
#endif
            return;
        }

        ParticleStreamRuntime runtime = EnsureRuntime();
        ParticleGroupAuthoring authoring = CaptureAuthoring();
        float authoredLife = L2FxAdjustParticleLife.ReadAuthoredMaxLife(
            ResolveSlotRenderer(),
            _duration);
        float particleLife = authoredLife;
        if (authoring.stretchParticleLifeToWindow &&
            _emissionWindow > authoredLife + 1e-4f)
        {
            particleLife = _emissionWindow;
            _lifetimePolicy = ParticleLifetimePolicy.StretchParticleLifetimeToCast;
        }

        authoring.authoredParticleLife = authoredLife;
        authoring.targetParticleLife = particleLife;
        runtime.Bind(authoring);
        runtime.DebugName = name;
        runtime.SetVisible(_streamVisible);
        float now = Now();
        float emitWindow = _hostOwnedEmission
            ? float.PositiveInfinity
            : Mathf.Max(_emissionWindow, particleLife);
        runtime.Start(now, emitWindow, particleLife);
        if (WindOrbTrace.Matches(this))
        {
            _windFlightTrace = true;
            _windCaster = WindOrbTrace.Caster(this);
            _windPlayAt = Time.time;
            _windTraceStage = 0;
            Debug.Log(
                "[WINDORB] play caster=" + _windCaster +
                " emitter=" + name +
                " warmup=" + _relativeWarmupTime.ToString("0.###") +
                " authored=" + authoredLife.ToString("0.###") +
                " particleLife=" + particleLife.ToString("0.###") +
                " window=" + _emissionWindow.ToString("0.###") +
                " emitWindow=" + (float.IsInfinity(emitWindow) ? "inf" : emitWindow.ToString("0.###")) +
                " respawn=" + authoring.respawnDeadParticles +
                " max=" + _maxCount +
                " cps=" + _countPerSecond.ToString("0.###") +
                " gpu=" + runtime.IsGpuDraw +
                " slots=" + runtime.SlotCount +
                " visible=" + _streamVisible +
                " hostOwned=" + _hostOwnedEmission);
        }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        _loggedHiddenTick = false;
        int slotCount = runtime.SlotCount;
        float warmupFrac = Mathf.Max(0f, _relativeWarmupTime);
        // Debug.Log(
            // "[ParticleGroupV2] Play START '" + name +
            // "' path=" + GetHierarchyPath() +
            // " visible=" + _streamVisible +
            // " gpu=" + runtime.IsGpuDraw +
            // " slots=" + slotCount +
            // " cps=" + _countPerSecond +
            // " max=" + _maxCount +
            // " burst=" + _isBurstSpawning +
            // " hasFixedDuration=" + _hasFixedDuration +
            // " externalWindow=" + _hasExternalEmissionWindow +
            // " instantKill=" + _instantKillAtCastEnd +
            // " window=" + _emissionWindow.ToString("0.###") +
            // " duration=" + _duration.ToString("0.###") +
            // " shaderLife=" + particleLife.ToString("0.###") +
            // " authoredLife=" + authoredLife.ToString("0.###") +
            // " stretch=" + authoring.stretchParticleLifeToWindow +
            // " respawn=" + authoring.respawnDeadParticles +
            // " warmupFrac=" + warmupFrac.ToString("0.###") +
            // " warmupTicks=" + _warmupTicksPerSecond.ToString("0.###") +
            // " now=" + now.ToString("0.###"));
        if (_compareLogEnabled)
        {
            // ParticleGroupV2CompareLog.WritePlay(
                // this,
                // authoring,
                // runtime.GpuMaterials != null && runtime.GpuMaterials.Length > 0
                    // ? runtime.GpuMaterials[0]
                    // : ResolveSlotRenderer() != null
                        // ? ResolveSlotRenderer().sharedMaterial
                        // : null);
        }

        // if (NpcDeco2911Trace.Matches(name))
        {
            // NpcDeco2911Trace.Log(
                // "PlayPart hostOwned=" + _hostOwnedEmission +
                // " streamVisible=" + _streamVisible +
                // " gpuAfterStart=" + runtime.IsGpuDraw +
                // " spawnedTotal=" + runtime.SpawnedTotal);
            // NpcDeco2911Trace.DumpGroup(this);
        }
#endif
    }

    public override void StopPart()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.Stop(_runtime.InstantKillAtCastEnd ? EmitterStopMode.Kill : EmitterStopMode.Drain);
    }

    public override void Setup(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        ResolveEmissionWindow(settings, castData);
        EnsureRuntime().Bind(CaptureAuthoring());
    }

    public ParticleGroupAuthoring CaptureAuthoring()
    {
        return new ParticleGroupAuthoring
        {
            particles = _particles,
            startDelay = _startDelay,
            countPerSecond = _countPerSecond,
            maxCount = _maxCount,
            cloneToMaxCount = _cloneParticlesToMaxCount,
            cloneLimit = _cloneParticleLimit,
            useGpuInstancing = _useGpuInstancing,
            coordinateSystem = _coordinateSystem,
            isBurstSpawning = _isBurstSpawning,
            relativeWarmupTime = _relativeWarmupTime,
            warmupTicksPerSecond = _warmupTicksPerSecond,
            duration = _duration,
            hasFixedDuration = _hasFixedDuration || _forceAuthoredTiming,
            instantKillAtCastEnd = _instantKillAtCastEnd,
            // Cast-window: UC RespawnDeadParticles (serialized). Fixed-duration
            // parts keep the old no-respawn rule so existing projectile prefabs
            // without this field do not start looping.
            respawnDeadParticles =
                _hostOwnedEmission ||
                (!_forceAuthoredTiming && !_hasFixedDuration && _respawnDeadParticles),
            hostOwnedEmission = _hostOwnedEmission,
            stretchParticleLifeToWindow =
                !_forceAuthoredTiming && _stretchParticleLifeToWindow
        };
    }
}
