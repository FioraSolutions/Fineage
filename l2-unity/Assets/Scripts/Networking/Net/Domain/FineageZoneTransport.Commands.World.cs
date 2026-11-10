using Fineage.Net.Contract;
using UnityEngine;

public sealed partial class FineageZoneTransport
{
    bool TrySendWorldCommands(INetworkCommand command)
    {
        MoveToCommand netMove = command as MoveToCommand;
        if (netMove != null)
        {
            SendMove(netMove.From, netMove.To);
            return true;
        }

        ClickActionCommand netAction = command as ClickActionCommand;
        if (netAction != null)
        {
            World world = WorldApplyIncomingPacketServices.GameWorld;
            if (world != null)
            {
                Entity clicked = world.GetEntityNoLockSync(netAction.ObjectId);
                if (clicked is ItemEntity)
                {
                    Session.SendPickupRequest(netAction.ObjectId);
                    return true;
                }

                TargetManager attackTargets = ControllerPacketServices.Targets;
                bool forceAttack = GameCtrl.IsHeld ||
                    (attackTargets != null && attackTargets.IsAttackTargetSet());
                if (EntityInteraction.ShouldTalkOnClick(clicked, forceAttack))
                {
                    Session.SendSelectTarget(netAction.ObjectId);
                    if (NetNpcWindows.InRange(clicked))
                        Session.SendNpcTalk(netAction.ObjectId);
                    else
                        NetChatFeed.NoteSystem("You are too far away.");
                    return true;
                }
            }

            Session.SendSelectTarget(netAction.ObjectId);
            TargetManager targets = ControllerPacketServices.Targets;
            if (GameCtrl.IsHeld || (targets != null && targets.IsAttackTargetSet()))
                Session.SendAttackRequest(netAction.ObjectId);
            return true;
        }

        ZonePickupRequestCommand netPickup = command as ZonePickupRequestCommand;
        if (netPickup != null)
        {
            Session.SendPickupRequest(netPickup.GroundObjectId);
            return true;
        }

        RequestDropItemCommand netDrop = command as RequestDropItemCommand;
        if (netDrop != null)
        {
            Session.SendDropItemRequest(
                netDrop.ObjectId, netDrop.Count, netDrop.X, netDrop.Y, netDrop.Z);
            return true;
        }

        // Mobius RequestDestroyItem: the trash basket. It shares nothing with a drop, so it needs its own opcode.
        RequestDestroyItemCommand netDestroy = command as RequestDestroyItemCommand;
        if (netDestroy != null)
        {
            Session.SendDestroyItemRequest(netDestroy.ObjectId, netDestroy.Count);
            return true;
        }

        // Mobius enchant window: the scroll click and the roll travel as their own opcodes.
        RequestEnchantItemCommand netEnchant = command as RequestEnchantItemCommand;
        if (netEnchant != null)
        {
            if (netEnchant.ObjectId > 0)
                Session.SendEnchantRequest(netEnchant.ObjectId);
            else
                Session.SendEnchantCancel();
            return true;
        }

        UseItemCommand netUse = command as UseItemCommand;
        if (netUse != null)
        {
            Session.SendUseItem(netUse.ObjectId);
            return true;
        }

        // Mobius RequestAutoSoulShot: the hotbar right-click toggle for soulshots / spiritshots.
        RequestAutoSoulShotCommand netAutoShot = command as RequestAutoSoulShotCommand;
        if (netAutoShot != null)
        {
            if (netAutoShot.ItemId > 0)
                Session.SendRequestAutoSoulShot(netAutoShot.ItemId, netAutoShot.Enabled);
            return true;
        }

        UnequipItemCommand netUnequip = command as UnequipItemCommand;
        if (netUnequip != null)
        {
            Session.SendUnequipItem(netUnequip.PaperdollSlot);
            return true;
        }

        RequestShortCutRegCommand netShortCutReg = command as RequestShortCutRegCommand;
        if (netShortCutReg != null)
        {
            Session.SendShortCutReg(
                netShortCutReg.TypeId, netShortCutReg.WorldSlot, netShortCutReg.Id, netShortCutReg.Level);
            return true;
        }

        RequestShortCutDelCommand netShortCutDel = command as RequestShortCutDelCommand;
        if (netShortCutDel != null)
        {
            Session.SendShortCutDel(netShortCutDel.WorldSlot);
            return true;
        }

        ValidatePositionCommand netValidate = command as ValidatePositionCommand;
        if (netValidate != null)
        {
            Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(new Vector3(netValidate.X, netValidate.Y, netValidate.Z));
            Session.SendValidatePosition(
                Mathf.RoundToInt(l2.x),
                Mathf.RoundToInt(l2.y),
                Mathf.RoundToInt(l2.z),
                netValidate.Heading);
            return true;
        }

        return false;
    }}

}
