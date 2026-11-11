using System;

using System.Collections.Generic;
using UnityEngine;

public partial class ObjectPoolManager : AbstractPoolManager, IPoolManager
{
    private void Awake()
    {
        Instance = this;
        ObjectPoolPacketServices.Bind(pool: this);
    }

    private void OnDestroy()
    {
        if (ReferenceEquals(_instance, this))
            _instance = null;
    }
}
