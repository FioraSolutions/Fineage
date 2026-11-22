using UnityEngine;

public static partial class WorldPlacement
{
    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask = null)
    {
        return Resolve(kind, serverUnityPos, groundMask, null);
    }

    public static Vector3 Resolve(
        PlacementKind kind,
        Vector3 serverUnityPos,
        LayerMask? groundMask,
        PlacementFailContext? failContext)
    {
        LayerMask mask = WalkableMask(groundMask);
        if (RequiresFloorSearch(kind))
            return ResolveGroundFloor(serverUnityPos, mask, failContext);

        if (TrySnapWalkable(serverUnityPos, mask, out Vector3 snapped))
            return snapped;
        return serverUnityPos;
    }

    static Vector3 ResolveGroundFloor(
        Vector3 serverUnityPos,
        LayerMask mask,
        PlacementFailContext? failContext)
    {
        if (TrySnapWalkable(serverUnityPos, mask, out Vector3 toleranceSnap))
            return toleranceSnap;

        if (WorldGrounding.TryPickFloorBelow(serverUnityPos.y, _scratchYs, out float meshFloor))
            return new Vector3(serverUnityPos.x, meshFloor, serverUnityPos.z);

        if (TryResolveGeodataY(serverUnityPos, out float geoY)
            && geoY <= serverUnityPos.y + 0.5f)
            return new Vector3(serverUnityPos.x, geoY, serverUnityPos.z);

        LogGroundFloorFailure(serverUnityPos, failContext, _scratchYs);
        return serverUnityPos;
    }

    static void LogGroundFloorFailure(
        Vector3 serverUnityPos,
        PlacementFailContext? failContext,
        System.Collections.Generic.List<float> walkableYs)
    {
        string nearestDetail;
        if (walkableYs != null && walkableYs.Count > 0
            && WorldGrounding.TryGetNearestWalkableY(serverUnityPos.y, walkableYs, out float nearestWalkable))
        {
            nearestDetail = "nearestWalkableY=" + nearestWalkable.ToString("F3");
        }
        else
        {
            Geodata geo = GeodataPacketServices.Geodata;
            nearestDetail = geo == null || !geo.Loaded
                ? "no walkable / geo not loaded"
                : "no walkable / geo had no node";
        }

        if (failContext.HasValue)
        {
            PlacementFailContext ctx = failContext.Value;
            Debug.LogError(
                "[WorldPlacement] ItemDrop floor snap failed"
                + " objectId=" + ctx.ObjectId
                + " itemId=" + ctx.ItemId
                + " serverY=" + serverUnityPos.y.ToString("F3")
                + " " + nearestDetail
                + " pos=(" + serverUnityPos.x.ToString("F1") + "," + serverUnityPos.z.ToString("F1") + ")");
            return;
        }

        Debug.LogError(
            "[WorldPlacement] ItemDrop floor snap failed"
            + " serverY=" + serverUnityPos.y.ToString("F3")
            + " " + nearestDetail
            + " pos=(" + serverUnityPos.x.ToString("F1") + "," + serverUnityPos.z.ToString("F1") + ")");
    }
}