using System;

/// <summary>
/// Combat apply relay in Incoming namespace (#142 batch slice 165).
/// Delegates to <see cref="CombatApplyIncomingPacketServices"/>.
/// </summary>
public static class CombatPacketApplyIncomingPacketServices
{
    public static PlayerInventory Inventory => CombatApplyIncomingPacketServices.Inventory;

    public static EnchantWindow Enchant => CombatApplyIncomingPacketServices.Enchant;

    public static TargetManager Targets => CombatApplyIncomingPacketServices.Targets;

    public static void ApplyWorld(Action<WorldPacketApply> apply) =>
        CombatApplyIncomingPacketServices.ApplyWorld(apply);

    public static void QueueWorld(Action<WorldPacketApply> apply) =>
        CombatApplyIncomingPacketServices.QueueWorld(apply);

    public static void Queue(Action action) => CombatApplyIncomingPacketServices.Queue(action);
}
