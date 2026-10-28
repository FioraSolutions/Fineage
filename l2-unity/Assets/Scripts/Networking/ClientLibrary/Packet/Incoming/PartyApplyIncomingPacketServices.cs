using System;

/// <summary>
/// Party invitation apply hub (#142 batch slice 161; UI hub slice 167).
/// </summary>
public static class PartyApplyIncomingPacketServices
{
    public static PartyInvitationWindow PartyInvite => UiApplyIncomingPacketServices.PartyInvite;

    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);
}
