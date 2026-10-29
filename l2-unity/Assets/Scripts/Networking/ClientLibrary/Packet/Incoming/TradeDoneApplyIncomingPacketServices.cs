using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150; locator queue slice 255). Re-homes from <see cref="TradeDoneIncomingPacketServices"/>.
/// 
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeDoneApplyIncomingPacketServices
{
    public static void ApplyTradeDone() => LocatorApplyIncomingPacketServices.Trade.HideWindow();
}
