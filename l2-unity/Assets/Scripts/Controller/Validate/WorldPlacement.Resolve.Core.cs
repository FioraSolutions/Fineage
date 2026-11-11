using UnityEngine;

public static partial class WorldPlacement
{
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
