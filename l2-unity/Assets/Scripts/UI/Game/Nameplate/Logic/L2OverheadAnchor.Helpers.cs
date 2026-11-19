using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class L2OverheadAnchor
{
    public static Vector3 GetWorldPos(Transform root)
    {
        if (root == null)
            return Vector3.zero;

        return ResolveWorldPos(root.GetInstanceID(), useEntityCache: false, root, 0f);
    }

    static Vector3 ResolveWorldPos(int cacheKey, bool useEntityCache, Transform root, float collisionHeight)
    {
        if (TryLiveHeadBone(cacheKey, useEntityCache, root, collisionHeight, out Transform head)
            && head != null)
            return head.position + Vector3.up * NameOffsetMeters;

        // Official Interlude: name base is feet + 2×collision height + NameOffset.
        if (collisionHeight > 0.0001f)
            return root.position + Vector3.up * L2NameplateAnchor.HeightFromFeet(collisionHeight, NameOffsetMeters);

        return root.position + Vector3.up * FallbackLocalTopY(root);
    }

    static bool TryLiveHeadBone(
        int cacheKey,
        bool useEntityCache,
        Transform root,
        float collisionHeight,
        out Transform head)
    {
        head = null;
        Dictionary<int, Transform> cache = useEntityCache ? _headBoneByEntity : _headBoneByInstance;
        if (cacheKey != 0 && cache.TryGetValue(cacheKey, out Transform cached))
        {
            if (cached == null)
                cache.Remove(cacheKey);
            else
                head = cached;
        }

        if (head == null)
        {
            if (!TryFindHeadBone(root, out head) || head == null)
                return false;
            if (cacheKey != 0)
                cache[cacheKey] = head;
        }

        // Only trust the bone when it still sits near the standing head height.
        // A death pose drops the bone; collision height then keeps the name stable
        // until the pawn stands up and the bone rises back into the band.
        if (collisionHeight > 0.0001f)
        {
            float expected = L2NameplateAnchor.HeightFromFeet(collisionHeight, 0f);
            float localY = root.InverseTransformPoint(head.position).y;
            if (expected > 0.05f && (localY < expected * 0.45f || localY > expected * 1.6f))
                return false;
        }

        return true;
    }

    static float CollisionHeightOf(Entity entity)
    {
        if (entity == null || entity.Appearance == null)
            return 0f;
        return entity.Appearance.CollisionHeight;
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
            return Mathf.Min(maxY, 8f) + NameOffsetMeters;

        return 1.2f + NameOffsetMeters;
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
