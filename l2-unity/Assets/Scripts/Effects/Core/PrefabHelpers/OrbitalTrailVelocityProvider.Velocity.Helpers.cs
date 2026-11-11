using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class OrbitalTrailVelocityProvider
{
    static Vector3 UnityMetersToUeUu(Vector3 unityMeters)
    {
        return new Vector3(
            unityMeters.x * UnityMetersToUu,
            unityMeters.z * UnityMetersToUu,
            unityMeters.y * UnityMetersToUu);
    }

    private bool TryGetSyntheticVelocity(TrailBinding binding, out Vector3 velocity)
    {
        velocity = Vector3.zero;
        if (_rotatorSource == null || _orbitCenter == null)
        {
            return false;
        }

        Vector3 axisWorld = _rotatorSource.RotationAxis.sqrMagnitude > 0f
            ? _rotatorSource.RotationAxis.normalized
            : Vector3.up;
        if (_rotatorSource.RotationSpace == Space.Self)
        {
            axisWorld = _rotatorSource.transform.TransformDirection(axisWorld).normalized;
        }

        float angularSpeedRad = _rotatorSource.DegreesPerSecond * Mathf.Deg2Rad;
        if (_clockwise)
        {
            angularSpeedRad *= -1f;
        }

        Vector3 radius = ResolveOrbitRadiusWorld(binding);
        if (radius.sqrMagnitude < (_minimumOrbitRadius * _minimumOrbitRadius))
        {
            return false;
        }

        velocity = Vector3.Cross(axisWorld * angularSpeedRad, radius);
        return velocity.sqrMagnitude > 0f;
    }

    private Vector4 BuildRangeVector(float value, float rangeSpread)
    {
        float spread = Mathf.Max(_minimumAxisRange, Mathf.Abs(value) * rangeSpread);
        return new Vector4(value - spread, value + spread, 0f, 0f);
    }
}
