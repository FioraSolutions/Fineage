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

        if (tier >= CrowdDetailTier.T2Mid)
        {
            SkinnedMeshRenderer[] renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(false);
            for (int i = 0; i < renderers.Length; i++)
            {
                SkinnedMeshRenderer smr = renderers[i];
                if (smr == null || smr.sharedMesh == null)
                    continue;
                smr.updateWhenOffscreen = tier < CrowdDetailTier.T3Far;
                Bounds local = smr.sharedMesh.bounds;
                if ((smr.localBounds.extents - local.extents).sqrMagnitude > 0.01f)
                    smr.localBounds = local;
            }
        }
    }
}