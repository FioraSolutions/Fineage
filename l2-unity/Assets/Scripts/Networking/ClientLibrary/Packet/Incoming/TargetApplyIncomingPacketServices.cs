using System;

/// <summary>
/// Target selection apply hub (#142 batch slice 163; combat packet hub slice 167).
/// </summary>
public static class TargetApplyIncomingPacketServices
{
    public static TargetManager Targets => CombatPacketApplyIncomingPacketServices.Targets;

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply);
}
