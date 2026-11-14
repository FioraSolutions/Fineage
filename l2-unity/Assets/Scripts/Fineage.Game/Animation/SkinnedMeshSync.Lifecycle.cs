using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    // Bone bind, relative to this anim root. Captured before the Animator
    // leaves the imported pose. Face FBX rest poses (FOrc head) do not match
    // the body skeleton; retarget uses this instead of the live pose.
    Dictionary<Transform, Matrix4x4> _restLocal;

    void Awake()
    {
        CaptureRestLocal();
    }

    void Start()
    {
        SyncMesh();
    }

    void CaptureRestLocal()
    {
        _restLocal = new Dictionary<Transform, Matrix4x4>(128);
        Matrix4x4 toRoot = transform.worldToLocalMatrix;
        Transform[] all = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform bone = all[i];
            if (bone != null)
                _restLocal[bone] = toRoot * bone.localToWorldMatrix;
        }
    }

    public void SyncMesh()
    {
        if (_restLocal == null || _restLocal.Count == 0)
            CaptureRestLocal();

        Transform gearContainer = transform.parent;
        if (gearContainer == null)
            return;

        // Skeleton = anim hierarchy transforms. Never copy bones[] from a placeholder SMR
        // (boots-only bone list → torso collapse / "gingerbread").
        Dictionary<string, Transform> skeleton = BuildSkeletonMap(transform);
        if (skeleton.Count == 0)
            return;

        if (_rootSkinnedRenderer == null)
            _rootSkinnedRenderer = FindRootBoneHint(transform);

        List<SkinnedMeshRenderer> destinations = CollectGearSkinnedMeshes(gearContainer, transform);
        _destSkinnedRenderer = destinations.ToArray();

        Transform fallbackRoot = ResolveRootBone(skeleton, _rootSkinnedRenderer);
        for (int i = 0; i < destinations.Count; i++)
            RemapToSkeleton(destinations[i], skeleton, fallbackRoot, _rootSkinnedRenderer, _restLocal, transform);

        L2ActorMeshPolicy.DisableSkeletonPlaceholders(transform);
    }

    static List<SkinnedMeshRenderer> CollectGearSkinnedMeshes(Transform gearContainer, Transform animRoot)
    {
        var dest = new List<SkinnedMeshRenderer>(16);
        for (int i = 0; i < gearContainer.childCount; i++)
        {
            Transform child = gearContainer.GetChild(i);
            if (child == animRoot)
                continue;

            SkinnedMeshRenderer[] renderers = child.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int r = 0; r < renderers.Length; r++)
            {
                SkinnedMeshRenderer smr = renderers[r];
                if (smr != null)
                    dest.Add(smr);
            }
        }
        return dest;
    }
}
