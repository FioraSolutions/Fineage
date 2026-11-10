using UnityEngine;

public partial class MovementData
{
    private void CharacterMove(Entity entity , Vector3 direction , float speed)
    {
        CharacterController character = GetControllerToTypeEntity(entity);
        // Match PlayerController: gravity (Y) is integrated separately so the
        // vertical velocity is NOT scaled by run speed. Combining them made the
        // monsters/NPCs bob "light" — the faster they ran, the weaker gravity felt.
        Vector3 planar = new Vector3(direction.x, 0f, direction.z);
        float vy = ApplyGravity(Time.deltaTime);
        StartMove(character, planar, speed, vy);
    }

    private void StartMove(CharacterController character, Vector3 direction, float speed, float verticalVelocity)
    {
        if (character == null || !character.enabled || !character.gameObject.activeInHierarchy)
            return;

        if (character.radius <= 0.001f || character.height <= 0.001f)
            return;

        Vector3 planar = direction * speed * Time.deltaTime;
        planar.y = verticalVelocity * Time.deltaTime;
        character.Move(planar);
    }

    /// </summary>
    /// PhysicsVolume.TerminalVelocity (2500 UU/s → 47.62 m/s).
    /// CharacterController touching the floor. Airborne: gravity capped at
    /// PlayerController.ApplyGravity). Grounded: small downward bias keeps the
    /// Interlude physWalking / physFalling vertical integrator (matches
    /// <summary>
    private float ApplyGravity(float deltaTime)
    {
        var character = GetControllerToTypeEntity(_entity);

        if (character == null)
            return 0f;

        if (character.isGrounded)
        {
            _verticalVelocity = -2f;
        }
        else
        {
            _verticalVelocity -= _gravity * deltaTime;
            if (_verticalVelocity < -TerminalVelocity)
                _verticalVelocity = -TerminalVelocity;
        }
        return _verticalVelocity;
    }

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
