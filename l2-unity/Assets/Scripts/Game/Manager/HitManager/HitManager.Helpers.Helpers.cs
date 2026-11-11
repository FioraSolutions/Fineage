using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    public void HandleHitBody(GameObject source, Transform target, Vector3 hitPointCollider, Vector3 hitDirection)
    {
        string sourceNameLower = source.name.ToLower();
        string etcNameLower = ETC_NAME.ToLower();

        if (sourceNameLower.IndexOf(etcNameLower) > -1)
        {
            HandleArrowHit(source, target, hitPointCollider, hitDirection);
        }
        else
        {
            // Debug.LogWarning("HitManager> HandleHit Errors no detected hit type");
        }
    }

    public void HandleHitCollider(
        Entity attaker,
        Transform attacker,
        Entity targetEntity,
        Vector3 hitCollider,
        Vector3 hitColliderDirection)
    {
        string attackerName = attacker != null ? attacker.name : "null";
        // Debug.Log(
            // $"[HIT_FX] 6.HitManager.HandleHitCollider ENTER frame={Time.frameCount} t={Time.time:F3} " +
            // $"attacker={attackerName} target={(targetEntity != null ? targetEntity.name : "null")} " +
            // $"soulshot={(attaker != null && attaker.IsSoulshotCharged)} point={hitCollider}");

        if (targetEntity == null)
        {
            // Debug.LogWarning("[HIT_FX] 6.HitManager SKIP targetEntity=null");
            return;
        }

        Vector3 impactDir = ResolveSoulshotImpactDirection(attaker, targetEntity, hitCollider);
        Vector3 spawnPoint = ResolveSoulshotSpawnPoint(targetEntity, impactDir, hitCollider);

        EffectManager effects = GlobalEffectsPacketServices.Effects;
        if (effects == null)
        {
            // Debug.LogWarning("[HIT_FX] 6.HitManager SKIP effects=null");
            return;
        }

        if (attaker != null && attaker.IsSoulshotCharged)
        {
            LogSoulshotImpactOrientation(attaker, targetEntity, hitCollider, spawnPoint, impactDir);
            effects.PlayerImpactEffect(SoulshotImpactEffectId, spawnPoint, impactDir);
            attaker.IsSoulshotCharged = false;
            return;
        }

        // Spiritshot only charges the weapon cast; never show melee SoulshotImpact on the mob.
        if (attaker != null && attaker.IsSpiritshotCharged)
            attaker.IsSpiritshotCharged = false;

        effects.PlayerImpactEffect(NormalImpactEffectId, spawnPoint, impactDir);
    }

    private Entity ResolveEntityFromWorld(int entityId)
    {
        World world = GameWorld;
        if (entityId <= 0 || world == null)
            return null;

        return world.GetEntityNoLockSync(entityId);
    }

    private void HandleArrowHit(GameObject arrow, Transform target, Vector3 hitPointCollider, Vector3 hitDirection)
    {
        if (!target.CompareTag("Npc"))
        {
            return;
        }

        MonsterEntity entity = target.GetComponent<MonsterEntity>();
        if (entity == null)
        {
            return;
        }

        entity.AttachArrowToNearestBone(arrow, hitPointCollider, target, hitDirection);

        Collider arrowCollider = arrow.GetComponent<Collider>();
        if (arrowCollider != null)
        {
            arrowCollider.enabled = false;
        }
    }
}
