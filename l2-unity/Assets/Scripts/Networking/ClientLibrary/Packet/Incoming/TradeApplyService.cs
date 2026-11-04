using VContainer;

/// <summary>
/// Trade window apply set (start / confirm / offer / done), constructed by the container
/// instead of reached through static facades (#142 slice 3). <see cref="TradeWindow"/>
/// arrives through <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class TradeApplyService
{
    private readonly ILateBound<TradeWindow> _trade;

    [Inject]
    public TradeApplyService(ILateBound<TradeWindow> trade)
    {
        _trade = trade;
    }

    public TradeWindow Trade => _trade != null ? _trade.Value : null;

    public void ApplyStart(TradeStartDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            TradeWindow trade = Trade;
            if (trade == null)
                return;

            trade.AddData(packet);
            string partner = packet.PartnerName;
            if (!string.IsNullOrEmpty(partner))
                trade.ShowWindow(partner);
            else
                trade.ShowWindow("Partner");
        });

    public void ApplyPressOwnOk(bool confirmed) =>
        QueueApplyIncomingPacketServices.Queue(
            () => Trade?.SetOwnConfirmState(confirmed));

    public void ApplyPressOtherOk(bool confirmed) =>
        QueueApplyIncomingPacketServices.Queue(
            () => Trade?.SetPartnerConfirmState(confirmed));

    public void ApplyOwnAdd(ItemInstance item) =>
        QueueApplyIncomingPacketServices.Queue(
            () => Trade?.ApplyOwnOfferItem(item));

    public void ApplyOtherAdd(ItemInstance item) =>
        QueueApplyIncomingPacketServices.Queue(
            () => Trade?.ApplyPartnerOfferItem(item));

    public void ApplyDone()
    {
        TradeWindow trade = Trade;
        if (trade != null)
            trade.HideWindow();
    }
}
