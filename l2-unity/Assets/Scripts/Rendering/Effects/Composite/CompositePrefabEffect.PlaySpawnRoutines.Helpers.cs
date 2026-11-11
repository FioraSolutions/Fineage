using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void StartPendingPartsRoutineIfNeeded()
    {
        if (_pendingParts.Count > 0 && _pendingSpawnRoutine == null)
        {
            _pendingSpawnRoutine = StartCoroutine(SpawnPendingPartsRoutine());
        }
    }

    IEnumerator SpawnPendingPartsRoutine()
    {
        while (_pendingParts.Count > 0)
        {
            float now = Time.time;
            for (int i = _pendingParts.Count - 1; i >= 0; i--)
            {
                PendingCompositePart pending = _pendingParts[i];
                if (pending == null || pending.Part == null)
                {
                    _pendingParts.RemoveAt(i);
                    continue;
                }

                if (now >= pending.SpawnAtTime)
                {
                    SpawnPart(pending.Part);
                    _pendingParts.RemoveAt(i);
                }
            }

            yield return null;
        }

        _pendingSpawnRoutine = null;
    }

    void DestroyCompositeByLifetime()
    {
        EffectSettings lifeTimeSettings = SelectLifetimeSettings();
        if (lifeTimeSettings != null)
        {
            DestoryEffect(lifeTimeSettings, _castData);
        }
    }
}
