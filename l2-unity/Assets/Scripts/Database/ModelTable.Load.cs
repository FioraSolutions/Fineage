using System;
using System.Collections.Generic;
using UnityEngine;

public partial class ModelTable : AbstractCache
{
    private Dictionary<string, GameObject> LoadItemModels<T>(
        Dictionary<int, T> items,
        Func<T, string> modelOf,
        string label)
    {
        Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();
        int success = 0;

        foreach (KeyValuePair<int, T> pair in items)
        {
            string model = modelOf(pair.Value);
            if (string.IsNullOrEmpty(model) || cache.ContainsKey(model))
                continue;

            GameObject go = LoadWeaponModel(model);
            if (go == null)
                continue;

            cache[model] = go;
            success++;
        }

        // Debug.Log("Successfully loaded " + success + "/" + items.Count + " " + label + " model(s).");
        return cache;
    }
}
