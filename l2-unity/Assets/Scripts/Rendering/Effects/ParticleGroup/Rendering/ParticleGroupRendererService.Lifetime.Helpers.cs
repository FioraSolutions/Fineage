using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public float ReadLifetimeMax(float fallbackDuration)
    {
        if (_particles == null || _particles.Length == 0 || _particles[0] == null)
            return fallbackDuration;

        foreach (Material material in _particles[0].sharedMaterials)
        {
            if (material != null && material.HasProperty("_LifetimeRange"))
                return material.GetVector("_LifetimeRange").y;
        }

        return 0.5f;
    }

    public float ReadLifetimeCenter(float fallbackDuration)
    {
        if (_particles == null || _particles.Length == 0 || _particles[0] == null)
        {
            return fallbackDuration;
        }

        foreach (Material material in _particles[0].sharedMaterials)
        {
            if (material == null || !material.HasProperty("_LifetimeRange"))
            {
                continue;
            }

            Vector4 range = material.GetVector("_LifetimeRange");
            float min = range.x;
            float max = range.y;
            if (min > 0f || max > 0f)
            {
                if (min <= 0f)
                {
                    min = max;
                }

                if (max <= 0f)
                {
                    max = min;
                }

                return (min + max) * 0.5f;
            }
        }

        return fallbackDuration;
    }
}
