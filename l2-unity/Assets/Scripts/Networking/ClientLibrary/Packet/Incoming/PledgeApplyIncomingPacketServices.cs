using System;

/// <summary>
/// Clan / pledge apply hub (#142 batch slice 160).
/// </summary>
public static class PledgeApplyIncomingPacketServices
{
    public static ClanWindow Clan => IncomingPacketLocatorApplyIncomingPacketServices.Clan;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
