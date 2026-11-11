using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public float ReadShaderSlotDuration(float fallbackDuration)
    {
        float lifetime = ReadLifetimeMax(fallbackDuration);
        float maxDelay = 0f;
        if (_particles != null && _particles.Length > 0 && _particles[0] != null)
        {
            foreach (Material material in _particles[0].sharedMaterials)
            {
                if (material != null && material.HasProperty("_InitialDelayRange"))
                    maxDelay = material.GetVector("_InitialDelayRange").y;
            }
        }

        return lifetime + maxDelay + 0.03f;
    }
}
