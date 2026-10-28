using System;

/// <summary>
/// Party invitation apply hub (#142 batch slice 161).
/// </summary>
public static class PartyApplyIncomingPacketServices
{
    public static PartyInvitationWindow PartyInvite => IncomingPacketLocatorApplyIncomingPacketServices.PartyInvite;

    public static void Queue(Action action) => IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
