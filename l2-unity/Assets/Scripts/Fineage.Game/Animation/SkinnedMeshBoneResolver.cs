using System;

/// <summary>
/// Resolves Blender-export bone names onto the animated player skeleton (shared by every gear SMR).
/// Handles numeric suffix duplicates such as <c>Bip01_L_Finger01.001</c> parented under the opposite
/// hand's <c>Finger0</c> chain — the MFighter glove/thumb regression path (#1467, #1471).
/// </summary>
public static class SkinnedMeshBoneResolver
{
    public static bool TryResolveDuplicateBoneName(string boneName, string parentBoneName, out string skeletonBoneName)
    {
        skeletonBoneName = null;
        if (string.IsNullOrEmpty(boneName))
            return false;

        int dot = boneName.LastIndexOf('.');
        if (dot <= 0)
            return false;
        string tail = boneName.Substring(dot + 1);
        for (int i = 0; i < tail.Length; i++)
        {
            if (!char.IsDigit(tail[i]))
                return false;
        }

        string baseName = boneName.Substring(0, dot);
        int parentSide = SideOf(parentBoneName);
        int baseSide = SideOf(baseName);
        string candidate = baseName;
        if (parentSide > 0 && baseSide < 0)
            candidate = SwapSide(baseName, toRight: true);
        else if (parentSide < 0 && baseSide > 0)
            candidate = SwapSide(baseName, toRight: false);

        skeletonBoneName = NormalizeBoneName(candidate);
        return !string.IsNullOrEmpty(skeletonBoneName);
    }

    /// <summary>Maps a hand <c>Finger0</c> thumb duplicate to <c>Bip01_{side}_Finger01</c>.</summary>
    public static bool TryResolveThumbFromFinger0Parent(string boneName, string parentBoneName, out string skeletonBoneName)
    {
        skeletonBoneName = null;
        if (string.IsNullOrEmpty(boneName) || string.IsNullOrEmpty(parentBoneName))
            return false;
        if (parentBoneName.IndexOf("Finger0", StringComparison.OrdinalIgnoreCase) < 0)
            return false;
        if (boneName.IndexOf("Finger01", StringComparison.OrdinalIgnoreCase) < 0)
            return false;

        int side = SideOf(parentBoneName);
        if (side == 0)
            return false;
        skeletonBoneName = side > 0 ? "Bip01_R_Finger01" : "Bip01_L_Finger01";
        return true;
    }

    public static string NormalizeBoneName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;
        return name.Replace(" Bip01 ", "_Bip01_")
            .Replace(" Bip01", "_Bip01")
            .Replace("bip01 ", "Bip01_")
            .Replace(" ", "_")
            .Replace("__", "_");
    }

    public static int SideOf(string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0;
        if (name.IndexOf("_R_", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf(" R ", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("_R ", StringComparison.OrdinalIgnoreCase) >= 0)
            return 1;
        if (name.IndexOf("_L_", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf(" L ", StringComparison.OrdinalIgnoreCase) >= 0
            || name.IndexOf("_L ", StringComparison.OrdinalIgnoreCase) >= 0)
            return -1;
        return 0;
    }

    public static string SwapSide(string name, bool toRight)
    {
        if (toRight)
            return name.Replace("_L_", "_R_").Replace("_l_", "_r_").Replace(" L ", " R ");
        return name.Replace("_R_", "_L_").Replace("_r_", "_l_").Replace(" R ", " L ");
    }
}