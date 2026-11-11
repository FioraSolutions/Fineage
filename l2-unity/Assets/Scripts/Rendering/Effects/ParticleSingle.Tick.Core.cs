using UnityEngine.Rendering;
using UnityEngine;

public partial class ParticleSingle
{
    private void Update()
    {
        if (_testSpawnOnlyNoTeardown)
        {
            return;
        }

        if (_lifetime.Stopped && !_holdController.CastEndFadeRequested)
        {
            return;
        }

        if (!_holdController.CanApplyHoldUpdates(_lifetime.Stopped))
        {
            return;
        }

        ApplyCompositeShaderHoldToAllRuntimeMaterials(Now());
    }
}
