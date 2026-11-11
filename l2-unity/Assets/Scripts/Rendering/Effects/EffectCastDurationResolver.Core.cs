using UnityEngine;

public static partial class EffectCastDurationResolver
{
    public static float Resolve(
        float prefabDuration,
        bool hasFixedDuration,
        EffectSettings settings,
        MagicCastData castData,
        out float legacyHitDuration,
        out bool serverHitOverridesSettings)
    {
        legacyHitDuration = 0f;
        serverHitOverridesSettings = false;

        if (hasFixedDuration)
        {
            return prefabDuration;
        }

        float castHitDuration = castData != null && castData.HitTime > 0f ? castData.HitTime : 0f;
        float settingsDuration = settings != null && settings.defaultLifeTime > 0f ? settings.defaultLifeTime : 0f;

        float authoritativeDuration;
        if (castHitDuration > 0f)
        {
            authoritativeDuration = castHitDuration;
            serverHitOverridesSettings = settingsDuration > castHitDuration + 0.01f;
        }
        else if (settingsDuration > 0f)
        {
            authoritativeDuration = settingsDuration;
        }
        else
        {
            authoritativeDuration = 0f;
        }

        return Mathf.Max(prefabDuration, authoritativeDuration);
    }
}
