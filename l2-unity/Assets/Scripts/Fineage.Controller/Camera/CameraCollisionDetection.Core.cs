using UnityEngine;

public partial class CameraCollisionDetection
{
    public CameraCollisionDetection(Camera camera, Transform target, Vector3 cameraOffset, LayerMask collisionmask) {
        _camera = camera;
        _target = target;
        _offset = cameraOffset;
        _collisionLayer = collisionmask;
        _adjustedDistance = 0f;
    }

    public void SetMask(LayerMask collisionMask)
    {
        _collisionLayer = collisionMask;
    }
}
