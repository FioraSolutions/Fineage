using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class AbstractProjectile : MonoBehaviour
{
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
