using UnityEngine;

/// <summary>
/// Collision mask <c>World</c> pushes onto the gameplay camera (#131 S39a).
/// <c>GameLifetimeScope</c> registers the scene <c>CameraController</c> as this interface.
/// </summary>
public interface ICameraCollisionMask
{
    void SetMask(LayerMask collisionMask);
}
