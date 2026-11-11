using UnityEngine;
using System.Collections.Generic;

public partial class ParticleGroupV2
{
    public void BindHostOwnedEmission()
    {
        _hostOwnedEmission = true;
    }

    public void BindAuthoredTiming()
    {
        _forceAuthoredTiming = true;
        _hasExternalEmissionWindow = false;
        _lifetimePolicy = ParticleLifetimePolicy.Authored;
        _emissionWindow = Mathf.Max(0.01f, _duration);
    }

    public void SetStreamVisible(bool visible)
    {
        _streamVisible = visible;
        _runtime?.SetVisible(visible);
    }

    public void SetEmissionWindow(float windowSeconds, EmitterStopMode stopMode)
    {
        _hasExternalEmissionWindow = true;
        _emissionWindow = Mathf.Max(0.01f, windowSeconds);
        _instantKillAtCastEnd = stopMode == EmitterStopMode.Kill;
        _lifetimePolicy = ParticleLifetimePolicy.EmissionWindowFromCast;
    }
}
