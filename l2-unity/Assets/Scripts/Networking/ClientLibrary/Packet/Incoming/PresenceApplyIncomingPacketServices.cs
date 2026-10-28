using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163; PlayerApply orchestration slice 239).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static GameClient Game => PlayerApplyIncomingPacketServices.Game;

    public static World GameWorld => PlayerApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => PlayerApplyIncomingPacketServices.Domain;

    public static void Queue(Action action) => PlayerApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        PlayerApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PlayerApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PlayerApplyIncomingPacketServices.QueueWorld(apply);
}
