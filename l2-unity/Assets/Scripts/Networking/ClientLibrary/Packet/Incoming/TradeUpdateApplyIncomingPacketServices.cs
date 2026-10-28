using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="TradeUpdateIncomingPacketServices"/>.
/// 
/// Trade session wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeUpdateApplyIncomingPacketServices
{
    public static TradeWindow Trade => GameApplyIncomingPacketServices.Trade;

    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyUpdate(TradeUpdateIncomingDto dto)
    {
    }
}
