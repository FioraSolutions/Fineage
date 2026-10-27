using System;

/// <summary>
/// Combat incoming packet handler access via domain facades and IncomingPacketActions fallback (#142 sub-slice 48).
/// </summary>
public static class CombatIncomingPacketServices
{
    public static PlayerInventory Inventory => InventoryPacketServices.Inventory;

    public static EnchantWindow Enchant => ItemEnchantPacketServices.Enchant;

    public static TargetManager Targets => CombatPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyPacketServices.QueueWorld(apply);

    public static void Queue(Action action) =>
        WorldApplyPacketServices.Queue(action);
}
