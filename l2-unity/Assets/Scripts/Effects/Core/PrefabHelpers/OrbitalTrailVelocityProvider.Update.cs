using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class OrbitalTrailVelocityProvider
{
    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f)
        {
            return;
        }

        for (int i = 0; i < _bindings.Count; i++)
        {
            TrailBinding binding = _bindings[i];
            if (binding == null || binding.tailRoot == null)
            {
                continue;
            }

            if (_useSyntheticOrbitVelocity && TryGetSyntheticVelocity(binding, out Vector3 rawVelocity))
            {
                ProcessBinding(binding, rawVelocity, dt);
                continue;
            }

            Vector3 sourcePosition = GetVelocitySourcePosition(binding);
            if (!binding.hasLastPosition)
            {
                binding.lastPosition = sourcePosition;
                binding.hasLastPosition = true;
                continue;
            }

            rawVelocity = (sourcePosition - binding.lastPosition) / dt;
            binding.lastPosition = sourcePosition;
            ProcessBinding(binding, rawVelocity, dt);
        }
    }

    private void ProcessBinding(TrailBinding binding, Vector3 rawVelocity, float dt)
    {
        float lerpT = Mathf.Clamp01(_smoothing * dt);
        binding.smoothedVelocity = Vector3.Lerp(binding.smoothedVelocity, rawVelocity, lerpT);

        float sign = binding.trailSign >= 0f ? 1f : -1f;
        if (binding.invertTrailDirection)
        {
            sign *= -1f;
        }

        Vector3 shaderVelocity = binding.smoothedVelocity * (binding.velocityScale * sign);
        if (binding.convertWorldToLocal)
        {
            Transform localRef = binding.localSpaceReference != null ? binding.localSpaceReference : binding.tailRoot;
            shaderVelocity = localRef.InverseTransformDirection(shaderVelocity);
        }

        if (_debugLogs && !binding.loggedFirstVelocity && shaderVelocity.sqrMagnitude > 0.000001f)
        {
            binding.loggedFirstVelocity = true;
            // Debug.Log($"[OrbitalTrailVelocityProvider] binding='{binding.name}' raw={rawVelocity:F4} shader={shaderVelocity:F4}", this);
        }

        ApplyVelocityToMaterials(ResolveBindingMaterials(binding), binding, shaderVelocity);
    }
}
