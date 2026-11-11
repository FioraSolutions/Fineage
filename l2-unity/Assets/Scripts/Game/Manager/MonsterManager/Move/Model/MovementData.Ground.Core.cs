using UnityEngine;

public partial class MovementData
{
    public MovementTarget GetMovementTarget()
    {
        return _movementTarget;
    }

    public Transform GetTransform()
    {
        if (_transform == null && _entity != null)
            _transform = _entity.transform;
        return _transform;
    }

    public float GetDistance()
    {
        return _movementTarget.GetDistance();
    }

    public bool IsMove()
    {
        return _isMove;
    }

    public void SetIsMove(bool isMove)
    {
        _isMove = isMove;
    }
}
