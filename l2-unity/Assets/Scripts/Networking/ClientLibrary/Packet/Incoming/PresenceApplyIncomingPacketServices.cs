using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
