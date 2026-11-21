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

            if (IsAlphaOverlayRenderer(renderer))
            {
                renderer.enabled = false;
                continue;
            }

            Material paperdoll = L2ActorMaterialSetup.EnsurePaperdollActorMaterial(
                material,
                armorRoot.name + "/" + renderer.name);
            renderer.sharedMaterial = paperdoll;
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

            if (IsSkeletonPlaceholder(renderer.name) || IsAlphaOverlayRenderer(renderer) || renderer.sharedMaterial == null)
                renderer.enabled = false;
        }
    }

    /// <summary>
    /// Armor FBX ships a second mesh, <c>.ao</c>, whose UVs sit on the black
    /// card of the texture. Drawn opaque it is a black plane on the arms.
    /// The solid piece is <c>.mo</c>. Keep the overlay off.
    /// </summary>
    public static void DisableAlphaOverlays(Transform root)
    {
        if (root == null)
            return;

        Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (IsAlphaOverlayRenderer(renderer))
                renderer.enabled = false;
        }
    }
}
