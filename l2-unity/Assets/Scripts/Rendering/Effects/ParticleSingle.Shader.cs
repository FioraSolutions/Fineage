using UnityEngine;
using UnityEngine.Rendering;

public partial class ParticleSingle
{
    private void ApplyCompositeShaderHoldToAllRuntimeMaterials(float now)
    {
        if (!_holdController.CanApplyHoldUpdates(_lifetime.Stopped))
        {
            return;
        }

        Renderer renderer = ResolveRenderer();
        if (renderer == null)
        {
            return;
        }

        if (_holdController.ApplyToMaterials(
                renderer.materials,
                BuildHoldSettings(),
                BuildCastTimeline(now),
                name,
                _lifetime.SpawnedAt,
                _lifetime.BaseShaderLifetime))
        {
            _lifetime.Stopped = true;
        }
    }

    private L2ShaderHoldController.Settings BuildHoldSettings()
    {
        return new L2ShaderHoldController.Settings
        {
            ShaderHold = _shaderHold,
            ReleaseByCastProgress = _releaseShaderHoldByCastProgress,
            ReleaseStartNormalized = _shaderHoldReleaseStartNormalized,
            SmoothRelease = _smoothShaderHoldRelease,
            CastEndHoldReleaseLead = _castEndHoldReleaseLead
        };
    }

    private L2ShaderHoldController.CastTimeline BuildCastTimeline(float now)
    {
        float castStart = _lifetime.LastEnable;
        if (_castData != null && _castData.StartTime > 0f)
        {
            castStart = _castData.StartTime;
        }

        return new L2ShaderHoldController.CastTimeline
        {
            Now = now,
            CastStartTime = castStart,
            SlotDuration = _duration,
            Settings = _settings
        };
    }

    private void UpdateOwnerWorldPos()
    {
        Renderer renderer = ResolveRenderer();
        if (renderer == null)
        {
            return;
        }

        Material[] materials = renderer.materials;
        for (int i = 0; i < materials.Length; i++)
        {
            SetOwnerWorldPos(materials[i]);
        }
    }
}
