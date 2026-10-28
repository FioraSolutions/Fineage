using System;
using UnityEngine;

/// <summary>
/// Combat incoming packet handler access via domain facades and IncomingPacketActions fallback (#142 sub-slices 48, 77, 78).
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

    public static void ApplyInventoryUpdate(InventoryUpdateDto packet) =>
        Inventory.UpdateInventory(packet.Items, packet.EquipItems);

    public static void ApplyItemList(ItemListDto packet) =>
        Inventory.SetInventory(
            packet.Items,
            packet.EquipItems,
            packet.ShowWindow,
            packet.AdenaCount,
            packet.Items.Count + packet.EquipItems.Count);

    public static void ApplyMoveToPawn(MoveToPawnDto packet) =>
        MoveToPawnIncomingPacketServices.ApplyPawn(packet);

    public static void ApplyStopMove(StopMoveDto packet) =>
        StopMoveIncomingPacketServices.ApplyStop(packet);

    public static void ApplySetupGauge(SetupGaugeDto packet) =>
        SetupGaugeIncomingPacketServices.ApplySetupGauge(packet);

    public static void ApplyActionFailed(ActionFailedDto packet) =>
        ActionFailedIncomingPacketServices.ApplyActionFailed(packet);
}
