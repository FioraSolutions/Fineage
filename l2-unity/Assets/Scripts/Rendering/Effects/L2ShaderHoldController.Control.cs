using UnityEngine;

public partial class L2ShaderHoldController
{
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
