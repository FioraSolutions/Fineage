using System;

/// <summary>
/// Merchant / dealer window apply hub (#142 batch slice 160; locator slice 170).
/// </summary>
public static class MerchantApplyIncomingPacketServices
{
    public static DealerWindow Dealer => IncomingPacketLocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
