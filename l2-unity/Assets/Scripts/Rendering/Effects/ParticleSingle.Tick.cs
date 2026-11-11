using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    private void Update()
    {
        if (_testSpawnOnlyNoTeardown)
        {
            return;
        }

        if (_lifetime.Stopped && !_holdController.CastEndFadeRequested)
        {
            return;
        }

        if (!_holdController.CanApplyHoldUpdates(_lifetime.Stopped))
        {
            return;
        }

        ApplyCompositeShaderHoldToAllRuntimeMaterials(Now());
    }

    public void FixedUpdate()
    {
        if (_testSpawnOnlyNoTeardown)
        {
            return;
        }

        if (_lifetime.Stopped && !_holdController.CastEndFadeRequested)
        {
            return;
        }

        float now = Now();
        if (_lifetime.IsBeforeStartDelay(now, _startDelay))
        {
            return;
        }

        bool shouldLoopContinuously = _lifetime.ShouldLoopContinuously(_forceContinuousSpawning);
        if (_lifetime.IsSlotExpired(now, _preserveShaderTimeInContinuousLoop))
        {
            // ParticleSingleLifetimeDebug.LogSlotOff(BuildDebugSnapshot(now), now);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DropAdenaDestroyLog.Event(
                // "SLOT_EXPIRED",
                // this,
                // $"spawnedAt={_lifetime.SpawnedAt:F3}s duration={_lifetime.Duration:F3}s " +
                // $"age={(now - _lifetime.SpawnedAt):F3}s maxCount={_maxCount} spawned={_lifetime.SpawnedCount}",
                // includeStack: true);
#endif
            SetActive(false, "slot_expired");
            _lifetime.Active = false;
        }

        if (!_lifetime.Active)
        {
            if (_lifetime.SpawnedCount >= _maxCount && !shouldLoopContinuously)
            {
                // ParticleSingleLifetimeDebug.LogStopMax(BuildDebugSnapshot(now), now);
                _lifetime.Stopped = true;
                return;
            }

            if (_isBurstSpawning && !shouldLoopContinuously)
            {
                Spawn(now);
                _lifetime.SpawnedCount = _maxCount;
                return;
            }

            float spawnInterval = 1f / Mathf.Max(1f, _countPerSecond);
            if (now - _lifetime.LastLoop >= spawnInterval)
            {
                _lifetime.LastLoop = now;
                Spawn(now);
                _lifetime.SpawnedCount += 1;
            }
        }
        else
        {
            UpdateOwnerWorldPos();
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (ParticleSingleLifetimeDebug.ShouldTrace(name, _owner, transform)
                // && _lifetime.Active
                // && !_loggedHalfSecondCheckpoint
                // && now - _lifetime.SpawnedAt >= 0.45f)
            {
                _loggedHalfSecondCheckpoint = true;
                Renderer checkRenderer = ResolveRenderer();
                Material checkMat = checkRenderer != null && checkRenderer.materials != null && checkRenderer.materials.Length > 0
                    ? checkRenderer.materials[0]
                    : null;
                // ParticleSingleLifetimeDebug.LogCheck500ms(BuildDebugSnapshot(now), now, checkRenderer, checkMat);
            }

            // if (ParticleSingleLifetimeDebug.ShouldTrace(name, _owner, transform) && now - _lastShaderFadeDiagLog >= 0.25f)
            {
                _lastShaderFadeDiagLog = now;
                // ParticleSingleLifetimeDebug.LogTick(BuildDebugSnapshot(now), now);
            }

            if (L2FxQuadSizeDiagnostic.ShouldTrace(name, _owner, transform)
                && _lifetime.Active
                && now - _lastQuadSizeDiagLog >= L2FxQuadSizeDiagnostic.LogIntervalSec)
            {
                _lastQuadSizeDiagLog = now;
                Renderer quadRenderer = ResolveRenderer();
                Material runtimeMat = quadRenderer != null && quadRenderer.materials != null && quadRenderer.materials.Length > 0
                    ? quadRenderer.materials[0]
                    : null;
                L2FxQuadSizeDiagnostic.Log(name, quadRenderer, now, runtimeMat);
            }
#endif
        }
    }

    public ParticleGroupAuthoring CaptureAuthoring()
    {
        if (_particles == null || _particles.Length == 0)
        {
            _particles = GetComponentsInChildren<Renderer>(true);
        }

        return new ParticleGroupAuthoring
        {
            particles = _particles,
            startDelay = _startDelay,
            countPerSecond = _countPerSecond,
            maxCount = Mathf.Max(1, _maxCount),
            cloneToMaxCount = false,
            cloneLimit = 8,
            useGpuInstancing = false,
            isBurstSpawning = _isBurstSpawning || _maxCount <= 1,
            relativeWarmupTime = _relativeWarmupTime,
            duration = _duration,
            hasFixedDuration = _hasFixedDuration,
            instantKillAtCastEnd = _instantKillAtCastEnd,
            respawnDeadParticles = false,
            stretchParticleLifeToWindow = _maxCount <= 1
        };
    }
}
