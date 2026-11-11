using System;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class AbstractCache : AbstractGetCache
{

    protected T[] LoadAllArmorResources<T>(List<string> allModels, Func<string, T> loadResourceFunc)
    {
        if (allModels.Count == 0)
        {
            return Array.Empty<T>();
        }

        T[] resources = new T[allModels.Count];

        for (int i = 0; i < allModels.Count; i++)
        {
            string model = allModels[i];

            if (!string.IsNullOrEmpty(model))
            {
                resources[i] = loadResourceFunc(model);
            }
        }

        return resources;
    }












}
