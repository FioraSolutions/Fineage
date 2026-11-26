using UnityEngine;

/// <summary>
/// Floor and pose queries <c>PlayerController</c> uses while it moves (#131 S41a).
/// <c>GameLifetimeScope</c> registers <c>WorldPlacementAdapter</c> as this interface.
/// The static <c>WorldPlacement</c> methods stay the implementation.
/// </summary>
public interface IWorldPlacement
{
    bool HasFloorWithin(Vector3 feet, float meters);

    bool TryResolveGeodataY(Vector3 serverUnityPos, out float y);

    bool TryLiftOutOfFloor(Vector3 feet, out float y);

    void ApplyPose(GameObject go, Vector3 position, Quaternion rotation);
}
