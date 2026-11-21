using UnityEngine;

/// <summary>
/// Shared L2 horizontal plane (X,Y) for world map, detailed maps, and party pins.
/// Matches Interlude NCMinimapWnd::WorldToMap axis packing: ConvertPosToUnity stores
/// L2 (x,y,z) as Unity (y,z,x)/52.5, so map L2 x = Unity z and map L2 y = Unity x.
/// </summary>
public static class L2MapCoordinates
{
    public static Vector2 MapPlaneFromUnity(Vector3 unityWorld)
    {
        float l2X = unityWorld.z / VectorUtils.L2UuToUnity;
        float l2Y = unityWorld.x / VectorUtils.L2UuToUnity;
        return new Vector2(l2X, l2Y);
    }

    public static Vector2 MapPlaneFromL2jVector(Vector3 l2jVector)
    {
        // ConvertPosUnityToL2j returns (L2.x, L2.z, L2.y) — use .x and .z for the map plane.
        return new Vector2(l2jVector.x, l2jVector.z);
    }

    public static bool TryMapPlaneFromLocalPlayer(out Vector2 l2XY)
    {
        l2XY = default;
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local == null || local.Identity == null)
            return false;

        Vector3 unity = local.transform.position;
        Vector3 l2j = local.Identity.GetL2jPos(unity);
        l2XY = MapPlaneFromL2jVector(l2j);
        return true;
    }
}