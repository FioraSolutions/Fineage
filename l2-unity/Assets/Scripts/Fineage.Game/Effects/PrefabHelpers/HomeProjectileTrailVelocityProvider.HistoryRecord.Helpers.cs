using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    private float GetRendererTrailT(TrailBinding binding, Renderer renderer, float fallbackLinearT, float headLag)
    {
        if (!binding.placeByParticleAge || renderer == null)
        {
            return fallbackLinearT;
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(_propertyBlock);
        float startTime = _propertyBlock.GetFloat(StartTimeID);
        Material material = renderer.sharedMaterial;
        if (startTime <= 0f)
        {
            if (material == null || !material.HasProperty(StartTimeID))
            {
                return fallbackLinearT;
            }

            startTime = material.GetFloat(StartTimeID);
            if (startTime <= 0f)
            {
                return fallbackLinearT;
            }
        }

        float age = Mathf.Max(0f, Time.time - startTime);
        float travelSeconds = binding.trailTravelSeconds;
        if (travelSeconds <= 0f && material != null && material.HasProperty(LifetimeRangeID))
        {
            travelSeconds = material.GetVector(LifetimeRangeID).y;
        }

        float ageT = Mathf.Clamp01(age / Mathf.Max(0.001f, travelSeconds));
        return Mathf.Lerp(headLag, 1f, ageT);
    }

    private Vector3 SampleHistoryByDistance(TrailBinding binding, float sampleDistance)
    {
        List<TrailSample> history = binding.history;
        if (history == null || history.Count == 0)
        {
            return binding.velocitySource != null ? binding.velocitySource.position : binding.tailRoot.position;
        }

        if (sampleDistance <= history[0].distance)
        {
            return history[0].position;
        }

        int last = history.Count - 1;
        if (sampleDistance >= history[last].distance)
        {
            return history[last].position;
        }

        for (int i = 1; i < history.Count; i++)
        {
            TrailSample next = history[i];
            if (next.distance < sampleDistance)
            {
                continue;
            }

            TrailSample prev = history[i - 1];
            float span = Mathf.Max(0.0001f, next.distance - prev.distance);
            float t = Mathf.Clamp01((sampleDistance - prev.distance) / span);
            return Vector3.Lerp(prev.position, next.position, t);
        }

        return history[last].position;
    }
}
