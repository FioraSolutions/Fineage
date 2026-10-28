using System;

/// <summary>
/// Combat world dispatch hub for wire apply facades (#142 batch slice 171; session leaf slice 179).
/// </summary>
public static class CombatWorldApplyIncomingPacketServices
{
    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => QueueApplyIncomingPacketServices.Queue(action);
}
