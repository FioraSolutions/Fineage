using UnityEngine;

public abstract partial class AbstractAttackEvents
{
    private void OnHitColliderMonster(Transform attacker, Transform target, Vector3 hitPointCollider, Vector3 hitDirection)
    {
        // Melee only (SwordCollisionService). Bow uses HitManager ← OnHitMonster.
        string attackerName = attacker != null ? attacker.name : "null";
        string targetName = target != null ? target.name : "null";
        Entity entity = DomainPacketServices.LocalPlayer != null ? DomainPacketServices.LocalPlayer.GetTargetEntity() : null;

        // Debug.Log(
            // $"[HIT_FX] 5.OnHitColliderMonster frame={Time.frameCount} t={Time.time:F3} " +
            // $"attacker={attackerName} target={targetName} " +
            // $"playerTarget={(entity != null ? entity.name : "null")} " +
            // $"isMonster={entity is MonsterEntity} point={hitPointCollider}");

        if (entity is MonsterEntity)
        {
            MonsterEntity monster = (MonsterEntity)entity;
            bool missed = _stateMachine != null &&
                          _stateMachine.Player != null &&
                          _stateMachine.Player.HitIsMissed();

            if (missed)
            {
                // Debug.Log(
                    // $"[HIT_FX] 5.OnHitColliderMonster SKIP HitIsMissed=true " +
                    // $"monster={monster.name} — EffectManager NOT called");
            }
            else if (CombatPacketServices.Hits == null)
            {
                // Debug.LogWarning("[HIT_FX] 5.OnHitColliderMonster SKIP hits=null");
            }
            else
            {
                // Debug.Log(
                    // $"[HIT_FX] 5.OnHitColliderMonster → HitManager.HandleHitCollider " +
                    // $"monster={monster.name}");
                CombatPacketServices.Hits.HandleHitCollider(
                    DomainPacketServices.LocalPlayer,
                    attacker,
                    monster,
                    hitPointCollider,
                    hitDirection);
            }

            IfMonsterDead(DomainPacketServices.LocalPlayer.GetTargetEntity());
        }
        else
        {
            // Debug.Log(
                // $"[HIT_FX] 5.OnHitColliderMonster SKIP not MonsterEntity " +
                // $"type={(entity != null ? entity.GetType().Name : "null")}");
        }
    }

    private void OnHitEffectProjectile(GameObject prefab, Transform target, Vector3 hitPointCollider, Vector3 hitDirection, int attackerEntityId)
    {
        string prefabName = prefab != null ? prefab.name : "null";
        string targetName = target != null ? target.name : "null";
        // Debug.Log($"{PROJECTILE_CHAIN_LOG} OnHitEffectProjectile prefab={prefabName} target={targetName} attackerId={attackerEntityId} point={hitPointCollider} dir={hitDirection}");
    }
}
