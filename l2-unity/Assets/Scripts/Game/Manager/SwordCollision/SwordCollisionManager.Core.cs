using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SwordCollisionService
{
    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(gameWorld: _world);
        SwordCollisionPacketServices.Bind(collision: this);
    }
}
