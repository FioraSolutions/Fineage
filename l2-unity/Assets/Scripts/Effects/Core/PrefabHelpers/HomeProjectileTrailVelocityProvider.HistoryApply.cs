using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
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
}
