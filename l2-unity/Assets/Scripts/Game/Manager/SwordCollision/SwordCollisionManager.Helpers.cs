using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionService : MonoBehaviour
{
    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        SwordCollisionPacketServices.Bind(collision: this);
        _entityMask = LayerMask.GetMask("EntityClick");
    }

    private void Start()
    {
        WireGamePacketServices();
    }
}
