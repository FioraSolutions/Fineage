using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Editor/play-mode recovery when SerializeReference _v2Parts fails to load (hub-split).
/// Reads the authored *_composite.prefab YAML and rebuilds parts with GUID→prefab resolve.
/// </summary>
public static class CompositeEffectPrefabYamlParser
{
    static readonly Regex RidLine = new Regex(
        @"^\s*-\s*rid:\s*(-?\d+)\s*$",
        RegexOptions.Compiled);
    static readonly Regex TypeLine = new Regex(
        @"^\s*type:\s*\{class:\s*([^,]*),\s*ns:\s*([^,]*),\s*asm:\s*([^}]*)\}",
        RegexOptions.Compiled);
    static readonly Regex PrefabGuid = new Regex(
        @"prefab:\s*\{fileID:\s*\d+,\s*guid:\s*([a-f0-9]+)",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static bool TryBuildPartsFromYaml(string compositeGoName, out CompositePart[] parts)
    {
        parts = null;
        string stem = compositeGoName != null
            ? compositeGoName.Replace("(Clone)", string.Empty).Trim()
            : string.Empty;
        const string suffix = "_composite";
        if (stem.EndsWith(suffix, StringComparison.Ordinal))
            stem = stem.Substring(0, stem.Length - suffix.Length);
        if (string.IsNullOrEmpty(stem))
            return false;

        string path = Path.Combine(
            Application.dataPath,
            "Resources",
            "Data",
            "Effects",
            stem,
            stem + "_composite.prefab");
        if (!File.Exists(path))
            return false;

        string[] lines = File.ReadAllLines(path);
        List<RefBlock> blocks = ParseRefBlocks(lines);
        if (blocks.Count == 0)
            return false;

        List<CompositePart> built = new List<CompositePart>(blocks.Count);
        for (int i = 0; i < blocks.Count; i++)
        {
            RefBlock block = blocks[i];
            if (string.IsNullOrEmpty(block.ClassName) ||
                block.ClassName == "FeetPlacement" ||
                block.ClassName == "ChestPlacement" ||
                block.ClassName == "HitPointPlacement" ||
                block.ClassName == "BonePlacement" ||
                block.ClassName == "WeaponPlacement" ||
                block.ClassName.EndsWith("Placement", StringComparison.Ordinal))
            {
                continue;
            }

            CompositePart part = CreatePart(block.ClassName);
            if (part == null)
                continue;

            ApplyCommonFields(part, block.Body, stem);
            if (part.prefab == null)
                continue;

            ShotProjectilePart shot = part as ShotProjectilePart;
            if (shot != null)
                ApplyShotFields(shot, block.Body);

            built.Add(part);
        }

        if (built.Count == 0)
            return false;

        parts = built.ToArray();
        return true;
    }

    static CompositePart CreatePart(string className)
    {
        switch (className)
        {
            case "StationaryPart":
                return new StationaryPart();
            case "ShotProjectilePart":
                return new ShotProjectilePart();
            case "ImpactPart":
                return new ImpactPart();
            case "IndependentEffectPart":
                return new IndependentEffectPart();
            case "WeaponTransientPart":
                return new WeaponTransientPart();
            case "HomeFlightPart":
                return new HomeFlightPart();
            default:
                return null;
        }
    }

    static void ApplyCommonFields(CompositePart part, string body, string stem)
    {
        part.name = ReadString(body, "name") ?? part.name;
        part.scale = ReadFloat(body, "scale", 1f);
        part.spawnTiming = (CompositePartSpawnTiming)ReadInt(body, "spawnTiming", 0);
        part.spawnDelaySeconds = ReadFloat(body, "spawnDelaySeconds", 0f);
        part.spawnAttachmentPoint = (EffectAttachmentPoint)ReadInt(body, "spawnAttachmentPoint", 0);
        part.boneName = ReadString(body, "boneName") ?? string.Empty;
        part.follow = ReadInt(body, "follow", 1) != 0;
        part.inheritRotation = ReadInt(body, "inheritRotation", 0) != 0;

        Match prefab = PrefabGuid.Match(body);
        if (prefab.Success)
            part.prefab = LoadBaseEffectByGuid(prefab.Groups[1].Value);

        if (part.prefab == null && !string.IsNullOrEmpty(part.name))
            part.prefab = LoadBaseEffectByResourcesName(stem, part.name);
    }

    static void ApplyShotFields(ShotProjectilePart shot, string body)
    {
        shot.launchMode = (ProjectileLaunchMode)ReadInt(body, "launchMode", (int)ProjectileLaunchMode.OnAnimationShoot);
        shot.showBeforeAnimationShoot = ReadInt(body, "showBeforeAnimationShoot", 0) != 0;
        shot.impactType = (ProjectileImpactType)ReadInt(body, "impactType", (int)ProjectileImpactType.EffectOnly);
        shot.speed = ReadFloat(body, "speed", 10f);
    }

    static BaseEffect LoadBaseEffectByGuid(string guid)
    {
#if UNITY_EDITOR
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(assetPath))
            return null;
        GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if (go == null)
            return null;
        BaseEffect effect = go.GetComponent<BaseEffect>();
        return effect != null ? effect : go.GetComponentInChildren<BaseEffect>(true);
#else
        return null;
#endif
    }

    static BaseEffect LoadBaseEffectByResourcesName(string stem, string partName)
    {
        // Authored layout: Resources/Data/Effects/<stem>/<partName>/<partName>
        string[] candidates =
        {
            "Data/Effects/" + stem + "/" + partName + "/" + partName,
            "Data/Effects/" + stem + "/" + partName,
            "Data/Effects/" + partName + "/" + partName,
            "Data/Effects/" + partName
        };

        for (int i = 0; i < candidates.Length; i++)
        {
            GameObject go = Resources.Load<GameObject>(candidates[i]);
            if (go == null)
                continue;
            BaseEffect effect = go.GetComponent<BaseEffect>();
            if (effect == null)
                effect = go.GetComponentInChildren<BaseEffect>(true);
            if (effect != null)
                return effect;
        }

        // Aura flare aliases: el_aura_flare_ta_1 → el_aura_flare_ta
        if (partName != null && partName.Length > 2 &&
            char.IsDigit(partName[partName.Length - 1]) &&
            partName[partName.Length - 2] == '_')
        {
            string baseName = partName.Substring(0, partName.Length - 2);
            if (!string.Equals(baseName, partName, StringComparison.Ordinal))
                return LoadBaseEffectByResourcesName(stem, baseName);
        }

        return null;
    }

    static string ReadString(string body, string key)
    {
        Match m = Regex.Match(body, @"^\s*" + key + @":\s*(.*)\s*$", RegexOptions.Multiline);
        if (!m.Success)
            return null;
        string raw = m.Groups[1].Value.Trim();
        if (raw.Length >= 2 && raw[0] == '"' && raw[raw.Length - 1] == '"')
            raw = raw.Substring(1, raw.Length - 2);
        return raw;
    }

    static int ReadInt(string body, string key, int fallback)
    {
        string raw = ReadString(body, key);
        int value;
        return int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
            ? value
            : fallback;
    }

    static float ReadFloat(string body, string key, float fallback)
    {
        string raw = ReadString(body, key);
        float value;
        return float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value)
            ? value
            : fallback;
    }

    sealed class RefBlock
    {
        public long Rid;
        public string ClassName;
        public string Body;
    }

    static List<RefBlock> ParseRefBlocks(string[] lines)
    {
        List<RefBlock> blocks = new List<RefBlock>();
        int i = 0;
        while (i < lines.Length && !lines[i].Contains("RefIds:"))
            i++;
        if (i >= lines.Length)
            return blocks;
        i++;

        while (i < lines.Length)
        {
            Match ridMatch = RidLine.Match(lines[i]);
            if (!ridMatch.Success)
            {
                i++;
                continue;
            }

            long rid = long.Parse(ridMatch.Groups[1].Value, CultureInfo.InvariantCulture);
            i++;
            string className = string.Empty;
            if (i < lines.Length)
            {
                Match typeMatch = TypeLine.Match(lines[i]);
                if (typeMatch.Success)
                {
                    className = typeMatch.Groups[1].Value.Trim();
                    i++;
                }
            }

            if (i < lines.Length && lines[i].TrimStart().StartsWith("data:", StringComparison.Ordinal))
                i++;

            int bodyStart = i;
            while (i < lines.Length)
            {
                string t = lines[i].TrimStart();
                if (t.StartsWith("- rid:", StringComparison.Ordinal))
                    break;
                if (t.StartsWith("---", StringComparison.Ordinal))
                    break;
                i++;
            }

            string body = string.Join("\n", lines, bodyStart, Math.Max(0, i - bodyStart));
            blocks.Add(new RefBlock
            {
                Rid = rid,
                ClassName = className,
                Body = body
            });
        }

        return blocks;
    }
}
