using UnityEngine;

public partial class L2ShaderHoldController
{
    public void ApplySpawnHoldSpecs(Material runtimeMat, in Settings settings, float holdValue)
    {
        if (runtimeMat == null)
        {
            return;
        }

        if (runtimeMat.HasProperty(L2MaterialPropertyCopier.HoldSizeReferenceId))
        {
            runtimeMat.SetFloat(L2MaterialPropertyCopier.HoldSizeReferenceId, settings.ShaderHold);
        }

        if (runtimeMat.HasProperty(L2MaterialPropertyCopier.HoldId))
        {
            runtimeMat.SetFloat(L2MaterialPropertyCopier.HoldId, holdValue);
        }
    }

    private void ApplyCastEndEmitterFade(Material[] materials, in CastTimeline timeline)
    {
        if (!_castEndFadeRequested || materials == null)
        {
            return;
        }

        float fadeMul = ComputeCastEndFadeMul(in timeline);
        L2MaterialPropertyCopier.SetFloatOnMaterials(
            materials,
            L2MaterialPropertyCopier.EmitterAlphaId,
            _baseEmitterAlpha * fadeMul);
    }

    private bool TryCompleteCastEndFade(
        Material[] materials,
        in Settings settings,
        in CastTimeline timeline,
        float holdValue,
        string debugGroupName,
        float spawnedAt,
        float baseShaderLifetime)
    {
        if (!_castEndFadeRequested)
        {
            return false;
        }

        float fadeMul = ComputeCastEndFadeMul(in timeline);
        float castDur = ResolveCastDuration(in timeline);
        float elapsed = ResolveCastElapsed(in timeline);

        bool castTimelineEnded = elapsed >= castDur - 0.02f;
        bool fadeDone = fadeMul <= 0.02f;
        if (!castTimelineEnded && !fadeDone)
        {
            return false;
        }

        // ParticleSingleLifetimeDebug.EnsureHoldReleaseResumeLogged(
            // debugGroupName,
            // in settings,
            // in timeline,
            // holdValue,
            // fadeMul,
            // materials,
            // spawnedAt: spawnedAt,
            // baseShaderLifetime: baseShaderLifetime,
            // slotDuration: timeline.SlotDuration,
            // ref _loggedReleaseResume);

        // ParticleSingleLifetimeDebug.LogHoldReleaseDone(debugGroupName, in timeline);
        // ParticleSingleLifetimeDebug.ResetCastEndFadeAlphaLog(debugGroupName);
        CompleteImmediateStop();
        return true;
    }

    private float ComputeCastEndFadeMul(in CastTimeline timeline)
    {
        float castDur = ResolveCastDuration(in timeline);
        float elapsed = ResolveCastElapsed(in timeline);
        float fadeWindow = ResolveCastEndFadeWindow(in timeline);
        float fadeStart = Mathf.Max(0f, castDur - fadeWindow);
        return elapsed <= fadeStart
            ? 1f
            : 1f - Mathf.Clamp01((elapsed - fadeStart) / Mathf.Max(1e-4f, fadeWindow));
    }

    private float ComputeCastEndHoldReleaseMul(in Settings settings, in CastTimeline timeline)
    {
        float castDur = ResolveCastDuration(in timeline);
        float elapsed = ResolveCastElapsed(in timeline);
        float fadeWindow = ResolveCastEndFadeWindow(in timeline);
        float fadeStart = Mathf.Max(0f, castDur - fadeWindow);
        float releaseStart = Mathf.Max(0f, fadeStart - fadeWindow * ResolveCastEndHoldReleaseLead(in settings));
        if (elapsed <= releaseStart)
        {
            return 1f;
        }

        return 1f - Mathf.Clamp01((elapsed - releaseStart) / Mathf.Max(1e-4f, fadeWindow));
    }
}
