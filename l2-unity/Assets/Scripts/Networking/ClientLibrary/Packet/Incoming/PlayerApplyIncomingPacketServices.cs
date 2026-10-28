using System;

/// <summary>
/// Player session apply hub (#142 batch slice 164; compat pass-through slice 180).
/// </summary>
public static class PlayerApplyIncomingPacketServices
{
    public static GameClient Game => PresenceApplyIncomingPacketServices.Game;

    public static World GameWorld => PresenceApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => PresenceApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => PresenceApplyIncomingPacketServices.Queue(action);
}
