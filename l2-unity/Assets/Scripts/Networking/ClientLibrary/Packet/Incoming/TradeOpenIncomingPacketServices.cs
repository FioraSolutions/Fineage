using System;

/// <summary>
/// Trade open / close wire packet handler access via GameIncomingPacketServices (#142 sub-slice 63).
/// </summary>
public static class TradeOpenIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplySendTradeRequest(SendTradeRequestDto packet) =>
        Queue(() =>
        {
            GameIncomingPacketServices.TradeRequest.AddData(packet);
            GameIncomingPacketServices.TradeRequest.ShowWindow();
        });

    public static void ApplyTradeStart(TradeStartDto packet) =>
        Queue(() =>
        {
            GameIncomingPacketServices.Trade.AddData(packet);
            GameIncomingPacketServices.Trade.ShowWindow();
        });

    public static void ApplyTradeDone() => GameIncomingPacketServices.Trade.HideWindow();
}
