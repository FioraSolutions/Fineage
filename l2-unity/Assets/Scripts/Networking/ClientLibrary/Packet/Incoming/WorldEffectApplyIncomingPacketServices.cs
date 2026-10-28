using System;

/// <summary>
/// World packet-apply dispatch hub (#142 batch slice 164).
/// </summary>
public static class WorldEffectApplyIncomingPacketServices
{
    public static World GameWorld => IncomingPacketLocatorApplyIncomingPacketServices.GameWorld;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);
}
