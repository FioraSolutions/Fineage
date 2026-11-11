using System;
using UnityEngine;
using System.Collections.Generic;

public partial class HomeProjectileTrailVelocityProvider
{
    private static void BuildTrailPerpendicularBasis(Vector3 tangent, out Vector3 right, out Vector3 up)
    {
        if (tangent.sqrMagnitude < 1e-8f)
        {
            tangent = Vector3.forward;
        }
        else
        {
            tangent = tangent.normalized;
        }

        Vector3 worldUp = Mathf.Abs(Vector3.Dot(tangent, Vector3.up)) > 0.92f ? Vector3.forward : Vector3.up;
        right = Vector3.Cross(worldUp, tangent);
        if (right.sqrMagnitude < 1e-8f)
        {
            right = Vector3.right;
        }
        else
        {
            right.Normalize();
        }

        up = Vector3.Cross(tangent, right).normalized;
    }

    private void ApplyVelocity(TrailBinding binding)
    {
        Vector3 velocity = binding.smoothedVelocity * binding.velocityScale;
        if (binding.invertTrailDirection)
        {
            velocity = -velocity;
        }

        velocity *= binding.trailSign >= 0f ? 1f : -1f;

        if (binding.convertWorldToLocal && binding.localSpaceReference != null)
        {
            velocity = binding.localSpaceReference.InverseTransformDirection(velocity);
        }

        if (_debugLogs && velocity.sqrMagnitude > 0.000001f)
        {
            // Debug.Log($"[HomeProjectileTrailVelocityProvider] binding='{binding.name}' velocity={velocity:F4}", this);
        }

        if (binding.targetRenderers == null)
        {
            return;
        }

        for (int i = 0; i < binding.targetRenderers.Length; i++)
        {
            Renderer renderer = binding.targetRenderers[i];
            if (renderer == null)
            {
                continue;
            }

            Material[] materials = renderer.materials;
            for (int m = 0; m < materials.Length; m++)
            {
                Material mat = materials[m];
                if (mat == null)
                {
                    continue;
                }

                if (mat.HasProperty(StartVelocityRangeXID))
                {
                    mat.SetColor(StartVelocityRangeXID, BuildRangeColor(velocity.x, binding.rangeSpread));
                }
                if (mat.HasProperty(StartVelocityRangeYID))
                {
                    mat.SetColor(StartVelocityRangeYID, BuildRangeColor(velocity.y, binding.rangeSpread));
                }
                if (mat.HasProperty(StartVelocityRangeZID))
                {
                    mat.SetColor(StartVelocityRangeZID, BuildRangeColor(velocity.z, binding.rangeSpread));
                }
            }
        }
    }
}
