using System.Collections;
using UnityEngine;

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










}
