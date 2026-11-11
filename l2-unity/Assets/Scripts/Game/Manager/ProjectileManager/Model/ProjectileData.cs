using UnityEngine;

public partial class ProjectileData
{



    /// <summary>
    /// Default easing unused for ArrowStick/EffectOnly — both use ANProjectile (t/T)².
    /// </summary>

    public int id;
    public GameObject prefab;
    public Transform transform;
    public Vector3 startPosition;
    public Transform targetTransform;
    public Vector3 targetPosition;
    public float speed;
    public float damage;
    public float lifetime;
    public AnimationCurve speedCurve;
    public float startTime;
    public float distance;
    public bool useGravity;
    public float gravityScale;
    public bool isActive;
    public Vector3 hitPoint;
    public Vector3 hitNormal;
    public float flytime;
    public Vector3 hitPointCollider;
    public Vector3 hitNormalCollider;
    public Vector3 hitDirection;
    public BoxCollider arrowCollider;
    public ProjectileImpactType impactType;
    public Vector3 lastPosition;
    public float castStartTimeSnapshot;
    public float castServerShootSnapshot;
    public float castServerHitSnapshot;
    public float projectileLaunchGlobalFromCast;
    public int attackerEntityId;
}
