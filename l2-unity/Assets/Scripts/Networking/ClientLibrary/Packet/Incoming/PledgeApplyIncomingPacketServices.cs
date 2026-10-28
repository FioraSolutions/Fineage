using System;

/// <summary>
/// Clan / pledge apply hub (#142 batch slice 160; pledge orchestration slice 185).
/// </summary>
public static class PledgeApplyIncomingPacketServices
{
    public static ClanWindow Clan => LocatorApplyIncomingPacketServices.Clan;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
