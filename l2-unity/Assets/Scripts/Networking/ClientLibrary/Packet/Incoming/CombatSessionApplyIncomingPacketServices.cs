using System;

/// <summary>
/// Combat session orchestration hub (#142 batch slice 172; combat leaf hub slice 211).
/// </summary>
public static class CombatSessionApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => EnchantApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => TargetApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);
}
