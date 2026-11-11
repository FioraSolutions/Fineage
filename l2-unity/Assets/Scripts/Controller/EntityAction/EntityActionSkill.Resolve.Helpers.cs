using System;
using System.Collections.Generic;
using UnityEngine;

public partial class EntityActionSkill : IEntityActionProcess
{
    public static bool IsLocalPlayer(int objectId)
    {
        return DomainPacketServices.LocalPlayer != null &&
            DomainPacketServices.LocalPlayer.Identity != null &&
            DomainPacketServices.LocalPlayer.Identity.Id == objectId;
    }

    public static void FinishRemoteCast(int objectId)
    {
        if (IsLocalPlayer(objectId))
            return;

        Entity entity = ResolveEntity(objectId);
        if (entity == null)
            return;

        CombatPacketServices.Facing?.EndFollow(objectId, "skill-smb-idle");
        UserEntity user = entity as UserEntity;
        if (user != null)
            user.ClearAttackVisual();

        bool moving = entity.Identity != null && ControllerPacketServices.Moves != null &&
            ControllerPacketServices.Moves.IsMoving(entity.Identity.Id);
        bool moveAction = entity.ActionSlot != null && entity.ActionSlot.Action == EntityActionKind.Move;

        MagicGaitTrace.BeginPostShotWatch(entity);
        MagicGaitTrace.Event(entity, "FinishRemoteCast",
            "moving=" + moving + " moveAction=" + moveAction +
            " → " + (moving || moveAction
                ? (moving ? "ReplayUserGait" : "KEEP action=Move")
                : "Set Idle"));

        if (moving || moveAction)
        {
            if (moving)
            {
                MovementData move = ControllerPacketServices.Moves.GetMove(entity.Identity.Id);
                if (move != null)
                    move.ReplayUserGait();
            }
            return;
        }

        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.Set(entity, EntityActionKind.Idle, null);
    }
}
