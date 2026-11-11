using UnityEngine;

public sealed partial class HomeProjectilePath
{
    public void Configure(CompositeHomeProjectileConfig config, ParticleGroupHomeFlightProfile profile)
    {
        useArc = config.usePathArc;
        startLineFactor = config.pathStartLineFactor;
        apexAlongLine = config.pathApexAlongLine;
        peakHeightAlongLine = config.pathPeakHeightAlongLine;
        sideOffset = config.pathSideOffset * Mathf.Max(0.01f, profile.pathSideOffsetScale);
        heightOffset = config.pathHeightOffset * Mathf.Max(0.01f, profile.pathHeightOffsetScale);
        distanceHeightFactor = config.pathDistanceHeightFactor;
        earlyClimbFactor = Mathf.Clamp01(config.pathEarlyClimbFactor);
        ascentSpeedScale = Mathf.Max(0.01f, config.pathAscentSpeedScale);
        descentSpeedScale = Mathf.Max(0.01f, config.pathDescentSpeedScale);
        sideOffsetMultiplier = profile.pathSideOffsetMultiplier;
        heightOffsetScale = Mathf.Max(0.01f, profile.pathHeightOffsetScale);
    }

    public Vector3 Step(
        Vector3 start,
        Vector3 current,
        Vector3 target,
        float speed,
        float deltaTime,
        ref float traveledDistance,
        ref float pathDistanceEstimate,
        out bool pathCompleted)
    {
        pathCompleted = false;
        if (!useArc)
        {
            return Vector3.MoveTowards(current, target, speed * deltaTime);
        }

        pathDistanceEstimate = Mathf.Max(pathDistanceEstimate, Estimate(start, target));
        float currentPathT = traveledDistance / Mathf.Max(0.01f, pathDistanceEstimate);
        float step = speed * ResolveSpeedScale(currentPathT) * deltaTime;
        traveledDistance += step;
        float pathT = traveledDistance / Mathf.Max(0.01f, pathDistanceEstimate);
        if (pathT >= 1f)
        {
            pathCompleted = true;
            return target;
        }

        ResolveControlPoints(start, target, out Vector3 controlA, out Vector3 controlB);
        return EvaluateCubicBezier(start, controlA, controlB, target, pathT);
    }

    public float Estimate(Vector3 start, Vector3 target)
    {
        ResolveControlPoints(start, target, out Vector3 controlA, out Vector3 controlB);
        return Vector3.Distance(start, controlA) +
               Vector3.Distance(controlA, controlB) +
               Vector3.Distance(controlB, target);
    }

    float ResolveSpeedScale(float pathT)
    {
        return pathT < ResolveApexT() ? ascentSpeedScale : descentSpeedScale;
    }
}
