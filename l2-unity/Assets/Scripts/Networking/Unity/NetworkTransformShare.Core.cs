using System;
using UnityEngine;
using VContainer;

public partial class NetworkTransformShare : MonoBehaviour
{
    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        NetworkingPacketServices.Bind(world: _world);
        World world = GameWorld;
        if (world != null && world.OfflineMode)
        {
            enabled = false;
            return;
        }

        _characterController = GetComponent<CharacterController>();
        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
        _rotationShareEnabled = false; //rotation is now calculated based on movedirection
    }

    void Update()
    {
        if (ShouldSharePosition())
        {
            SharePosition();
        }

        if (ShouldShareRotation && _rotationShareEnabled)
        {
            ShareRotation();
        }
    }
}
