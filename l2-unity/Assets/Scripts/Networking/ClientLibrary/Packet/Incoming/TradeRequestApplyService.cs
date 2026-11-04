using System;

/// <summary>
/// Trade request window and ground-item removal applies, constructed by the container
/// instead of reached through static facades (#142 slice 3).
/// </summary>
public sealed class TradeRequestApplyService
{
    public void ApplySendTradeRequest(SendTradeRequestDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            LocatorApplyIncomingPacketServices.TradeRequest.AddData(packet);
            LocatorApplyIncomingPacketServices.TradeRequest.ShowWindow();
        });
}
