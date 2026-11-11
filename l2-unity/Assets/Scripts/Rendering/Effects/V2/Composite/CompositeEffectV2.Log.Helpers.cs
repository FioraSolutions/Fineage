using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class CompositeEffectV2 : CompositePrefabEffect
{
    static void LogHomeSpawnPart(
        CompositePart part,
        Transform followTransform,
        Vector3 worldPosition,
        Vector3 spawnPosition,
        Vector3 afterInstantiate,
        Vector3 afterAttach,
        BaseEffect instance,
        Transform setupOwner)
    {
        Transform target = null;
        Vector3 targetPos = Vector3.zero;
        if (followTransform != null)
        {
            target = followTransform;
            targetPos = followTransform.position;
        }

        Vector3 rendererPos = instance != null ? instance.transform.position : Vector3.zero;
        Renderer rend = instance != null ? instance.GetComponentInChildren<Renderer>(true) : null;
        if (rend != null)
        {
            rendererPos = rend.bounds.center;
        }

        string spawnPoint = part.spawnAttachmentPoint.ToString();
        string placementName = part.placement != null ? part.placement.GetType().Name : "null";
        float dYResolved = worldPosition.y - targetPos.y;
        float dYGo = instance != null ? instance.transform.position.y - targetPos.y : 0f;

        // Debug.LogWarning(
            // $"[HOME_SPAWN] SPAWN part='{part.name}' spawnPoint={spawnPoint} type={part.GetType().Name} " +
            // $"placement={placementName} follow={part.follow} " +
            // $"followTf='{(followTransform != null ? followTransform.name : "null")}' " +
            // $"setupOwner='{(setupOwner != null ? setupOwner.name : "null")}'\n" +
            // $"  targetPos={targetPos:F3} resolvedWorld={worldPosition:F3} spawnPos={spawnPosition:F3}\n" +
            // $"  afterInstantiate={afterInstantiate:F3} afterAttach={afterAttach:F3} " +
            // $"goPos={(instance != null ? instance.transform.position.ToString("F3") : "-")} " +
            // $"parent='{(instance != null && instance.transform.parent != null ? instance.transform.parent.name : "null")}'\n" +
            // $"  rendererCenter={rendererPos:F3} " +
            // $"dY_go-target={dYGo:F3} dY_resolved-target={dYResolved:F3}");

        Debug.DrawLine(targetPos, worldPosition, Color.magenta, 8f);
        Debug.DrawRay(worldPosition, Vector3.up * 0.4f, Color.cyan, 8f);
    }

    void LogV2TaDestroyState(string stage)
    {
        int delayedTa = 0;
        for (int i = 0; i < _pendingDelayed.Count; i++)
        {
            // if (_pendingDelayed[i] != null && V2TaSpawnLog.Matches(_pendingDelayed[i].Part))
            {
                delayedTa++;
            }
        }

        int shootTa = 0;
        for (int i = 0; i < _pendingAnimationShoot.Count; i++)
        {
            // if (V2TaSpawnLog.Matches(_pendingAnimationShoot[i]))
            {
                shootTa++;
            }
        }

        if (delayedTa == 0 && shootTa == 0)
        {
            return;
        }

        // V2TaSpawnLog.Warn(
            // "DESTROY " + stage +
            // " composite='" + name +
            // "' delayedTa=" + delayedTa +
            // " shootPendingTa=" + shootTa +
            // " spawned=" + _spawned.Count +
            // " sincePlay=" + (Time.time - _playStartedAt).ToString("0.###") +
            // "s frame=" + Time.frameCount);
    }
}
