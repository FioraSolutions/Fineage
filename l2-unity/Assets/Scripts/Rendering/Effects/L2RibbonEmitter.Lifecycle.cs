using System.Collections.Generic;
using UnityEngine;

public sealed partial class L2RibbonEmitter : EffectPart
{
    public override void Initialize(EffectSettings settings, float baseSize)
    {
        base.Initialize(settings, baseSize);
        EnsureMesh();
    }

    public override void Setup(EffectSettings settings, MagicCastData castData)
    {
        _settings = settings;
        _castData = castData;
        EnsureMesh();
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        Log(
            $"Setup owner='{(OwnerTarget != null ? OwnerTarget.name : "null")}' " +
            $"follow='{(FollowTarget != null ? FollowTarget.name : "null")}'");
    }

    public override void PlayPart()
    {
        EnsureMesh();
        if (propBlock == null)
        {
            propBlock = new MaterialPropertyBlock();
        }

        ResolveBladeAnchors();
        ApplyOpacityToMaterial();
        _points.Clear();
        _sampleTimer = 0f;
        _sampleAttempts = 0;
        _sampleAccepted = 0;
        _playing = true;
        _loggedPlayOnce = false;
        _nextDebugLogTime = 0f;
        _traceFrame = 0;
        _havePrevBlade = false;
        _playStartedAt = Time.time;
        _lastLoggedPointCount = -1;

        if (_meshFilter != null)
        {
            _meshFilter.sharedMesh = _mesh;
        }

        if (targetRenderer != null)
        {
            targetRenderer.enabled = true;
        }

        bool ok = TrySample(force: true);
        if (ok && _seedStarterStrip && _points.Count == 1)
        {
            SeedStarterStrip();
        }

        RebuildMesh();

        Shader shader = targetRenderer != null && targetRenderer.sharedMaterial != null
            ? targetRenderer.sharedMaterial.shader
            : null;
        Log(
            $"PlayPart okSample={ok} tip='{(_swordTip != null ? _swordTip.name : "null")}' " +
            $"base='{(_swordBase != null ? _swordBase.name : "null")}' " +
            $"bone='{(_sampleBone != null ? _sampleBone.name : "null")}' " +
            $"source={_lastSampleSource} points={_points.Count} " +
            $"owner='{(OwnerTarget != null ? OwnerTarget.name : "null")}' " +
            $"renderer={(targetRenderer != null ? targetRenderer.enabled.ToString() : "null")} " +
            $"mat={(targetRenderer != null && targetRenderer.sharedMaterial != null ? targetRenderer.sharedMaterial.name : "null")} " +
            $"shader={(shader != null ? shader.name : "null")} supported={(shader != null && shader.isSupported)} " +
            $"pos={transform.position}");
        _loggedPlayOnce = true;
    }
}
