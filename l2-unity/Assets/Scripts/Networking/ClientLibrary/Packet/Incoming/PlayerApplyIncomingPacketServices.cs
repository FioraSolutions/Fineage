using System;

/// <summary>
/// Player session apply hub (#142 batch slice 164; compat pass-through slice 180).
/// </summary>
public static class PlayerApplyIncomingPacketServices
{
    public static GameClient Game => WorldApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);
}
