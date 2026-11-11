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
}
