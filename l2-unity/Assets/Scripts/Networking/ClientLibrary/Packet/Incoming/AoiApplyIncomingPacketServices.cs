using System;

/// <summary>
/// World AOI apply hub (#142 batch slice 163; world orchestration slice 180).
/// </summary>
public static class AoiApplyIncomingPacketServices
{
    public static World GameWorld => PresenceApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => PresenceApplyIncomingPacketServices.Domain;

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PresenceApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.QueueWorld(apply);
}
