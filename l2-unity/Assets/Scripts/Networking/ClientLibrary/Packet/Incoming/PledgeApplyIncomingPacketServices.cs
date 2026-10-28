using System;

/// <summary>
/// Clan / pledge apply hub (#142 batch slice 160; game hub slice 176).
/// </summary>
public static class PledgeApplyIncomingPacketServices
{
    public static ClanWindow Clan => LocatorApplyIncomingPacketServices.Clan;

    public static void Queue(Action action) =>
        GameApplyIncomingPacketServices.Queue(action);
}
