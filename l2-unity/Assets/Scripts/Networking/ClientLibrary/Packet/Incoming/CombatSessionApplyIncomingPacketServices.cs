using System;

/// <summary>
/// Combat session orchestration hub (#142 batch slice 172; CombatPacket relay slice 221).
/// </summary>
public static class CombatSessionApplyIncomingPacketServices
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
