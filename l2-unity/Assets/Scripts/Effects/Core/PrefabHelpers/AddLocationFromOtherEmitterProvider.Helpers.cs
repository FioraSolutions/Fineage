using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class AddLocationFromOtherEmitterProvider : MonoBehaviour, IParticleSpawnLocationAddProvider
{
    static Material ResolveSourceMaterial(ParticleGroupV2 sourceEmitter)
    {
        if (sourceEmitter == null)
        {
            return null;
        }

        Material[] gpuMaterials = sourceEmitter.GpuMaterials;
        if (gpuMaterials != null)
        {
            for (int i = 0; i < gpuMaterials.Length; i++)
            {
                if (gpuMaterials[i] != null)
                {
                    return gpuMaterials[i];
                }
            }
        }

        Renderer renderer = sourceEmitter.GetComponentInChildren<Renderer>(true);
        return renderer != null ? renderer.sharedMaterial : null;
    }

    static Transform ResolveSourceTransform(ParticleGroupV2 sourceEmitter)
    {
        Renderer renderer = sourceEmitter.GetComponentInChildren<Renderer>(true);
        return renderer != null ? renderer.transform : sourceEmitter.transform;
    }

    static Vector3 ReadRangeMid(Material mat, int id)
    {
        if (mat == null || !mat.HasProperty(id))
        {
            return Vector3.zero;
        }

        Vector4 range = mat.GetVector(id);
        float mid = (range.x + range.y) * 0.5f;
        if (id == StartLocationRangeXUcID)
        {
            return new Vector3(mid, 0f, 0f);
        }

        if (id == StartLocationRangeYUcID)
        {
            return new Vector3(0f, mid, 0f);
        }

        return new Vector3(0f, 0f, mid);
    }

    static Vector3 RotateAroundUcZ(Vector3 v, float radians)
    {
        float c = Mathf.Cos(radians);
        float s = Mathf.Sin(radians);
        return new Vector3(
            v.x * c - v.y * s,
            v.x * s + v.y * c,
            v.z);
    }
}
