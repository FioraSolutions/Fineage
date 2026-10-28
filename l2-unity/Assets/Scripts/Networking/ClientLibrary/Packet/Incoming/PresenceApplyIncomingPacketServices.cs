using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163; world apply orchestration slice 213).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static GameClient Game => WorldApplyIncomingPacketServices.Game;

    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => WorldApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        WorldApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.QueueWorld(apply);
}
