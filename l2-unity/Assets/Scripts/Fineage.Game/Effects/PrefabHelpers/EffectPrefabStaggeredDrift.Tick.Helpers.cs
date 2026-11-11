using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public sealed partial class EffectPrefabStaggeredDrift : MonoBehaviour
{
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
