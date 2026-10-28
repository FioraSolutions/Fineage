using System;

/// <summary>
/// Manor / crop UI apply hub (#142 batch slice 161; locator slice 169).
/// </summary>
public static class ManorApplyIncomingPacketServices
{
    public static SeedInfoWindow SeedInfo => IncomingPacketLocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => IncomingPacketLocatorApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);
}
