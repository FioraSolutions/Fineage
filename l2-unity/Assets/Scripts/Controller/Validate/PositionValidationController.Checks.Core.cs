using System;
using UnityEngine;
using System.Collections.Generic;
using VContainer;

public partial class PositionValidationController
{
    private void ReStartAnimationPlayer(PlayerStateMachine stateMachine)
    {
        if (Player != null && Player.RunningToDestination)
        {
            if (stateMachine.State == PlayerState.WALKING) stateMachine.ChangeState(PlayerState.WALKING);
            if (stateMachine.State == PlayerState.RUNNING) stateMachine.ChangeState(PlayerState.RUNNING);
            stateMachine.NotifyEvent(Event.MOVE_TO);
        }
    }

    private void ReplayMoveIfNeeded(Entity entity)
    {
        if (entity == null || entity.Identity == null || ControllerPacketServices.Moves == null)
            return;
        if (!ControllerPacketServices.Moves.IsMoving(entity.Identity.Id))
            return;
        EntityActionVisual.StartMove(entity, !entity.Running);
    }

    private void StartWalk(Entity entity , Vector3 newPosition)
    {
        if (entity.GetType() == typeof(MonsterEntity))
        {
            MonsterEntity monsterEntity = (MonsterEntity)entity;
            if (monsterEntity.ActionSlot.Action == EntityActionKind.Move)
                return;
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.Set(monsterEntity, EntityActionKind.Move, newPosition);

        }else if (entity.GetType() == typeof(NpcEntity))
        {
            NpcEntity npcEntity = (NpcEntity)entity;
            if (npcEntity.ActionSlot.Action == EntityActionKind.Move)
                return;
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.Set(npcEntity, EntityActionKind.Move, newPosition);
        }
        else if (entity is UserEntity)
        {
            // EntityActionCombatLog.LogCiPawn(entity,
                // "VL StartWalk UserEntity NO_HANDLER nick=" + EntityActionCombatLog.NameOf(entity) +
                // " dest=" + EntityActionCombatLog.Vec(newPosition));
        }

    }
}
