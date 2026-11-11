using System;

using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager : MonoBehaviour
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




    /// <summary>
    /// Impact FX parent. Scene Active under World is destroyed on relogin;
    /// keep a live child on this (DDOL) manager instead of aborting PlayEffect.
    /// </summary>
}
