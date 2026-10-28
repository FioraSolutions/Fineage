using System;

/// <summary>
/// Manor / crop UI apply hub (#142 batch slice 161; world hub slice 167).
/// </summary>
public static class ManorApplyIncomingPacketServices
{
    public static SeedInfoWindow SeedInfo => WorldApplyIncomingPacketServices.SeedInfo;

    public static SellCropListWindow SellCrop => WorldApplyIncomingPacketServices.SellCrop;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);
}
