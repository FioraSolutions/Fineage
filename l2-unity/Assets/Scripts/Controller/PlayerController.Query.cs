using System;

using UnityEngine;

public partial class PlayerController
{
    public float GetMoveTimer()
    {
        return _moveTimer;
    }

    public bool IsWalkGait()
    {
        return L2PlayerMoveGait.IsWalkStart(_moveTimer, RunMode(), false);
    }

    public float GetLiveMoveSpeed()
    {
        return GetMovementSpeed();
    }

    public Vector3 GetPlayerPosition()
    {
        return transform.position;
    }

    public Vector3 GetBodyPosition()
    {
        Transform tr = _gearEffects.GetBodyCenter();
        return tr.position;
    }

    public Vector3 GetCollisionSelf(Transform target)
    {
        float height = target.GetComponent<Entity>().Appearance.CollisionHeight;
        float radius = target.GetComponent<Entity>().Appearance.CollisionRadius;

        float heightC = VectorUtils.ConvertL2jDataOffset(height);
        float radiusC = VectorUtils.ConvertL2jDataOffsetRadius(radius);
        return VectorUtils.GetCollisionEffect(transform, target, heightC, radiusC);
    }
}
