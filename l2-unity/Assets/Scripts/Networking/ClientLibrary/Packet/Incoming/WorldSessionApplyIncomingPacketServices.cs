using System;

/// <summary>
/// Core world session locator + queue hub (#142 batch slice 171).
/// </summary>
public static class WorldSessionApplyIncomingPacketServices
{
    public static GameClient Game => IncomingPacketLocatorApplyIncomingPacketServices.Game;

    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        QueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.QueueWorld(apply);
}
