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
}
