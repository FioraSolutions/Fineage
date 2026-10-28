using System;

/// <summary>
/// Combat world dispatch hub for wire apply facades (#142 batch slice 171; CombatPacket relay slice 223).
/// </summary>
public static class CombatWorldApplyIncomingPacketServices
{
    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatSessionApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatSessionApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatSessionApplyIncomingPacketServices.Queue(action);
}
