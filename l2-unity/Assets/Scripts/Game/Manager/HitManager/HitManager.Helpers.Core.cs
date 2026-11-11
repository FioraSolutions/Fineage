using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager
{
    public void BindProjectileHits()
    {
        TrySubscribeProjectileHits();
    }

    private void TrySubscribeProjectileHits()
    {
        if (_projectileHitSubscribed || ProjectilePacketServices.Projectiles == null)
        {
            return;
        }

        ProjectilePacketServices.Projectiles.OnHitMonster += OnProjectileHitMonster;
        _projectileHitSubscribed = true;
    }

    private void UnsubscribeProjectileHits()
    {
        if (!_projectileHitSubscribed)
        {
            return;
        }

        if (ProjectilePacketServices.Projectiles != null)
        {
            ProjectilePacketServices.Projectiles.OnHitMonster -= OnProjectileHitMonster;
        }

        _projectileHitSubscribed = false;
    }

    private void OnProjectileHitMonster(
        GameObject prefab,
        Transform target,
        Vector3 hitPointCollider,
        Vector3 hitDirection,
        int attackerEntityId)
    {
        if (prefab != null && target != null)
        {
            HandleHitBody(prefab, target, hitPointCollider, hitDirection);
        }

        Entity attacker = ResolveEntityFromWorld(attackerEntityId);
        if (attacker == null)
            attacker = DomainPacketServices.LocalPlayer;
        Entity targetEntity = target != null
            ? target.GetComponentInParent<Entity>()
            : null;
        if (targetEntity == null && attacker != null)
            targetEntity = attacker.GetTargetEntity();

        MonsterEntity monster = targetEntity as MonsterEntity;
        if (monster == null)
        {
            // Debug.Log(
                // $"[HIT_FX] OnProjectileHitMonster SKIP not MonsterEntity " +
                // $"type={(targetEntity != null ? targetEntity.GetType().Name : "null")}");
            return;
        }

        if (attacker != null && attacker.HitIsMissed())
        {
            // Debug.Log(
                // $"[HIT_FX] OnProjectileHitMonster SKIP HitIsMissed=true monster={monster.name}");
            return;
        }

        Transform attackerTf = prefab != null ? prefab.transform : null;
        // Debug.Log(
            // $"[HIT_FX] OnProjectileHitMonster → HandleHitCollider monster={monster.name} " +
            // $"soulshot={(attacker != null && attacker.IsSoulshotCharged)}");
        HandleHitCollider(
            attacker,
            attackerTf,
            monster,
            hitPointCollider,
            hitDirection);

        if (monster.IsDead() || monster.CalculateRemainingHp() <= 0)
        {
            monster.SetDead(true);
            if (EntityActionPacketServices.Actions != null)
                EntityActionPacketServices.Actions.Die(monster, false, "hit_hp0");
        }
    }
}
