using UnityEngine;

/// <summary>
/// Client height / pose SoT (#930).
/// Policy: prefer geodata Y when <see cref="TryResolveGeodataY"/> succeeds; else mesh hit
/// <b>closest to server Y</b> (absolute-highest was a village/DE crutch that parked pawns on tree canopies).
/// Multi-deck: among hits nearly tied for closeness, prefer the higher one.
/// Foot settle + CharacterController disable + <see cref="Physics.SyncTransforms"/> on pose write.
/// </summary>
public static class WorldPlacement
{
    public enum PlacementKind
    {
        PlayerTeleport,
        NpcSpawn,
        MonsterSpawn,
        ItemDrop,
        WalkValidate
    }

    public const float HighOriginY = 750f;
    public const float HighMaxDistance = 1000f;
    public const float DefaultStartAbove = 50f;
    public const float DefaultMaxDistance = 300f;

    /// <summary>Near-tie epsilon: prefer higher deck when Δ(serverY) is almost equal.</summary>
    const float NearTieMeters = 0.75f;

    /// <summary>Resolve a Unity world position for the given placement kind.</summary>
    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask = null)
    {
        Vector3 pos = serverUnityPos;
        if (TryResolveGeodataY(pos, out float geoY))
        {
            pos.y = geoY;
            return pos;
        }

        if (TryResolveMeshY(pos, groundMask, out float meshY))
            pos.y = meshY;
        return pos;
    }

    /// <summary>
    /// Geodata Y when nodes are loaded. Never throws — dormant geodata must not break spawn/teleport.
    /// </summary>
    public static bool TryResolveGeodataY(Vector3 serverUnityPos, out float y)
    {
        y = serverUnityPos.y;
        Geodata geo = GeodataPacketServices.Geodata;
        if (geo == null || !geo.Loaded)
            return false;

        if (!geo.TryGetClosestNodeAt(serverUnityPos, out Node node) || node == null)
            return false;

        y = node.center.y;
        return true;
    }

    /// <summary>
    /// Walkable mesh Y closest to server Y (not absolute highest — that snaps onto trees/roofs).
    /// </summary>
    public static bool TryResolveMeshY(
        Vector3 position,
        LayerMask? groundMask,
        out float y)
    {
        y = position.y;
        LayerMask mask = WalkableMask(groundMask);

        if (TryClosestHit(position, position.y + DefaultStartAbove, DefaultStartAbove + DefaultMaxDistance, mask, 0f, out y))
            return true;
        if (TryClosestHit(position, HighOriginY, HighMaxDistance, mask, 0f, out y))
            return true;
        if (TryClosestHit(position, HighOriginY, HighMaxDistance, mask, 0.4f, out y))
            return true;
        return false;
    }

    /// <summary>
    /// Lift the root so skinned bounds <c>min.y</c> sits on <paramref name="groundPos"/>.y.
    /// </summary>
    public static Vector3 SettleFeet(GameObject go, Vector3 groundPos)
    {
        if (go == null)
            return groundPos;

        SkinnedMeshRenderer[] skins = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        if (skins == null || skins.Length == 0)
            return groundPos;

        bool has = false;
        float minY = 0f;
        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null)
                continue;
            float sy = skins[i].bounds.min.y;
            if (!has || sy < minY)
            {
                minY = sy;
                has = true;
            }
        }

        if (!has)
            return groundPos;

        float sink = groundPos.y - minY;
        if (sink <= 0.001f || sink > 5f)
            return go.transform.position;

        Vector3 p = go.transform.position;
        p.y += sink;
        ApplyPose(go, p, go.transform.rotation);
        return p;
    }

    /// <summary>Write pose with CharacterController disabled + Physics.SyncTransforms.</summary>
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
