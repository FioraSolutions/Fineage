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
}
