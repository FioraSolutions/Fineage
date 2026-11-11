using System;
using UnityEngine;

public sealed partial class ParticleStreamRuntime
{
    void ApplyLifeStretchToRenderer(Renderer renderer)
    {
        if (!ShouldStretchLife())
        {
            return;
        }

        L2FxAdjustParticleLife.ApplyToRenderer(
            renderer,
            _authoring.authoredParticleLife,
            _authoring.targetParticleLife);
    }

    void ApplyLifeStretchToGpuMaterials()
    {
        if (!ShouldStretchLife() || !_batch.Bound)
        {
            return;
        }

        Material[] materials = _simulation.GpuMaterials;
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            L2FxAdjustParticleLife.ApplyToMaterial(
                materials[i],
                _authoring.authoredParticleLife,
                _authoring.targetParticleLife);
        }
    }

    bool ShouldStretchLife()
    {
        return _authoring.stretchParticleLifeToWindow &&
               Mathf.Abs(_authoring.targetParticleLife - _authoring.authoredParticleLife) > 1e-4f;
    }

    Vector4 ResolveSpawnLocationAddUe(float spawnTime)
    {
        if (_host == null)
        {
            return Vector4.zero;
        }

        IParticleSpawnLocationAddProvider provider =
            _host.GetComponentInParent<IParticleSpawnLocationAddProvider>();
        if (provider != null &&
            provider.TryGetSpawnLocationAddUe(_host, spawnTime, out Vector4 addUe))
        {
            return addUe;
        }

        return Vector4.zero;
    }
}
