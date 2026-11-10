using UnityEngine;

/// <summary>
/// L2 _Hold math, cast-end fade, and composite loop override state.
/// </summary>
public partial class L2ShaderHoldController
{
    /// <summary>
    /// SizeScale tail starts this fraction of hideWindow before alpha fade (shrink while still visible).
    /// </summary>
    private const float DefaultCastEndHoldReleaseLead = 0.1f;

    public struct Settings
    {
        public float ShaderHold;
        public bool ReleaseByCastProgress;
        public float ReleaseStartNormalized;
        public bool SmoothRelease;
        /// <summary>Fraction of composite hideTime: size release starts this much before alpha fade (cast-end defer only).</summary>
        public float CastEndHoldReleaseLead;
    }

    public struct CastTimeline
    {
        public float Now;
        public float CastStartTime;
        public float SlotDuration;
        public EffectSettings Settings;
    }

    private bool _hasRuntimeLoopOverride;
    private bool _runtimeLoopOverrideValue;
    private bool _castEndFadeRequested;
    private float _castEndHoldSnapshot = -1f;
    private float _baseEmitterAlpha = 1f;
    private bool _loggedReleaseStart;
    private bool _loggedReleaseResume;

    public bool CastEndFadeRequested => _castEndFadeRequested;
    public bool HasRuntimeLoopOverride => _hasRuntimeLoopOverride;
    public bool RuntimeLoopOverrideValue => _runtimeLoopOverrideValue;
    public float BaseEmitterAlpha => _baseEmitterAlpha;

    public bool CanApplyHoldUpdates(bool stopped)
    {
        if (stopped && !_castEndFadeRequested)
        {
            return false;
        }

        return _hasRuntimeLoopOverride || _castEndFadeRequested;
    }

    public void SetRuntimeLoopOverride(bool hasOverride, bool value)
    {
        _hasRuntimeLoopOverride = hasOverride;
        _runtimeLoopOverrideValue = value;
    }

    public void ResetReleaseLogs()
    {
        _loggedReleaseStart = false;
        _loggedReleaseResume = false;
    }

    public void ResetCastEndFade()
    {
        _castEndFadeRequested = false;
        _castEndHoldSnapshot = -1f;
    }

    public void CaptureBaseEmitterAlpha(Material[] materials)
    {
        _baseEmitterAlpha = L2MaterialPropertyCopier.ReadFloatFromFirstMaterial(
            materials,
            L2MaterialPropertyCopier.EmitterAlphaId,
            1f);
    }

    public bool ShouldDeferStopForRelease(in Settings settings)
    {
        return settings.ShaderHold > 1e-4f && settings.ReleaseByCastProgress;
    }

    public bool TryBeginCastEndFadeDefer(in Settings settings, in CastTimeline timeline)
    {
        if (!ShouldDeferStopForRelease(in settings))
        {
            return false;
        }

        _castEndFadeRequested = true;
        _castEndHoldSnapshot = settings.ShaderHold;
        return true;
    }

    public void CompleteImmediateStop()
    {
        _hasRuntimeLoopOverride = true;
        _runtimeLoopOverrideValue = false;
        _castEndFadeRequested = false;
        _castEndHoldSnapshot = -1f;
    }

    public float EvaluateHold(in Settings settings, in CastTimeline timeline)
    {
        if (_castEndFadeRequested)
        {
            float snapshot = _castEndHoldSnapshot >= 0f ? _castEndHoldSnapshot : settings.ShaderHold;
            float holdReleaseMul = ComputeCastEndHoldReleaseMul(in settings, in timeline);
            return snapshot * holdReleaseMul;
        }

        if (!_hasRuntimeLoopOverride || !_runtimeLoopOverrideValue)
        {
            return 0f;
        }

        return EvaluateHoldForCastNow(in settings, in timeline);
    }

    public bool ApplyToMaterials(
        Material[] materials,
        in Settings settings,
        in CastTimeline timeline,
        string debugGroupName,
        float spawnedAt,
        float baseShaderLifetime)
    {
        if (materials == null || materials.Length == 0)
        {
            return false;
        }

        float holdValue = EvaluateHold(in settings, in timeline);
        L2MaterialPropertyCopier.SetFloatOnMaterials(materials, L2MaterialPropertyCopier.HoldId, holdValue);

        Material holdLogMat = null;
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null && materials[i].HasProperty(L2MaterialPropertyCopier.HoldId))
            {
                holdLogMat = materials[i];
                break;
            }
        }

        ApplyCastEndEmitterFade(materials, in timeline);
        if (_castEndFadeRequested)
        {
            float fadeMul = ComputeCastEndFadeMul(in timeline);
            float holdReleaseMul = ComputeCastEndHoldReleaseMul(in settings, in timeline);
            // ParticleSingleLifetimeDebug.LogCastEndFadeAlpha(
                // debugGroupName,
                // in timeline,
                // holdLogMat,
                // holdValue,
                // _baseEmitterAlpha,
                // fadeMul,
                // holdReleaseMul);
        }

        // ParticleSingleLifetimeDebug.TryLogHoldReleaseTransition(
            // debugGroupName,
            // in settings,
            // in timeline,
            // holdValue,
            // holdLogMat,
            // spawnedAt,
            // baseShaderLifetime,
            // timeline.SlotDuration,
            // _hasRuntimeLoopOverride,
            // _runtimeLoopOverrideValue,
            // _castEndFadeRequested,
            // ref _loggedReleaseStart,
            // ref _loggedReleaseResume);

        return TryCompleteCastEndFade(materials, in settings, in timeline, holdValue, debugGroupName, spawnedAt, baseShaderLifetime);
    }

}
