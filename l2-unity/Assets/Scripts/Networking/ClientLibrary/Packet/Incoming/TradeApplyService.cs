using System;

/// <summary>
/// Trade window apply set (start / confirm / offer / done), constructed by the container
/// instead of reached through static facades (#142 slice 3).
/// </summary>
public sealed class TradeApplyService
{
    public void ApplyStart(TradeStartDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            LocatorApplyIncomingPacketServices.Trade.AddData(packet);
            string partner = packet.PartnerName;
            if (!string.IsNullOrEmpty(partner))
                LocatorApplyIncomingPacketServices.Trade.ShowWindow(partner);
            else
                LocatorApplyIncomingPacketServices.Trade.ShowWindow("Partner");
        });

    public void ApplyPressOwnOk(bool confirmed) =>
        QueueApplyIncomingPacketServices.Queue(
            () => LocatorApplyIncomingPacketServices.Trade?.SetOwnConfirmState(confirmed));

    public void ApplyPressOtherOk(bool confirmed) =>
        QueueApplyIncomingPacketServices.Queue(
            () => LocatorApplyIncomingPacketServices.Trade?.SetPartnerConfirmState(confirmed));

    public void ApplyOwnAdd(ItemInstance item) =>
        QueueApplyIncomingPacketServices.Queue(
            () => LocatorApplyIncomingPacketServices.Trade?.ApplyOwnOfferItem(item));

    public void ApplyOtherAdd(ItemInstance item) =>
        QueueApplyIncomingPacketServices.Queue(
            () => LocatorApplyIncomingPacketServices.Trade?.ApplyPartnerOfferItem(item));

    public void ApplyDone() => LocatorApplyIncomingPacketServices.Trade.HideWindow();
}
