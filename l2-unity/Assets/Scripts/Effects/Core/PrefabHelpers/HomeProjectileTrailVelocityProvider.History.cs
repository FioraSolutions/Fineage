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

    private static void BuildTrailPerpendicularBasis(Vector3 tangent, out Vector3 right, out Vector3 up)
    {
        if (tangent.sqrMagnitude < 1e-8f)
        {
            tangent = Vector3.forward;
        }
        else
        {
            tangent = tangent.normalized;
        }

        Vector3 worldUp = Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) > 0.92f ? Vector3.forward : Vector3.up;
        right = Vector3.Cross(worldUp, tangent);
        if (right.sqrMagnitude < 1e-8f)
        {
            right = Vector3.right;
        }
        else
        {
            right.Normalize();
        }

        up = Vector3.Cross(tangent, right).normalized;
    }

    private void ApplyVelocity(TrailBinding binding)
    {
        Vector3 velocity = binding.smoothedVelocity * binding.velocityScale;
        if (binding.invertTrailDirection)
        {
            velocity = -velocity;
        }

        velocity *= binding.trailSign >= 0f ? 1f : -1f;

        if (binding.convertWorldToLocal && binding.localSpaceReference != null)
        {
            velocity = binding.localSpaceReference.InverseTransformDirection(velocity);
        }

        if (_debugLogs && velocity.sqrMagnitude > 0.000001f)
        {
            // Debug.Log($"[HomeProjectileTrailVelocityProvider] binding='{binding.name}' velocity={velocity:F4}", this);
        }

        if (binding.targetRenderers == null)
        {
            return;
        }

        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material[] materials = renderer.materials;
            for (int m = 0; m < materials.Length; m++)
            {
                Material mat = materials[m];
                if (mat == null)
                {
                    continue;
                }

                if (mat.HasProperty(StartVelocityRangeXID))
                {
                    mat.SetColor(StartVelocityRangeXID, BuildRangeColor(velocity.x, binding.rangeSpread));
                }
                if (mat.HasProperty(StartVelocityRangeYID))
                {
                    mat.SetColor(StartVelocityRangeYID, BuildRangeColor(velocity.y, binding.rangeSpread));
                }
                if (mat.HasProperty(StartVelocityRangeZID))
                {
                    mat.SetColor(StartVelocityRangeZID, BuildRangeColor(velocity.z, binding.rangeSpread));
                }
            }
        }
    }

    private void ApplyTrailPathFade(TrailBinding binding, Renderer renderer, float spreadT)
    {
        if (!binding.fadeAlphaOverTrail || renderer == null)
        {
            return;
        }

        if (_propertyBlock == null)
        {
            _propertyBlock = new MaterialPropertyBlock();
        }

        float fadeT = Mathf.Pow(Mathf.Clamp01(spreadT), Mathf.Max(0.01f, binding.trailFadePower));
        renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(TrailPathFadeTID, fadeT);
        _propertyBlock.SetFloat(TrailPathFadeHeadID, binding.trailFadeHead);
        _propertyBlock.SetFloat(TrailPathFadeTailID, binding.trailFadeTail);
        renderer.SetPropertyBlock(_propertyBlock);
    }
    private static void ApplyTrailScale(TrailBinding binding, Transform target, float spreadT)
    {
        if (!binding.scaleOverTrail || target == null)
        {
            return;
        }

        Vector3 baseScale = target.localScale;
        if (binding.baseLocalScales != null && binding.baseLocalScales.TryGetValue(target, out Vector3 cachedScale))
        {
            baseScale = cachedScale;
        }

        float scaleT = Mathf.Pow(Mathf.Clamp01(spreadT), Mathf.Max(0.01f, binding.particleScalePower));
        float scale = Mathf.Lerp(binding.particleScaleHead, binding.particleScaleTail, scaleT);
        target.localScale = baseScale * Mathf.Max(0.01f, scale);
    }
    private Vector3 GetTrailTangentByDistance(TrailBinding binding, float sampleDistance)
    {
        const float delta = 0.01f;
        Vector3 before = SampleHistoryByDistance(binding, sampleDistance - delta);
        Vector3 after = SampleHistoryByDistance(binding, sampleDistance + delta);
        Vector3 deltaPos = after - before;
        if (deltaPos.sqrMagnitude > 1e-8f)
        {
            return deltaPos.normalized;
        }

        if (binding.smoothedVelocity.sqrMagnitude > 1e-8f)
        {
            return binding.smoothedVelocity.normalized;
        }

        return Vector3.forward;
    }
    private Color BuildRangeColor(float value, float rangeSpread)
    {
        float spread = Mathf.Max(_minimumAxisRange, Mathf.Abs(value) * rangeSpread);
        return new Color(value - spread, value + spread, 0f, 0f);
    }
}
