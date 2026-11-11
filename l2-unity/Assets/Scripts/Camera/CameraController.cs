using System;

using UnityEngine;

public partial class CameraController : MonoBehaviour
{
    // L2 rotator: 65536 = 360°. CameraPitch hardcoded -2700 (= -14.83° in UE).
    // Unity Euler X is inverted vs UE pitch: negative UE pitch (look down) → positive Unity X.
    public const float L2PitchDegrees = 2700f * 360f / 65536f;
    public const float L2FovHorizontal = 90f;
    public const float L2DistanceMeters = 2.3f;
    public const float L2HeightOffsetMeters = 0.2f;
    public const float L2CollisionRadiusMeters = 0.1f;
    public const float L2TraceExtraMeters = 0.3f;
    public const float L2MinDistanceMeters = 1f;
    public const float L2MaxDistanceMeters = 6f;
    public const float L2ReferencePlayerHeight = 1.6f;

    private Vector3 _lerpLookAt;
    [SerializeField] private float _x, _y = L2PitchDegrees;
    private Vector3 _lookAt;
    private LayerMask _collisionMask;

    [SerializeField] private Transform _target;

    [Header("L2 scale (Unity player is shorter than L2 1.6m)")]
    [SerializeField] private float _unityPlayerHeight = 0.85f;
    [SerializeField] private float _l2PlayerHeight = L2ReferencePlayerHeight;
    [SerializeField] private bool _applyL2Fov = true;

    [Header("Camera controls")]
    [SerializeField] private bool _smoothCamera = true;
    [SerializeField] private Vector3 _camOffset = Vector3.zero;
    [SerializeField] private float _lookHeightAdjust = -0.3f;
    [SerializeField] private float _smoothness = 8f;
    [SerializeField] private float _camSpeed = 3f;
    [SerializeField] private float _pitchAngle = L2PitchDegrees;
    [Tooltip("A/B: skip collision pull-in; camera stays at zoom distance (test nameplate wave).")]
    [SerializeField] private bool _disableCollisionZoom = false;

    [Header("Zoom controls")]
    [SerializeField] private float _minDistance = L2MinDistanceMeters * (0.85f / L2ReferencePlayerHeight);
    [SerializeField] private float _maxDistance = 14f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _camDistance = L2DistanceMeters * (0.85f / L2ReferencePlayerHeight) + 0.5f;
    [SerializeField] private float _currentDistance = 0;

    [Header("Bone stickiness")]
    [SerializeField] private bool _stickToBone = false;
    [SerializeField] private Transform _rootBone;
    [SerializeField] private float _rootBoneHeight = 0;

    [SerializeField] private CameraCollisionDetection _collisionDetector;
    private Camera _camera;
    private float _resolvedPlayerHeight;

    public Transform Target { get { return _target; } set { _target = value; } }

    public bool StickToBone { get { return _stickToBone; } set { _stickToBone = value; } }
    public float CurrentDistance { get { return _currentDistance; } }
    public float MaxDistance { get { return _maxDistance; } }

    public float WorldScale
    {
        get
        {
            float l2Height = _l2PlayerHeight > 0.01f ? _l2PlayerHeight : L2ReferencePlayerHeight;
            return _unityPlayerHeight / l2Height;
        }
    }

    private static CameraController _instance;

    [System.Obsolete("Use CameraPacketServices.Controller or [Inject] CameraController instead")]
    public static CameraController Instance { get { return _instance; } }









}
