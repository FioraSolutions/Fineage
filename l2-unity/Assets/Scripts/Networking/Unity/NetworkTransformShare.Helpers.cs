using System;

public partial class NetworkTransformShare : MonoBehaviour
{
    public bool ShouldSharePosition()
    {
        if (Vector3.Distance(transform.position, _lastPos) > .25f || Time.time - _lastSharedPosTime >= 10f)
        {
            return true;
        }

        return false;
    }

    public void SharePosition()
    {
        _lastSharedPosTime = Time.time;
        _lastPos = transform.position;
    }

    public void ShareRotation()
    {
        if (Vector3.Angle(_lastRot, transform.forward) >= 10.0f)
            _lastRot = transform.forward;
    }

    public void ShareAnimation(byte id, float value)
    {
    }
}
