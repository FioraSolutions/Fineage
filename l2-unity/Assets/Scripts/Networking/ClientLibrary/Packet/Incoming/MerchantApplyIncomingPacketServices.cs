using System;

/// <summary>
/// Merchant / dealer window apply hub (#142 batch slice 160; merchant orchestration slice 186).
/// </summary>
public static class MerchantApplyIncomingPacketServices
{
    public static DealerWindow Dealer => LocatorApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) =>
        GameApplyIncomingPacketServices.Queue(action);
}
