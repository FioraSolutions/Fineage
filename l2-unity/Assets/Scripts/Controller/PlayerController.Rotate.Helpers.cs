using System;

using UnityEngine;

public partial class PlayerController
{
    private float GetFinalAngle(Vector3 targetPosition, Vector3 currentPosition)
    {
        Vector3 relativeDirection = targetPosition - currentPosition;
        float angleInRadians = Mathf.Atan2(relativeDirection.x, relativeDirection.z);
        float angleInDegrees = Mathf.Rad2Deg * angleInRadians;
        angleInDegrees = (angleInDegrees + 360) % 360; // Нормализация угла
        return angleInDegrees;
    }

    private Quaternion GetNewRotate(float finalAngle)
    {
        return Quaternion.Euler(Vector3.up * finalAngle);
    }

    public bool IsTurnsAround()
    {
        return _turnsAround;
    }

    public void StartRotateFollow(Entity monster)
    {
        _monster = monster;
        _isRotate = true;
        _turnsAround = true;
    }

    public void UpdateFinalAngleToLookAt(Transform target)
    {
        if (target == null)
        {
            return;
        }

        float angle = Mathf.Atan2(target.position.x - transform.position.x, target.position.z - transform.position.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f);
        angle *= 45f;
        _finalAngle = angle;
    }
}
