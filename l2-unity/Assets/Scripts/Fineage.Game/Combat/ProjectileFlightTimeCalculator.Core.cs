using UnityEngine;

public static partial class ProjectileFlightTimeCalculator
{
    public static float CalculateL2ArrowFlightTimeSeconds(float distanceUnity)
    {
        return CalculateL2AccelFlightTimeSeconds(distanceUnity);
    }

    public static float CalculateL2SkillFlightTimeSeconds(float distanceUnity)
    {
        return CalculateL2AccelFlightTimeSeconds(distanceUnity);
    }

    public static float CalculateL2ProjectileFlightTimeSeconds(float distanceUnity)
    {
        return CalculateL2AccelFlightTimeSeconds(distanceUnity);
    }
}
