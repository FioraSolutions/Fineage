using System;
using System.Collections.Generic;
using UnityEngine;

public static partial class CompositePlaybackSubscriptions
{
    public static void SubscribeHitIfNeeded(
        CompositePrefabPart[] parts,
        Action<GameObject, Transform, Vector3, Vector3, int> handler,
        ref bool isSubscribed)
    {
        if (!CompositePartScheduler.RequiresHitColliderSpawn(parts) ||
            ProjectilePacketServices.Projectiles == null ||
            isSubscribed)
        {
            return;
        }

        ProjectilePacketServices.Projectiles.OnHitEffectProjectile += handler;
        isSubscribed = true;
    }

    public static void UnsubscribeHit(
        Action<GameObject, Transform, Vector3, Vector3, int> handler,
        ref bool isSubscribed)
    {
        if (ProjectilePacketServices.Projectiles != null && isSubscribed)
        {
            ProjectilePacketServices.Projectiles.OnHitEffectProjectile -= handler;
        }

        isSubscribed = false;
    }

    public static bool IsFromLaunchedCompositeProjectile(
        Transform attacker,
        Dictionary<CompositePrefabPart, BaseEffect> spawnedPartInstances)
    {
        if (attacker == null || spawnedPartInstances == null)
        {
            return false;
        }

        foreach (KeyValuePair<CompositePrefabPart, BaseEffect> pair in spawnedPartInstances)
        {
            if (!CompositeProjectileLaunchHelper.IsProjectilePart(pair.Key) || pair.Value == null)
            {
                continue;
            }

            Transform spawnedTransform = pair.Value.transform;
            if (attacker == spawnedTransform || attacker.IsChildOf(spawnedTransform))
            {
                return true;
            }
        }

        return false;
    }
}
