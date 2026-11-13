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
            if (hit.collider == null || IsFoliageCollider(hit.collider) || IsOverheadHeightmap(hit, position.y))
                continue;
            if (IsDarkElfCave(position) && !IsCaveFloorHit(hit, position.y))
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
            if (hit.collider == null || IsFoliageCollider(hit.collider) || IsOverheadHeightmap(hit, position.y))
                continue;
            if (IsDarkElfCave(position) && !IsCaveFloorHit(hit, position.y))
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

    /// <summary>
    /// Dark Elf village floor is under the 20_18 heightmap. A hit on that sheet
    /// from below is the ceiling, not the ground — snapping to it throws the body up.
    /// </summary>
    static bool IsOverheadHeightmap(RaycastHit hit, float bodyY)
    {
        if (hit.collider == null || hit.point.y <= bodyY + 3f)
            return false;
        Transform t = hit.collider.transform;
        return t != null && t.name == "20_18";
    }

    /// <summary>Tile 20_18 under the cave mouth. Unity X is L2 Y, Unity Z is L2 X.</summary>
    public static bool IsDarkElfCave(Vector3 unityPos)
    {
        return unityPos.y < 45f
            && unityPos.x >= 0f && unityPos.x < 624f
            && unityPos.z >= 0f && unityPos.z < 624f;
    }

    /// <summary>True when a walkable surface is within <paramref name="meters"/> under the feet.</summary>
    public static bool HasFloorWithin(Vector3 feet, float meters)
    {
        return TryClosestHit(feet, feet.y + 0.2f, meters + 0.2f, WalkableMask(null), 0.2f, out _);
    }

    static bool IsCaveFloorHit(RaycastHit hit, float bodyY)
    {
        if (hit.normal.y < 0.5f)
            return false;
        // A cornice above the body is not the street. The basement shell is ~9 m down.
        float drop = bodyY - hit.point.y;
        return drop >= -0.5f && drop <= CaveDeckMeters;
    }

    /// <summary>
    /// Village shell floor under this point, or the nearest one within a few meters.
    /// Props such as the gate pillar are not a floor.
    /// </summary>
    public static bool TryCaveStreet(Vector3 serverPos, out Vector3 street)
    {
        street = serverPos;
        if (TryShellFloor(serverPos, serverPos.y, out RaycastHit under))
        {
            street = under.point;
            return true;
        }

        float best = float.PositiveInfinity;
        bool found = false;
        float[] radii = { 2f, 3.4f };
        for (int r = 0; r < radii.Length; r++)
        {
            for (int i = 0; i < 8; i++)
            {
                float a = (i + 0.5f) * Mathf.PI * 2f / 8f;
                Vector3 sample = serverPos + new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a)) * radii[r];
                if (!TryShellFloor(sample, serverPos.y, out RaycastHit hit))
                    continue;
                float score = radii[r] + Mathf.Abs(hit.point.y - serverPos.y);
                if (score >= best)
                    continue;
                best = score;
                street = hit.point;
                found = true;
            }

            if (found)
                break;
        }

        if (!found)
            return false;

        Vector3 away = street - serverPos;
        away.y = 0f;
        if (away.sqrMagnitude > 0.04f)
        {
            Vector3 nudged = street + away.normalized * 0.6f;
            if (TryShellFloor(nudged, serverPos.y, out RaycastHit again))
                street = again.point;
        }

        return true;
    }

    static bool TryShellFloor(Vector3 sample, float refY, out RaycastHit picked)
    {
        picked = default;
        int brush = LayerMask.NameToLayer("Brush");
        if (brush < 0)
            return false;

        // Street sits up to ~5 m under the server Z. The next shell is ~9 m down.
        const float streetDrop = 6f;
        Vector3 origin = new Vector3(sample.x, refY + 0.5f, sample.z);
        RaycastHit[] hits = Physics.RaycastAll(
            origin, Vector3.down, streetDrop + 0.5f, 1 << brush, QueryTriggerInteraction.Ignore);
        bool found = false;
        float bestDrop = float.PositiveInfinity;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.normal.y < 0.5f)
                continue;
            float drop = refY - hit.point.y;
            if (drop < -0.5f || drop > streetDrop || drop >= bestDrop)
                continue;
            bestDrop = drop;
            picked = hit;
            found = true;
        }

        return found;
    }
}
