using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Loads the skillgrp desc table, the skill-effect-placement table, and the effect-prefab index
/// once. Every cast asks this catalog; a missing file is an error, not a GlobalEffect guess.
/// </summary>
public static class SkillVisualRuntime
{
    const string Folder = "Data/Meta/SkillVisual";

    static SkillVisualCatalog _catalog;
    static Dictionary<string, string> _prefabs;
    static bool _failed;
    static readonly Dictionary<string, BaseEffect> _loaded = new Dictionary<string, BaseEffect>(StringComparer.Ordinal);

    public static bool TryCatalog(out SkillVisualCatalog catalog)
    {
        EnsureLoaded();
        catalog = _catalog;
        return catalog != null;
    }

    public static float FlightSeconds(int skillId, int level)
    {
        SkillVisualCatalog catalog;
        if (!TryCatalog(out catalog))
            return 0f;

        // Asset presence does not change the authored flying time.
        SkillVisualResolution resolution = catalog.Resolve(skillId, level, Present);
        if (resolution.Status == SkillVisualStatus.None)
            return 0f;
        return resolution.FlyingTime > 0f ? resolution.FlyingTime : 0f;
    }

    public static bool TryLoadPrefab(string assetKey, out BaseEffect prefab, out string resourcePath)
    {
        prefab = null;
        resourcePath = null;
        EnsureLoaded();
        if (_prefabs == null || string.IsNullOrEmpty(assetKey) || !_prefabs.TryGetValue(assetKey, out resourcePath))
            return false;

        if (_loaded.TryGetValue(assetKey, out prefab) && prefab != null)
            return true;

        GameObject go = Resources.Load<GameObject>(resourcePath);
        if (go == null)
        {
            Debug.LogError("[SkillVisual] Resources.Load failed asset=" + assetKey + " path=" + resourcePath);
            return false;
        }

        prefab = go.GetComponent<BaseEffect>();
        if (prefab == null)
            prefab = go.GetComponentInChildren<BaseEffect>(true);
        if (prefab == null)
        {
            Debug.LogError("[SkillVisual] prefab has no BaseEffect asset=" + assetKey + " path=" + resourcePath);
            return false;
        }

        _loaded[assetKey] = prefab;
        return true;
    }

    static bool Present(string assetKey)
    {
        return true;
    }

    static void EnsureLoaded()
    {
        if (_catalog != null || _failed)
            return;

        string root = Path.Combine(Application.streamingAssetsPath, Folder);
        string descPath = Path.Combine(root, "skillgrp-desc.txt");
        string placementPath = Path.Combine(root, "skill-effect-placement.txt");
        string prefabPath = Path.Combine(root, "effect-prefabs.txt");
        if (!File.Exists(descPath) || !File.Exists(placementPath) || !File.Exists(prefabPath))
        {
            _failed = true;
            Debug.LogError(
                "[SkillVisual] catalog files missing under " + root +
                " desc=" + File.Exists(descPath) +
                " placement=" + File.Exists(placementPath) +
                " prefabs=" + File.Exists(prefabPath));
            return;
        }

        string error;
        if (!SkillVisualCatalog.TryParse(File.ReadAllText(descPath), File.ReadAllText(placementPath), out _catalog, out error))
        {
            _failed = true;
            _catalog = null;
            Debug.LogError("[SkillVisual] catalog parse failed: " + error);
            return;
        }

        _prefabs = new Dictionary<string, string>(StringComparer.Ordinal);
        string[] lines = File.ReadAllText(prefabPath).Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            if (line.Length == 0 || line[0] == '#')
                continue;
            string[] cols = line.Split('\t');
            if (cols.Length < 2)
                continue;
            if (string.Equals(cols[0], "asset_key", StringComparison.OrdinalIgnoreCase))
                continue;
            _prefabs[cols[0].Trim()] = cols[1].Trim();
        }

        if (_prefabs.Count == 0)
        {
            _failed = true;
            _catalog = null;
            Debug.LogError("[SkillVisual] effect prefab index is empty: " + prefabPath);
        }
    }
}
