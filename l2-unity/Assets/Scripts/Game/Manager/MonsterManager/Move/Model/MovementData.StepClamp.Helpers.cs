using UnityEngine;

public partial class MovementData
{
    public void OnFinish(Vector3 target)
    {
        _isMove = false;
        Vector3 dest;
        if (_movementTarget.IsActorTarget())
        {
            // MoveToPawn: already inside Dist of the pawn. Do not snap to pawn origin
            // (that ran UserEntity into the monster center).
            dest = _entity.transform.position;
        }
        else
        {
            dest = new Vector3(target.x, _entity.transform.position.y, target.z);
        }
        dest = GroundSnapHelper.SnapToGroundOrKeep(dest);
        if (_entity is UserEntity)
            EntitySpawnShared.ApplyGroundedTransform(_entity.gameObject, dest, _entity.transform.rotation);
        else
            _entity.transform.position = dest;
        SetEventToTypeEntity(_entity);
        EntitySpawnShared.LogStriderGait("arrived", _entity);
    }

    public void SetEventToTypeEntity(Entity entity)
    {
        if (entity is PlayerEntity)
            return;

        if (EntityActionPacketServices.Actions != null)
            EntityActionPacketServices.Actions.NotifyArrived(entity);
    }

    public CharacterController GetControllerToTypeEntity(Entity entity)
    {
        CharacterController character = null;

        if (entity.GetType() == typeof(MonsterEntity))
        {
            var _mEntity = (MonsterEntity)entity;
            character = _mEntity.GetCharacterController();

        }
        else if (entity.GetType() == typeof(NpcEntity))
        {
            var _mEntity = (NpcEntity)entity;
            character = _mEntity.GetCharacterController();

        }
        else if (entity.GetType() == typeof(UserEntity))
        {
            character = ((UserEntity)entity).GetCharacterController();
        }
        return character;
    }
}
