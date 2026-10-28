using System;

/// <summary>
/// Party invitation apply hub (#142 batch slice 161; UI hub slice 176).
/// </summary>
public static class PartyApplyIncomingPacketServices
{
    public static PartyInvitationWindow PartyInvite =>
        LocatorApplyIncomingPacketServices.PartyInvite;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
