using UnityEngine;

public static partial class EffectCastDurationResolver
{
    public static void LogMismatchIfNeeded(
        string context,
        string groupName,
        float castHit,
        float settingsLife,
        float resolvedDuration,
        bool serverHitOverridesSettings)
    {
        if (castHit <= 0f || settingsLife <= 0f)
        {
            return;
        }

        if (serverHitOverridesSettings)
        {
            // Debug.LogWarning(
                // $"[EFFECT_LIFETIME_MISMATCH] {context} group='{groupName}' " +
                // $"serverHit={castHit:F3}s settingsDefaultLife={settingsLife:F3}s " +
                // $"resolvedDuration={resolvedDuration:F3}s — using server HitTime, not asset defaultLifeTime.");
            return;
        }

        if (Mathf.Abs(resolvedDuration - castHit) > 0.05f && Mathf.Abs(resolvedDuration - settingsLife) < 0.05f)
        {
            // Debug.LogWarning(
                // $"[EFFECT_LIFETIME_MISMATCH] {context} group='{groupName}' " +
                // $"resolvedDuration={resolvedDuration:F3}s differs from serverHit={castHit:F3}s " +
                // $"(settingsLife={settingsLife:F3}s).");
        }
    }
}
