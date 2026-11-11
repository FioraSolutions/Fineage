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

    private void ResolveBindingSources(TrailBinding binding)
    {
        if (binding.autoCollectChildren && (binding.targetRenderers == null || binding.targetRenderers.Length == 0))
        {
            binding.targetRenderers = binding.tailRoot.GetComponentsInChildren<Renderer>(true);
        }

        if (binding.localSpaceReference == null)
        {
            binding.localSpaceReference = _orbitCenter != null ? _orbitCenter : binding.tailRoot;
        }

        if (binding.velocitySource != null || string.IsNullOrEmpty(binding.velocitySourceName))
        {
            return;
        }

        Transform searchRoot = binding.tailRoot.parent != null ? binding.tailRoot.parent : transform;
        Transform[] candidates = searchRoot.GetComponentsInChildren<Transform>(true);
        Transform bestMatch = null;
        float bestSqrDistance = float.MaxValue;
        for (int i = 0; i < candidates.Length; i++)
        {
            Transform candidate = candidates[i];
            if (candidate == null || !candidate.name.Equals(binding.velocitySourceName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            float sqrDistance = (candidate.position - binding.tailRoot.position).sqrMagnitude;
            if (sqrDistance < bestSqrDistance)
            {
                bestSqrDistance = sqrDistance;
                bestMatch = candidate;
            }
        }

        binding.velocitySource = bestMatch;
    }

    private Vector3 GetVelocitySourcePosition(TrailBinding binding)
    {
        return binding.velocitySource != null ? binding.velocitySource.position : binding.tailRoot.position;
    }

    private Vector3 ResolveOrbitRadiusWorld(TrailBinding binding)
    {
        if (binding.useOrbitRadiusOverride)
        {
            Transform basis = _orbitCenter != null ? _orbitCenter : binding.tailRoot;
            return basis.TransformDirection(binding.orbitRadiusLocalOverride);
        }

        if (binding.preferSelfPositionForRadius && _orbitCenter != null)
        {
            Vector3 fromTail = binding.tailRoot.position - _orbitCenter.position;
            if (fromTail.sqrMagnitude > 0.000001f)
            {
                return fromTail;
            }
        }

        if (binding.velocitySource != null && _orbitCenter != null)
        {
            return binding.velocitySource.position - _orbitCenter.position;
        }

        return Vector3.zero;
    }
}
