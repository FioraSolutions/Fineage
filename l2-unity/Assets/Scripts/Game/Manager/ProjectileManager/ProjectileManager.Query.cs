using UnityEngine;

public partial class ProjectileManager
{
    public bool HasInboundToward(Entity target)
    {
        if (target == null || activeProjectiles.Count == 0)
            return false;
        foreach (var pair in activeProjectiles)
        {
            ProjectileData projectile = pair.Value;
            if (projectile == null || !projectile.isActive)
                continue;
            Entity owner = ResolveProjectileTarget(projectile);
            if (owner == target)
                return true;
        }
        return false;
    }

    static Entity ResolveProjectileTarget(ProjectileData projectile)
    {
        if (projectile == null || projectile.targetTransform == null)
            return null;
        return projectile.targetTransform.GetComponentInParent<Entity>();
    }
}
