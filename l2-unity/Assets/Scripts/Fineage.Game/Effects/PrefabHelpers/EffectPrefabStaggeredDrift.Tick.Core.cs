using System.Collections;
using UnityEngine;

public sealed partial class EffectPrefabStaggeredDrift : MonoBehaviour
{
    private void LateUpdate()
    {
        if (_resolvedTargets == null || _resolvedTargets.Length == 0)
        {
            return;
        }

        // Полное время, прошедшее с момента истинной активации системы
        float elapsed = Time.time - _enabledAt;

        for (int i = 0; i < _resolvedTargets.Length; i++)
        {
            Transform t = _resolvedTargets[i];
            if (t == null) continue;

            // Точный математический момент старта конкретного кольца
            float targetStart = _startDelaySeconds + (_staggerDelaySeconds * i);

            // --- БЛОКИРОВКА СБРОСА ШЕЙДЕРА (ФИКС БАГА С ПОВТОРОМ) ---
            // Вычисляем, какое ИСТИННОЕ стартовое время должно быть у этого кольца в мире Unity
            float correctShaderStartTime = _enabledAt + targetStart;

            // Находим материал кольца и принудительно прописываем ему правильное время старта.
            // Это не даст внешнему скрипту CompositePrefabEffect сбросить анимацию размера.
            if (t.TryGetComponent<MeshRenderer>(out var renderer) && renderer.material != null)
            {
                if (renderer.material.HasProperty("_StartTime"))
                {
                    renderer.material.SetFloat("_StartTime", correctShaderStartTime);
                }
            }

            // Если время этого кольца еще не пришло, удерживаем его на стартовой позиции
            if (elapsed < targetStart)
            {
                t.localPosition = _initialLocalPositions[i];
                continue;
            }

            // --- ГЕОМЕТРИЯ ПАДЕНИЯ ---
            float localTime = elapsed - targetStart;
            float v0 = _speedMetersPerSecond;
            float a = _accelerationMetersPerSecond2;

            // Равноускоренное движение вниз
            float currentDrift = (v0 * localTime) + (0.5f * a * localTime * localTime);

            if (_maxDriftMeters > 0f && currentDrift >= _maxDriftMeters)
            {
                currentDrift = _maxDriftMeters;
            }

            _driftMeters[i] = currentDrift;
            ApplyPosition(t, i, currentDrift);
        }

        if (_debugLog && Time.time >= _nextDebugLogAt)
        {
            _nextDebugLogAt = Time.time + Mathf.Max(0.05f, _debugLogIntervalSeconds);
            LogState("LateUpdate");
        }
    }

    private void ApplyPosition(Transform t, int index, float driftMeters)
    {
        Vector3 worldAxis = GetWorldAxisVector();

        // --- ЛОГИКА ПРОСТРАНСТВЕННОГО НАХЛЁСТА ---
        // Узнаем, какую МАКСИМАЛЬНУЮ дистанцию должно пройти одно кольцо
        // Если мы хотим идеальный стык встык, то начальный сдвиг = _maxDriftMeters * index.
        // Если хотим НАХЛЁСТ (чтобы они немного перекрывали друг друга), умножаем на 0.8 или 0.9.
        float overlapFactor = 0.85f; // 15% нахлёста для бесшовности текстуры
        float chainOffset = _maxDriftMeters * index * overlapFactor;

        // Итоговое смещение = стартовый сдвиг по цепочке + текущее падение по времени
        float totalDrift = chainOffset + driftMeters;

        Vector3 worldOffset = worldAxis * totalDrift;

        if (_driftSpace == Space.World)
        {
            t.position = _initialWorldPositions[index] + worldOffset;
            return;
        }

        Transform parentTransform = t.parent;
        Vector3 localOffset = parentTransform != null
            ? parentTransform.InverseTransformVector(worldOffset)
            : worldOffset;

        t.localPosition = _initialLocalPositions[index] + localOffset;
    }

    private Vector3 GetWorldAxisVector()
    {
        return _direction == EffectDriftDirection.Down ? Vector3.down : Vector3.up;
    }
}
