using System;

using UnityEngine;

public partial class PlayerController
{
    public void RotateToAttacker(Vector3 attackerPosition)
    {
        _isRotateAttacker = true;
        _attackerPosition = attackerPosition;
    }

    private void TurnTowardsAttacker(Vector3 attackerPosition)
    {
        StepPawnYawToward(attackerPosition, Time.deltaTime);
    }

    public float GetLiveRotateSpeed()
    {
        return _pawnYaw != null
            ? _pawnYaw.DegreesPerSecond(L2PawnYaw.RotationRateYaw)
            : L2PawnHeadingMath.DegreesPerSecond(L2PawnHeadingMath.RotationRateYaw);
    }

    public float GetDesiredYawDegrees()
    {
        if (_movementTarget == null)
            return transform.eulerAngles.y;
        Vector3 characterPosition = VectorUtils.To2D(transform.position);
        Vector3 targetPosition = VectorUtils.To2D(_movementTarget.GetTarget());
        return GetFinalAngle(targetPosition, characterPosition);
    }
}
