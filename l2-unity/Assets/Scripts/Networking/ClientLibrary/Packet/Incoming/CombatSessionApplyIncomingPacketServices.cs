using System;

/// <summary>
/// Combat session orchestration hub (#142 batch slice 172; wire hub slice 178).
/// </summary>
public static class CombatSessionApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatPacketApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => CombatPacketApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => CombatPacketApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatWorldApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatWorldApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatWorldApplyIncomingPacketServices.Queue(action);
}
