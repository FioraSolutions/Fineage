using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class PositionValidationController : MonoBehaviour
{
    private void Jump(Entity entity, Vector3 newPosition)
    {
        if (entity.GetType() == typeof(MonsterEntity))
        {
            MonsterEntity monsterEntity = (MonsterEntity)entity;
            monsterEntity.HideObject();
            NewCalcGravityMonster(monsterEntity, newPosition);
            monsterEntity.ShowObject();
            
            ReplayMoveIfNeeded(monsterEntity);

        }else if (entity.GetType() == typeof(PlayerEntity))
        {
            int objectId = entity.Identity.Id;
            Dictionary<string, float> floatValues  = Animations.PlayerGetAllFloat(objectId);
            entity.HideObject();
            NewCalcGravity(Player, newPosition);
            entity.ShowObject();

            Animations.PlayerSetAllFloat(objectId , floatValues);
            ReStartAnimationPlayer(PlayerStatePacketServices.PlayerState);

        }else if (entity.GetType() == typeof(NpcEntity))
        {
            NpcEntity npcEntity = (NpcEntity)entity;
            npcEntity.HideObject();
            NewCalcGravityNpc(npcEntity, newPosition);
            npcEntity.ShowObject();

            ReplayMoveIfNeeded(npcEntity);
        }
        else if (entity is UserEntity)
        {
            // EntityActionCombatLog.LogCiPawn(entity,
                // "VL JUMP UserEntity NO_HANDLER nick=" + EntityActionCombatLog.NameOf(entity) +
                // " dest=" + EntityActionCombatLog.Vec(newPosition));
        }
    }

    private void NewCalcGravityNpc(Entity npcEntity, Vector3 newPosition)
    {
        npcEntity.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }

    private void NewCalcGravityMonster(MonsterEntity monsterEntity , Vector3 newPosition)
    {
        monsterEntity.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }

    private void NewCalcGravity(PlayerController playerController , Vector3 newPosition)
    {
        playerController.transform.position = GroundSnapHelper.SnapToGroundOrKeep(newPosition);
    }
}
