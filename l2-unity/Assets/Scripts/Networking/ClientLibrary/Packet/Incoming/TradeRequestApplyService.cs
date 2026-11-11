using VContainer;
using System.Collections.Generic;

/// <summary>
/// Trade request window apply, constructed by the container instead of reached through the
/// static facade (#142 slice 3). <see cref="TradeRequestWindow"/> arrives through
/// <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class TradeRequestApplyService
{
    private readonly ILateBound<TradeRequestWindow> _tradeRequest;

    [Inject]
    public TradeRequestApplyService(ILateBound<TradeRequestWindow> tradeRequest)
    {
        _tradeRequest = tradeRequest;
    }

    public void ApplySendTradeRequest(SendTradeRequestDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            TradeRequestWindow window = _tradeRequest != null ? _tradeRequest.Value : null;
            if (window == null)
                return;

            window.AddData(packet);
            window.ShowWindow();
        });
}
