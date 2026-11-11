using UnityEngine;
using System;

public static partial class L2ActorMeshPolicy
{
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
}
