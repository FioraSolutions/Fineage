using System;

/// <summary>
/// Combat apply relay in Incoming namespace (#142 batch slice 165; orchestration slice 171).
/// </summary>
public static class CombatPacketApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => EnchantApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => TargetApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        QueueApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) =>
        QueueApplyIncomingPacketServices.Queue(action);
}
