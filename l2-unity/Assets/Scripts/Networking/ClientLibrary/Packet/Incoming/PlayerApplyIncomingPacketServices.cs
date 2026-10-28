using System;

/// <summary>
/// Player session apply hub (#142 batch slice 164; world relay slice 236).
/// </summary>
public static class PlayerApplyIncomingPacketServices
{
    public static GameClient Game => PresenceApplyIncomingPacketServices.Game;

    public static World GameWorld => PresenceApplyIncomingPacketServices.GameWorld;

    public static IWorldDomainSink Domain => PresenceApplyIncomingPacketServices.Domain;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) => PresenceApplyIncomingPacketServices.Queue(action);

    public static void QueueDomain(Action<IWorldDomainSink> apply) =>
        PresenceApplyIncomingPacketServices.QueueDomain(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.QueueWorld(apply);
}
