using System;

/// <summary>
/// Ask-join-party wire packet handler entry (#142 batch slice 148).
/// Splits party invite apply out of <see cref="PartyIncomingPacketServices"/> (sub-slice 74 encapsulation).
/// </summary>
public static class AskJoinPartyApplyIncomingPacketServices
{
    public static void Queue(Action action) => UiApplyIncomingPacketServices.Queue(action);

    public static void ApplyAskJoinParty(AskJoinPartyDto packet) =>
        Queue(() =>
        {
            UiApplyIncomingPacketServices.PartyInvite.AddData(packet);
            UiApplyIncomingPacketServices.PartyInvite.ShowWindow();
        });
}
