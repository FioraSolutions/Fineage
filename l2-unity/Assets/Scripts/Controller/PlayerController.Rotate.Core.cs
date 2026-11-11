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
        Vector3 directionToAttacker = (_attackerPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToAttacker);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    public float GetLiveRotateSpeed()
    {
        return _calcSpeed != null ? _calcSpeed.GetSpeedRotate(_behindPlayer) : 0f;
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
