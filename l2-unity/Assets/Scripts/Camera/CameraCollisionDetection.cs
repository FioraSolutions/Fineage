using UnityEngine;

public partial class CameraCollisionDetection
{
    [SerializeField] private LayerMask _collisionLayer;
    [SerializeField] private float _adjustedDistance;
    [SerializeField] private Transform _collisionObject;
    [SerializeField] private Vector2 _clipPlaneOffset = new Vector2(2f, 1f);
    [SerializeField] private bool _debug = false;
    private Camera _camera;
    private Transform _target;
    private Vector3 _offset;

    public float AdjustedDistance { get { return _adjustedDistance; } }





}
