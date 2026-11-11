using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class OrbitalTrailVelocityProvider
{
    void ApplyVelocityToMaterials(Material[] materials, TrailBinding binding, Vector3 velocity)
    {
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat == null)
            {
                continue;
            }

            if (binding.setDirectionFlags)
            {
                if (mat.HasProperty(UseDirectionAsID))
                {
                    mat.SetFloat(UseDirectionAsID, 1f);
                }
                if (mat.HasProperty(GetVelocityDirectionFromID))
                {
                    mat.SetFloat(GetVelocityDirectionFromID, 1f);
                }
            }

            // Unified V2 sprite/mesh: StartVelocityRange*Uc is UU, Z-up.
            Vector3 velocityUe = UnityMetersToUeUu(velocity);
            SetRangeProperty(mat, StartVelocityRangeXUcID, velocityUe.x, binding.rangeSpread);
            SetRangeProperty(mat, StartVelocityRangeYUcID, velocityUe.y, binding.rangeSpread);
            SetRangeProperty(mat, StartVelocityRangeZUcID, velocityUe.z, binding.rangeSpread);
            // Legacy ice-bolt / vampiric shaders still read the Unity-meter names.
            SetRangeProperty(mat, StartVelocityRangeXID, velocity.x, binding.rangeSpread);
            SetRangeProperty(mat, StartVelocityRangeYID, velocity.y, binding.rangeSpread);
            SetRangeProperty(mat, StartVelocityRangeZID, velocity.z, binding.rangeSpread);
        }
    }

    void SetRangeProperty(Material mat, int id, float value, float rangeSpread)
    {
        if (mat.HasProperty(id))
        {
            mat.SetVector(id, BuildRangeVector(value, rangeSpread));
        }
    }

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
