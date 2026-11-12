using UnityEngine;

public static partial class WorldPlacement
{
    static LayerMask ResolveDefaultGroundMask()
    {
        World world = ControllerPacketServices.GameWorld;
        if (world != null)
            return world.GroundMask;
        return Physics.DefaultRaycastLayers;
    }

    static bool TryClosestHit(
        Vector3 position,
        float originY,
        float maxDistance,
        LayerMask mask,
        float sphereRadius,
        out float y)
    {
        y = position.y;
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

        if (hits == null || hits.Length == 0)
            return false;

        // Pass 1: closest |ΔY| to server. Pass 2: among near-ties, pick higher deck (cave vs village).
        // Skip foliage-named colliders so canopy MeshColliders on StaticMesh cannot become height SoT (#931).
        float minAbs = float.PositiveInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || IsFoliageCollider(hit.collider))
                continue;
            float abs = Mathf.Abs(hit.point.y - position.y);
            if (!found || abs < minAbs)
            {
                minAbs = abs;
                found = true;
            }
        }

        if (!found)
            return false;

        float bestY = float.NegativeInfinity;
        bool picked = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || IsFoliageCollider(hit.collider))
                continue;
            float abs = Mathf.Abs(hit.point.y - position.y);
            if (abs > minAbs + NearTieMeters)
                continue;
            if (!picked || hit.point.y > bestY)
            {
                bestY = hit.point.y;
                picked = true;
            }
        }

        if (!picked)
            return false;

        y = bestY;
        return true;
    }

    /// <summary>
    /// Import stopgap (#931): tree/bush mesh names on StaticMesh until colliders leave the walk mask.
    /// Prefer geodata Y; long-term move foliage to Deco / non-walk at import.
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
}
