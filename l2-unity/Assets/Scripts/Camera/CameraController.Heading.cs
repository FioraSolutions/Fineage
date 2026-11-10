using System;
using UnityEngine;

public partial class CameraController
{
    public void SetHeading(float heading)
    {
        _x = heading;
    }

    Vector3 LookAtPoint()
    {
        float boneOffset = 0f;
        if (_stickToBone && _rootBone != null)
        {
            boneOffset = _rootBone.position.y - _target.position.y - _rootBoneHeight;
        }

        float aboveHead = L2HeightOffsetMeters * WorldScale;
        CharacterController cc = FindCharacterController();
        if (cc != null && cc.height > 0.1f)
        {
            Vector3 localTop = cc.center + Vector3.up * (cc.height * 0.5f);
            return cc.transform.TransformPoint(localTop) + _camOffset + Vector3.up * (aboveHead + _lookHeightAdjust + boneOffset);
        }

        float lookHeight = _resolvedPlayerHeight + aboveHead + _lookHeightAdjust;
        return _target.position + _camOffset + Vector3.up * (lookHeight + boneOffset);
    }

    float ResolvePlayerHeight()
    {
        CharacterController cc = FindCharacterController();
        if (cc != null && cc.height > 0.1f)
        {
            return cc.height;
        }

        return _unityPlayerHeight;
    }

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
