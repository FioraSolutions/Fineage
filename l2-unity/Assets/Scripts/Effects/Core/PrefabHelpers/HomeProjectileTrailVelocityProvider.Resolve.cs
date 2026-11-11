using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    private void ResolveBinding(TrailBinding binding)
    {
        if (binding.autoCollectChildren)
        {
            Renderer[] childRenderers = binding.tailRoot.GetComponentsInChildren<Renderer>(true);
            if (binding.targetRenderers == null || binding.targetRenderers.Length != childRenderers.Length)
            {
                binding.targetRenderers = childRenderers;
            }
        }

        if (binding.localSpaceReference == null)
        {
            binding.localSpaceReference = binding.tailRoot;
        }

        if (binding.velocitySource != null || string.IsNullOrEmpty(binding.velocitySourceName))
        {
            return;
        }

        Transform searchRoot = binding.tailRoot.parent != null ? binding.tailRoot.parent : transform;
        Transform[] candidates = searchRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < candidates.Length; i++)
        {
            Transform candidate = candidates[i];
            if (candidate != null && candidate.name.Equals(binding.velocitySourceName, StringComparison.OrdinalIgnoreCase))
            {
                binding.velocitySource = candidate;
                return;
            }
        }
    }

    private static Transform FindChildByName(Transform searchRoot, string childName)
    {
        if (searchRoot == null || string.IsNullOrEmpty(childName))
        {
            return null;
        }

        if (searchRoot.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
        {
            return searchRoot;
        }

        Transform[] children = searchRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            Transform child = children[i];
            if (child != null && child.name.Equals(childName, StringComparison.OrdinalIgnoreCase))
            {
                return child;
            }
        }

        return null;
    }

    private static TrailBinding CloneBinding(TrailBinding source)
    {
        return new TrailBinding
        {
            name = source.name,
            tailRoot = source.tailRoot,
            velocitySource = source.velocitySource,
            velocitySourceName = source.velocitySourceName,
            targetRenderers = null,
            autoCollectChildren = source.autoCollectChildren,
            followSourcePosition = source.followSourcePosition,
            placeRenderersOnHistory = source.placeRenderersOnHistory,
            historySeconds = source.historySeconds,
            headLagPercent = source.headLagPercent,
            useCylinderSpread = source.useCylinderSpread,
            cylinderRadiusHead = source.cylinderRadiusHead,
            cylinderRadiusTail = source.cylinderRadiusTail,
            cylinderRadiusPower = source.cylinderRadiusPower,
            scaleOverTrail = source.scaleOverTrail,
            particleScaleHead = source.particleScaleHead,
            particleScaleTail = source.particleScaleTail,
            particleScalePower = source.particleScalePower,
            alongTrailPower = source.alongTrailPower,
            placeByParticleAge = source.placeByParticleAge,
            trailTravelSeconds = source.trailTravelSeconds,
            fadeAlphaOverTrail = source.fadeAlphaOverTrail,
            trailFadeHead = source.trailFadeHead,
            trailFadeTail = source.trailFadeTail,
            trailFadePower = source.trailFadePower,
            onlyActiveRenderers = source.onlyActiveRenderers,
            convertWorldToLocal = source.convertWorldToLocal,
            localSpaceReference = source.localSpaceReference,
            velocityScale = source.velocityScale,
            rangeSpread = source.rangeSpread,
            invertTrailDirection = source.invertTrailDirection,
            trailSign = source.trailSign
        };
    }

    private List<Renderer> CollectPositionedRenderers(TrailBinding binding)
    {
        List<Renderer> result = new List<Renderer>(binding.targetRenderers.Length);
        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            if (binding.onlyActiveRenderers &&
                !renderer.gameObject.activeInHierarchy &&
                !renderer.enabled)
            {
                continue;
            }

            result.Add(renderer);
        }

        return result;
    }

    private void CacheBaseLocalScales(TrailBinding binding)
    {
        if (binding.targetRenderers == null)
        {
            return;
        }

        if (binding.baseLocalScales == null)
        {
            binding.baseLocalScales = new Dictionary<Transform, Vector3>();
        }
        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer == null || renderer.transform == null)
            {
                continue;
            }

            if (!binding.baseLocalScales.ContainsKey(renderer.transform))
            {
                binding.baseLocalScales.Add(renderer.transform, renderer.transform.localScale);
            }
        }
    }
}
