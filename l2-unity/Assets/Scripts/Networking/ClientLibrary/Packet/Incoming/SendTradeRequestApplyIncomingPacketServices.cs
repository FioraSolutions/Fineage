using System;

/// <summary>
/// Trade apply facade (#142 batch slice 150). Re-homes from <see cref="SendTradeRequestIncomingPacketServices"/>.
/// 
/// Trade wire packet handler entry (#142 batch slice 142).
/// </summary>
public static class SendTradeRequestApplyIncomingPacketServices
{
    public static void Queue(Action action) => GameIncomingPacketServices.Queue(action);

    public static void ApplySendTradeRequest(SendTradeRequestDto packet) =>
        Queue(() =>
        {
            GameIncomingPacketServices.TradeRequest.AddData(packet);
            GameIncomingPacketServices.TradeRequest.ShowWindow();
        });
}
