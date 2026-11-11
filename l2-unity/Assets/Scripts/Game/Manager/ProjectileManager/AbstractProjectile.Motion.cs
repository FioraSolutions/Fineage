using System;
using UnityEngine;
using UnityEngine.ProBuilder;

public partial class AbstractProjectile : MonoBehaviour
{
    protected void SetPosition(GameObject readyProjectile, Vector3 startPos)
    {
        readyProjectile.transform.position = startPos;
    }

    protected void SetRotation(GameObject readyProjectile, Quaternion rotation)
    {
        readyProjectile.transform.rotation = rotation;
    }

    protected void SetRotation(ProjectileData projectile, Quaternion rotation)
    {
        projectile.transform.rotation = rotation;
    }

    protected void SetPosition(ProjectileData projectile, Vector3 targetPosition)
    {
        if (projectile == null)
        {
            // Debug.LogWarning("[ProjectileSetPosition] projectile data is null.");
            return;
        }
        if (projectile.transform == null)
        {
            // Debug.LogWarning(
                // $"[ProjectileSetPosition] missing transform for id={projectile.id} prefabNull={(projectile.prefab == null)} " +
                // $"isActive={projectile.isActive} start={projectile.startPosition} target={projectile.targetPosition}");
            return;
        }
        projectile.transform.position = targetPosition;
    }

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
