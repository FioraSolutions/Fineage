using UnityEngine;

public static partial class L2ActorMeshPolicy
{
    public static void ApplyPaperdollMaterial(GameObject armorRoot, Material material)
    {
        if (armorRoot == null || material == null)
            return;

        SkinnedMeshRenderer[] renderers = armorRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            if (renderer == null)
                continue;

            if (IsAlphaOverlay(renderer.name))
            {
                renderer.enabled = false;
                continue;
            }

            renderer.sharedMaterial = material;
            renderer.enabled = true;
            renderer.updateWhenOffscreen = true;
        }
    }

    public static void DisableSkeletonPlaceholders(Transform animRoot)
    {
        if (animRoot == null)
            return;

        SkinnedMeshRenderer[] renderers = animRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            SkinnedMeshRenderer renderer = renderers[i];
            if (renderer == null)
                continue;

            if (IsSkeletonPlaceholder(renderer.name) || renderer.sharedMaterial == null)
                renderer.enabled = false;
        }
    }
}
