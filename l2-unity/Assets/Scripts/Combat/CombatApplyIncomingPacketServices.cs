using System;

/// <summary>
/// Combat incoming packet handler access via locator and queue relay (#142 sub-slices 48, 77, 78; slice 166).
/// </summary>
public static class CombatApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => IncomingPacketLocatorApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => IncomingPacketLocatorApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => IncomingPacketLocatorApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        IncomingPacketQueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) =>
        IncomingPacketQueueApplyIncomingPacketServices.Queue(action);
}
