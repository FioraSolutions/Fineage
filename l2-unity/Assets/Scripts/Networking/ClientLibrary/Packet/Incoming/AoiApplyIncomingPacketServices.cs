using System;

/// <summary>
/// NPC/pet AOI knownlist apply hub (#142 batch slice 164).
/// </summary>
public static class AoiApplyIncomingPacketServices
{
    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => IncomingPacketLocatorApplyIncomingPacketServices.Domain;

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
