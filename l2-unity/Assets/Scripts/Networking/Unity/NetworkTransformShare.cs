using System;

public partial class NetworkTransformShare : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld => NetworkingPacketServices.GameWorld;
    private CharacterController _characterController;
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;

    [SerializeField] public Vector3 _serverPosition;
    [SerializeField] public bool _shouldShareRotation;
    public bool _rotationShareEnabled;

    public bool ShouldShareRotation { get { return _shouldShareRotation; } set { _shouldShareRotation = value; } }

    private static NetworkTransformShare _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static NetworkTransformShare Instance { get { return _instance; } }




    // Share position every 0.25f and based on delay



}
