using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class AbstractProjectile : MonoBehaviour
{
    protected LayerMask _entityMask;
    protected float HIT_OFFSET = 0f;//default 0.3f


    public event Action<Transform, Transform, Vector3, Vector3> OnHitCollider;













}
