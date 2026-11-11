using Unity.Jobs;
using UnityEngine;

public partial class ParticleGroup
{
    public bool TryGetHomeFlightProfile(out ParticleGroupHomeFlightProfile profile)
    {
        if (_hasRuntimeHomeFlightProfile)
        {
            profile = _runtimeHomeFlightProfile;
            return profile.isFlightAnchor;
        }

        if (!_homeFlightAnchor)
        {
            profile = default;
            return false;
        }

        profile = new ParticleGroupHomeFlightProfile
        {
            isFlightAnchor = true,
            pathSideOffsetMultiplier = _homePathSideOffsetMultiplier,
            pathSideOffsetScale = _homePathSideOffsetScale,
            pathHeightOffsetScale = _homePathHeightOffsetScale,
            speedScale = _homeFlightSpeedScale
        };
        return true;
    }

    public void ApplyRuntimeHomeFlightProfile(ParticleGroupHomeFlightProfile profile)
    {
        _hasRuntimeHomeFlightProfile = true;
        _runtimeHomeFlightProfile = profile;
    }

    public void ClearRuntimeHomeFlightProfile()
    {
        _hasRuntimeHomeFlightProfile = false;
        _runtimeHomeFlightProfile = default;
    }
}
