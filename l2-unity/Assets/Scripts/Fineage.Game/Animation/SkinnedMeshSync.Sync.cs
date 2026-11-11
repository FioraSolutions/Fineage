using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static void RemapToSkeleton(
        SkinnedMeshRenderer renderer,
        Dictionary<string, Transform> skeleton,
        Transform rootBone)
    {
        if (renderer == null)
            return;

        Transform[] srcBones = renderer.bones;
        if (srcBones == null || srcBones.Length == 0)
            return;

        bool faceOrHair = IsFaceOrHairRenderer(renderer);
        Transform missFallback = rootBone;
        // Face/hair verts that miss must NOT fall back to Pelvis — that stretches the
        // skull open from pelvis to head. Prefer Bip01_head for leftovers.
        if (faceOrHair && TryFindBone(skeleton, "Bip01_head", out Transform headBone))
            missFallback = headBone;
        else if (faceOrHair && TryFindBone(skeleton, "Bip01 Head", out headBone))
            missFallback = headBone;

        Transform[] mapped = new Transform[srcBones.Length];
        int hits = 0;
        for (int b = 0; b < srcBones.Length; b++)
        {
            Transform src = srcBones[b];
            if (src == null)
            {
                mapped[b] = missFallback != null ? missFallback : rootBone;
                continue;
            }

            if (TryFindBone(skeleton, src.name, out Transform donor)
                || TryFindBoneAlias(skeleton, src.name, out donor))
            {
                mapped[b] = donor;
                hits++;
            }
            else
            {
                // Never keep the piece's static bind bone next to animated donors.
                mapped[b] = missFallback != null ? missFallback : src;
            }
        }

        if (hits == 0)
            return;

        renderer.bones = mapped;
        if (faceOrHair && missFallback != null)
            renderer.rootBone = missFallback;
        else if (rootBone != null)
            renderer.rootBone = rootBone;
        renderer.updateWhenOffscreen = true;
    }

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
