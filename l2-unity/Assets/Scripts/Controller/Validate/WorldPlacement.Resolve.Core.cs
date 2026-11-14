using UnityEngine;

public static partial class WorldPlacement
{
    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask = null)
    {
        // Gatekeeper stands inside the gate pillar. The street mesh starts a few
        // meters aside and three meters lower; leave her on that shell, not on a ledge.
        // The dark-elf school floor is already under the creation point. Pulling that
        // point down to the next brush (~5 m) drops the pawn into the sealed shell
        // under the temple, which reads as a prison with no exit.
        if ((kind == PlacementKind.NpcSpawn || kind == PlacementKind.PlayerTeleport)
            && IsDarkElfCave(serverUnityPos)
            && !HasFloorWithin(serverUnityPos, CaveDeckMeters)
            && TryCaveStreet(serverUnityPos, out Vector3 street))
            return street;

        Vector3 pos = serverUnityPos;
        // Plaza NPCs keep the server Z, which is the geodata node, while the stone
        // they should stand on is the first upward face under that point.
        if (PreferMeshForKind(kind)
            && !IsDarkElfCave(pos)
            && TryFloorUnder(pos, WalkableMask(groundMask), out float floorY))
        {
            pos.y = floorY;
            return pos;
        }

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
            pos.y = geoY;
        else if (hasMesh)
            pos.y = meshY;

        // Geodata sits on the node, often half a metre to two metres above the
        // brush. Drop onto that face. Do not climb: a roof above the node is ignored.
        // A point already under the brush (attack stop) is lifted back onto it.
        if ((kind == PlacementKind.PlayerTeleport || kind == PlacementKind.WalkValidate)
            && TryStreetDeck(pos, WalkableMask(groundMask), out float deckY))
            pos.y = deckY;

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
