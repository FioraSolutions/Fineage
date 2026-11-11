using System;
using UnityEngine;
using System.Collections.Generic;

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
}
