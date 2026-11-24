using UnityEngine;

/// <summary>
/// Forwards <see cref="IWorldPlacement"/> to the static <see cref="WorldPlacement"/> methods (#131 S41a).
/// </summary>
public sealed class WorldPlacementAdapter : IWorldPlacement
{
    public bool HasFloorWithin(Vector3 feet, float meters) =>
        WorldPlacement.HasFloorWithin(feet, meters);

    public bool TryResolveGeodataY(Vector3 serverUnityPos, out float y) =>
        WorldPlacement.TryResolveGeodataY(serverUnityPos, out y);

    public bool TryLiftOutOfFloor(Vector3 feet, out float y) =>
        WorldPlacement.TryLiftOutOfFloor(feet, out y);

    public void ApplyPose(GameObject go, Vector3 position, Quaternion rotation) =>
        WorldPlacement.ApplyPose(go, position, rotation);
}
