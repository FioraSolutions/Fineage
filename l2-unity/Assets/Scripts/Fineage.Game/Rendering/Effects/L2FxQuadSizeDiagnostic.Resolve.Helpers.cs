using UnityEngine;

public static partial class L2FxQuadSizeDiagnostic
{
    private static float ResolveLifetime(Material mat)
    {
        if (!mat.HasProperty("_LifetimeRange"))
        {
            return 2f;
        }

        Vector4 lifetimeRange = mat.GetVector("_LifetimeRange");
        float seed = mat.HasProperty("_Seed") ? mat.GetFloat("_Seed") : 0f;
        float startTime = mat.HasProperty("_StartTime") ? mat.GetFloat("_StartTime") : 0f;
        return Mathf.Max(RandomRange(new Vector2(lifetimeRange.x, lifetimeRange.y), seed, startTime, 7f), 1e-4f);
    }

    private static float ResolveAgeSeconds(float now, Material mat, float delay)
    {
        float startTime = mat.HasProperty("_StartTime") ? mat.GetFloat("_StartTime") : 0f;
        if (startTime <= 0f)
        {
            return now;
        }

        return Mathf.Max(0f, now - startTime - delay);
    }

    private static float ResolveParticleAgeNorm(float now, Material mat, float ageSec, float lifetimeSec)
    {
        float startTime = mat.HasProperty("_StartTime") ? mat.GetFloat("_StartTime") : 0f;
        float age = ageSec;
        if (startTime <= 0f)
        {
            age = now;
        }

        bool loopPreview = mat.HasProperty("_LoopSizeScalePreview") && mat.GetFloat("_LoopSizeScalePreview") > 0.5f;
        if (loopPreview)
        {
            return Frac(age / lifetimeSec);
        }

        return Mathf.Clamp01(age / lifetimeSec);
    }
}
