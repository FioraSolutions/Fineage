using System.Collections.Generic;
using UnityEngine;

public partial class ParticleGroupV2
{
    public bool TryGetMaterialVector(int propertyId, out Vector4 value)
    {
        Material material = ResolveSlotMaterial();
        if (material == null || !material.HasProperty(propertyId))
        {
            value = Vector4.zero;
            return false;
        }

        value = material.GetVector(propertyId);
        return true;
    }

    public void SetRuntimeMaterialVector(int propertyId, Vector4 value)
    {
        EnsureRuntimeMaterialInstances();
        WriteMaterialVector(_runtimeMaterialInstances, propertyId, value);
        Material[] gpu = GpuMaterials;
        if (gpu != null && gpu != _runtimeMaterialInstances)
        {
            WriteMaterialVector(gpu, propertyId, value);
        }
    }

    static void WriteMaterialVector(Material[] materials, int propertyId, Vector4 value)
    {
        if (materials == null)
        {
            return;
        }

        for (int i = 0; i < materials.Length; i++)
        {
            Material material = materials[i];
            if (material != null && material.HasProperty(propertyId))
            {
                material.SetVector(propertyId, value);
            }
        }
    }
}
