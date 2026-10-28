using System;

/// <summary>
/// Combat incoming packet handler access via session hub (#142 sub-slices 48, 77, 78; slice 172).
/// </summary>
public static class CombatApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatSessionApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => CombatSessionApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => CombatSessionApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatSessionApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatSessionApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatSessionApplyIncomingPacketServices.Queue(action);
}
