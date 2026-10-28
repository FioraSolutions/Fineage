using System;

/// <summary>
/// World packet-apply dispatch hub (#142 batch slice 164; world session orchestration slice 205).
/// </summary>
public static class WorldEffectApplyIncomingPacketServices
{
    public static World GameWorld => PresenceApplyIncomingPacketServices.GameWorld;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.ApplyWorld(apply);

    public static void Queue(Action action) => PresenceApplyIncomingPacketServices.Queue(action);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        PresenceApplyIncomingPacketServices.QueueWorld(apply);
}
