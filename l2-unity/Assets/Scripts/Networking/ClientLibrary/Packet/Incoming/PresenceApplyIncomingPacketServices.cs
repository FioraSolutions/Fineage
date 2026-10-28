using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163; presence session orchestration slice 197).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static GameClient Game => WorldSessionApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldSessionApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldSessionApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueWorld(apply);
}
