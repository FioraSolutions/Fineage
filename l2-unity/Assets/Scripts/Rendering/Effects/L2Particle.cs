using UnityEngine;
using System.Collections.Generic;

public partial class L2Particle : BaseEffect, IRegisteredBillboard
{
    private const string LifetimeTraceEffectName = "el_wind_strike_ta";

    [Tooltip("Не вызывать DestoryEffect при Play — объект не удаляется по таймеру (отладка шейдера, Hold и т.п.). Выключено по умолчанию.")]
    [SerializeField] private bool _skipScheduledDestroyForDebug;

    [HideInInspector]
    [SerializeField] private Vector3 _surfaceNormal;
    [Header("Billboard")]
    [SerializeField] private bool _billboardToCamera;
    [SerializeField] private Vector3 _billboardRotationOffsetEuler;
    [HideInInspector]
    [SerializeField] private PooledEffect _pooledEffect;
    [SerializeField] private EffectPart[] _particleGroups;
    private EffectSettings _settings;
    private MagicCastData _castData;
    private float _playStartedAt = -1f;
    private string _firstPlayStack;
    public PooledEffect PooledEffect { get { return _pooledEffect; } }
    public Vector3 SurfaceNormal { get { return _surfaceNormal; } set { _surfaceNormal = value; } }

}
