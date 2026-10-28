using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="TradeUpdateIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeUpdateApplyIncomingPacketServices
{
    public static TradeWindow Trade => IncomingPacketServices.Trade;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyUpdate(TradeUpdateIncomingDto dto)
    {
    }
}
