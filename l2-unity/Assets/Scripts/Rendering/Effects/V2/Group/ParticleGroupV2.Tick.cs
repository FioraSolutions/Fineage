using System.Collections.Generic;
using UnityEngine;

public partial class ParticleGroupV2
{
    void Update()
    {
        if (!_streamVisible)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (!_loggedHiddenTick && _runtime != null)
            {
                _loggedHiddenTick = true;
                // Debug.LogWarning(
                    // "[ParticleGroupV2] Tick SKIP hidden '" + name +
                    // "' — streamVisible=false, particles will not spawn/draw.");
            }
#endif
            TraceWindOrb();
            return;
        }

        _runtime?.Tick(Now());
        TraceWindOrb();
    }

    void LateUpdate()
    {
        if (!_streamVisible)
        {
            return;
        }

        _runtime?.LateDraw();
    }

    void OnDisable()
    {
        if (_runtime == null)
        {
            return;
        }

        _runtime.Stop(_destroying ? EmitterStopMode.Kill : EmitterStopMode.Drain);
    }

    void OnDestroy()
    {
        _destroying = true;
        _runtime?.Stop(EmitterStopMode.Kill);
        _runtime?.Dispose();
        _runtime = null;
        _batch?.Release();
        _batch = null;
        _slots = null;
        DestroyRuntimeMaterialInstances();
    }

    void TraceWindOrb()
    {
        if (!_windFlightTrace || _runtime == null || _windTraceStage >= 2)
        {
            return;
        }

        float age = Time.time - _windPlayAt;
        if (_windTraceStage == 0)
        {
            _windTraceStage = 1;
        }
        else if (age < 0.2f)
        {
            return;
        }
        else
        {
            _windTraceStage = 2;
        }

        Debug.Log(
            "[WINDORB] t=" + age.ToString("0.###") +
            " caster=" + _windCaster +
            " emitter=" + name +
            " active=" + _runtime.CountActive() +
            " spawned=" + _runtime.SpawnedTotal +
            " parent=" + (transform.parent != null ? transform.parent.name : "none"));
    }
}
