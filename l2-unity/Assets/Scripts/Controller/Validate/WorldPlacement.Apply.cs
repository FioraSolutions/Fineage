using UnityEngine;

public static partial class WorldPlacement
{
    public static void ApplyPose(GameObject go, Vector3 position, Quaternion rotation)
    {
        if (go == null)
            return;

        CharacterController[] controllers = go.GetComponentsInChildren<CharacterController>(true);
        bool[] wasEnabled = new bool[controllers.Length];
        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            wasEnabled[i] = controllers[i].enabled;
            controllers[i].enabled = false;
        }

        go.transform.SetPositionAndRotation(position, rotation);
        Physics.SyncTransforms();

        for (int i = 0; i < controllers.Length; i++)
        {
            if (controllers[i] == null)
                continue;
            controllers[i].enabled = wasEnabled[i];
        }
    }

    static LayerMask WalkableMask(LayerMask? groundMask)
    {
        LayerMask mask = groundMask.HasValue
            ? groundMask.Value
            : ResolveDefaultGroundMask();
        mask |= LayerMask.GetMask("Default", "Terrain", "StaticMesh", "Brush", "AllowWalk", "Deco", "Obstacle");
        mask &= ~LayerMask.GetMask("Entity", "EntityClick", "DropItem", "Player", "UI", "Ignore Raycast", "SkillEffect");
        return mask;
    }

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
        float minAbs = float.PositiveInfinity;
        bool found = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.collider == null)
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
            if (hit.collider == null)
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
}
