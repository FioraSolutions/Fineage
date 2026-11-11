using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    private void RecordHistory(TrailBinding binding, Vector3 sourcePosition)
    {
        if (binding.history == null)
        {
            binding.history = new List<TrailSample>(32);
        }

        float now = Time.time;
        float distance = 0f;
        if (binding.history.Count > 0)
        {
            TrailSample last = binding.history[binding.history.Count - 1];
            distance = last.distance + Vector3.Distance(last.position, sourcePosition);
        }

        binding.history.Add(new TrailSample { time = now, distance = distance, position = sourcePosition });

        float keepSeconds = Mathf.Max(0.02f, binding.historySeconds * 1.5f);
        float oldestAllowed = now - keepSeconds;
        while (binding.history.Count > 1 && binding.history[0].time < oldestAllowed)
        {
            binding.history.RemoveAt(0);
        }
    }

    private void PositionRenderersOnHistory(TrailBinding binding)
    {
        if (!binding.placeRenderersOnHistory || binding.targetRenderers == null || binding.history == null || binding.history.Count == 0)
        {
            return;
        }

        List<Renderer> renderers = CollectPositionedRenderers(binding);
        int count = renderers.Count;
        if (count == 0)
        {
            return;
        }

        float headLag = Mathf.Clamp01(binding.headLagPercent);
        float alongPower = Mathf.Max(0.01f, binding.alongTrailPower);
        float radiusPower = Mathf.Max(0.01f, binding.cylinderRadiusPower);
        float latestDistance = binding.history[binding.history.Count - 1].distance;
        float oldestDistance = binding.history[0].distance;
        float availableDistance = Mathf.Max(0.001f, latestDistance - oldestDistance);

        for (int i = 0; i < count; i++)
        {
            float fallbackLinearT = count <= 1 ? headLag : Mathf.Lerp(headLag, 1f, (float)i / (count - 1));
            float linearT = GetRendererTrailT(binding, renderers[i], fallbackLinearT, headLag);
            float trailT = Mathf.Pow(linearT, alongPower);
            float sampleDistance = latestDistance - availableDistance * trailT;
            Vector3 center = SampleHistoryByDistance(binding, sampleDistance);
            float spreadT = headLag >= 1f - 1e-4f
                ? 1f
                : Mathf.Clamp01((linearT - headLag) / (1f - headLag));

            ApplyTrailScale(binding, renderers[i].transform, spreadT);
            ApplyTrailPathFade(binding, renderers[i], spreadT);

            if (!binding.useCylinderSpread)
            {
                renderers[i].transform.position = center;
                continue;
            }

            float radius = Mathf.Lerp(
                binding.cylinderRadiusHead,
                binding.cylinderRadiusTail,
                Mathf.Pow(spreadT, radiusPower));

            BuildTrailPerpendicularBasis(GetTrailTangentByDistance(binding, sampleDistance), out Vector3 right, out Vector3 up);
            float angle = (i + 0.5f) * GoldenAngleRad;
            Vector3 radial = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * radius;
            renderers[i].transform.position = center + radial;
        }
    }
}
