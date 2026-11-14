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

    /// <summary>
    /// Highest upward face under the server point, at most <see cref="TownFloorDropMeters"/> down.
    /// A lid or prop that is closer to server Z but above this face is not the street.
    /// </summary>
    static bool TryFloorUnder(Vector3 position, LayerMask mask, out float y)
    {
        y = position.y;
        Vector3 origin = new Vector3(position.x, position.y + 0.2f, position.z);
        bool prevBackfaces = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = false;
        RaycastHit[] hits;
        try
        {
            hits = Physics.RaycastAll(
                origin, Vector3.down, TownFloorDropMeters + 0.2f, mask, QueryTriggerInteraction.Ignore);
        }
        finally
        {
            Physics.queriesHitBackfaces = prevBackfaces;
        }

        if (hits == null || hits.Length == 0)
            return false;

        float bestY = float.NegativeInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || IsFoliageCollider(hit.collider) || IsActorCollider(hit.collider))
                continue;
            if (hit.normal.y < 0.55f)
                continue;
            float drop = position.y - hit.point.y;
            if (drop < -0.12f || drop > TownFloorDropMeters)
                continue;
            if (!found || hit.point.y > bestY)
            {
                bestY = hit.point.y;
                found = true;
            }
        }

        if (!found)
            return false;

        y = bestY;
        return true;
    }

    /// <summary>
    /// Feet clipped through the brush. A face just above the capsule is that floor.
    /// A roof higher than the body is left alone.
    /// </summary>
    public static bool TryLiftOutOfFloor(Vector3 feet, out float y)
    {
        y = feet.y;
        if (IsDarkElfCave(feet))
            return false;

        LayerMask mask = WalkableMask(null);
        const float below = 0.8f;
        const float above = 0.7f;
        if (TryFloorWithin(feet, mask, below, out _))
            return false;

        return TryFloorAbove(feet, mask, above, out y);
    }

    static bool TryFloorWithin(Vector3 position, LayerMask mask, float maxDrop, out float y)
    {
        y = position.y;
        Vector3 origin = new Vector3(position.x, position.y + 0.15f, position.z);
        RaycastHit[] hits = Physics.RaycastAll(
            origin, Vector3.down, maxDrop + 0.15f, mask, QueryTriggerInteraction.Ignore);
        float best = float.NegativeInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || IsFoliageCollider(hit.collider) || IsActorCollider(hit.collider))
                continue;
            if (hit.normal.y < 0.55f)
                continue;
            float drop = position.y - hit.point.y;
            if (drop < -0.05f || drop > maxDrop)
                continue;
            if (!found || hit.point.y > best)
            {
                best = hit.point.y;
                found = true;
            }
        }

        if (!found)
            return false;
        y = best;
        return true;
    }

    static bool TryFloorAbove(Vector3 position, LayerMask mask, float maxLift, out float y)
    {
        y = position.y;
        Vector3 origin = new Vector3(position.x, position.y + maxLift, position.z);
        RaycastHit[] hits = Physics.RaycastAll(
            origin, Vector3.down, maxLift, mask, QueryTriggerInteraction.Ignore);
        float best = float.PositiveInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null || IsFoliageCollider(hit.collider) || IsActorCollider(hit.collider))
                continue;
            if (hit.normal.y < 0.55f)
                continue;
            float lift = hit.point.y - position.y;
            if (lift < 0.05f || lift > maxLift)
                continue;
            if (!found || hit.point.y < best)
            {
                best = hit.point.y;
                found = true;
            }
        }

        if (!found)
            return false;
        y = best;
        return true;
    }

    /// <summary>
    /// Street under a town point. A roof above the point is never chosen.
    /// Lift only when nothing walkable sits below, and the body is already
    /// inside the brush (attack step that pushed the capsule through).
    /// </summary>
    public static bool TryStreetDeck(Vector3 position, LayerMask mask, out float y)
    {
        y = position.y;
        if (IsDarkElfCave(position))
            return false;
        if (TryFloorUnder(position, mask, out y))
            return true;
        return TryFloorAbove(position, mask, 0.7f, out y);
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

        // Village street is about 3 m under a server point that sits in a pillar.
        // The school creation point is only 0.7 m above its own floor; the next
        // brush, ~5 m down, is the sealed shell under the temple.
        const float streetDrop = CaveDeckMeters;
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
