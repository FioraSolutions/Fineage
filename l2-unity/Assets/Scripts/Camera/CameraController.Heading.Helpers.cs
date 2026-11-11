using System;
using UnityEngine;

public partial class CameraController
{
    CharacterController FindCharacterController()
    {
        if (_target == null)
        {
            return null;
        }

        CharacterController cc = _target.GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = _target.GetComponentInChildren<CharacterController>();
        }

        return cc;
    }

    void ApplyL2Fov()
    {
        if (!_applyL2Fov)
        {
            return;
        }

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        if (_camera == null)
        {
            return;
        }

        _camera.fieldOfView = VerticalFovFromHorizontal(L2FovHorizontal, _camera.aspect);
    }

    static float VerticalFovFromHorizontal(float horizontalFov, float aspect)
    {
        float h = horizontalFov * Mathf.Deg2Rad;
        float v = 2f * Mathf.Atan(Mathf.Tan(h * 0.5f) / Mathf.Max(0.01f, aspect));
        return v * Mathf.Rad2Deg;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
