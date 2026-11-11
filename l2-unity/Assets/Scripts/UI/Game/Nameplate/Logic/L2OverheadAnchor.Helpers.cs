using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2OverheadAnchor
{
    public static Vector3 GetWorldPos(Transform root)
    {
        if (root == null)
            return Vector3.zero;

        float localY = ResolveLocalHeadY(root.GetInstanceID(), useEntityCache: false, root);
        return root.TransformPoint(new Vector3(0f, localY, 0f)) + Vector3.up * NameOffsetMeters;
    }

    static float ResolveLocalHeadY(int cacheKey, bool useEntityCache, Transform root)
    {
        Dictionary<int, float> cache = useEntityCache ? _localHeadYByEntity : _localHeadYByInstance;
        if (cacheKey != 0 && cache.TryGetValue(cacheKey, out float cached))
            return cached;

        float localY = SampleLocalHeadY(root);
        if (cacheKey != 0)
            cache[cacheKey] = localY;
        return localY;
    }

    static float SampleLocalHeadY(Transform root)
    {
        if (TryFindHeadBone(root, out Transform head))
        {
            float localY = root.InverseTransformPoint(head.position).y;
            if (localY > 0f && localY <= 5f)
                return localY;
        }

        // Monster / arachnid imports often lack a biped head or land outside the biped band.
        // Throwing here aborted the whole nameplate paint pass — Nerupa/mobs lost names while
        // townsfolk with valid Bip01 Head kept theirs.
        return FallbackLocalTopY(root);
    }

    static float FallbackLocalTopY(Transform root)
    {
        SkinnedMeshRenderer[] skins = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        bool has = false;
        float maxY = 0f;
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
                continue;
            float y = root.InverseTransformPoint(skins[i].bounds.max).y;
            if (!has || y > maxY)
            {
                maxY = y;
                has = true;
            }
        }

        if (has && maxY > 0.05f)
            return Mathf.Min(maxY, 8f);

        return 1.2f;
    }

    static bool TryFindHeadBone(Transform root, out Transform head)
    {
        head = null;
        float bestY = float.NegativeInfinity;
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < all.Length; i++)
        {
            Transform t = all[i];
            if (t == null || string.IsNullOrEmpty(t.name))
                continue;
            string n = t.name;
            if (!n.Equals("Bip01_head", StringComparison.OrdinalIgnoreCase)
                && !n.Equals("Bip01 Head", StringComparison.OrdinalIgnoreCase)
                && !n.Equals("Bip01_Head", StringComparison.OrdinalIgnoreCase)
                && !n.Equals("Bip01 head", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            float y = root.InverseTransformPoint(t.position).y;
            if (y > bestY)
            {
                bestY = y;
                head = t;
            }
        }

        return head != null;
    }
}
