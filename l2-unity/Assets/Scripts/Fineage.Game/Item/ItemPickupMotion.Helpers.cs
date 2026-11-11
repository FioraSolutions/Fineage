using UnityEngine;

public sealed partial class ItemPickupMotion : MonoBehaviour
{
    void Update()
    {
        if (_settled)
        {
            enabled = false;
            return;
        }

        float dt = Time.deltaTime;
        _elapsed += dt;
        Vector3 current = transform.eulerAngles;

        current.x = ApproachAxis(current.x, _targetEuler.x, _rotationRateEuler.x, dt);
        current.y = ApproachAxis(current.y, _targetEuler.y, _rotationRateEuler.y, dt);
        current.z = ApproachAxis(current.z, _targetEuler.z, _rotationRateEuler.z, dt);
        transform.eulerAngles = current;

        if (_elapsed >= _spinSeconds || NearTarget(current, _targetEuler))
        {
            transform.eulerAngles = _targetEuler;
            _settled = true;
            enabled = false;
        }
    }

    static bool NearTarget(Vector3 a, Vector3 b)
    {
        return Mathf.Abs(Mathf.DeltaAngle(a.x, b.x)) < 2f
            && Mathf.Abs(Mathf.DeltaAngle(a.y, b.y)) < 2f
            && Mathf.Abs(Mathf.DeltaAngle(a.z, b.z)) < 2f;
    }

    static float ApproachAxis(float current, float target, float rate, float dt)
    {
        float delta = Mathf.DeltaAngle(current, target);
        float step = rate * dt;
        if (Mathf.Abs(step) < 0.0001f)
            return current;

        if (rate >= 0f)
        {
            if (delta >= 0f && delta < step)
                return target;
            if (delta < 0f && delta > -step)
                return target;
        }
        else
        {
            if (delta <= 0f && delta > step)
                return target;
            if (delta > 0f && delta < -step)
                return target;
        }

        return current + step;
    }
}
