using System;

/// <summary>
/// Target selection apply hub (#142 batch slice 163; locator slice 171).
/// </summary>
public static class TargetApplyIncomingPacketServices
{
    public static TargetManager Targets => IncomingPacketLocatorApplyIncomingPacketServices.Targets;

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);
}
