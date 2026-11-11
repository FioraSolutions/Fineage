using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static bool IsFaceOrHairRenderer(SkinnedMeshRenderer renderer)
    {
        if (renderer == null)
            return false;
        string n = renderer.name;
        if (string.IsNullOrEmpty(n))
            return false;
        // MOrc_m000_f / MOrc_f_0 / MOrc_m000_m00_bh / MOrc_h_0_0_bh
        return n.IndexOf("_f", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_h_", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_bh", System.StringComparison.OrdinalIgnoreCase) >= 0
            || n.IndexOf("_ah", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    static bool TryFindBoneAlias(Dictionary<string, Transform> skeleton, string name, out Transform donor)
    {
        donor = null;
        if (string.IsNullOrEmpty(name))
            return false;
        if (name.EndsWith("_end", System.StringComparison.OrdinalIgnoreCase))
        {
            string trimmed = name.Substring(0, name.Length - 4);
            if (TryFindBone(skeleton, trimmed, out donor))
                return true;
        }
        // Spaces vs underscores (NPC clergy style vs player underscore style).
        string alt = name.IndexOf(' ') >= 0
            ? name.Replace(' ', '_')
            : name.Replace('_', ' ');
        return !string.Equals(alt, name, System.StringComparison.Ordinal)
            && TryFindBone(skeleton, alt, out donor);
    }
}
