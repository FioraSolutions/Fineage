using System;

/// <summary>
/// World AOI / ground item apply hub (#142 batch slice 163; world session slice 171).
/// </summary>
public static class PresenceApplyIncomingPacketServices
{
    public static World GameWorld => WorldSessionApplyIncomingPacketServices.GameWorld;

    public static void Queue(Action action) => WorldSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldSessionApplyIncomingPacketServices.QueueWorld(apply);
}
