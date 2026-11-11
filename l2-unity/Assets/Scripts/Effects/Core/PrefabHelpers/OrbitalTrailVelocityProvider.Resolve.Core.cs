using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class OrbitalTrailVelocityProvider
{
    static Material[] ResolveBindingMaterials(TrailBinding binding)
    {
        if (binding.tailRoot != null)
        {
            ParticleGroupV2 group = binding.tailRoot.GetComponent<ParticleGroupV2>();
            if (group != null && group.IsGpuDraw)
            {
                Material[] gpuMaterials = group.GpuMaterials;
                if (gpuMaterials != null && gpuMaterials.Length > 0)
                {
                    return gpuMaterials;
                }
            }
        }

        if (binding.targetRenderers == null)
        {
            return null;
        }

        int count = 0;
        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer != null && renderer.sharedMaterial != null)
            {
                count++;
            }
        }

        if (count == 0)
        {
            return null;
        }

        Material[] materials = new Material[count];
        int write = 0;
        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material material = renderer.sharedMaterial;
            if (material != null)
            {
                materials[write++] = material;
            }
        }

        return materials;
    }

    private void ResolveSharedSources()
    {
        if (_rotatorSource == null)
        {
            _rotatorSource = GetComponent<EffectPrefabRotator>();
            if (_rotatorSource == null)
            {
                _rotatorSource = GetComponentInParent<EffectPrefabRotator>();
            }
        }

        if (_orbitCenter == null && _rotatorSource != null)
        {
            _orbitCenter = _rotatorSource.transform;
        }
    }
}
