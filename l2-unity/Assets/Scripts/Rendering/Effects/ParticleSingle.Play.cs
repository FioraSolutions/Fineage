using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    public override void PlayPart()
    {
        if (!isActiveAndEnabled)
            return;

        if (_fitToBounds && OwnerTarget != null)
        {
            FitToOwnerWidth(OwnerTarget);
        }

        if (_particles == null || _particles.Length == 0)
        {
            _particles = GetComponentsInChildren<Renderer>(true);
        }

        ExpandVertexMotionBounds();

        _spriteMotionAppRandBaseState = L2MaterialPropertyCopier.CreateFiniteAppRandState();
        _spriteSpinAppRandBaseState = L2MaterialPropertyCopier.AdvanceAppRandState(
            _spriteMotionAppRandBaseState,
            L2AppRand.SpriteSpinDraws);
        _meshAppRandBaseState = L2MaterialPropertyCopier.CreateFiniteAppRandState();

        if (_testSpawnOnlyNoTeardown)
        {
            float t = Now();
            _lifetime.LastEnable = t;
            _lifetime.SpawnedAt = t;
            _lifetime.SpawnedCount = 1;
            _lifetime.Active = true;
            _lifetime.Stopped = true;
            Spawn(t);
            return;
        }

        float now = Now();
        _lifetime.ResetForPlayPart(now);
        _lastShaderFadeDiagLog = 0f;
        _lastQuadSizeDiagLog = 0f;
        _loggedHalfSecondCheckpoint = false;
        _holdController.ResetReleaseLogs();
        _holdController.ResetCastEndFade();

        Renderer slotRenderer = ResolveRenderer();
        float materialLifetime = _lifetime.ReadLifetimeFromSharedMaterials(
            slotRenderer,
            name,
            _owner,
            transform,
            _duration);
        float slotDuration = materialLifetime + ReadMaxInitialDelay(slotRenderer) + 0.03f;
        if (slotDuration > _lifetime.Duration)
            _lifetime.Duration = slotDuration;
        _lifetime.PrepareForPlay(
            materialLifetime,
            _hasFixedDuration,
            _holdController.HasRuntimeLoopOverride,
            _holdController.RuntimeLoopOverrideValue);
        if (slotDuration > _lifetime.Duration)
            _lifetime.Duration = slotDuration;
        _duration = _lifetime.Duration;

        SetActive(false, "playpart_reset");
        // ParticleSingleLifetimeDebug.LogPlay(BuildDebugSnapshot(now));
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropAdenaDestroyLog.Event(
            // "PLAY_PART",
            // this,
            // $"duration={_duration:F3}s materialLife={materialLifetime:F3}s " +
            // $"slotDuration={slotDuration:F3}s delay={ReadMaxInitialDelay(slotRenderer):F3}s " +
            // $"fixed={_hasFixedDuration} maxCount={_maxCount}",
            // includeStack: false);
#endif
    }

    public override void StopPart()
    {
        if (_testSpawnOnlyNoTeardown)
        {
            return;
        }

        float now = Now();
        Renderer stopRenderer = ResolveRenderer();
        Material stopMat = stopRenderer != null && stopRenderer.materials != null && stopRenderer.materials.Length > 0
            ? stopRenderer.materials[0]
            : null;
        // ParticleSingleLifetimeDebug.LogStopPart(BuildDebugSnapshot(now), now, stopRenderer, stopMat);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropAdenaDestroyLog.Event(
            // "STOP_PART",
            // this,
            // $"duration={_duration:F3}s spawnedAt={_lifetime.SpawnedAt:F3}s active={_lifetime.Active}",
            // includeStack: true);
#endif

        _lifetime.RuntimeContinuousLoop = false;
        _lifetime.SpawnedCount = _maxCount;

        L2ShaderHoldController.Settings holdSettings = BuildHoldSettings();
        L2ShaderHoldController.CastTimeline timeline = BuildCastTimeline(now);

        if (_holdController.TryBeginCastEndFadeDefer(holdSettings, timeline))
        {
            EffectShaderLifetimeHelper.FreezeShaderLoopWithoutExpiry(transform);
            ApplyCompositeShaderHoldToAllRuntimeMaterials(now);
            // ParticleSingleLifetimeDebug.LogHoldReleaseDefer(name, timeline, holdSettings);
            return;
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (holdSettings.ShaderHold > 1e-4f && holdSettings.ReleaseByCastProgress)
        {
            // Debug.LogWarning(
                // $"[PARTICLE_SINGLE_STOPPART_NO_DEFER] group='{name}' hold={holdSettings.ShaderHold:F3} " +
                // "cast-end fade defer не сработал — мгновенная остановка.");
        }
#endif

        CompleteImmediateStop(now);
    }

    public override void Setup(EffectSettings s, MagicCastData c)
    {
        _settings = s;
        _castData = c;
        _lifetime.ResolveDurationFromSetup(
            ref _duration,
            _hasFixedDuration,
            _settings,
            _castData,
            out _);
        _lifetime.Duration = _duration;
    }

    public void FitToOwnerWidth(Transform target)
    {
        if (target == null)
        {
            return;
        }

        CharacterController controller = target.GetComponent<CharacterController>();
        if (controller == null)
        {
            return;
        }

        float targetWidth = controller.radius * 2f;
        transform.localScale = new Vector3(targetWidth * 4f, 1f, targetWidth * 4f);
    }

    public void SetRuntimeContinuousLoopOverride(bool hasOverride, bool value)
    {
        _holdController.SetRuntimeLoopOverride(hasOverride, value);

        if ((_lifetime.Active && !_lifetime.Stopped) || _holdController.CastEndFadeRequested)
        {
            ApplyCompositeShaderHoldToAllRuntimeMaterials(Now());
        }
    }
}
