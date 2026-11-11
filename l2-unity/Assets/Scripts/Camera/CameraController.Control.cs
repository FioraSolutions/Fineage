using System;

using UnityEngine;

public partial class CameraController : MonoBehaviour
{
    private void Update()
    {
        if (_target != null)
        {
            UpdateZoom();
        }
    }

    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        UpdateInputs();
        UpdatePosition();
    }

    public void SetTarget(GameObject go)
    {
        _target = go.transform;
        _resolvedPlayerHeight = ResolvePlayerHeight();
        _lookAt = LookAtPoint();
        _lerpLookAt = _lookAt;
        _y = _pitchAngle;
        _currentDistance = _camDistance;

        _rootBone = _target.FindRecursive(child => child.tag == "Root");
        if (_rootBone != null)
        {
            _rootBoneHeight = _rootBone.position.y - _target.position.y;
        }

        if (_camera == null)
        {
            _camera = GetComponent<Camera>();
        }

        _collisionDetector = new CameraCollisionDetection(_camera, _target, _camOffset, _collisionMask);
        ApplyL2Fov();
    }

    public bool IsObjectVisible(Transform target)
    {
        if (_collisionDetector == null || target == null)
        {
            return false;
        }

        RaycastHit hit;
        Vector3[] cameraClips = _collisionDetector.GetCameraViewPortPoints();
        if (cameraClips.Length == 0)
        {
            return false;
        }

        bool visible = false;
        Vector3 head = target.position + Vector3.up * _resolvedPlayerHeight;
        for (int i = 0; i < cameraClips.Length; i++)
        {
            if (!Physics.Linecast(cameraClips[i], head, out hit, _collisionMask))
            {
                visible = true;
                break;
            }
        }

        if (!visible)
        {
            return false;
        }

        Camera cam = _camera != null ? _camera : Camera.main;
        if (cam == null)
        {
            return false;
        }

        Vector3 viewPort = cam.WorldToViewportPoint(target.position);
        return viewPort.x <= 1 && viewPort.x >= 0 && viewPort.y <= 1 && viewPort.y >= 0 && viewPort.z >= -0.2f;
    }
}
