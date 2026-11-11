using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class AbstractProjectile : MonoBehaviour
{
    protected void InvokeOnHitCollider(Transform attacker, Transform target, Vector3 hitPoint, Vector3 hitDirection)
    {
        OnHitCollider?.Invoke(attacker, target, hitPoint, hitDirection);
    }

    public ProjectileData CreateData(int projectileId , float distance, GameObject readyProjectile , Vector3 startPos , Transform target , Vector3 adjustedTarget , float requiredSpeed , ProjectileData settings, ProjectileData defaultSettings)
    {
        AnimationCurve curve = settings?.speedCurve ?? defaultSettings?.speedCurve;
        if (curve == null)
            curve = ProjectileData.CreateDefaultSpeedCurve();

        return new ProjectileData
        {
            id = projectileId,
            prefab = readyProjectile,
            transform = readyProjectile.transform,
            startPosition = startPos,
            targetTransform = target,
            targetPosition = adjustedTarget,
            speed = requiredSpeed,
            damage = settings?.damage ?? defaultSettings.damage,
            lifetime = settings?.lifetime ?? defaultSettings.lifetime,
            speedCurve = curve,
            startTime = Time.time,
            distance = distance,
            useGravity = settings?.useGravity ?? defaultSettings.useGravity,
            gravityScale = settings?.gravityScale ?? defaultSettings.gravityScale,
            hitPointCollider = Vector3.zero,
            hitNormalCollider = Vector3.zero,
            isActive = true,
            impactType = settings?.impactType ?? defaultSettings.impactType,
            lastPosition = Vector3.zero
        };
    }

    protected void ClearParentObject(GameObject readyProjectile)
    {
        if (readyProjectile.transform.parent != null)
            readyProjectile.transform.SetParent(null);
    }

    protected void CheckProjectileCollision(ProjectileData projectile, Vector3 currentPosition, Vector3 lastPosition)
    {
        if (projectile.targetTransform == null || projectile.hitPointCollider != Vector3.zero)
            return;

        if (lastPosition != Vector3.zero)
        {

            float extraMargin = 0.2f;
            Vector3 movementVector = currentPosition - lastPosition;
            float distance = movementVector.magnitude + extraMargin;
            float realDistance = movementVector.magnitude;
            Vector3 direction = movementVector.normalized;


            if (distance > 0)
            {
                float distanceNpc = Vector3.Distance(projectile.startPosition, projectile.targetTransform.position);
                if (distanceNpc > 2)
                {
                    SphereCast(lastPosition, direction, distance, projectile);
                }
                else if (distanceNpc <= 2)
                {
                    RayCast(lastPosition, direction, movementVector, projectile);
                }
            }
        }
    }
}
