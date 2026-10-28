using System;

/// <summary>
/// Combat incoming packet handler access via session hub (#142 sub-slices 48, 77, 78; session slice 180).
/// </summary>
public static class CombatApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => CombatPacketApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => CombatPacketApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatPacketApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatPacketApplyIncomingPacketServices.Queue(action);
}
