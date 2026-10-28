using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="TradeOtherAddIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeOtherAddApplyIncomingPacketServices
{
    public static TradeWindow Trade => IncomingPacketServices.Trade;

    public static void Queue(Action action) => IncomingPacketServices.Queue(action);

    public static void ApplyOtherAdd(TradeOtherAddIncomingDto dto)
    {
    }
}
