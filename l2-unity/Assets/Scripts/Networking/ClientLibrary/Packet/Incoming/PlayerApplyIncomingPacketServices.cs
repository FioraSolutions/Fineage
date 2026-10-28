using System;

/// <summary>
/// Local player / user-info apply hub (#142 batch slice 164).
/// </summary>
public static class PlayerApplyIncomingPacketServices
{
    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
