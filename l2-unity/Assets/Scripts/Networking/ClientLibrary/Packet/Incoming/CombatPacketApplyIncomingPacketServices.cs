using System;

/// <summary>
/// Combat apply relay in Incoming namespace (#142 batch slice 165; queue relay slice 166).
/// </summary>
public static class CombatPacketApplyIncomingPacketServices
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
