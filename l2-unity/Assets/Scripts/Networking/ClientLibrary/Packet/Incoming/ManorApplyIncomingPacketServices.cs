using System;

/// <summary>
/// Manor / crop UI apply hub (#142 batch slice 161; world hub slice 177).
/// </summary>
public static class ManorApplyIncomingPacketServices
{
    public static SeedInfoWindow SeedInfo => LocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => LocatorApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);
}
