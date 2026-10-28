using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150; locator queue slice 255). Re-homes from <see cref="TradeStartIncomingPacketServices"/>.
/// 
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeStartApplyIncomingPacketServices
{
    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyTradeStart(TradeStartDto packet) =>
        Queue(() =>
        {
            LocatorApplyIncomingPacketServices.Trade.AddData(packet);
            LocatorApplyIncomingPacketServices.Trade.ShowWindow();
        });
}
