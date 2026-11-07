using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Remaps equipped armor / face / hair skinned meshes onto the race anim skeleton.
/// Lives on the anim root; gear pieces are siblings under the <c>model</c> container.
/// </summary>
public class SkinnedMeshSync : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _rootSkinnedRenderer;
    [SerializeField] private SkinnedMeshRenderer[] _destSkinnedRenderer;

    readonly Queue<GameObject> _waitingObjects = new();
    readonly List<Renderer> _temporarilyHiddenRenderers = new();
    bool _isProcessing;

    void Start()
    {
        SyncMesh();
    }

    public void SyncMesh()
    {
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
            RemapToSkeleton(destinations[i], skeleton, fallbackRoot);

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

    /// <summary>
    /// Blender PSK exports add <c>_end</c> leaf bones that the anim FBX omits.
    /// </summary>
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
