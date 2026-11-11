using System.Collections;
using UnityEngine;

public sealed partial class EffectPrefabStaggeredDrift : MonoBehaviour
{
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
}
