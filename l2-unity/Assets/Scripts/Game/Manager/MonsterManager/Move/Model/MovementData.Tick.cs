using UnityEngine;

public partial class MovementData
{
    public bool IsOwnerDead()
    {
        return _entity != null && _entity.IsDead();
    }

    public static bool IsPlayerPawn(Entity entity)
    {
        return entity is UserEntity || entity is PlayerEntity;
    }

    public void InheritMoveTimer(MovementData prev)
    {
        if (prev == null || !IsPlayerPawn(_entity))
            return;
        // Persist only the walk->run timer. Do NOT copy _gaitAnim: it is a
        // per-MovementData animation cache. Copying it makes SyncUserGait think the
        // clip is already playing and skip StartMove, so point->pawn transitions
        // (and other fresh moves) never re-trigger the run/walk animation.
        _moveTimer = prev._moveTimer;
    }

    public void RetargetPoint(Vector3 destination, bool running)
    {
        _isMove = true;
        if (_movementTarget == null)
            _movementTarget = new MovementTarget(destination, 0.1f, running);
        else
            _movementTarget.Set(destination, 0.1f, running);
    }

    public void RetargetPawn(Transform pawn, float stopDistance, bool running)
    {
        _isMove = true;
        if (_movementTarget == null)
            _movementTarget = new MovementTarget(pawn, stopDistance, running);
        else
            _movementTarget.Set(pawn, stopDistance, running);
    }

    public Entity GetEntity()
    {
        return _entity;
    }

    public bool IsEntity()
    {
        return _entity != null;
    }

    public void SetLastPosition(Vector3 lastPos)
    {
        _lastPos = lastPos;
        _hasLastPos = true;
    }

    public bool HasLastPosition()
    {
        return _hasLastPos;
    }

    public Vector3 GetLastPosition()
    {
        return _lastPos;
    }
}
