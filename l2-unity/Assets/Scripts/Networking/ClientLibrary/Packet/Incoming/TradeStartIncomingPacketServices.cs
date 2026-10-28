using System;

/// <summary>
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class TradeStartIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplyTradeStart(TradeStartDto packet) =>
        Queue(() =>
        {
            GameIncomingPacketServices.Trade.AddData(packet);
            GameIncomingPacketServices.Trade.ShowWindow();
        });
}
