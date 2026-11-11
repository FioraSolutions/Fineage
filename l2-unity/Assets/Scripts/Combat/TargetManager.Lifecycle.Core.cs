using System;
using UnityEngine;
using VContainer;

public partial class TargetManager
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _considerCandidate = ConsiderCandidate;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        _target = null;
        _attackTarget = null;
        WireControllerPacketServices();
    }

    [Inject]
    void WireControllerPacketServices()
    {
        ControllerPacketServices.Bind(targets: this, gameWorld: GameWorld);
        TargetPacketServices.Bind(targets: this);
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
