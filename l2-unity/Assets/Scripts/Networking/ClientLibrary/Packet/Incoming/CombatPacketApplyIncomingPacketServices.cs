using System;

/// <summary>
/// Combat apply relay in Incoming namespace (#142 batch slice 165; combat wire orchestration slice 188).
/// </summary>
public static class CombatPacketApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => LocatorApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => LocatorApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => LocatorApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
