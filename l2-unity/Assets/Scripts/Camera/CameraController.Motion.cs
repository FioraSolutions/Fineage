using System;

public partial class CameraController
{
    private void UpdateInputs()
    {
        InputManager input = ControllerPacketServices.Input;
        if (input == null || !input.TurnCamera)
        {
            return;
        }

        Vector2 axis = input.CameraAxis;
        _x += axis.x * _camSpeed;
        _y -= axis.y * _camSpeed;
        _y = ClampAngle(_y, -80f, 80f);
    }

    private void UpdateZoom()
    {
        InputManager input = ControllerPacketServices.Input;
        if (input == null)
        {
            return;
        }

        float scrollAxis = input.ZoomAxis;
        _camDistance = Mathf.Clamp(_camDistance - scrollAxis * _zoomSpeed * 0.1f, _minDistance, _maxDistance);
    }

    private void UpdatePosition()
    {
        Quaternion rotation = Quaternion.Euler(_y, _x, 0f);
        _lookAt = LookAtPoint();

        if (_smoothCamera)
        {
            if (_lerpLookAt == Vector3.zero)
            {
                _lerpLookAt = _lookAt;
            }

            _lerpLookAt = Vector3.Lerp(_lerpLookAt, _lookAt, _smoothness * Time.deltaTime);
        }
        else
        {
            _lerpLookAt = _lookAt;
        }

        float desiredDistance = _camDistance;
        if (!_disableCollisionZoom)
        {
            if (_collisionDetector == null && _camera != null)
            {
                _collisionDetector = new CameraCollisionDetection(_camera, _target, _camOffset, _collisionMask);
            }

            if (_collisionDetector != null)
            {
                _collisionDetector.DetectSphereCollision(
                    _lerpLookAt,
                    rotation,
                    desiredDistance,
                    L2CollisionRadiusMeters * WorldScale,
                    L2TraceExtraMeters * WorldScale);
                desiredDistance = _collisionDetector.AdjustedDistance;
            }
        }

        _currentDistance = desiredDistance;
        Vector3 adjustedPosition = _lerpLookAt + rotation * (Vector3.forward * -_currentDistance);
        if (float.IsNaN(adjustedPosition.x))
        {
            return;
        }

        transform.SetPositionAndRotation(adjustedPosition, rotation);
    }

}
