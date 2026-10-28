using System;

/// <summary>
/// Target selection apply hub (#142 batch slice 163; session hub slice 179).
/// </summary>
public static class TargetApplyIncomingPacketServices
{
    public static TargetManager Targets => LocatorApplyIncomingPacketServices.Targets;

    public static void Queue(Action action) =>
        CombatSessionApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatSessionApplyIncomingPacketServices.ApplyWorld(apply);
}
