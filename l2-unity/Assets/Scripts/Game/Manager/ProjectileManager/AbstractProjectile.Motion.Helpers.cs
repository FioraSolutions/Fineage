using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class AbstractProjectile : MonoBehaviour
{
    protected Vector3 GetCurrentPosition(ProjectileData projectile, float journeyProgress)
    {
        return Vector3.Lerp(
             projectile.startPosition,
             projectile.targetPosition,
             journeyProgress
         );
    }

    protected void RefreshHitPosition(ProjectileData projectile)
    {
        projectile.hitPoint = projectile.targetPosition;
        projectile.hitNormal = Vector3.up;
        projectile.hitDirection = VectorUtils.CalcHitDirection(projectile.targetPosition, projectile.startPosition);
    }

    protected float GetCurveSpeed(ProjectileData projectile, float baseJourneyProgress, float speedMultiplier)
    {
        return projectile.speedCurve?.Evaluate(baseJourneyProgress) ?? speedMultiplier;
    }

    protected void CalcNewTargetPosition(ProjectileData projectile)
    {
        if (projectile.targetTransform != null)
        {

            Vector3 previousTargetPosition = projectile.targetPosition;
            Vector3 newTargetPosition = VectorWorldUtils.GetCollision(projectile.startPosition, projectile.targetTransform);
            float moveThreshold = 0.1f;

            if (Vector3.Distance(previousTargetPosition, newTargetPosition) > moveThreshold)
            {
                projectile.targetPosition = newTargetPosition;
            }
        }
    }
}
