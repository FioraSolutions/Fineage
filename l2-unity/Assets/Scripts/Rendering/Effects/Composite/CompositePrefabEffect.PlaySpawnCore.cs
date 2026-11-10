using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class CompositePrefabEffect
{
    protected void RaiseStartSpawnAndMaybeSpawnLight()
    {
        StartSpawn?.Invoke(this, _context);
        if (!_useLight || _lightSettings == null || _lightSpawned)
        {
            return;
        }

        if (!EffectLightPlacement.TryResolve(_context, _lightSettings, out Vector3 lightPoint, out Vector3 lightDir))
        {
            // Debug.LogWarning($"{DebugPrefix} useLight=true but light placement failed (settings={_lightSettings.name}).");
            return;
        }

        _lightSpawned = true;
        FNManagerLight.Ensure().SpawnHitFlash(lightPoint, lightDir, _lightSettings);
    }

    protected virtual void PreparePartPlayback(CompositePrefabPart part, BaseEffect instance)
    {
    }
}
