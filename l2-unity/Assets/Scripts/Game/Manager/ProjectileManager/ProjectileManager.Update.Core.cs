using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ProjectileManager
{
    private void Update()
    {

        List<int> projectilesToRemove = new List<int>();

        foreach (var pair in activeProjectiles)
        {
            int projectileId = pair.Key;
            ProjectileData projectile = pair.Value;
            try
            {
                if (!projectile.isActive || !UpdateProjectile(projectile))
                {
                    projectilesToRemove.Add(projectileId);
                }
            }
            catch (InvalidCastException)
            {
                // Debug.LogError($"Error updating projectile {projectileId}: {ex.Message}");
                projectilesToRemove.Add(projectileId);
            }
            catch (MissingReferenceException)
            {
                // Debug.LogError(
                    // $"[ProjectileUpdateException] MissingReference id={projectileId} " +
                    // $"prefabNull={(projectile?.prefab == null)} transformNull={(projectile?.transform == null)} " +
                    // $"targetNull={(projectile?.targetTransform == null)} msg={ex.Message}");
                projectilesToRemove.Add(projectileId);
            }


        }

        foreach (int projectileId in projectilesToRemove)
        {
            if (activeProjectiles.TryGetValue(projectileId, out ProjectileData projectile))
            {
                if (projectile.prefab != null && projectile.impactType == ProjectileImpactType.EffectOnly)
                {
                    Destroy(projectile.prefab);
                }
                activeProjectiles.Remove(projectileId);
            }
        }
    }
}
