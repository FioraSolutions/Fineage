using UnityEngine;

public static partial class CompositePartSpawnHelper
{
    public static void ApplyHideTimeOverride(CompositePrefabPart part, EffectSettings partSettings)
    {
        if (part == null || partSettings == null || !part.overrideHideTime)
        {
            return;
        }

        float hide = part.customHideTime > 1e-4f
            ? part.customHideTime
            : (part.disableShaderLifetime ? 0.5f : 0f);
        partSettings.hideTime = Mathf.Max(0f, Mathf.Min(hide, partSettings.defaultLifeTime));
    }

    public static void ApplyScale(float scale, Transform instanceTransform)
    {
        if (instanceTransform == null || Mathf.Approximately(scale, 1f))
        {
            return;
        }

        instanceTransform.localScale *= scale;
    }

    public static void ApplyScale(CompositePrefabPart part, Transform instanceTransform)
    {
        if (part == null)
        {
            return;
        }

        ApplyScale(part.scale, instanceTransform);
    }

    public static void ApplyLoopOverrides(CompositePrefabPart part, Transform instanceTransform)
    {
        if (part == null || instanceTransform == null)
        {
            return;
        }

        ParticleGroup[] groups = instanceTransform.GetComponentsInChildren<ParticleGroup>(true);
        for (int i = 0; i < groups.Length; i++)
        {
            ParticleGroup group = groups[i];
            if (group != null)
            {
                group.SetRuntimeContinuousLoopOverride(part.overrideContinuousLoop, part.continuousLoop);
            }
        }

        ParticleSingle[] singles = instanceTransform.GetComponentsInChildren<ParticleSingle>(true);
        for (int i = 0; i < singles.Length; i++)
        {
            ParticleSingle single = singles[i];
            if (single != null)
            {
                single.SetRuntimeContinuousLoopOverride(part.overrideContinuousLoop, part.continuousLoop);
            }
        }
    }
}
