using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class HitManager : MonoBehaviour
{
    [Inject] World _world;

    private World GameWorld =>
        _world != null ? _world : GamePacketServices.GameWorld;

    private static HitManager _instance;

    [System.Obsolete("Use HitPacketServices.Hits or [Inject] HitManager instead")]
    public static HitManager Instance { get { return _instance; } }
    private const string ETC_NAME = "etc_";
    private const string IMPACT_DEBUG_TAG = "[HIT_DEBUG]";
    private const int SoulshotImpactEffectId = 99998;
    private const int NormalImpactEffectId = 99997;
    private bool _projectileHitSubscribed;
    private readonly Dictionary<int, RemoteMeleeHit> _remoteHits = new Dictionary<int, RemoteMeleeHit>();
    private readonly List<int> _remoteHitScratch = new List<int>();
    private AttackDto _localAttack;
    private int _localAttackToken;
    private int _localAttackFiredToken;


    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(gameWorld: _world);
        HitPacketServices.Bind(hits: this);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            HitPacketServices.Bind(hits: this);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void OnEnable()
    {
        TrySubscribeProjectileHits();
    }

    private void Start()
    {
        WireGamePacketServices();
        TrySubscribeProjectileHits();
    }

    private void OnDisable()
    {
        UnsubscribeProjectileHits();
        ClearRemoteMeleeHits();
    }

    private void Update()
    {
        TickRemoteMeleeHits();
    }

    /// <summary>
    /// Called from ProjectileManager.Awake when Instance becomes available (order-safe).
    /// Subscribe once for HitManager lifetime; unsubscribe only on HitManager disable/destroy.
    /// </summary>
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

    /// <summary>
    /// Bow ArrowStick Hit Time — lives on HitManager so AttackDto→Idle cannot drop the subscription.
    /// </summary>
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
