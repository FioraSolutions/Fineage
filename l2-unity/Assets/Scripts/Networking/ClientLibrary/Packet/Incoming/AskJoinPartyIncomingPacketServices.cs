using System;

/// <summary>
/// Ask-join-party wire packet handler entry (#142 sub-slice 137).
/// Splits party invite apply out of <see cref="PartyIncomingPacketServices"/> (sub-slice 74 encapsulation).
/// </summary>
public static class AskJoinPartyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);

    public static void ApplyAskJoinParty(AskJoinPartyDto packet) =>
        Queue(() =>
        {
            UiIncomingPacketServices.PartyInvite.AddData(packet);
            UiIncomingPacketServices.PartyInvite.ShowWindow();
        });
}
