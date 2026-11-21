using UnityEngine;

/// <summary>Player-backed map plane read — lives outside Fineage.UI asmdef (needs Entity/DI).</summary>
public static class L2MapCoordinatesPlayer
{
    public static bool TryMapPlaneFromLocalPlayer(out Vector2 l2XY)
    {
        l2XY = default;
        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local == null || local.Identity == null)
            return false;

        Vector3 unity = local.transform.position;
        Vector3 l2j = local.Identity.GetL2jPos(unity);
        l2XY = L2MapCoordinates.MapPlaneFromL2jVector(l2j);
        return true;
    }
}