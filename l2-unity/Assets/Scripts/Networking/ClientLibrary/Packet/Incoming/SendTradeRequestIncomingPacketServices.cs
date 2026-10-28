using System;

/// <summary>
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class SendTradeRequestIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplySendTradeRequest(SendTradeRequestDto packet) =>
        Queue(() =>
        {
            GameIncomingPacketServices.TradeRequest.AddData(packet);
            GameIncomingPacketServices.TradeRequest.ShowWindow();
        });
}
