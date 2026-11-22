using UnityEngine;

/// <summary>Roadmap step 14: tier-driven animator rate and skinned culling.</summary>
public static class CrowdTierVisualPolicy
{
    public static void Apply(Transform root, CrowdDetailTier tier, CrowdLodProfile profile)
    {
        if (root == null || profile == null)
            return;

        AnimatorCullingMode cull = profile.AnimatorCullingForTier(tier);

        Animator[] animators = root.GetComponentsInChildren<Animator>(false);
        for (int i = 0; i < animators.Length; i++)
        {
            Animator animator = animators[i];
            if (animator == null)
                continue;
            // Locomotion rate is run_speed / walk_speed from LocomotionGait — do not scale Animator.speed.
            if (animator.cullingMode != cull)
                animator.cullingMode = cull;
        }

        L2ActorMeshPolicy.DisableSkeletonPlaceholders(root);
        L2ActorMeshPolicy.DisableAlphaOverlays(root);

        if (tier >= CrowdDetailTier.T2Mid)
        {
            SkinnedMeshRenderer[] renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(false);
            for (int i = 0; i < renderers.Length; i++)
            {
                SkinnedMeshRenderer smr = renderers[i];
                if (smr == null || smr.sharedMesh == null)
                    continue;
                smr.updateWhenOffscreen = tier < CrowdDetailTier.T3Far;
                ShrinkInflatedSkinnedBoundsIfNeeded(smr);

                if (!smr.enabled)
                    continue;

                string meshName = smr.sharedMesh != null ? smr.sharedMesh.name : smr.name;
                Material mat = smr.sharedMaterial;
                if (mat != null)
                    L2ActorMaterialSetup.LogMissingBaseMapOnce(mat, meshName);
            }
        }
    }

    /// <summary>
    /// Step 14 only shrinks stale <em>inflated</em> localBounds (weapon reach). Replacing
    /// tuned bounds with raw mesh.bounds culled distant skinned bodies (#1608).
    /// </summary>
    public static void ShrinkInflatedSkinnedBoundsIfNeeded(SkinnedMeshRenderer smr)
    {
        Bounds meshLocal = smr.sharedMesh.bounds;
        Bounds current = smr.localBounds;
        Vector3 meshSize = meshLocal.size;
        Vector3 curSize = current.size;
        const float inflateFactor = 1.35f;
        if (curSize.x > meshSize.x * inflateFactor
            || curSize.y > meshSize.y * inflateFactor
            || curSize.z > meshSize.z * inflateFactor)
            smr.localBounds = meshLocal;
    }
}