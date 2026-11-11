using System;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public partial class EffectManager
{
    public void PlayerImpactEffect(int id, Vector3 point, MagicCastData castData = null)
    {
        PlayerImpactEffect(id, point, Vector3.zero, castData);
    }

    public void PlayerImpactEffect(int id, Vector3 point, Vector3 impactDirection, MagicCastData castData = null)
    {
        // Debug.Log(
            // $"[HIT_FX] 7.EffectManager.PlayerImpactEffect ENTER frame={Time.frameCount} t={Time.time:F3} " +
            // $"effectId={id} point={point} dir={impactDirection}");

        if (!TryGetRuntime(id, out RuntimeEffect entry) || entry.Prefab == null)
        {
            Debug.LogWarning(
                "[EffectManager] PlayerImpactEffect abort id=" + id +
                " found=" + _byId.ContainsKey(id) +
                " prefab=" + (entry.Prefab != null ? entry.Prefab.name : "null") +
                " comment=" + entry.Comment);
            return;
        }

        Transform effectsRoot = EnsureActiveEffectsContainer();

        Vector3 dir = ResolveImpactDirection(point, impactDirection);
        Quaternion rotation = Quaternion.LookRotation(dir, Vector3.up) *
            Quaternion.Euler(0f, IMPACT_HIT_DIRECTION_YAW_OFFSET_DEGREES, 0f);

        GameObject dummy = new GameObject("HitPointProxy");
        dummy.transform.SetPositionAndRotation(point, rotation);
        if (effectsRoot != null)
            dummy.transform.SetParent(effectsRoot, true);

        BaseEffect instance = Instantiate(entry.Prefab, point, rotation, dummy.transform);
        InjectSpawned(instance);
        _impactSpawnCounter += 1;
        string pointKey = $"{Mathf.Round(point.x * 10f) / 10f},{Mathf.Round(point.y * 10f) / 10f},{Mathf.Round(point.z * 10f) / 10f}";
        float now = Time.time;
        if (_impactWindowByPoint.TryGetValue(pointKey, out var state))
        {
            if (now - state.windowStartSec > IMPACT_WINDOW_SEC)
            {
                state = (0, now);
            }
            state.count += 1;
            _impactWindowByPoint[pointKey] = state;
        }
        else
        {
            _impactWindowByPoint[pointKey] = (1, now);
            state = _impactWindowByPoint[pointKey];
        }
        Vector3 emitterRight = rotation * Vector3.right;
        Vector3 emitterFwd = rotation * Vector3.forward;
        Vector3 playerToHit = Vector3.zero;
        Vector3 playerFwd = Vector3.zero;
        if (DomainPacketServices.LocalPlayer != null)
        {
            playerToHit = hitPointFlat(point - DomainPacketServices.LocalPlayer.transform.position);
            playerFwd = hitPointFlat(DomainPacketServices.LocalPlayer.transform.forward);
        }

        float angDirVsPlayerToHit = AngleFlatDeg(dir, playerToHit);
        // Debug.Log(
            // $"{IMPACT_DEBUG_TAG} PlayerImpactEffect spawn#{_impactSpawnCounter} effectId={id} " +
            // $"point={point} dirIn={impactDirection} dirResolved={dir} " +
            // $"emitterRight(+X travel)={emitterRight} emitterFwd={emitterFwd} " +
            // $"playerToHit={playerToHit} playerFwd={playerFwd} angDirVsPlayerToHit={angDirVsPlayerToHit:F1} " +
            // $"yawOffset={IMPACT_HIT_DIRECTION_YAW_OFFSET_DEGREES} " +
            // $"countNearPointWindow={state.count} windowSec={IMPACT_WINDOW_SEC:F2}");

        instance.gameObject.SetActive(true);
        instance.Setup(entry.Settings, castData, dummy.transform);
        // Child parts often use inheritRotation=0 / CasterCenter — push dir into composite context
        // so ResolvePartSpawnRotation orients meshes even when proxy rotation would be ignored.
        if (instance is CompositePrefabEffect composite)
        {
            composite.SetImpactHit(point, dir);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // EffectDoublePlayLog.TrackManagerPlay(id, entry.Prefab != null ? entry.Prefab.name : "null");
#endif
        instance.Play();
        // Debug.Log(
            // $"[HIT_FX] 7.EffectManager PLAY OK spawn#{_impactSpawnCounter} effectId={id} " +
            // $"prefab={entry.Prefab.name} point={point}");
    }
}
