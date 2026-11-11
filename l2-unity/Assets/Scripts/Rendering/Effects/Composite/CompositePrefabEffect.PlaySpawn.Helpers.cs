using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    void StartFinalShaderLifetimeRoutineIfNeeded(
        CompositePrefabPart part,
        Transform instanceTransform,
        EffectSettings partSettings)
    {
        if (part == null || instanceTransform == null || !part.disableShaderLifetime ||
            !part.enableFinalShaderLifetimeOnFade || partSettings == null)
        {
            return;
        }

        float finalWindow = Mathf.Max(part.finalShaderLifetimeMax, part.finalShaderLifetimeMin, 0f);
        float delay = Mathf.Max(0f, partSettings.defaultLifeTime - partSettings.hideTime - finalWindow);
        StartCoroutine(EnableFinalShaderLifetimeAfterDelay(
            instanceTransform, delay, part.finalShaderLifetimeMin, part.finalShaderLifetimeMax));
    }

    IEnumerator EnableFinalShaderLifetimeAfterDelay(Transform instanceTransform, float delay, float rangeMin, float rangeMax)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        if (instanceTransform == null)
        {
            yield break;
        }

        EffectShaderLifetimeHelper.Apply(instanceTransform, true, rangeMin, rangeMax);
    }

    void LogSpawnedPart(CompositePrefabPart part, EffectSettings partSettings, Transform setupOwner)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Debug.Log(
            // $"{DebugPrefix} Spawned part='{part.name}' point={part.attachmentPoint} " +
            // $"follow={part.followResolvedTransform} hitTime={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"lifeTime={partSettings.defaultLifeTime:F3}s scale={part.scale:F2} offset={part.positionOffset} owner='{(setupOwner != null ? setupOwner.name : "null")}'.");
#endif
    }
}
