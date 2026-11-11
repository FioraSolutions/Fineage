using UnityEngine;

public partial class L2ShaderHoldController
{
    public bool CanApplyHoldUpdates(bool stopped)
    {
        if (stopped && !_castEndFadeRequested)
        {
            return false;
        }

        return _hasRuntimeLoopOverride || _castEndFadeRequested;
    }

    public void SetRuntimeLoopOverride(bool hasOverride, bool value)
    {
        _hasRuntimeLoopOverride = hasOverride;
        _runtimeLoopOverrideValue = value;
    }

    public void ResetReleaseLogs()
    {
        _loggedReleaseStart = false;
        _loggedReleaseResume = false;
    }

    public void ResetCastEndFade()
    {
        _castEndFadeRequested = false;
        _castEndHoldSnapshot = -1f;
    }

    public void CaptureBaseEmitterAlpha(Material[] materials)
    {
        _baseEmitterAlpha = L2MaterialPropertyCopier.ReadFloatFromFirstMaterial(
            materials,
            L2MaterialPropertyCopier.EmitterAlphaId,
            1f);
    }
}
