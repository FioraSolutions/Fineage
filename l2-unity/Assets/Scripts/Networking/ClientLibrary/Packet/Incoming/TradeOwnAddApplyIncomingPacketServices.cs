using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="TradeOwnAddIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeOwnAddApplyIncomingPacketServices
{
    public static TradeWindow Trade => TradeApplyIncomingPacketServices.Trade;

    public static void Queue(Action action) => TradeApplyIncomingPacketServices.Queue(action);

    public static void ApplyOwnAdd(TradeOwnAddIncomingDto dto)
    {
    }
}
