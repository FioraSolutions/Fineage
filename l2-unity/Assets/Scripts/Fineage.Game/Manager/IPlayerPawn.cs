using UnityEngine;

/// <summary>
/// Movement and heading other types use on the local pawn (#131 S40).
/// <c>GameLifetimeScope</c> registers <c>PlayerController</c> as this interface.
/// </summary>
public interface IPlayerPawn
{
    bool Running { get; set; }

    bool RunningToDestination { get; }

    Vector3 MoveDirection { get; }

    Vector3 GetPlayerPosition();

    void StopMove();

    void RotateToAttacker(Vector3 attackerPosition);

    void UpdateRunSpeed(float unityMetersPerSecond);

    void UpdateWalkSpeed(float unityMetersPerSecond);
}
