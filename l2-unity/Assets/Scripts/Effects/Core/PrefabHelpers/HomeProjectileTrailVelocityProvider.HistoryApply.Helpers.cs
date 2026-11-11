using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
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
