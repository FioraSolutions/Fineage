using System;

/// <summary>
/// Merchant / dealer window apply hub (#142 batch slice 160; game hub slice 166).
/// </summary>
public static class MerchantApplyIncomingPacketServices
{
    public static DealerWindow Dealer => GameApplyIncomingPacketServices.Dealer;

    public static void Queue(Action action) => GameApplyIncomingPacketServices.Queue(action);
}
