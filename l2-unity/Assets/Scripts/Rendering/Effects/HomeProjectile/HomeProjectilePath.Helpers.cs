using UnityEngine;

public sealed partial class HomeProjectilePath
{
    float ResolveApexT()
    {
        return apexAlongLine > 0f
            ? Mathf.Clamp01(apexAlongLine)
            : Mathf.Clamp01(0.46f + startLineFactor * 0.2f);
    }

    void ResolveControlPoints(Vector3 start, Vector3 target, out Vector3 controlA, out Vector3 controlB)
    {
        Vector3 lateral = ResolveLateralDirection(start, target);
        Vector3 flat = target - start;
        flat.y = 0f;
        float horizontalDistance = flat.magnitude;
        float peakHeight = heightOffset + horizontalDistance * distanceHeightFactor * heightOffsetScale;
        float apexT = ResolveApexT();
        float peakHeightT = peakHeightAlongLine > 0f
            ? Mathf.Clamp01(peakHeightAlongLine)
            : 0.5f;
        float signedSide = sideOffset * sideOffsetMultiplier;

        controlA = Vector3.LerpUnclamped(start, target, startLineFactor);
        controlA += lateral * signedSide;
        controlA.y = Mathf.Max(controlA.y, start.y + peakHeight * earlyClimbFactor);

        Vector3 launchAnchor = Vector3.LerpUnclamped(start, target, apexT);
        controlB = launchAnchor + lateral * signedSide;

        Vector3 peakHeightAnchor = Vector3.Lerp(start, target, peakHeightT);
        float peakBaseY = Mathf.Max(start.y, peakHeightAnchor.y);
        controlB.y = Mathf.Max(controlB.y, peakBaseY + peakHeight);
    }

    Vector3 ResolveLateralDirection(Vector3 start, Vector3 target)
    {
        if (caster != null)
        {
            Vector3 casterSide = -caster.right;
            casterSide.y = 0f;
            if (casterSide.sqrMagnitude > 0.000001f)
            {
                return casterSide.normalized;
            }
        }

        Vector3 horizontal = target - start;
        horizontal.y = 0f;
        if (horizontal.sqrMagnitude > 0.000001f)
        {
            return Vector3.Cross(Vector3.up, horizontal.normalized);
        }

        return Vector3.left;
    }

    static Vector3 EvaluateCubicBezier(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 end, float t)
    {
        float inv = 1f - t;
        return inv * inv * inv * start +
               3f * inv * inv * t * controlA +
               3f * inv * t * t * controlB +
               t * t * t * end;
    }
}
