using UnityEngine;

/// <summary>
/// Interlude actor mesh conventions shared by gear dress-up and skinned remapping.
/// <list type="bullet">
/// <item><c>.mo</c> — opaque body piece (receives paperdoll albedo).</item>
/// <item><c>.ao</c> — alpha overlay; needs its own masked mat, not the body albedo.</item>
/// <item><c>*_m000_b.mo</c> — anim FBX skeleton placeholder (bare feet); equipped boots replace it.</item>
/// </list>
/// </summary>
public static class L2ActorMeshPolicy
{
    public const string OpaqueMeshSuffix = ".mo";
    public const string AlphaOverlaySuffix = ".ao";
    public const string SkeletonPlaceholderSuffix = "_m000_b.mo";

    public static bool IsAlphaOverlay(string rendererName)
    {
        return !string.IsNullOrEmpty(rendererName)
            && rendererName.EndsWith(AlphaOverlaySuffix, System.StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsSkeletonPlaceholder(string rendererName)
    {
        if (string.IsNullOrEmpty(rendererName))
            return false;
        // Anim FBX carrier mesh: MOrc_m000_b.mo / MOrc_m000_b.ao / MOrc_m000_b
        // (Blender PSA exports often keep .ao). Must stay hidden or bare legs show
        // under gear and look enormous next to dressed pieces.
        if (rendererName.EndsWith(SkeletonPlaceholderSuffix, System.StringComparison.OrdinalIgnoreCase))
            return true;
        if (rendererName.EndsWith("_m000_b.ao", System.StringComparison.OrdinalIgnoreCase))
            return true;
        if (rendererName.EndsWith("_m000_b", System.StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    /// <summary>
    /// Apply one paperdoll material to opaque body SMRs; leave alpha overlays off until a real .ao mat exists.
    /// </summary>
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

    /// <summary>
    /// Hide anim-root reference meshes that only exist to carry the skeleton.
    /// </summary>
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
