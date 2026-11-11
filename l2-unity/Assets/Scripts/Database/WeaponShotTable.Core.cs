using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed partial class WeaponShotTable
{
    public static void Bind(WeaponShotTable table)
    {
        if (table == null)
            return;
        _instance = table;
        TablePacketServices.Bind(weaponShots: table);
    }

    public BaseEffect ResolvePrefab(Gear gear)
    {
        return ResolvePrefab(gear, false);
    }

    public BaseEffect ResolvePrefab(Gear gear, bool spirit)
    {
        string section = ResolveSection(gear);
        Dictionary<string, string> paths = spirit ? _spiritPrefabPaths : _prefabPaths;
        if (string.IsNullOrEmpty(section) || !paths.TryGetValue(section, out string resourcePath))
            return null;

        string cacheKey = (spirit ? "S:" : "P:") + section;
        if (_loaded.TryGetValue(cacheKey, out BaseEffect cached))
            return cached;

        BaseEffect prefab = Resources.Load<BaseEffect>(resourcePath);
        if (prefab == null)
        {
            // Debug.LogWarning("WeaponShotTable: prefab not found for [" + section + "] at " + resourcePath);
            return null;
        }

        _loaded[cacheKey] = prefab;
        return prefab;
    }
}
