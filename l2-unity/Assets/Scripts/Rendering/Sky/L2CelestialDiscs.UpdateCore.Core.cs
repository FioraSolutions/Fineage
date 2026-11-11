using System;
using Object = UnityEngine.Object;

public partial class L2CelestialDiscs
{
    void OnEnable()
    {
        Instance = this;
        SkyPacketServices.Bind(discs: this);
        L2CelestialLut.EnsureLoaded();
        EnsureDiscs();
    }
}
