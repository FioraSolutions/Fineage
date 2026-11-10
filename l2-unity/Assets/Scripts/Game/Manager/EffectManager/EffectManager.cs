using System;

﻿using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _instance;

    [System.Obsolete("Use GlobalEffectsPacketServices.Effects or [Inject] EffectManager instead")]
    public static EffectManager Instance { get { return _instance; } }

    public EffectDatabase database;
    // Scene ref is World/Effects/Active — dies on LoadMenu while EffectManager may DDOL.
    // EnsureActiveEffectsContainer() recreates under this transform when the scene ref is gone.
    [SerializeField] private Transform _activeEffectsContainer;
    [Inject] IObjectResolver _container;
    [Inject] IAnimationManager _animations;
    private const string IMPACT_DEBUG_TAG = "[HIT_DEBUG]";
    private int _impactSpawnCounter = 0;
    private readonly Dictionary<string, (int count, float windowStartSec)> _impactWindowByPoint
        = new Dictionary<string, (int count, float windowStartSec)>();
    private const float IMPACT_WINDOW_SEC = 0.35f;

    /// <summary>
    /// Session copy of GlobalEffect rows. EffectManager owns these refs so
    /// SceneLoader tile unload → implicit UnloadUnusedAssets cannot fake-null
    /// ScriptableObject prefab fields mid-play. Editor SoT stays GlobalEffect.asset.
    /// </summary>
    struct RuntimeEffect
    {
        public int Id;
        public BaseEffect Prefab;
        public EffectSettings Settings;
        public bool UseWeaponShot;
        public bool SpiritShot;
        public bool IgnoreFlightTimeForCast;
        public string Comment;
    }

    readonly Dictionary<int, RuntimeEffect> _byId = new Dictionary<int, RuntimeEffect>(256);
    // Exact comment → first matching session row (EnterEventgrp keys).
    readonly Dictionary<string, RuntimeEffect> _byComment = new Dictionary<string, RuntimeEffect>(64);
    // GC root for Resources FX + nested composite part prefabs.
    readonly List<UnityEngine.Object> _retainedAssets = new List<UnityEngine.Object>(256);
    bool _registryBuilt;

    public int RegistryCount => _byId.Count;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            GlobalEffectsPacketServices.Bind(effects: this);
            WireGamePacketServices();
            BuildRegistry();
        }
        else if (_instance != this)
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        WireGamePacketServices();
        if (!_registryBuilt)
            BuildRegistry();
    }

    private void WireGamePacketServices()
    {
        GamePacketServices.Bind(animations: _animations);
    }

    /// <summary>
    /// Impact FX parent. Scene Active under World is destroyed on relogin;
    /// keep a live child on this (DDOL) manager instead of aborting PlayEffect.
    /// </summary>
    Transform EnsureActiveEffectsContainer()
    {
        if (_activeEffectsContainer != null)
            return _activeEffectsContainer;

        Transform existing = transform.Find("ActiveEffects");
        if (existing != null)
        {
            _activeEffectsContainer = existing;
            return _activeEffectsContainer;
        }

        var go = new GameObject("ActiveEffects");
        go.transform.SetParent(transform, false);
        _activeEffectsContainer = go.transform;
        return _activeEffectsContainer;
    }

    void BuildRegistry()
    {
        _byId.Clear();
        _byComment.Clear();
        _retainedAssets.Clear();
        _registryBuilt = false;

        if (database == null || database.effects == null)
        {
            Debug.LogError("[EffectManager] BuildRegistry: database missing");
            return;
        }

        for (int i = 0; i < database.effects.Count; i++)
        {
            EffectDatabase.EffectData row = database.effects[i];
            if (row == null)
                continue;

            // Weapon-shot rows resolve prefab at play time from weapon_shot.int.
            if (!row.useWeaponShot && row.prefab == null)
            {
                Debug.LogError(
                    "[EffectManager] BuildRegistry missing prefab id=" + row.id +
                    " comment=" + row.comment);
                continue;
            }

            var entry = new RuntimeEffect
            {
                Id = row.id,
                Prefab = row.prefab,
                Settings = row.settings,
                UseWeaponShot = row.useWeaponShot,
                SpiritShot = row.spiritShot,
                IgnoreFlightTimeForCast = row.ignoreFlightTimeForCast,
                Comment = row.comment,
            };
            _byId[row.id] = entry;

            if (!string.IsNullOrEmpty(row.comment) && !_byComment.ContainsKey(row.comment))
                _byComment[row.comment] = entry;

            if (row.prefab != null)
            {
                _retainedAssets.Add(row.prefab);
                if (row.prefab.gameObject != null)
                    _retainedAssets.Add(row.prefab.gameObject);
                row.prefab.CollectReferencedPrefabs(_retainedAssets);
            }

            if (row.settings != null)
                _retainedAssets.Add(row.settings);
        }

        _registryBuilt = _byId.Count > 0;
    }

    void EnsureRegistry()
    {
        if (!_registryBuilt)
            BuildRegistry();
    }

    bool TryGetRuntime(int id, out RuntimeEffect entry)
    {
        EnsureRegistry();
        return _byId.TryGetValue(id, out entry);
    }

    /// <summary>
    /// Session flag from GlobalEffect.ignoreFlightTimeForCast (not live SO read).
    /// </summary>
    public bool ShouldIgnoreFlightTimeForCast(int effectId)
    {
        return TryGetRuntime(effectId, out RuntimeEffect entry) && entry.IgnoreFlightTimeForCast;
    }

    /// <summary>
    /// Exact comment match against the session registry (EnterEventgrp keys).
    /// </summary>
    public bool TryGetByComment(
        string comment,
        out int id,
        out BaseEffect prefab,
        out EffectSettings settings)
    {
        id = 0;
        prefab = null;
        settings = null;
        if (string.IsNullOrEmpty(comment))
            return false;

        EnsureRegistry();
        if (!_byComment.TryGetValue(comment, out RuntimeEffect entry) || entry.Prefab == null)
            return false;

        id = entry.Id;
        prefab = entry.Prefab;
        settings = entry.Settings;
        return true;
    }

    /// <summary>
    /// Debug helper: rows whose comment or prefab name contains <paramref name="token"/>.
    /// </summary>
    public int CountCommentOrPrefabMatches(string token)
    {
        if (string.IsNullOrEmpty(token))
            return 0;

        EnsureRegistry();
        int count = 0;
        foreach (RuntimeEffect entry in _byId.Values)
        {
            string prefabName = entry.Prefab != null ? entry.Prefab.name : string.Empty;
            string comment = entry.Comment != null ? entry.Comment : string.Empty;
            if (prefabName.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
                comment.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                count++;
            }
        }

        return count;
    }

    public void PlayEffect(int id, Transform target, MagicCastData castData = null)
    {
        bool windstrike = id == 4247;
        // PlayEffect parents under target — container is only for impact proxies.
        EnsureActiveEffectsContainer();

        if (windstrike)
        {
            Debug.Log(
                "[WINDSTRIKE] EffectManager registry=" + _registryBuilt +
                " count=" + _byId.Count +
                " container=" + (_activeEffectsContainer != null) +
                " target=" + (target != null ? target.name : "null"));
        }

        RuntimeEffect entry;
        bool found;
        using (MagicSkillUseTrace.Measure("effect.find"))
            found = TryGetRuntime(id, out entry);

        BaseEffect prefab;
        using (MagicSkillUseTrace.Measure("effect.prefab"))
            prefab = ResolvePrefab(found ? entry : default, found, target);

        if (!found || prefab == null || target == null)
        {
            Debug.LogWarning(
                "[EffectManager] abort id=" + id +
                " found=" + found +
                " prefab=" + (prefab != null ? prefab.name : "null") +
                " target=" + (target != null) +
                " comment=" + (found ? entry.Comment : ""));
            return;
        }

        if (windstrike)
            Debug.Log("[WINDSTRIKE] EffectManager spawn prefab=" + prefab.name);

        BaseEffect instance;
        using (MagicSkillUseTrace.Measure("effect.instantiate"))
            instance = Instantiate(prefab, target.position, target.rotation, target);
        using (MagicSkillUseTrace.Measure("effect.inject"))
            InjectSpawned(instance);

        instance.gameObject.SetActive(true);
        using (MagicSkillUseTrace.Measure("effect.setup"))
            instance.Setup(entry.Settings, castData, target);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // EffectDoublePlayLog.TrackManagerPlay(id, prefab != null ? prefab.name : "null");
#endif
        using (MagicSkillUseTrace.Measure("effect.play"))
            instance.Play();
        if (windstrike)
            Debug.Log("[WINDSTRIKE] EffectManager.Play returned instance=" + instance.name);
    }

    BaseEffect ResolvePrefab(RuntimeEffect entry, bool found, Transform target)
    {
        if (!found)
            return null;

        if (entry.UseWeaponShot)
        {
            Gear gear = target != null ? target.GetComponentInParent<Gear>() : null;
            BaseEffect listed = TablePacketServices.WeaponShots.ResolvePrefab(gear, entry.SpiritShot);
            if (listed != null)
                return listed;
        }

        return entry.Prefab;
    }

    /// <summary>
    /// Melee skill FX attached to weapon (fallback: entity). Builds cast data with
    /// <see cref="MagicCastData.SkillAnimationDuration"/> from SpAtk wall-time so composites
    /// with Match Lifetime To Skill Animation end with the swing.
    /// </summary>
    public void PlayEffectSyncedToSkillAnimation(
        int effectId,
        Entity entity,
        int hitTimeMs,
        AnimationCombo animCombo)
    {
        if (entity == null)
        {
            // Debug.LogWarning($"[SKILL_ANIM_FX] PlayEffectSyncedToSkillAnimation entity is null effectId={effectId}");
            return;
        }

        Transform weapon = entity.GetWeaponTransform();

        Transform attach = weapon != null ? weapon : entity.transform;
        IAnimationManager animations = _animations != null ? _animations : GamePacketServices.Animations;
        MagicCastData castData = SkillAnimationCastDataBuilder.Build(entity, hitTimeMs, animCombo, animations);
        PlayEffect(effectId, attach, castData);
    }

    // L2 Action_Attack: FVector::Rotation(targetLoc - attackerLoc) on XZ.
    // shot_N_atk travel = local +X. LookRotation aligns +Z; yaw -90 maps +X onto dir
    // (yaw +90 was mapping +X onto -dir — cone flew back toward the player).
    private const float IMPACT_HIT_DIRECTION_YAW_OFFSET_DEGREES = -90f;

    public void PlayerImpactEffect(int id, Vector3 point, MagicCastData castData = null)
    {
        PlayerImpactEffect(id, point, Vector3.zero, castData);
    }

    public void PlayerImpactEffect(int id, Vector3 point, Vector3 impactDirection, MagicCastData castData = null)
    {
        // Debug.Log(
            // $"[HIT_FX] 7.EffectManager.PlayerImpactEffect ENTER frame={Time.frameCount} t={Time.time:F3} " +
            // $"effectId={id} point={point} dir={impactDirection}");

        if (!TryGetRuntime(id, out RuntimeEffect entry) || entry.Prefab == null)
        {
            Debug.LogWarning(
                "[EffectManager] PlayerImpactEffect abort id=" + id +
                " found=" + _byId.ContainsKey(id) +
                " prefab=" + (entry.Prefab != null ? entry.Prefab.name : "null") +
                " comment=" + entry.Comment);
            return;
        }

        Transform effectsRoot = EnsureActiveEffectsContainer();

        Vector3 dir = ResolveImpactDirection(point, impactDirection);
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up) *
            Quaternion.Euler(0f, IMPACT_HIT_DIRECTION_YAW_OFFSET_DEGREES, 0f);

        GameObject dummy = new GameObject("HitPointProxy");
        dummy.transform.SetPositionAndRotation(point, rotation);
        if (effectsRoot != null)
            dummy.transform.SetParent(effectsRoot, true);

        BaseEffect instance = Instantiate(entry.Prefab, point, rotation, dummy.transform);
        InjectSpawned(instance);
        _impactSpawnCounter += 1;
        string pointKey = $"{Mathf.Round(point.x * 10f) / 10f},{Mathf.Round(point.y * 10f) / 10f},{Mathf.Round(point.z * 10f) / 10f}";
        float now = Time.time;
        if (_impactWindowByPoint.TryGetValue(pointKey, out var state))
        {
            if (now - state.windowStartSec > IMPACT_WINDOW_SEC)
            {
                state = (0, now);
            }
            state.count += 1;
            _impactWindowByPoint[pointKey] = state;
        }
        else
        {
            _impactWindowByPoint[pointKey] = (1, now);
            state = _impactWindowByPoint[pointKey];
        }
        Vector3 emitterRight = rotation * Vector3.right;
        Vector3 emitterFwd = rotation * Vector3.forward;
        Vector3 playerToHit = Vector3.zero;
        Vector3 playerFwd = Vector3.zero;
        if (DomainPacketServices.LocalPlayer != null)
        {
            playerToHit = hitPointFlat(point - DomainPacketServices.LocalPlayer.transform.position);
            playerFwd = hitPointFlat(DomainPacketServices.LocalPlayer.transform.forward);
        }

        float angDirVsPlayerToHit = AngleFlatDeg(dir, playerToHit);
        // Debug.Log(
            // $"{IMPACT_DEBUG_TAG} PlayerImpactEffect spawn#{_impactSpawnCounter} effectId={id} " +
            // $"point={point} dirIn={impactDirection} dirResolved={dir} " +
            // $"emitterRight(+X travel)={emitterRight} emitterFwd={emitterFwd} " +
            // $"playerToHit={playerToHit} playerFwd={playerFwd} angDirVsPlayerToHit={angDirVsPlayerToHit:F1} " +
            // $"yawOffset={IMPACT_HIT_DIRECTION_YAW_OFFSET_DEGREES} " +
            // $"countNearPointWindow={state.count} windowSec={IMPACT_WINDOW_SEC:F2}");

        instance.gameObject.SetActive(true);
        instance.Setup(entry.Settings, castData, dummy.transform);
        // Child parts often use inheritRotation=0 / CasterCenter — push dir into composite context
        // so ResolvePartSpawnRotation orients meshes even when proxy rotation would be ignored.
        if (instance is CompositePrefabEffect composite)
        {
            composite.SetImpactHit(point, dir);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // EffectDoublePlayLog.TrackManagerPlay(id, entry.Prefab != null ? entry.Prefab.name : "null");
#endif
        instance.Play();
        // Debug.Log(
            // $"[HIT_FX] 7.EffectManager PLAY OK spawn#{_impactSpawnCounter} effectId={id} " +
            // $"prefab={entry.Prefab.name} point={point}");
    }

    private static Vector3 hitPointFlat(Vector3 v)
    {
        v.y = 0f;
        return v.sqrMagnitude > 0.0001f ? v.normalized : Vector3.zero;
    }

    private static float AngleFlatDeg(Vector3 a, Vector3 b)
    {
        Vector3 af = hitPointFlat(a);
        Vector3 bf = hitPointFlat(b);
        if (af.sqrMagnitude < 0.0001f || bf.sqrMagnitude < 0.0001f)
        {
            return -1f;
        }

        return Vector3.Angle(af, bf);
    }

    private static Vector3 ResolveImpactDirection(Vector3 hitPoint, Vector3 impactDirection)
    {
        // Always flatten: sword hitDirection carries pitch and tilts the emitter.
        if (impactDirection.sqrMagnitude > 0.0001f)
        {
            Vector3 flat = impactDirection;
            flat.y = 0f;
            if (flat.sqrMagnitude > 0.0001f)
            {
                return flat.normalized;
            }
        }

        // L2: targetLoc - attackerLoc (horizontal).
        if (DomainPacketServices.LocalPlayer != null)
        {
            Vector3 fromAttacker = hitPoint - DomainPacketServices.LocalPlayer.transform.position;
            fromAttacker.y = 0f;
            if (fromAttacker.sqrMagnitude > 0.0001f)
            {
                return fromAttacker.normalized;
            }
        }

        return Vector3.forward;
    }

    void InjectSpawned(BaseEffect instance)
    {
        if (instance == null || _container == null)
        {
            return;
        }

        _container.InjectGameObject(instance.gameObject);
    }
}
