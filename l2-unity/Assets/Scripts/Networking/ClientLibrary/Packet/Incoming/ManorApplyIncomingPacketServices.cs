using System;

/// <summary>
/// Manor / seed UI apply hub (#142 batch slice 161).
/// </summary>
public static class ManorApplyIncomingPacketServices
{
    public static SeedInfoWindow SeedInfo => IncomingPacketLocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => IncomingPacketLocatorApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
