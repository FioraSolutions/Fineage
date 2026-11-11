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

    private void SphereCast(Vector3 lastPosition , Vector3 direction , float distance , ProjectileData projectile)
    {
        float sphereRadius = 0.3f;
        RaycastHit hitSphereCast;
        if (Physics.SphereCast(lastPosition, sphereRadius, direction, out hitSphereCast, distance, _entityMask))
        {
            var hitPos = hitSphereCast.point;
            OnHit(hitSphereCast, projectile);
        }
    }

    private void RayCast(Vector3 lastPosition , Vector3 direction , Vector3 movementVector , ProjectileData projectile)
    {
        RaycastHit hitRaycast;
        float distanceRaycast = movementVector.magnitude + 0.01f;
        if (Physics.Raycast(lastPosition, direction, out hitRaycast, distanceRaycast, _entityMask))
        {
            var hitPos = hitRaycast.point;
            OnHit(hitRaycast, projectile);
        }
    }

    private void OnHit(RaycastHit hit, ProjectileData projectile)
    {
        GameObject gameObject = hit.transform.parent.gameObject;
        Entity entity = gameObject.GetComponent<Entity>();
        GameObject targetGameObject = projectile.targetTransform.gameObject;
        Entity targetEntity = targetGameObject.GetComponent<Entity>();

        if (targetEntity == null || entity == null) return;

        if (entity.Identity.Id == targetEntity.Identity.Id)
        {

            projectile.hitPointCollider = hit.point;
            projectile.hitNormalCollider = hit.normal;
            projectile.hitDirection = VectorUtils.CalcHitDirection(hit.point, projectile.startPosition);

            // Impact FX is time-based at end-of-flight (ArrowStick / EffectOnly). Collider path unused.
        }
    }

    protected Quaternion GetRotation(Vector3 adjustedTarget, Vector3 startPos)
    {
        Vector3 direction = adjustedTarget - startPos;
        Quaternion rotation = Quaternion.LookRotation(direction);
        return rotation *= Quaternion.Euler(0, 90, 0);
    }
}
