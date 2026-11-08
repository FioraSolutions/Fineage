using UnityEngine;

/// <summary>
/// Places a world position on terrain via GroundMask raycast.
/// Shared by ValidateLocationDto jumps and player teleport.
/// </summary>
public static class GroundSnapHelper
{
    /// <summary>How far above the given Y to start the ray.</summary>
    public const float DefaultStartAbove = 50f;

    /// <summary>Max distance below the ray origin.</summary>
    public const float DefaultMaxDistance = 300f;

    /// <summary>
    /// Absolute-height fallback when the local cast misses (e.g. spawn Y below mesh).
    /// </summary>
    public const float FallbackOriginY = 200f;

    /// <summary>Same high origin geodata uses when GroundMask misses village grass.</summary>
    public const float HighOriginY = 750f;

    public const float HighMaxDistance = 1000f;

    const float MaxLift = 1.25f;
    const float MaxDrop = 15f;

    public static LayerMask ResolveGroundMask(LayerMask? groundMask)
    {
        if (groundMask.HasValue)
        {
            return groundMask.Value;
        }

        World world = ControllerPacketServices.GameWorld;
        if (world != null)
        {
            return world.GroundMask;
        }

        return Physics.DefaultRaycastLayers;
    }

    public static bool TrySnapToGround(
        Vector3 position,
        out Vector3 snapped,
        LayerMask? groundMask = null,
        float startAbove = DefaultStartAbove,
        float maxDistance = DefaultMaxDistance)
    {
        snapped = position;
        LayerMask mask = WalkableMask(groundMask);

        if (TryPickNearServerY(position, position.y + startAbove, startAbove + maxDistance, mask, 0f, out snapped))
            return true;
        if (TryPickNearServerY(position, HighOriginY, HighMaxDistance, mask, 0f, out snapped))
            return true;
        if (TryPickNearServerY(position, HighOriginY, HighMaxDistance, mask, 0.4f, out snapped))
            return true;

        return false;
    }

    public static Vector3 SnapToGroundOrKeep(
        Vector3 position,
        LayerMask? groundMask = null,
        float startAbove = DefaultStartAbove,
        float maxDistance = DefaultMaxDistance)
    {
        return TrySnapToGround(position, out Vector3 snapped, groundMask, startAbove, maxDistance)
            ? snapped
            : position;
    }

    static LayerMask WalkableMask(LayerMask? groundMask)
    {
        LayerMask mask = ResolveGroundMask(groundMask);
        mask |= LayerMask.GetMask("Default", "Terrain", "StaticMesh", "Brush", "AllowWalk", "Deco", "Obstacle");
        mask &= ~LayerMask.GetMask("Entity", "EntityClick", "DropItem", "Player", "UI", "Ignore Raycast", "SkillEffect");
        return mask;
    }

    static bool TryPickNearServerY(
        Vector3 position,
        float originY,
        float maxDistance,
        LayerMask mask,
        float sphereRadius,
        out Vector3 snapped)
    {
        snapped = position;
        Vector3 origin = new Vector3(position.x, originY, position.z);
        // Interlude HEV bridges (O_E_Brg/BrgT/Bridge): the top walk deck triangles can face away after the
        // UE->FBX->Unity conversion; one-sided MeshCollider raycasts skipped them and snapped onto the underside.
        bool prevBackfaces = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        RaycastHit[] hits;
        try
        {
            hits = sphereRadius > 0.001f
                ? Physics.SphereCastAll(origin, sphereRadius, Vector3.down, maxDistance, mask, QueryTriggerInteraction.Ignore)
                : Physics.RaycastAll(origin, Vector3.down, maxDistance, mask, QueryTriggerInteraction.Ignore);
        }
        finally { Physics.queriesHitBackfaces = prevBackfaces; }
        if (hits == null || hits.Length == 0)
            return false;

        // Prefer the highest walkable hit in the server-Y window. Closest-abs alone buries
        // Dark Elf Village NPCs on cave MeshColliders a few meters under the village floor.
        RaycastHit best = default;
        float bestY = float.NegativeInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null)
                continue;

            float dy = hit.point.y - position.y;
            if (dy > MaxLift || dy < -MaxDrop)
                continue;

            if (!found || hit.point.y > bestY)
            {
                bestY = hit.point.y;
                best = hit;
                found = true;
            }
        }

        if (!found)
            return false;

        snapped = new Vector3(position.x, best.point.y, position.z);
        return true;
    }
}
