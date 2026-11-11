using UnityEngine;

public partial class MovementTarget
{
    public float GetDistance()
    {
        return _distance;
    }

    public bool IsActorTarget()
    {
        return _objectPoint is Transform;
    }

    public Entity GetActorEntity()
    {
        Transform transform = _objectPoint as Transform;
        return transform != null ? transform.GetComponent<Entity>() : null;
    }

    public bool TryGetTarget(out Vector3 target)
    {
        target = Vector3.zero;
        switch (_objectPoint)
        {
            case Transform transform:
                if (transform == null)
                    return false;
                target = transform.position;
                return true;
            case Vector3 vector:
                target = vector;
                return true;
            default:
                return false;
        }
    }

    public Vector3 GetTarget()
    {
        return TryGetTarget(out Vector3 target) ? target : Vector3.zero;
    }
}
