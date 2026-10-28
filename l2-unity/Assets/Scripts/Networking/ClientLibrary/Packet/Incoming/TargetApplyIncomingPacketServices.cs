using System;

/// <summary>
/// Target selection apply hub (#142 batch slice 163; combat world slice 175).
/// </summary>
public static class TargetApplyIncomingPacketServices
{
    public static TargetManager Targets => LocatorApplyIncomingPacketServices.Targets;

    public static void Queue(Action action) =>
        CombatWorldApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatWorldApplyIncomingPacketServices.ApplyWorld(apply);
}
