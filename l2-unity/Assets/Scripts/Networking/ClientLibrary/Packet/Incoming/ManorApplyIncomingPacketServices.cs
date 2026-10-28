using System;

/// <summary>
/// Manor / crop UI apply hub (#142 batch slice 161; manor orchestration slice 185).
/// </summary>
public static class ManorApplyIncomingPacketServices
{
    public static SeedInfoWindow SeedInfo => LocatorApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => LocatorApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);
}
