using System;

/// <summary>
/// Player session apply hub (#142 batch slice 164; compat pass-through slice 180).
/// </summary>
public static class PlayerApplyIncomingPacketServices
{
    public static GameClient Game => WorldSessionApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldSessionApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);
}
