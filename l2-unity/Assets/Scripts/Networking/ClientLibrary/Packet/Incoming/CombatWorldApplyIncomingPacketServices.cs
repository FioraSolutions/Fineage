using System;

/// <summary>
/// Combat world dispatch hub for wire apply facades (#142 batch slice 171).
/// </summary>
public static class CombatWorldApplyIncomingPacketServices
{
    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);
}
