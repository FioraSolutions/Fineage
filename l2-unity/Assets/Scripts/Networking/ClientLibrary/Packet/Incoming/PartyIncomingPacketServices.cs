using System;

/// <summary>
/// Party invitation wire packet handler access via UiIncomingPacketServices (#142 sub-slice 74).
/// </summary>
public static class PartyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyAskJoinParty(AskJoinPartyDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.PartyInvite.AddData(packet);
            UiIncomingPacketServices.PartyInvite.ShowWindow();
        });
}
