using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    private void OnEnable()
    {
        for (int i = 0; i < _bindings.Count; i++)
        {
            TrailBinding binding = _bindings[i];
            if (binding == null || binding.tailRoot == null)
            {
                continue;
            }

            ResolveBinding(binding);
            binding.hasLastPosition = false;
            binding.smoothedVelocity = Vector3.zero;
            binding.history = new List<TrailSample>(32);
            binding.baseLocalScales = new Dictionary<Transform, Vector3>();
            CacheBaseLocalScales(binding);
        }
    }

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

            ResolveBinding(binding);
            CacheBaseLocalScales(binding);
            if (binding.velocitySource == null)
            {
                continue;
            }

            if (binding.followSourcePosition)
            {
                binding.tailRoot.position = binding.velocitySource.position;
            }

            Vector3 sourcePosition = binding.velocitySource.position;
            RecordHistory(binding, sourcePosition);
            if (!binding.hasLastPosition)
            {
                binding.lastPosition = sourcePosition;
                binding.hasLastPosition = true;
                PositionRenderersOnHistory(binding);
                continue;
            }

            Vector3 rawVelocity = (sourcePosition - binding.lastPosition) / dt;
            binding.lastPosition = sourcePosition;

            float lerpT = Mathf.Clamp01(_smoothing * dt);
            binding.smoothedVelocity = Vector3.Lerp(binding.smoothedVelocity, rawVelocity, lerpT);

            ApplyVelocity(binding);
            PositionRenderersOnHistory(binding);
        }
    }
}
