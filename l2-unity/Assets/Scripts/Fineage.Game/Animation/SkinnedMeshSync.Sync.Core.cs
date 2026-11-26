using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static void RemapToSkeleton(
        SkinnedMeshRenderer renderer,
        Dictionary<string, Transform> skeleton,
        SkinnedMeshRenderer skeletonMesh,
        Transform animRoot,
        string raceContext)
    {
        if (renderer == null)
            return;

        Transform[] srcBones = renderer.bones;
        if (srcBones == null || srcBones.Length == 0)
            return;

        Transform sourceRoot = renderer.rootBone;

        bool faceOrHair = IsFaceOrHairRenderer(renderer);
        Transform bodyHead = null;
        if (faceOrHair)
        {
            if (!TryFindBone(skeleton, "Bip01_head", out bodyHead))
                TryFindBone(skeleton, "Bip01 Head", out bodyHead);
        }

        Transform[] mapped = new Transform[srcBones.Length];
        int hits = 0;
        for (int b = 0; b < srcBones.Length; b++)
        {
            Transform src = srcBones[b];
            if (src == null)
            {
                LogUnresolvedBone(raceContext, renderer, "(null)", null, "null bind bone in mesh");
                mapped[b] = null;
                continue;
            }

            string parentName = src.parent != null ? src.parent.name : null;
            if (TryFindDuplicateBone(skeleton, src, out Transform donor)
                || TryFindBone(skeleton, src.name, out donor)
                || TryFindBoneAlias(skeleton, src.name, out donor))
            {
                mapped[b] = donor;
                hits++;
                continue;
            }

            LogUnresolvedBone(raceContext, renderer, src.name, parentName, "no matching body skeleton bone");
            // Keep the piece's static bind transform so verts stay local to the gear; never remap to pelvis/root.
            mapped[b] = src;
        }

        if (hits == 0)
        {
            if (faceOrHair && bodyHead != null)
            {
                AttachForeignHairChain(renderer, bodyHead, skeletonMesh);
                return;
            }

            LogZeroBoneHits(raceContext, renderer);
            return;
        }

        if (faceOrHair && bodyHead != null
            && TryDriveRootWeightedFaceFromHead(renderer, srcBones, mapped, bodyHead))
            return;

        renderer.bones = mapped;
        if (sourceRoot != null
            && (TryFindBone(skeleton, sourceRoot.name, out Transform mappedRoot)
                || TryFindBoneAlias(skeleton, sourceRoot.name, out mappedRoot)))
        {
            renderer.rootBone = mappedRoot;
        }
        else if (sourceRoot != null)
        {
            LogUnresolvedRootBone(raceContext, renderer, sourceRoot.name);
            Transform mappedRootFromMesh = FindFirstMappedDonor(mapped);
            if (mappedRootFromMesh != null)
                renderer.rootBone = mappedRootFromMesh;
        }

        renderer.updateWhenOffscreen = true;
    }

    static Transform FindFirstMappedDonor(Transform[] mapped)
    {
        if (mapped == null)
            return null;
        for (int i = 0; i < mapped.Length; i++)
        {
            if (mapped[i] != null)
                return mapped[i];
        }
        return null;
    }
}