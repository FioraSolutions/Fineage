using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager : MonoBehaviour
{
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            GlobalEffectsPacketServices.Bind(effects: this);
            WireGamePacketServices();
            BuildRegistry();
        }
        else if (_instance != this)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        WireGamePacketServices();
        if (!_registryBuilt)
            BuildRegistry();
    }
}
