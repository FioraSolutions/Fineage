using System;

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
    public void AddValidateLocation(ValidateLocationDto validateLocation)
    {
        if (!_validateList.Contains(validateLocation))
        {
            _validateList.Add(validateLocation);
        }
    }

    public void AddInitPosition(CharMoveToLocationDto location)
    {
        _validateInitPosition.Add(location);
    }


    //This function is a test function.It solves the problem with position synchronization when the server sends data that the client needs to move.
    //But the client did not load at this moment.Therefore, we collect these packages and after loading, we find the oldest one in time and move the NPC immediately to the end of this movement vector.
    //Ideally, you need to calculate the path traveled and move the npc there, but for the sake of 2 npc it makes no sense
    private void ValidInitNpsPosition()
    {
        if (_validateInitPosition.Count > 0)
        {
            for (int i = 0; i < _validateInitPosition.Count; i++)
            {
                CharMoveToLocationDto location =  _validateInitPosition[i];
                World world = GameWorld;
                if (world == null)
                    continue;
                Entity entity = world.GetEntityNoLockSync(location.ObjId);

                if(entity != null)
                {
                    // Debug.Log("object position 1 " + entity.transform.position + " go name " + entity.name + " end position " + location.NewPosition);
                    if (entity.isActiveAndEnabled)
                    {
                        entity.HideObject();
                        NewCalcGravityNpc(entity, location.NewPosition);
                        entity.ShowObject();
                    }
                    //Debug.Log("object position 2 " + entity.transform.position + " go name " + entity.name + " end position " + location.NewPosition);
                }

            }

            _validateInitPosition.Clear();
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }
}
