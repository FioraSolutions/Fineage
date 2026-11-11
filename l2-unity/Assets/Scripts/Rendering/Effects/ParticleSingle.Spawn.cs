using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    private void Spawn(float now)
    {
        Renderer renderer = ResolveRenderer();
        if (renderer == null)
        {
            return;
        }

        SetActive(true, "spawn");
        _lifetime.MarkSpawned(now);
        _holdController.ResetReleaseLogs();
        _holdController.ResetCastEndFade();

        float shaderStartTime = now - _relativeWarmupTime;
        float seed = Random.Range(-100f, 100f);
        // Sprite spin shares the same verified TLS chain as motion:
        // state before StartSpin = state before StartVelocity + 22 draws.
        L2ShaderHoldController.Settings holdSettings = BuildHoldSettings();
        float holdValue = _holdController.EvaluateHold(holdSettings, BuildCastTimeline(now));

        Material[] runtimeMaterials = renderer.materials;
        Material[] sharedMaterials = renderer.sharedMaterials;
        for (int i = 0; i < runtimeMaterials.Length; i++)
        {
            Material runtimeMat = runtimeMaterials[i];
            if (runtimeMat == null)
            {
                continue;
            }

            Material sharedMat = sharedMaterials != null && i < sharedMaterials.Length ? sharedMaterials[i] : null;

            if (runtimeMat.HasProperty(AlphaShaderId) && sharedMat != null && sharedMat.HasProperty(AlphaShaderId))
            {
                runtimeMat.SetFloat(AlphaShaderId, sharedMat.GetFloat(AlphaShaderId));
            }

            L2MaterialPropertyCopier.CopyLifetimeFadeAndFxFromShared(runtimeMat, sharedMat);
            L2MaterialPropertyCopier.CopyMeshAppRandStartSpinFromBaseState(
                runtimeMat,
                sharedMat,
                _meshAppRandBaseState,
                0);
            // MeshRenderer path never binds _L2FxParticleSlots. The instanced
            // shader variant would read StartTime from that unbound buffer.
            runtimeMat.enableInstancing = false;
            if (sharedMat != null)
                sharedMat.enableInstancing = false;
            runtimeMat.SetFloat(L2MaterialPropertyCopier.StartTimeId, shaderStartTime);
            runtimeMat.SetFloat(L2MaterialPropertyCopier.SeedId, seed);
            L2MaterialPropertyCopier.SetSpriteSpinRandState(runtimeMat, _spriteSpinAppRandBaseState);
            L2MaterialPropertyCopier.SetSpriteMotionRandState(runtimeMat, _spriteMotionAppRandBaseState);
            SetOwnerWorldPos(runtimeMat);

            if (SurfaceNormal != Vector3.zero)
            {
                runtimeMat.SetVector(SurfaceNormalsShaderId, SurfaceNormal);
            }

            _holdController.ApplySpawnHoldSpecs(runtimeMat, in holdSettings, holdValue);

            if (runtimeMat.HasProperty(L2MaterialPropertyCopier.EmitterAlphaId))
            {
                _holdController.CaptureBaseEmitterAlpha(new[] { runtimeMat });
            }

            // ParticleSingleLifetimeDebug.LogSpawn(
                // BuildDebugSnapshot(now),
                // i,
                // now,
                // shaderStartTime,
                // _relativeWarmupTime,
                // seed,
                // holdValue,
                // _releaseShaderHoldByCastProgress,
                // _shaderHoldReleaseStartNormalized,
                // _smoothShaderHoldRelease,
                // renderer,
                // runtimeMat);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (L2FxQuadSizeDiagnostic.ShouldTrace(name, _owner, transform))
            {
                L2FxQuadSizeDiagnostic.Log(name, renderer, now, runtimeMat);
            }
#endif
        }

    }

    private Renderer ResolveRenderer()
    {
        if (_particles == null || _particles.Length == 0)
        {
            return null;
        }

        return _particles[0];
    }

    private void SetActive(bool value, string reason = null)
    {
        Renderer renderer = ResolveRenderer();
        if (renderer == null)
        {
            // ParticleSingleLifetimeDebug.LogSetActive(BuildDebugSnapshot(Now()), value, reason, null, false);
            return;
        }

        bool wasActive = renderer.gameObject.activeSelf;
        renderer.gameObject.SetActive(value);
        // ParticleSingleLifetimeDebug.LogSetActive(BuildDebugSnapshot(Now()), value, reason, renderer, wasActive);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (!value)
        {
            // DropAdenaDestroyLog.Event(
                // "SET_ACTIVE_FALSE",
                // this,
                // $"reason={reason} slot='{renderer.name}' wasActive={wasActive}",
                // includeStack: true);
        }
#endif
    }
}
