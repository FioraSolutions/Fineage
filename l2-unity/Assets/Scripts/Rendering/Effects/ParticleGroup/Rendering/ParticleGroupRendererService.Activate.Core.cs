using System.Collections.Generic;
using UnityEngine;

public sealed partial class ParticleGroupRendererService
{
    public bool ApplyExpire(
        ParticleGroupSimulation simulation,
        bool[] managedActive,
        float[] managedTimes)
    {
        if (_particles == null || managedActive == null)
            return false;

        bool gpuDraw = simulation.GpuEnabled;
        bool anyActive = false;
        int count = _particles.Length;
        bool hasSim = simulation.HasLifetimeBuffers;
        for (int i = 0; i < count; i++)
        {
            bool wasActive = managedActive[i];
            bool nowActive = hasSim ? simulation.IsActive(i) : wasActive;
            if (wasActive && nowActive && !gpuDraw)
                UpdateWorldPositions(_particles[i]);

            if (wasActive && !nowActive)
            {
                if (!gpuDraw && _particles[i] != null)
                    _particles[i].gameObject.SetActive(false);
                managedActive[i] = false;
                continue;
            }

            if (hasSim)
            {
                managedActive[i] = nowActive;
                if (managedTimes != null)
                    managedTimes[i] = simulation.SpawnTime(i);
            }

            if (managedActive[i])
                anyActive = true;
        }

        return anyActive;
    }
}
