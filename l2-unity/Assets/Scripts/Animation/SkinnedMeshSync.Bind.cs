using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class SkinnedMeshSync : MonoBehaviour
{
    static Dictionary<string, Transform> BuildSkeletonMap(Transform animRoot)
    {
        var map = new Dictionary<string, Transform>(128);
        Transform[] all = animRoot.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || string.IsNullOrEmpty(t.name))
                continue;

            // Exact name first; lowercase alias for Interlude FBX case drift (bip01_spine vs Bip01_Spine).
            if (!map.ContainsKey(t.name))
                map[t.name] = t;

            string lower = t.name.ToLowerInvariant();
            if (!map.ContainsKey(lower))
                map[lower] = t;
        }
        return map;
    }

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

    IEnumerator ProcessWaitingObjects()
    {
        _isProcessing = true;
        while (_waitingObjects.Count > 0)
        {
            if (!gameObject.activeInHierarchy)
                break;

            GameObject current = _waitingObjects.Dequeue();
            yield return StartCoroutine(AttachAndSync(current));
            yield return new WaitForSeconds(0.1f);
        }
        _isProcessing = false;
    }

    IEnumerator AttachAndSync(GameObject newObject)
    {
        if (transform.parent == null || newObject == null)
            yield break;

        newObject.transform.SetParent(transform.parent, false);
        yield return null;
        SyncMesh();
        ShowRenderers(newObject);
    }

    void HideRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = false;
            _temporarilyHiddenRenderers.Add(renderers[i]);
        }
    }

    void ShowRenderers(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (_temporarilyHiddenRenderers.Contains(renderer))
                renderer.enabled = true;
        }
    }
}
