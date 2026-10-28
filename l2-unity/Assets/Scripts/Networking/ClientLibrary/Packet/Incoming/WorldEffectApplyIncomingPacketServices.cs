using System;

/// <summary>
/// World packet-apply dispatch hub (#142 batch slice 164; world hub slice 167).
/// </summary>
public static class WorldEffectApplyIncomingPacketServices
{
    public static World GameWorld => WorldApplyIncomingPacketServices.GameWorld;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) => WorldApplyIncomingPacketServices.Queue(action);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyIncomingPacketServices.QueueWorld(apply);
}
