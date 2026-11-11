using UnityEngine;

public sealed partial class ItemPickupMotion : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationRateEuler = new Vector3(0f, 220f, 0f);
    [SerializeField] private float _spinSeconds = 1.6f;

    Vector3 _targetEuler;
    float _elapsed;
    bool _settled = true;





    /// <summary>UE2-style approach with wrap (degrees).</summary>
}
