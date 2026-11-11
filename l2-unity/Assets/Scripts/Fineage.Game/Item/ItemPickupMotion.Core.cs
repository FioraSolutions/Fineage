using UnityEngine;

public sealed partial class ItemPickupMotion : MonoBehaviour
{
    void Awake()
    {
        enabled = false;
    }

    public void BeginSpin(int objectId)
    {
        float sign = (objectId & 1) == 0 ? 1f : -1f;
        _rotationRateEuler = new Vector3(0f, 220f * sign, 40f * sign);
        _targetEuler = transform.eulerAngles + new Vector3(0f, 360f * sign, 0f);
        _elapsed = 0f;
        _settled = false;
        enabled = true;
    }
}
