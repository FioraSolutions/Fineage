using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(gameWorld: _world);
        HitPacketServices.Bind(hits: this);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            HitPacketServices.Bind(hits: this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void OnEnable()
    {
        TrySubscribeProjectileHits();
    }

    private void Start()
    {
        WireGamePacketServices();
        TrySubscribeProjectileHits();
    }

    private void OnDisable()
    {
        UnsubscribeProjectileHits();
        ClearRemoteMeleeHits();
    }

    private void Update()
    {
        TickRemoteMeleeHits();
    }
}
