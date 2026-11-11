using System;

using UnityEngine;

public partial class CameraController : MonoBehaviour
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            CameraPacketServices.Bind(camera: this);
            _camera = GetComponent<Camera>();
            _resolvedPlayerHeight = _unityPlayerHeight;
            // Char-select presentation uses CharSelect layer; gameplay must not draw it.
            int lobbyLayer = LobbyPresentation.Layer;
            if (_camera != null && lobbyLayer >= 0)
                _camera.cullingMask &= ~(1 << lobbyLayer);
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    private void Start()
    {
        _lerpLookAt = Vector3.zero;
        _y = _pitchAngle;
        _currentDistance = _camDistance;
        ApplyL2Fov();
    }

    public void SetMask(LayerMask collisionMask)
    {
        _collisionMask = collisionMask;
        if (_collisionDetector != null)
        {
            _collisionDetector.SetMask(collisionMask);
        }
    }
}
