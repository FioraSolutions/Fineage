using UnityEngine;

public static partial class ProjectileFlightTimeCalculator
{
    public static float CalculateL2AccelFlightTimeSeconds(float distanceUnity)
    {
        if (distanceUnity <= 0f)
        {
            return 0.01f;
        }

        float flySec = Mathf.Sqrt((2f * distanceUnity) / L2ProjectileAccelUnityPerSec2);
        return Mathf.Max(flySec, 0.01f);
    }

    public static float CalculateL2ArrowFlightTimeIfConstantSpeed(float distanceUnity)
    {
        if (distanceUnity <= 0f)
        {
            return 0.01f;
        }

        return Mathf.Max(distanceUnity / L2ArrowSpeedUnityPerSec, 0.01f);
    }

    public static float CalculateL2AccelJourneyProgress(float elapsedSec, float flyTimeSec)
    {
        float t = Mathf.Clamp01(elapsedSec / Mathf.Max(flyTimeSec, 0.01f));
        return t * t;
    }

    public static float CalculateL2SkillJourneyProgress(float elapsedSec, float flyTimeSec)
    {
        return CalculateL2AccelJourneyProgress(elapsedSec, flyTimeSec);
    }
}
