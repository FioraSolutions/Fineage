using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    public static float ReadLifetimeMax(Material[] sharedMaterials, float fallback)
    {
        if (sharedMaterials == null)
        {
            return fallback;
        }

        for (int i = 0; i < sharedMaterials.Length; i++)
        {
            Material mat = sharedMaterials[i];
            if (mat != null && mat.HasProperty(LifetimeRangeId))
            {
                return mat.GetVector(LifetimeRangeId).y;
            }
        }

        return fallback;
    }

    public static void SetFloatOnMaterials(Material[] materials, int propertyId, float value)
    {
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat != null && mat.HasProperty(propertyId))
            {
                mat.SetFloat(propertyId, value);
            }
        }
    }

    public static void SetVectorOnMaterials(Material[] materials, int propertyId, Vector4 value)
    {
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat != null && mat.HasProperty(propertyId))
            {
                mat.SetVector(propertyId, value);
            }
        }
    }

    public static float ReadFloatFromFirstMaterial(Material[] materials, int propertyId, float fallback)
    {
        if (materials == null)
        {
            return fallback;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material mat = materials[i];
            if (mat != null && mat.HasProperty(propertyId))
            {
                return mat.GetFloat(propertyId);
            }
        }

        return fallback;
    }
}
