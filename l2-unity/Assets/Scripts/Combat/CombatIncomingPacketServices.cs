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

    public static void ApplyAttack(AttackDto packet) =>
        ApplyWorld(apply => apply.Attack(packet));

    public static void ApplyAutoAttackStart(AutoAttackStartDto packet) =>
        ApplyWorld(apply => apply.AutoAttackStart(packet));

    public static void ApplyAutoAttackStop(AutoAttackStopDto packet) =>
        ApplyWorld(apply => apply.AutoAttackStop(packet));

    public static void ApplyDie(DieDto packet) =>
        ApplyWorld(apply => apply.Die(packet));

    public static void ApplyMagicSkillCanceled(MagicSkillCanceledDto packet) =>
        ApplyWorld(apply => apply.MagicSkillCanceled(packet));

    public static void ApplyMagicSkillLaunched(MagicSkillLaunchedDto packet)
    {
        if (packet == null)
            return;
        QueueWorld(apply => apply.MagicSkillLaunched(packet));
    }

    public static void ApplyMagicSkillUse(MagicSkillUseDto packet)
    {
        // Temporary: skill cast/VFX hides CharInfo spawn cost in packetApply.txt.
        // Packet is still read, so the stream stays aligned. Restore the queue below to turn it back on.
        if (packet == null)
            return;
        QueueWorld(apply => apply.MagicSkillUse(packet));
    }

    public static void ApplyMyTargetSelected(MyTargetSelectDto packet)
    {
        Queue(() =>
        {
            if (packet.ObjectId == PlayerEntity.Instance.Identity.Id)
            {
                Targets.SetTarget(new ObjectData(PlayerEntity.Instance.transform.gameObject), "#ffffff");
            }
            else
            {
                Targets.NextTargetById(packet.ObjectId, packet.Color);
                ApplyWorld(apply => apply.SendArrivedPosition());
            }
        });
    }

    public static void ApplyChooseInventoryItem(ChooseInventoryItemDto packet) =>
        Queue(() => Enchant.ShowWindow(packet.Item));

    public static void ApplyEnchantResult(EnchantResultDto packet) =>
        Queue(() => Enchant.EnchantResult(packet.Result));

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
        MovementIncomingPacketServices.ApplyPawn(packet);

    public static void ApplyStopMove(StopMoveDto packet) =>
        MovementIncomingPacketServices.ApplyStop(packet);

    public static void ApplySetupGauge(SetupGaugeDto packet) =>
        SetupGaugeIncomingPacketServices.ApplySetupGauge(packet);

    public static void ApplyActionFailed(ActionFailedDto packet) =>
        ActionFailedIncomingPacketServices.ApplyActionFailed(packet);
}
