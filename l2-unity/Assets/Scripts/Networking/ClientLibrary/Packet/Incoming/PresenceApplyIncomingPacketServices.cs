using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163; world orchestration slice 180; presence wire slice 184).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.QueueWorld(apply);
}
