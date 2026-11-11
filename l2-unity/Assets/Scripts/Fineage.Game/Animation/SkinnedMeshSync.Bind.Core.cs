using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static bool TryFindBone(Dictionary<string, Transform> skeleton, string name, out Transform donor)
    {
        if (string.IsNullOrEmpty(name))
        {
            donor = null;
            return false;
        }

        if (skeleton.TryGetValue(name, out donor))
            return true;
        return skeleton.TryGetValue(name.ToLowerInvariant(), out donor);
    }

    static Transform ResolveRootBone(Dictionary<string, Transform> skeleton, SkinnedMeshRenderer hint)
    {
        if (hint != null && hint.rootBone != null
            && TryFindBone(skeleton, hint.rootBone.name, out Transform fromHint))
            return fromHint;

        if (TryFindBone(skeleton, "Bip01_Pelvis", out Transform pelvis))
            return pelvis;
        if (TryFindBone(skeleton, "Bip01", out Transform bip))
            return bip;

        return hint != null ? hint.rootBone : null;
    }

    static SkinnedMeshRenderer FindRootBoneHint(Transform animRoot)
    {
        SkinnedMeshRenderer[] all = animRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (all == null || all.Length == 0)
            return null;

        for (int i = 0; i < all.Length; i++)
        {
            SkinnedMeshRenderer smr = all[i];
            if (smr == null || L2ActorMeshPolicy.IsSkeletonPlaceholder(smr.name))
                continue;
            if (smr.name.IndexOf(L2ActorMeshPolicy.OpaqueMeshSuffix, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return smr;
        }

        return all[0];
    }

    public void AddObjectToQueue(GameObject newObject)
    {
        if (!gameObject.activeInHierarchy || newObject == null)
            return;

        HideRenderers(newObject);
        _waitingObjects.Enqueue(newObject);
        if (!_isProcessing)
            StartCoroutine(ProcessWaitingObjects());
    }
}
