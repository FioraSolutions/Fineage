using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="TradeStartIncomingPacketServices"/>.
/// 
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeStartApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);

    public static void ApplyTradeStart(TradeStartDto packet) =>
        Queue(() =>
        {
            GameApplyIncomingPacketServices.Trade.AddData(packet);
            GameApplyIncomingPacketServices.Trade.ShowWindow();
        });
}
