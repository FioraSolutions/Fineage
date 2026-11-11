using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HomeProjectileTrailVelocityProvider
{
    private Vector3 GetTrailTangentByDistance(TrailBinding binding, float sampleDistance)
    {
        const float delta = 0.01f;
        Vector3 before = SampleHistoryByDistance(binding, sampleDistance - delta);
        Vector3 after = SampleHistoryByDistance(binding, sampleDistance + delta);
        Vector3 deltaPos = after - before;
        if (deltaPos.sqrMagnitude > 1e-8f)
        {
            return deltaPos.normalized;
        }

        if (binding.smoothedVelocity.sqrMagnitude > 1e-8f)
        {
            return binding.smoothedVelocity.normalized;
        }

        return Vector3.forward;
    }

    private Color BuildRangeColor(float value, float rangeSpread)
    {
        float spread = Mathf.Max(_minimumAxisRange, Mathf.Abs(value) * rangeSpread);
        return new Color(value - spread, value + spread, 0f, 0f);
    }
}
