using System;
using UnityEngine;

public abstract partial class BaseEffectGroup : EffectPart
{
    [Header("Base Spawning")]
    [SerializeField] protected EffectInstance[] _effectInstances;

    [SerializeField] protected int _countPerSecond = 20;
    [SerializeField] protected int _maxCount = 20;
    [SerializeField] protected float _duration = 1f;
    [SerializeField] protected bool useFixedLifetime = false;
    [SerializeField] protected float minLifeTime = 0;
    [SerializeField] protected float _startDelay = 0f; // Задержка перед началом спавна (в секундах)

    protected bool _stopped = true;
    protected float _lastEnable;
    protected float _lastLoop;
    protected int _particleIndex = 0;




    protected abstract void ApplyShaderParams(Component source, float now, float seed);



}
