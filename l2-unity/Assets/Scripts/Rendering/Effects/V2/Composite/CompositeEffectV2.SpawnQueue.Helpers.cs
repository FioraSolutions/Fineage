using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class CompositeEffectV2
{
    void QueueV2Parts()
    {
        _pendingDelayed.Clear();
        _pendingHitCollider.Clear();
        _pendingAnimationShoot.Clear();

        for (int i = 0; i < _v2Parts.Length; i++)
        {
            CompositePart part = _v2Parts[i];
            if (part == null || !part.IsSpawnable)
            {
                continue;
            }

            if (part.spawnTiming == CompositePartSpawnTiming.OnHitCollider)
            {
                _pendingHitCollider.Add(part);
                continue;
            }

            if (part.spawnTiming == CompositePartSpawnTiming.OnAnimationShoot)
            {
                _pendingAnimationShoot.Add(part);
                if (TraceCrasherWind())
                    Debug.Log("[WINDSTRIKE] Composite queue shoot part=" + part.name);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // if (V2TaSpawnLog.Matches(part))
                {
                    // V2TaSpawnLog.Info(
                        // "QUEUE shoot part='" + part.name +
                        // "' hit=" + (_castData != null ? _castData.HitTime.ToString("0.###") : "-") +
                        // "s shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
                        // "s wantsShoot=" + part.WantsAnimationShoot);
                }
#endif
                continue;
            }

            float delay = CompositeEffectUtilities.ResolveSpawnDelay(part.spawnTiming, _castData);
            delay += Mathf.Max(0f, part.spawnDelaySeconds);
            if (delay <= 0f)
            {
                if (TraceCrasherWind())
                    Debug.Log("[WINDSTRIKE] Composite spawn now part=" + part.name);
                SpawnV2Part(part);
                continue;
            }

            float spawnAt = Time.time + delay;
            _pendingDelayed.Add(new PendingV2Part
            {
                Part = part,
                SpawnAtTime = spawnAt
            });
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // if (V2TaSpawnLog.Matches(part))
            {
                float compositeLife = _rootRuntimeSettings != null
                    ? _rootRuntimeSettings.defaultLifeTime
                    : (_settings != null ? _settings.defaultLifeTime : -1f);
                float destroyAt = Time.time + Mathf.Max(0f, compositeLife);
                // V2TaSpawnLog.Warn(
                    // "QUEUE delayed part='" + part.name + "' type=" + part.GetType().Name +
                    // " timing=" + part.spawnTiming +
                    // " delay=" + delay.ToString("0.###") +
                    // "s spawnAt=" + spawnAt.ToString("F3") +
                    // " compositeLife=" + compositeLife.ToString("0.###") +
                    // "s destroyAt=" + destroyAt.ToString("F3") +
                    // " hit=" + (_castData != null ? _castData.HitTime.ToString("0.###") : "-") +
                    // "s shoot=" + (_castData != null ? _castData.serverTimeToShoot.ToString("0.###") : "-") +
                    // "s outlives=" + part.OutlivesComposite +
                    // (destroyAt <= spawnAt + 0.02f
                        // ? " RACE: composite dies at/before delayed spawn"
                        // : string.Empty));
            }
#endif
        }
    }
}
