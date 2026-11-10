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

    private void LogState(string phase)
    {
        if (!_debugLog)
        {
            return;
        }

        float elapsed = _enabledAt > 0f ? Time.time - _enabledAt : 0f;
        int count = _resolvedTargets != null ? _resolvedTargets.Length : 0;
        if (count == 0)
        {
            // Debug.Log($"{LogTag} '{name}' {phase}: no targets elapsed={elapsed:F3}s frame={Time.frameCount}.", this);
            return;
        }

        Transform t0 = _resolvedTargets[0];
        float d0 = _driftMeters != null && _driftMeters.Length > 0 ? _driftMeters[0] : 0f;
        Vector3 initLocal = _initialLocalPositions != null && _initialLocalPositions.Length > 0
            ? _initialLocalPositions[0]
            : Vector3.zero;
        Vector3 initWorld = _initialWorldPositions != null && _initialWorldPositions.Length > 0
            ? _initialWorldPositions[0]
            : Vector3.zero;

        // Debug.Log(
            // $"{LogTag} '{name}' {phase}: elapsed={elapsed:F3}s targets={count} drift0={d0:F4}m " +
            // $"t0='{(t0 != null ? t0.name : "null")}' active={(t0 != null && t0.gameObject.activeSelf)} " +
            // $"local={FormatV(t0 != null ? t0.localPosition : Vector3.zero)} initLocal={FormatV(initLocal)} " +
            // $"world={FormatV(t0 != null ? t0.position : Vector3.zero)} initWorld={FormatV(initWorld)} " +
            // $"parentScale={FormatV(transform.lossyScale)} speed={_speedMetersPerSecond:F3} space={_driftSpace} frame={Time.frameCount}.",
            // this);
    }

    private static string FormatV(Vector3 v) => $"({v.x:F3},{v.y:F3},{v.z:F3})";

    private Transform[] ResolveTargets()
    {
        if (_targets != null && _targets.Length > 0)
        {
            int valid = 0;
            for (int i = 0; i < _targets.Length; i++)
            {
                if (_targets[i] != null)
                {
                    valid++;
                }
            }

            var assigned = new Transform[valid];
            int idx = 0;
            for (int i = 0; i < _targets.Length; i++)
            {
                if (_targets[i] != null)
                {
                    assigned[idx++] = _targets[i];
                }
            }

            return assigned;
        }

        int childCount = transform.childCount;
        if (childCount == 0)
        {
            return System.Array.Empty<Transform>();
        }

        var list = new System.Collections.Generic.List<Transform>(childCount);
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child == null)
            {
                continue;
            }

            if (!_includeInactiveChildren && !child.gameObject.activeSelf)
            {
                continue;
            }
            list.Add(child);
        }

        return list.ToArray();
    }

    private void RestoreInitialPositions()
    {
        if (_resolvedTargets == null || _initialLocalPositions == null)
        {
            return;
        }

        int count = Mathf.Min(_resolvedTargets.Length, _initialLocalPositions.Length);
        for (int i = 0; i < count; i++)
        {
            Transform t = _resolvedTargets[i];
            if (t == null)
            {
                continue;
            }

            if (_driftSpace == Space.World && _initialWorldPositions != null && i < _initialWorldPositions.Length)
            {
                t.position = _initialWorldPositions[i];
            }
            else
            {
                t.localPosition = _initialLocalPositions[i];
            }
        }
    }



}
