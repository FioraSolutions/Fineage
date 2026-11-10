using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HeadDisplay world point (Interlude DrawTargetName / SetupGauge).
/// <para>
/// Requires <c>Bip01 Head</c>. Height comes from the head bone (local Y, cached once).
/// XZ stays on the pawn root axis (Interlude Location XY) — head-bone XZ leans toward
/// the face on some races (Human worse than Orc) and must not drive the plate.
/// </para>
/// </summary>
public static class L2OverheadAnchor
{
    /// <summary>
    /// Gap above the skull to the bottom of the name stack.
    /// Interlude pawn NameOffset is 6 UU; Fineage keeps a little more hair clearance.
    /// </summary>
    public const float NameOffsetMeters = 10f / 52.5f;

    /// <summary>Entity objectId → local Y of head (root-local).</summary>
    static readonly Dictionary<int, float> _localHeadYByEntity = new Dictionary<int, float>(128);

    /// <summary>Transform instance id → same, for lobby pawns without Entity.</summary>
    static readonly Dictionary<int, float> _localHeadYByInstance = new Dictionary<int, float>(16);

    public static void Invalidate(int entityId)
    {
        if (entityId != 0)
            _localHeadYByEntity.Remove(entityId);
    }

    public static void InvalidateTransform(Transform root)
    {
        if (root != null)
            _localHeadYByInstance.Remove(root.GetInstanceID());
    }

    public static void ClearAll()
    {
        _localHeadYByEntity.Clear();
        _localHeadYByInstance.Clear();
    }

    /// <summary>
    /// World point at the bottom of the HeadDisplay stack (under name / cast gauge).
    /// Never throws on the paint path (#930): biped head in band → skinned top → constant.
    /// </summary>
    public static Vector3 GetWorldPos(Entity entity, Transform root)
    {
        if (root == null)
            return Vector3.zero;

        int id = entity != null && entity.Identity != null ? entity.Identity.Id : 0;
        float localY = ResolveLocalHeadY(id, useEntityCache: true, root);
        return root.TransformPoint(new Vector3(0f, localY, 0f)) + Vector3.up * NameOffsetMeters;
    }

    /// <summary>Lobby / no-Entity path. Never throws (#930).</summary>
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

    /// <summary>Pick the highest Bip01 Head match (some meshes expose a near-root stub first).</summary>
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
