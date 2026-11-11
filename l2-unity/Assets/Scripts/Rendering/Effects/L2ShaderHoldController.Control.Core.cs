using UnityEngine;

public partial class L2ShaderHoldController
{
    public bool ShouldDeferStopForRelease(in Settings settings)
    {
        return settings.ShaderHold > 1e-4f && settings.ReleaseByCastProgress;
    }

    public bool TryBeginCastEndFadeDefer(in Settings settings, in CastTimeline timeline)
    {
        if (!ShouldDeferStopForRelease(in settings))
        {
            return false;
        }

        _castEndFadeRequested = true;
        _castEndHoldSnapshot = settings.ShaderHold;
        return true;
    }
}
