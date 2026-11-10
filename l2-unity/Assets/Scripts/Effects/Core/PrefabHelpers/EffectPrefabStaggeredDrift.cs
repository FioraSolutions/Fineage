using System.Collections;
using UnityEngine;

/// <summary>
/// Moves child transforms one after another along up/down (staggered "falling rings").
/// Put on the parent group (e.g. windAround); targets default to direct children.
/// Speed is always real world meters/sec (works inside scaled effect prefabs).
/// </summary>
[DisallowMultipleComponent]
public sealed partial class EffectPrefabStaggeredDrift : MonoBehaviour
{
    [Header("Targets")]
    [Tooltip("Empty = all direct child transforms (in hierarchy order).")]
    [SerializeField] private Transform[] _targets;
    [Tooltip("Must be on when ParticleGroup disables renderer slots between bursts.")]
    [SerializeField] private bool _includeInactiveChildren = true;

    [Header("Motion")]
    [SerializeField] private EffectDriftDirection _direction = EffectDriftDirection.Down;
    [SerializeField] private float _speedMetersPerSecond = 0.2f;
    [Tooltip("0 = no limit.")]
    [SerializeField] private float _maxDriftMeters;
    [SerializeField] private Space _driftSpace = Space.Self;

    [Header("Stagger")]
    [SerializeField] private float _startDelaySeconds;
    [Tooltip("Delay before each next target starts drifting.")]
    [SerializeField] private float _staggerDelaySeconds = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool _debugLog = true;
    [SerializeField] private float _debugLogIntervalSeconds = 0.25f;
    [SerializeField] private float _accelerationMetersPerSecond2 = 1.0f; // Ускорение движения

    private const string LogTag = "[CURE_DRIFT]";

    private Transform[] _resolvedTargets;
    private Vector3[] _initialLocalPositions;
    private Vector3[] _initialWorldPositions;
    private float[] _driftMeters;
    private float _enabledAt = -1f;
    private float _nextDebugLogAt;
    private Coroutine _deferredRestartRoutine;

    private void OnEnable()
    {
        _enabledAt = Time.time; // Фиксируем время физического появления
        _nextDebugLogAt = 0f;
        Restart();
        ScheduleDeferredRestart();
        LogState("OnEnable");
    }

    private void Start()
    {
        _enabledAt = Time.time; // Фиксируем время на случай отложенного Start
        Restart();
        LogState("Start");
    }

    private void OnDisable()
    {
        if (_deferredRestartRoutine != null)
        {
            StopCoroutine(_deferredRestartRoutine);
            _deferredRestartRoutine = null;
        }

        RestoreInitialPositions();
    }

    public void Restart()
    {
        Transform[] newTargets = ResolveTargets();
        int count = newTargets != null ? newTargets.Length : 0;
        if (count == 0)
        {
            if (_debugLog)
            {
                // Debug.LogWarning(
                    // $"{LogTag} '{name}' Restart: no targets (children={transform.childCount}, assigned={(_targets != null ? _targets.Length : 0)}, includeInactive={_includeInactiveChildren}) — keeping previous list.",
                    // this);
            }
            return;
        }

        _resolvedTargets = newTargets;

        // Инициализируем массивы строго один раз при первом спавне
        if (_initialLocalPositions == null || _initialLocalPositions.Length != count)
        {
            _initialLocalPositions = new Vector3[count];
            _initialWorldPositions = new Vector3[count];
            _driftMeters = new float[count];

            for (int i = 0; i < count; i++)
            {
                Transform t = _resolvedTargets[i];
                if (t == null) continue;

                _initialLocalPositions[i] = t.localPosition;
                _driftMeters[i] = 0f;
            }
        }

        // Рассчитываем, сколько времени система УЖЕ реально движется к моменту этого вызова Restart()
        float systemElapsed = Time.time - _enabledAt;

        for (int i = 0; i < count; i++)
        {
            Transform t = _resolvedTargets[i];
            if (t == null) continue;

            // Временно возвращаем в локальный базис, чтобы Unity корректно 
            // применил новый lossyScale (0.09 вместо 0.07) от внешней системы
            t.localPosition = _initialLocalPositions[i];
            _initialWorldPositions[i] = t.position;

            // ИСПРАВЛЕНИЕ: Вычисляем правильный driftMeters для текущего кадра корутины,
            // вместо того чтобы грубо обнулять его в 0f!
            float targetStart = _startDelaySeconds + (_staggerDelaySeconds * i);
            if (systemElapsed >= targetStart)
            {
                float localTime = systemElapsed - targetStart;
                float v0 = _speedMetersPerSecond;
                float a = _accelerationMetersPerSecond2;

                // Восстанавливаем пройденную дистанцию на момент вызова
                float currentDrift = (v0 * localTime) + (0.5f * a * localTime * localTime);
                if (_maxDriftMeters > 0f && currentDrift >= _maxDriftMeters)
                {
                    currentDrift = _maxDriftMeters;
                }
                _driftMeters[i] = currentDrift;
            }
            else
            {
                _driftMeters[i] = 0f;
            }
        }

        if (_debugLog)
        {
            // Debug.Log(
                // $"{LogTag} '{name}' Restart: targets={count} speed={_speedMetersPerSecond:F3}m/s space={_driftSpace} " +
                // $"lossyScale={transform.lossyScale} active={gameObject.activeInHierarchy} frame={Time.frameCount}.",
                // this);
        }
    }




    private void ScheduleDeferredRestart()
    {
        if (_deferredRestartRoutine != null)
        {
            StopCoroutine(_deferredRestartRoutine);
        }

        _deferredRestartRoutine = StartCoroutine(DeferredRestartRoutine());
    }

    private IEnumerator DeferredRestartRoutine()
    {
        // PlayPart / composite attach can run after our first OnEnable — re-anchor transforms.
        yield return null;
        Restart();
        LogState("Deferred+1");
        yield return null;
        Restart();
        LogState("Deferred+2");
        _deferredRestartRoutine = null;
    }

}
