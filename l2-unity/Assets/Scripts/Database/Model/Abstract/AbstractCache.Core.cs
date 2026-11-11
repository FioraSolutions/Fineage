using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class AbstractCache : AbstractGetCache
{
    protected GameObject[] LoadAllArmorModels(List<string> allModels)
    {
        return LoadAllArmorResources(allModels, LoadArmorModel);
    }

    protected Material[] LoadAllArmorMaterials(List<string> allModels)
    {
        return LoadAllArmorResources(allModels, LoadArmorMaterial);
    }

    protected Material[] LoadAllMaterials(List<string> allModels)
    {
        return LoadAllArmorResources(allModels, LoadArmorMaterial);
    }
}
