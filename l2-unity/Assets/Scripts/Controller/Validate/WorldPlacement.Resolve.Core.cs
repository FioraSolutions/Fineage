using UnityEngine;

public static partial class WorldPlacement
{
    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask = null)
    {
        LayerMask mask = WalkableMask(groundMask);
        if (TrySnapWalkable(serverUnityPos, mask, out Vector3 snapped))
            return snapped;
        return serverUnityPos;
    }

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
