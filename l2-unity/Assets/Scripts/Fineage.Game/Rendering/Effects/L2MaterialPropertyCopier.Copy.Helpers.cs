using System;
using UnityEngine;

public static partial class L2MaterialPropertyCopier
{
    private static void CopyFloatIfPresent(Material runtimeMat, Material sharedMat, int propertyId)
    {
        if (runtimeMat.HasProperty(propertyId) && sharedMat.HasProperty(propertyId))
        {
            runtimeMat.SetFloat(propertyId, sharedMat.GetFloat(propertyId));
        }
    }

    private static void CopyVectorIfPresent(Material runtimeMat, Material sharedMat, int propertyId)
    {
        if (runtimeMat.HasProperty(propertyId) && sharedMat.HasProperty(propertyId))
        {
            runtimeMat.SetVector(propertyId, sharedMat.GetVector(propertyId));
        }
    }

    private static void CopyColorIfPresent(Material runtimeMat, Material sharedMat, int propertyId)
    {
        if (runtimeMat.HasProperty(propertyId) && sharedMat.HasProperty(propertyId))
        {
            runtimeMat.SetColor(propertyId, sharedMat.GetColor(propertyId));
        }
    }
}
