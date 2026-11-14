using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static void RemapToSkeleton(
        SkinnedMeshRenderer renderer,
        Dictionary<string, Transform> skeleton,
        Transform rootBone,
        SkinnedMeshRenderer skeletonMesh,
        Dictionary<Transform, Matrix4x4> restLocal,
        Transform animRoot)
    {
        if (renderer == null)
            return;

        Transform[] srcBones = renderer.bones;
        if (srcBones == null || srcBones.Length == 0)
            return;

        // Keep the piece's authored root (pelvis on a full-skeleton face, head on a
        // head-only hair mesh). Forcing every face onto Bip01_head makes the skull
        // orbit the neck on its own while the body animates.
        Transform sourceRoot = renderer.rootBone;

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
        {
            // FElf (and some FDarkElf / FFighter) back-hair is skinned only to Hair*
            // bones. Those match Bip01_head's bind but are not on the body skeleton,
            // so the piece stays in bind pose beside the moving face.
            if (faceOrHair && missFallback != null)
                AttachForeignHairChain(renderer, missFallback, skeletonMesh);
            return;
        }

        // Faces are one rigid weight. Humans, elves, dwarves, mages: Bip01_head,
        // and the face bind already matches the body, so the bindpose stays.
        // Rewriting it from the animated skeleton is what lifted those heads
        // off the neck. Orc and shaman weight the same mesh to Bip01; that one
        // weight moves onto the body head, using the face file's own head bone.
        if (faceOrHair && missFallback != null
            && TryDriveRootWeightedFaceFromHead(renderer, srcBones, mapped, missFallback))
            return;

        renderer.bones = mapped;
        if (sourceRoot != null
            && (TryFindBone(skeleton, sourceRoot.name, out Transform mappedRoot)
                || TryFindBoneAlias(skeleton, sourceRoot.name, out mappedRoot)))
            renderer.rootBone = mappedRoot;
        else if (rootBone != null)
            renderer.rootBone = rootBone;
        renderer.updateWhenOffscreen = true;
    }
}
