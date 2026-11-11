using UnityEngine;

public partial class MovementTarget
{
    public MovementTarget(object objectPoint , float distance , bool isRunning)
    {
        _objectPoint = objectPoint;
        _distance = distance;
        _isRunning = isRunning;
    }

    public void Set(object objectPoint, float distance, bool isRunning)
    {
        _objectPoint = objectPoint;
        _distance = distance;
        _isRunning = isRunning;
    }

    public MovementTarget(Entity entity, float distance)
    {
        _objectPoint = entity.transform;
        _distance = distance;
        _isRunning = true;
    }

    public bool IsRunningServer()
    {
        return _isRunning;
    }
}
