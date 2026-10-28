using System;

/// <summary>
/// World AOI apply hub (#142 batch slice 163; world orchestration slice 180).
/// </summary>
public static class AoiApplyIncomingPacketServices
{
    public static World GameWorld => PlayerApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => PlayerApplyIncomingPacketServices.Domain;

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PlayerApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PlayerApplyIncomingPacketServices.QueueWorld(apply);
}
