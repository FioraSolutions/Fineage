using System;

/// <summary>
/// Dealer list wire apply hub (#142 batch slice 167; game hub slice 180).
/// </summary>
public static class MerchantListApplyIncomingPacketServices
{
    public static DealerWindow Dealer => GameApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);
}
