using System;

/// <summary>
/// Party invitation wire packet handler access via UiIncomingPacketServices (#142 sub-slice 74).
/// Ask-join-party on <see cref="AskJoinPartyIncomingPacketServices"/> (sub-slice 137).
/// </summary>
public static class PartyIncomingPacketServices
{
    public static void Queue(Action action) => UiIncomingPacketServices.Queue(action);
}
