using UnityEngine;

/// <summary>
/// Thin façade over <see cref="WorldPlacement"/> for legacy call sites.
/// Prefer <see cref="WorldPlacement.Resolve"/> for new code (#930).
/// </summary>
public static class GroundSnapHelper
{
    public const float DefaultStartAbove = WorldPlacement.DefaultStartAbove;
    public const float DefaultMaxDistance = WorldPlacement.DefaultMaxDistance;
    public const float FallbackOriginY = 200f;
    public const float HighOriginY = WorldPlacement.HighOriginY;
    public const float HighMaxDistance = WorldPlacement.HighMaxDistance;

    public static LayerMask ResolveGroundMask(LayerMask? groundMask)
    {
        if (groundMask.HasValue)
            return groundMask.Value;

        World world = ControllerPacketServices.GameWorld;
        if (world != null)
            return world.GroundMask;

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
        if (!WorldPlacement.TryResolveMeshY(position, groundMask, out float y))
            return false;
        snapped = new Vector3(position.x, y, position.z);
        return true;
    }

    public static Vector3 SnapToGroundOrKeep(
        Vector3 position,
        LayerMask? groundMask = null,
        float startAbove = DefaultStartAbove,
        float maxDistance = DefaultMaxDistance)
    {
        return WorldPlacement.Resolve(WorldPlacement.PlacementKind.WalkValidate, position, groundMask);
    }
}
