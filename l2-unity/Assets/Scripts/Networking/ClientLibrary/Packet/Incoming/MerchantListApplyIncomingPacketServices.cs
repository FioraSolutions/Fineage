using System;

/// <summary>
/// Dealer list wire apply hub (#142 batch slice 167).
/// </summary>
public static class MerchantListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => MerchantApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => MerchantApplyIncomingPacketServices.Queue(action);
}
