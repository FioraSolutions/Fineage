using UnityEngine;

public partial class L2ShaderHoldController
{
    private static float ResolveCastEndHoldReleaseLead(in Settings settings)
    {
        if (settings.CastEndHoldReleaseLead > 1e-4f)
        {
            return Mathf.Clamp(settings.CastEndHoldReleaseLead, 0f, 0.5f);
        }

        return DefaultCastEndHoldReleaseLead;
    }

    private float EvaluateHoldForCastNow(in Settings settings, in CastTimeline timeline)
    {
        if (!settings.ReleaseByCastProgress)
        {
            return settings.ShaderHold;
        }

        // Cast-end defer fades via _EmitterAlpha — keep hold (and size) stable until StopPart.
        if (!_castEndFadeRequested && ShouldDeferStopForRelease(in settings))
        {
            return settings.ShaderHold;
        }

        float dur = ResolveCastDuration(in timeline);
        float u = Mathf.Clamp01(ResolveCastElapsed(in timeline) / dur);
        return EvaluateHoldForNormalizedCast(u, in settings);
    }

    private static float EvaluateHoldForNormalizedCast(float u, in Settings settings)
    {
        if (!settings.ReleaseByCastProgress)
        {
            return settings.ShaderHold;
        }

        u = Mathf.Clamp01(u);
        float start = Mathf.Clamp(settings.ReleaseStartNormalized, 0f, 0.999f);
        if (u < start)
        {
            return settings.ShaderHold;
        }

        if (!settings.SmoothRelease)
        {
            return 0f;
        }

        if (start >= 1f - 1e-4f)
        {
            return 0f;
        }

        float t = Mathf.InverseLerp(start, 1f, u);
        return Mathf.Lerp(settings.ShaderHold, 0f, t);
    }

    public static float ResolveCastDuration(in CastTimeline timeline)
    {
        float slotDuration = Mathf.Max(1e-4f, timeline.SlotDuration);
        if (timeline.Settings != null && timeline.Settings.defaultLifeTime > 1e-4f)
        {
            float settingsLife = timeline.Settings.defaultLifeTime;
            if (settingsLife > slotDuration + 0.01f)
            {
                return slotDuration;
            }

            return settingsLife;
        }

        return slotDuration;
    }

    public static float ResolveCastElapsed(in CastTimeline timeline)
    {
        return Mathf.Max(0f, timeline.Now - timeline.CastStartTime);
    }

    public static float ResolveCastEndFadeWindow(in CastTimeline timeline)
    {
        if (timeline.Settings != null && timeline.Settings.hideTime > 1e-4f)
        {
            return timeline.Settings.hideTime;
        }

        return 0.5f;
    }
}
