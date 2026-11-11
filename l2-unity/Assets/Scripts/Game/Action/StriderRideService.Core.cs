using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;
using Object = UnityEngine.Object;

public sealed partial class StriderRideService : ILateTickable
{
    public StriderRideService()
    {
        Instance = this;
        AnimationPacketServices.Bind(striderRide: this);
    }
}
