using UnityEngine;

public static partial class WorldPlacement
{
    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask = null)
    {
        Vector3 pos = serverUnityPos;
        bool hasGeo = TryResolveGeodataY(pos, out float geoY);
        bool hasMesh = TryResolveMeshY(pos, groundMask, out float meshY);

        // Spawned NPCs / monsters / drops: visual mesh is SoT outdoors (TI village geodata sat
        // below the mesh and buried Folks). Caves still pick the deck nearest server Z via
        // TryClosestHit. PlayerTeleport / WalkValidate stay geo-first.
        if (PreferMeshForKind(kind))
        {
            if (hasMesh)
                pos.y = meshY;
            else if (hasGeo)
                pos.y = geoY;
            return pos;
        }

        if (hasGeo)
        {
            pos.y = geoY;
            return pos;
        }

        if (hasMesh)
            pos.y = meshY;
        return pos;
    }

    static bool PreferMeshForKind(PlacementKind kind) =>
        kind == PlacementKind.NpcSpawn
        || kind == PlacementKind.MonsterSpawn
        || kind == PlacementKind.ItemDrop;

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
}
