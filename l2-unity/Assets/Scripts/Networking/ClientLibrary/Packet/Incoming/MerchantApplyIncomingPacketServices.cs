using System;

/// <summary>
/// Merchant / dealer window apply hub (#142 batch slice 160).
/// </summary>
public static class MerchantApplyIncomingPacketServices
{
    public static DealerWindow Dealer => GameApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
