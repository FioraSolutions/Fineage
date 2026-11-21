using System.Collections.Generic;
using UnityEngine;

public static partial class WorldPlacement
{
    static readonly List<float> _scratchYs = new List<float>(32);

    static LayerMask ResolveDefaultGroundMask()
    {
        World world = ControllerPacketServices.GameWorld;
        if (world != null)
            return world.GroundMask;
        return Physics.DefaultRaycastLayers;
    }

    /// <summary>
    /// Snap to the walkable floor nearest the server height at the server XY.
    /// Water is never a floor and never a reason to move: a pawn standing in a
    /// shallow pond stands on its bed, as in the retail client. If nothing walkable
    /// is within <see cref="WorldGrounding.SnapToleranceMeters"/>, the server height is kept.
    /// </summary>
    public static bool TrySnapWalkable(Vector3 serverPos, LayerMask mask, out Vector3 snapped)
    {
        CollectWalkableYs(serverPos, mask, _scratchYs);
        float y = WorldGrounding.PickHeight(serverPos.y, _scratchYs);
        snapped = new Vector3(serverPos.x, y, serverPos.z);
        for (int i = 0; i < _scratchYs.Count; i++)
        {
            if (Mathf.Abs(_scratchYs[i] - serverPos.y) <= WorldGrounding.SnapToleranceMeters)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Feet on the walkable sheet nearest to the current Y. A miss keeps Y;
    /// never free-falls onto a distant shell or a water surface.
    /// </summary>
    public static Vector3 KeepOnBrush(Vector3 feet)
    {
        if (TrySnapWalkable(feet, WalkableMask(null), out Vector3 snapped))
            return snapped;
        return feet;
    }

    /// <summary>
    /// Snap when a batched primary down-ray hit is walkable (first CollectWalkableYs probe).
    /// Returns false when the hit is unusable — callers should fall back to <see cref="KeepOnBrush"/>.
    /// </summary>
    public static bool TrySnapWalkableFromRaycastHit(Vector3 feet, RaycastHit hit, out Vector3 snapped)
    {
        snapped = feet;
        if (!IsWalkableHit(hit))
            return false;

        _scratchYs.Clear();
        _scratchYs.Add(hit.point.y);
        float y = WorldGrounding.PickHeight(feet.y, _scratchYs);
        snapped = new Vector3(feet.x, y, feet.z);
        for (int i = 0; i < _scratchYs.Count; i++)
        {
            if (Mathf.Abs(_scratchYs[i] - feet.y) <= WorldGrounding.SnapToleranceMeters)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Street under a point: same pipeline as spawn/move. Kept so existing
    /// call sites share the one snap rule.
    /// </summary>
    public static bool TryStreetDeck(Vector3 position, LayerMask mask, out float y)
    {
        y = position.y;
        if (!TrySnapWalkable(position, mask, out Vector3 snapped))
            return false;
        y = snapped.y;
        return Mathf.Abs(y - position.y) > 0.001f;
    }

    /// <summary>
    /// Feet clipped through the brush. A face just above the capsule is that floor.
    /// </summary>
    public static bool TryLiftOutOfFloor(Vector3 feet, out float y)
    {
        y = feet.y;
        LayerMask mask = WalkableMask(null);
        Vector3 probe = new Vector3(feet.x, feet.y + 0.7f, feet.z);
        CollectWalkableYs(probe, mask, _scratchYs);
        if (_scratchYs.Count == 0)
            return false;

        float best = float.PositiveInfinity;
        bool found = false;
        for (int i = 0; i < _scratchYs.Count; i++)
        {
            float lift = _scratchYs[i] - feet.y;
            if (lift < 0.05f || lift > 0.7f)
                continue;
            if (!found || _scratchYs[i] < best)
            {
                best = _scratchYs[i];
                found = true;
            }
        }

        if (!found)
            return false;
        y = best;
        return true;
    }

    /// <summary>True when a walkable surface is within <paramref name="meters"/> of the feet.</summary>
    public static bool HasFloorWithin(Vector3 feet, float meters)
    {
        _scratchYs.Clear();
        AppendHits(feet, feet.y + 0.2f, meters + 0.2f, WalkableMask(null), 0.2f, _scratchYs);
        for (int i = 0; i < _scratchYs.Count; i++)
        {
            if (Mathf.Abs(_scratchYs[i] - feet.y) <= meters)
                return true;
        }
        return false;
    }

    public static bool TryResolveMeshY(Vector3 position, LayerMask? groundMask, out float y)
    {
        y = position.y;
        if (!TrySnapWalkable(position, WalkableMask(groundMask), out Vector3 snapped))
            return false;
        y = snapped.y;
        return true;
    }

    static void CollectWalkableYs(Vector3 position, LayerMask mask, List<float> ys)
    {
        CollectWalkableYs(position, mask, ys, 0f);
    }

    static void CollectWalkableYs(Vector3 position, LayerMask mask, List<float> ys, float maxDrop)
    {
        ys.Clear();
        float originY = position.y + DefaultStartAbove;
        float distance = DefaultStartAbove + (maxDrop > 0.001f ? maxDrop : DefaultMaxDistance);
        AppendHits(position, originY, distance, mask, 0f, ys);
        if (ys.Count == 0)
            AppendHits(position, HighOriginY, HighMaxDistance, mask, 0f, ys);
        if (ys.Count == 0)
            AppendHits(position, HighOriginY, HighMaxDistance, mask, 0.4f, ys);
    }

    static void AppendHits(
        Vector3 position,
        float originY,
        float maxDistance,
        LayerMask mask,
        float sphereRadius,
        List<float> ys)
    {
        Vector3 origin = new Vector3(position.x, originY, position.z);
        bool prevBackfaces = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits;
        try
        {
            hits = sphereRadius > 0.001f
                ? Physics.SphereCastAll(origin, sphereRadius, Vector3.down, maxDistance, mask, QueryTriggerInteraction.Ignore)
                : Physics.RaycastAll(origin, Vector3.down, maxDistance, mask, QueryTriggerInteraction.Ignore);
        }
        finally
        {
            Physics.queriesHitBackfaces = prevBackfaces;
        }

        if (hits == null)
            return;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (!IsWalkableHit(hit))
                continue;
            ys.Add(hit.point.y);
        }
    }

    static bool IsWalkableHit(RaycastHit hit)
    {
        if (hit.collider == null)
            return false;
        if (hit.normal.y < 0.5f)
            return false;
        if (IsWaterCollider(hit.collider) || IsFoliageCollider(hit.collider) || IsActorCollider(hit.collider))
            return false;
        return true;
    }

    static bool IsWaterCollider(Collider collider)
    {
        if (collider == null)
            return false;

        int water = LayerMask.NameToLayer("Water");
        Transform t = collider.transform;
        for (int depth = 0; depth < 4 && t != null; depth++, t = t.parent)
        {
            if (water >= 0 && t.gameObject.layer == water)
                return true;
            string n = t.name;
            if (string.IsNullOrEmpty(n))
                continue;
            string lower = n.ToLowerInvariant();
            if (lower.Contains("watervolume")
                || lower.Contains("l2watervolumes")
                || lower == "l2water"
                || lower.Contains("waterplane")
                || lower.Contains("watersurface")
                || lower.Contains("water_surface"))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Import stopgap: tree/bush mesh names on StaticMesh until colliders leave the walk mask.
    /// </summary>
    static bool IsFoliageCollider(Collider collider)
    {
        if (collider == null)
            return false;

        Transform t = collider.transform;
        for (int depth = 0; depth < 4 && t != null; depth++, t = t.parent)
        {
            string n = t.name;
            if (string.IsNullOrEmpty(n))
                continue;
            string lower = n.ToLowerInvariant();
            if (lower.Contains("tree")
                || lower.Contains("bush")
                || lower.Contains("foliage")
                || lower.Contains("grass")
                || lower.Contains("leaf")
                || lower.Contains("plant"))
                return true;
        }

        return false;
    }

    static bool IsActorCollider(Collider collider)
    {
        if (collider == null)
            return false;

        Transform t = collider.transform;
        for (int depth = 0; depth < 6 && t != null; depth++, t = t.parent)
        {
            if (t.GetComponent<CharacterController>() != null)
                return true;
            string n = t.name;
            if (!string.IsNullOrEmpty(n) && n.IndexOf("click_area", System.StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Tile 20_18 under the cave mouth. Kept for the terrain-gate renderer,
    /// not used by grounding.
    /// </summary>
    public static bool IsDarkElfCave(Vector3 unityPos)
    {
        return unityPos.y < 45f
            && unityPos.x >= 0f && unityPos.x < 624f
            && unityPos.z >= 0f && unityPos.z < 624f;
    }
}
