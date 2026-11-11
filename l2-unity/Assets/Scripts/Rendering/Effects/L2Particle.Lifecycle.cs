using System.Collections.Generic;
using UnityEngine;

public partial class L2Particle
{
    private void OnEnable()
    {
        if (_billboardToCamera)
        {
            RegisteredBillboards.Register(this);
        }
    }

    private void OnDisable()
    {
        if (_billboardToCamera)
        {
            RegisteredBillboards.Unregister(this);
        }
    }

    private void Awake()
    {
        _pooledEffect.ResetTimerCallback = () =>
        {
            ResetTimer();
        };
    }

    public void OnCameraPreRender(Camera camera)
    {
        ApplyBillboardRotation(camera);
    }

    private void LateUpdate()
    {
        if (_billboardToCamera)
        {
            ApplyBillboardRotation(Camera.main);
        }

        TryDestroyWhenAuthoredStreamsComplete();
    }

    private void ApplyBillboardRotation(Camera camera)
    {
        if (!_billboardToCamera || camera == null)
        {
            return;
        }

        // Copy camera orientation so the whole mesh turns as an object, preserving its authored 3D shape.
        transform.rotation = camera.transform.rotation * Quaternion.Euler(_billboardRotationOffsetEuler);
    }

    protected override void OnDestroy()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        float now = Time.time;
        float elapsed = _playStartedAt > 0f ? now - _playStartedAt : -1f;
        // DropAdenaDestroyLog.Event(
            // "L2PARTICLE_ON_DESTROY",
            // this,
            // $"wallLived={elapsed:F3}s configuredLife={(_settings != null ? _settings.defaultLifeTime : -1f):F3}s " +
            // $"castHit={(_castData != null ? _castData.HitTime : -1f):F3}s " +
            // $"settings='{(_settings != null ? _settings.name : "null")}'",
            // includeStack: true);
        if (ShouldTraceLifetime())
        {
            // Debug.Log(
                // $"[L2Particle] FX_DESTROY effect='{name}' now={now:F3}s " +
                // $"wallLived={elapsed:F3}s configuredLife={(_settings != null ? _settings.defaultLifeTime : -1f):F3}s " +
                // $"castHit={(_castData != null ? _castData.HitTime : -1f):F3}s.");
        }
#endif
        base.OnDestroy();
    }
}
