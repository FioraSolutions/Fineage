using UnityEngine;
using System.Collections.Generic;

public partial class ParticleGroupV2
{
    ParticleStreamRuntime EnsureRuntime()
    {
        if (_runtime != null)
        {
            return _runtime;
        }

        _slots ??= new ParticleSlotSet(this);
        _batch ??= new ParticleDrawBatch();
        _runtime = new ParticleStreamRuntime(this, _slots, _batch);
        return _runtime;
    }

    Renderer ResolveSlotRenderer()
    {
        if (_particles != null)
        {
            for (int i = 0; i < _particles.Length; i++)
            {
                if (_particles[i] != null)
                {
                    return _particles[i];
                }
            }
        }

        return GetComponentInChildren<Renderer>(true);
    }

    Material ResolveSlotMaterial()
    {
        if (_runtimeMaterialInstances != null)
        {
            for (int i = 0; i < _runtimeMaterialInstances.Length; i++)
            {
                if (_runtimeMaterialInstances[i] != null)
                {
                    return _runtimeMaterialInstances[i];
                }
            }
        }

        Renderer renderer = ResolveSlotRenderer();
        return renderer != null ? renderer.sharedMaterial : null;
    }
}
