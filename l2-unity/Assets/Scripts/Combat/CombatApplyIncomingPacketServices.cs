using System;

/// <summary>
/// Combat incoming packet handler access via domain facades and IncomingPacketActions fallback (#142 sub-slices 48, 77, 78).
/// </summary>
public static class CombatApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => IncomingPacketLocatorApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => IncomingPacketLocatorApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => IncomingPacketLocatorApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        WorldApplyPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        WorldApplyPacketServices.QueueWorld(apply);

    public static void Queue(Action action) =>
        WorldApplyPacketServices.Queue(action);
}
